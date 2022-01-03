using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;
using Amazon.Util;
using LGUVirtualOffice.Framework;
using System.Security.Cryptography;
using System.Text;

namespace LGUVirtualOffice {
	public class AWSSQSUtil: IMessageQueueUtility
	{
		AmazonSQSClient sqsClient;
		SendMessageRequest sendRequest;
		SendMessageBatchRequest batchSendRequest;
		ReceiveMessageRequest getRequest;
		DeleteMessageRequest deleteRequest;
		JsonSerializerSettings jsonSerializerSettings;
		List<EventMessageModel> messageList = new List<EventMessageModel>();
		string Message_Attribute_Send_Time= "SentTimestamp";
		Dictionary<string, DateTime> receivedMessagePool = new Dictionary<string, DateTime>();
		BindableProperty<int> deleteMessageCount = new BindableProperty<int>();
		List<string> deleteMessageList = new List<string>();
		int Message_Delete_Time;
		int Max_Delete_Message_Count;
		int Delete_Count;

		//AWS SQS will not delete the messages which have already been received before the retention period expired(if the dead-letter queue
		//was set,then the condition would be the receive count which set with the dead-letter queue) .Because of this mechanisms,
		//there should be a lot of duplicated messages,so we need to disgard them,and if we keep receiving the same message,
		//we need to delete it if it was sent <messageDeleteTime> second ago(idealy,the message should be processed by all clients within less than one second) 
		public void GetMessage(Action<List<EventMessageModel>> onReceivedMessage, string queueUrl = null)
        {
			if (!InitAWSSQSClient())
			{
				return;
			}
			
			Task<ReceiveMessageResponse> response = sqsClient.ReceiveMessageAsync(getRequest);
			var awaiter = response.GetAwaiter();
			awaiter.OnCompleted(()=> {
				if (response.IsFaulted)
				{
					LogUtil.LogError("GetMessage Failed", response.Exception.GetBaseException());
				}
				else if (!response.IsCanceled)
				{
					List<Message> receivedMessageList = awaiter.GetResult().Messages;
					if (receivedMessageList != null && receivedMessageList.Count > 0) 
					{
						messageList.Clear();
						foreach (var item in receivedMessageList)
						{
							if (IsDuplicated(item)) 
							{   //if a message received not only once,and if it was sent a long time ago,we should delete it
								if (IsSentLongTimeAgo(item)) 
								{
									//delete the message
									DeleteMessage(item);
								}
								continue;
							}
							messageList.Add(JsonConvert.DeserializeObject<EventMessageModel>(item.Body));
						}
						if (messageList.Count > 0) 
						{
							onReceivedMessage.Invoke(messageList);
						}
					}
				}
			});
		}
		private bool IsSentLongTimeAgo(Message message) 
		{
            if (receivedMessagePool[message.MessageId] < DateTime.Now)
            {
                return true;
            }
            return false;
		}

		private bool IsDuplicated(Message message) 
		{
			if (receivedMessagePool.ContainsKey(message.MessageId)) 
			{
				return true;
			}
			DateTime sendTime = AWSSDKUtils.ConvertFromUnixEpochMilliseconds(long.Parse(message.Attributes[Message_Attribute_Send_Time]));
			receivedMessagePool.Add(message.MessageId, sendTime.AddSeconds(Message_Delete_Time));
			return false;
		}

		private void DeleteMessage(Message message) 
		{
			if (!deleteMessageList.Contains(message.MessageId)) 
			{
				deleteMessageList.Add(message.MessageId);
				deleteMessageCount.Value++;
			}
			deleteRequest.ReceiptHandle = message.ReceiptHandle;
			sqsClient.DeleteMessageAsync(deleteRequest);
		}

		public void SendMessage(EventMessageModel message, Action onMessageSendFailed,string queueUrl = null)
        {
			if (!InitAWSSQSClient())
			{
				onMessageSendFailed.Invoke();
			}
			/*if (!string.IsNullOrEmpty(queueUrl)) 
			{
				sendRequest.QueueUrl = queueUrl;
			}*/
			sendRequest.MessageBody = JsonConvert.SerializeObject(message, jsonSerializerSettings);
			Task<SendMessageResponse> response=sqsClient.SendMessageAsync(sendRequest);
			var awaiter = response.GetAwaiter();
			awaiter.OnCompleted(()=> {
				if (response.IsFaulted||response.IsCanceled)
				{
					LogUtil.LogError(response.Exception.GetBaseException().StackTrace, response.Exception.GetBaseException());
					onMessageSendFailed?.Invoke();
				}
				else 
				{
					LogUtil.LogDebug("SendMessage SUCCESS: "+response.Result.MessageId);
				}
			});
        }

