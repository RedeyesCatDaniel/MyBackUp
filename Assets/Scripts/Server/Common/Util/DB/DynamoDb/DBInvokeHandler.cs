using System;
using System.Collections.Generic;

namespace LGUVirtualOffice {
	public class DBInvokeHandler<T>
	{
		private Action onFailed;
		private Action<T> onComplete;
		//call back for process success
		public void OnCompleted(Action<T> mOnComplete) 
		{
			onComplete = mOnComplete;
		}
		//call back for process failed
		public void OnFailed(Action mOnFailed) 
		{
			onFailed = mOnFailed;
		}
		public void TriggerOnFailed() 
		{
			onFailed?.Invoke();
		}
		public void TriggerOnCompleted(T result) 
		{
			onComplete?.Invoke(result);
		}
	}
}