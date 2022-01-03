using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LGUVirtualOffice
{
    public class avEventSync : MonoBehaviourPunCallbacks
    {
        public List<UnityEvent> events;

        [PunRPC]
        public void SyncEvent(int index)
        {
            if (index>=0 && index<events.Count) {
                events[index].Invoke();
            }
        }
    }
}