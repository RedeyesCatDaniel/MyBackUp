using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    public interface netISymchronizable {
        public void Push(System.Action OnPush);
        public void Pull(System.Action OnPull);
    }
}