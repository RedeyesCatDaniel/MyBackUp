using System;

namespace LGUVirtualOffice
{
	public class ServerReturnModel<T>
	{

		private Action<T> onSuccess;
		private Action<string> onFailed;
		public void OnSuccess(Action<T> mOnSuccess)
		{
			onSuccess = mOnSuccess;
		}
		public void OnFailed(Action<string> mOnFailed)
		{
			onFailed = mOnFailed;
		}
		public void TriggerOnSuccess(T result)
		{
			onSuccess?.Invoke(result);
		}
		public void TriggerOnFailed(string errorCode)
		{
			onFailed?.Invoke(errorCode);
		}
	}
}