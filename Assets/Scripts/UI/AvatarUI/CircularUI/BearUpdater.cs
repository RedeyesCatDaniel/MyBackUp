using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LGUVirtualOffice
{
    public class BearUpdater : MonoBehaviour
    {
        // Start is called before the first frame update
        public UnityEvent OnUpdate;

        private void Update()
        {
            OnUpdate.Invoke();
        }
    }
}