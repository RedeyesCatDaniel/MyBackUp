using LGUVirtualOffice.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    public class Member : AbstractModel, IMember
    {
        private static Dictionary<string, IMember> members = new Dictionary<string, IMember>();

        public string id;
        public string ID { get => id; set => id = value; }

        private Dictionary<string, bool> dataPool = new Dictionary<string, bool>();
        private Dictionary<string, string> dataContainer = new Dictionary<string, string>();


        protected override void OnInit(){}

        public Member(string id) {
            this.id = id;
            members[id] = this;
        }

        public bool HasData(string key) {
            if (dataPool.ContainsKey(key)) {
                return dataPool[key];
            }
            return false;
        }
    }
}
