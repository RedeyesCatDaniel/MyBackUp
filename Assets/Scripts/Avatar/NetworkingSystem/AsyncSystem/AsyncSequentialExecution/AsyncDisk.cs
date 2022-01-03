using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    public class AsyncDisk: IAsyncAction 
    {
        public string key;
        public string[] dependency;

        public void Act()
        {
            throw new NotImplementedException();
        }

        public void OnFinish(Action action)
        {
            throw new NotImplementedException();
        }

        public void Fullfill(string key) { 
        
        }
    }
}