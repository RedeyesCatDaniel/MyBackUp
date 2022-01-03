using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LGUVirtualOffice
{
    public class avAvatarMemberInfoManager : MonoBehaviour
    {
        public PhotonView view;
        public string id;
        public int gender;
        public void FetchGender(System.Action<int> genderAction) {


            var handler = memService.PullData<int>(id, "Gender");
            handler.OnCompleted((x) => {

                if (x.TryGetValue("Gender", out int gender))
                {
                    genderAction(gender);
                }
                else {
                    memService.PushData<int>(id, "Gender",0);
                    genderAction(0);
                }
            });

            handler.OnFailed(() => {
                Debug.Log("Fail to read an empty gender");
            });


        }



        public bool FetchID(System.Action<string> idAction) {
            if (id != null)
            {
                idAction(id);
                return true;
            }
            else {
                if (view.Owner.CustomProperties.TryGetValue("UserId", out object value))
                {
                    id = (string)value;
                    idAction(id);
                    return true;
                }
                else {
                    return false;
                }
            }
        }

        public void UpdateID() {
            if (view.Owner.CustomProperties.TryGetValue("UserId", out object value))
            {
                id = (string)value;

            }
            
          
            
        }
    }
}