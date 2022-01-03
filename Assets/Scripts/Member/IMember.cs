using LGUVirtualOffice.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    public interface IMember:IModel
    {
        public string ID { get; set; }
        public bool HasData(string key);
    }
}
