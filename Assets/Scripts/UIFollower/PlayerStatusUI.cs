using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

namespace LGUVirtualOffice
{
    public class PlayerStatusUI : MonoBehaviourPunCallbacks
    {
        
        public GameObject followCharacter;
        public RectTransform tmp;
        public Vector2 offset;
        string name = "";
        public RectTransform followTMP;

        public avAvatarMemberInfoManager aamim;

        private void Init()
        {
            if (NameCanvas.nameCanvas == null)
            {
                NameCanvas.Instance.CreatCanvas();
            }
            followTMP = Instantiate<RectTransform>(tmp, NameCanvas.nameCanvas.transform);
        }

        private void Dead() {
            if (followTMP != null)
            {

                Destroy(followTMP.gameObject);
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            Init();
        }

        public override void OnDisable()
        {
            base.OnDisable();
            Dead();
        }

        private void Start()
        {
            SetName();
        }

        private void FixedUpdate()
        {
            if (IsInView(transform.position))
            {
                followTMP.gameObject.SetActive(true);
                UIFollower(followCharacter, followTMP);
            }
            else
            {
                followTMP.gameObject.SetActive(false);
            }
        }

        void UIFollower(GameObject followObj, RectTransform follower)
        {
            if (follower == null) return; 
            Vector2 screenPos = Camera.main.WorldToScreenPoint(followObj.transform.position);
            follower.position = screenPos + offset;
        }





        public void SetName()
        {
            aamim.FetchData<string>("Name", (x) => {

                name = x;
                followTMP.GetComponent<TextMeshProUGUI>().text = name;
            });
        }

        bool IsInView(Vector3 worldpos)
        {
            Transform camTransform = Camera.main.transform;
            Vector2 viewPos = Camera.main.WorldToViewportPoint(worldpos);
            Vector3 dir = (worldpos - camTransform.position).normalized;
            float dot = Vector3.Dot(camTransform.forward, dir);

            if (dot > 0 && viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
            {
                return true;
            }
            return false;
        }

    }
}
