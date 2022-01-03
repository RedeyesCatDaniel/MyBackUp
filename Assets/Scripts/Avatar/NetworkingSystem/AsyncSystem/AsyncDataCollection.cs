using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LGUVirtualOffice
{

    [CreateAssetMenu(menuName = "DataCollection/AsyncDataCollection")]
    public class AsyncDataCollection : ScriptableObject
    {
        public AsyncData data;
        [ContextMenu(itemName: "Execute")]
        public void Execute() {
            data.Execute();
        }

        public void DebugPrint(string info)
        {
            Debug.Log(info);
        }
    }

    [System.Serializable]
    public class AsyncData {
        public UnityEvent onStart;
        public List<AsyncTool> tools;
        public AsyncEventAction onFinish;

        public void Execute()
        {
            onStart.Invoke();
            List<IAsyncAction> actions = new List<IAsyncAction>();
            actions.AddRange(tools);
            actions.Add(onFinish);
            actions.SequentialAct();
        }


    }
}