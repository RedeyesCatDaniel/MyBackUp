using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    [CreateAssetMenu(menuName = "AsyncCommand/AsyncEventCommand")]
    public class AsyncEventCommand : AsyncTool
    {
        public AsyncEventAction aaction;
        public override void Act()
        {
            aaction.Act();
        }

        public override void OnFinish(System.Action action)
        {
            aaction.OnFinish(action);
        }

        
    }
}