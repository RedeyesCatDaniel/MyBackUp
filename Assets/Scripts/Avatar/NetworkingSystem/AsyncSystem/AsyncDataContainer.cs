using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LGUVirtualOffice
{
    public class AsyncDataContainer : MonoBehaviour
    {
        public AsyncData data;

        public void Execute() {
            data.Execute();
        }
        
    }
}