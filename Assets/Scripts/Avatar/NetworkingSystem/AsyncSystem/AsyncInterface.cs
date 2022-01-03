using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LGUVirtualOffice
{
    public interface IAsyncHandler
    {
        public void OnEnd(System.Action end);
    }

    public class AsyncHandler : IAsyncHandler
    {
        public System.Action DOnEnd;
        public void OnEnd(Action end)
        {
            DOnEnd += DOnEnd;
        }
    }

    public interface IAsyncAction {
        public void Act();
        public void OnFinish(System.Action action);
    }

    [System.Serializable]
    public class AsyncEventAction : IAsyncAction
    {
        public UnityEvent Actions = new UnityEvent();
        private System.Action DOnFinish;
        public void Act()
        {
            Actions.Invoke();
            DOnFinish?.Invoke();
            DOnFinish = null;
        }

        public void OnFinish(Action action){
            DOnFinish = action;
        }
    }

    public abstract class AsyncTool : ScriptableObject, IAsyncAction
    {
        public abstract void Act();

        public abstract void OnFinish(Action action);
    }
    public static class SequentialIAsyncAction {
        public static void SequentialAct(this IEnumerable<IAsyncAction> actions) {
            IAsyncAction head = null;
            IAsyncAction curr = null;
            foreach (var item in actions)
            {
                if (head == null)
                {
                    head = item;                  
                }
                else { 
                    curr.OnFinish(item.Act);
                }
                curr = item;
            }

            if (head != null) {
                head.Act();
            }
        }
    }
}