		public void SendMessageBatch(List<EventMessageModel> messageList, Action<List<EventMessageModel>> onMessageSendFailed,string queueUrl = null) 
		{
			if (!InitAWSSQSClient()) 
			{
				onMessageSendFailed.Invoke(messageList);
			}
			/*if (!string.IsNullOrEmpty(queueUrl)) 
			{
				batchSendRequest.QueueUrl=queueUrl;
			}*/
			List<SendMessageBatchRequestEntry> requestEntries = new List<SendMessageBatchRequestEntry>();
			for (int i = 0; i < messageList.Count; i++)
            {
				var messageContent = messageList[i];
				SendMessageBatchRequestEntry entry = new SendMessageBatchRequestEntry();
				entry.Id = i.ToString();
				entry.MessageBody = JsonConvert.SerializeObject(messageContent, jsonSerializerSettings);
				requestEntries.Add(entry);
			} 
			 
			Task<SendMessageBatchResponse> response= sqsClient.SendMessageBatchAsync(batchSendRequest);
			var awaiter = response.GetAwaiter();
			awaiter.OnCompleted(()=> {
				if (response.IsFaulted)
				{
					onMessageSendFailed.Invoke(messageList);
				}
				else 
				{
					List<EventMessageModel> failedMessageList = null;
					if ( response.Result != null && response.Result.Failed!=null) 
					{
						if (response.Result.Failed.Count > 0) 
						{
							failedMessageList = new List<EventMessageModel>();
							foreach (var item in response.Result.Successful)
							{
								failedMessageList.Add(messageList[int.Parse(item.Id)]);
							}
							onMessageSendFailed.Invoke(failedMessageList);
						}
					}
				}
			});
		}
		private void RemoveMessageFromMessagePool(int deleteCount) {
			//everytime we got twenty messages which need to be deleted,we remove the first ten messages from the pool
			if (deleteCount == Max_Delete_Message_Count) 
			{
                for (int i = 0; i < Delete_Count; i++)
                {
					receivedMessagePool.Remove(deleteMessageList[0]);
					deleteMessageList.RemoveAt(0);
				}
				deleteMessageCount.Value -= Delete_Count;
			}
		}
        private bool InitAWSSQSClient() 
		{
			var cred=AWSUtil.Instance.GetCognitoAWSCredentials(AWSUtil.Instance._user.UserID);
			if (cred == null) 
			{
				LogUtil.LogDebug("InitAWSSQSClient Failed");
				return false;
			}
            if (sqsClient == null)
            {
				AWSSetting awsSetting = AWSUtil.Instance.GetAWSSetting();
				string queueUrl = awsSetting.SQS_Queue_Url;
				sqsClient = new AmazonSQSClient(cred, AWSRegionMapper.regionMapper[AWSUtil.Instance.GetAWSSetting().region]);
				sendRequest = new SendMessageRequest(queueUrl,null);
				getRequest = new ReceiveMessageRequest(queueUrl);
				batchSendRequest = new SendMessageBatchRequest(queueUrl,null);
				getRequest.AttributeNames = new List<string>() { Message_Attribute_Send_Time };
				getRequest.MaxNumberOfMessages = awsSetting.SQS_Max_Message_Num;
				Max_Delete_Message_Count = awsSetting.SQS_Max_Delete_Count;
				Delete_Count = awsSetting.SQS_Delete_Message_Count;
				Message_Delete_Time = awsSetting.SQS_Message_Delete_Time;
				deleteRequest = new DeleteMessageRequest(queueUrl,null);
				jsonSerializerSettings = new JsonSerializerSettings() { DateFormatString=AWSUtil.Instance.GetAWSSetting().dateTime_convert_format};
				deleteMessageCount.Subscribe(RemoveMessageFromMessagePool);
			}
			return true;
		}
	}
}