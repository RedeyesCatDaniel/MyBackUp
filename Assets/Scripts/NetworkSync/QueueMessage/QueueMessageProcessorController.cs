using UnityEngine;
using LGUVirtualOffice.Framework;
using System.Collections;

namespace LGUVirtualOffice {
	public class QueueMessageProcessorController : AbstractController
	{
		private QueueMessageProcessorController instance;
		private IQueueMessageService queueMessageService;
		private float messageProcessInterval;
		IEnumerator processMessageCoroutine;
		void Start()
		{
			if (instance == null)
			{
				instance = this;
				DontDestroyOnLoad(this);
			}
			else 
			{
				Destroy(gameObject);
			}
			messageProcessInterval = AWSUtil.Instance.GetAWSSetting().SQS_Message_Process_interval;
			queueMessageService = this.GetService<IQueueMessageService>();
			this.SubscribeEvent<JoinPhotonRoomSuccessEvent>(OnJoinedPhotonRoom).UnSubScribeWhenGameObjectDestroyed(gameObject);
			this.SubscribeEvent<DisconnectFromServerEvent>(OnDisconnectedFromServer).UnSubScribeWhenGameObjectDestroyed(gameObject);
		}
        private void Update()
        {
			if (Input.GetKeyDown(KeyCode.Space)) 
			{
				processMessageCoroutine = ProcessMessage();
				StartCoroutine(processMessageCoroutine);
			}
        }
        private void OnJoinedPhotonRoom(JoinPhotonRoomSuccessEvent e) 
		{
			LogUtil.LogDebug("JoinPhotonRoomSuccessEvent Triggered!");
			processMessageCoroutine = ProcessMessage();
			StartCoroutine(processMessageCoroutine);
		}
        private void OnDisconnectedFromServer(DisconnectFromServerEvent e)
        {
			if (processMessageCoroutine != null) 
			{
				StopCoroutine(processMessageCoroutine);
			}
        }
        IEnumerator ProcessMessage()
		{
			while (true) 
			{
				yield return new WaitForSeconds(messageProcessInterval);
				queueMessageService.ProcessMessage();
			}
		}
	}
}