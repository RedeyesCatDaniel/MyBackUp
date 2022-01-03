using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    public class AsyncDataCollectionContainer : MonoBehaviour
    {
        public AsyncDataCollection data;
        //public UnityEvent onStart;
        //public List<AsyncTool> tools;
        //public AsyncEventAction onFinish;

        [ContextMenu(itemName: "Execute")]
        public void Execute()
        {
            data.Execute();
        }

        public void DebugPrint(string info)
        {
            Debug.Log(info);
        }
    }
}