using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using LGUVirtualOffice.Framework;
namespace LGUVirtualOffice
{
    public class UIAudioController : AbstractController, IPointerClickHandler
    {

        public AudioSetting audioSetting;

        public void OnPointerClick(PointerEventData eventData)
        {
            audioSetting.PlayAudio(audioSetting.onClick);
        }
        private void OnEnable()
        {
            audioSetting.PlayAudio(audioSetting.onenable);

        }
        private void OnDisable()
        {

            audioSetting.PlayAudio(audioSetting.onDisable);
        }
        //void Awake()
        //{
        //    audioSetting.GetAudioSource();
        //}
        //public void Play(AudioClip audioClip)
        //{
        //    ui_Player = AudioPlayer.Instance.UiPlayer;
        //    if (!ui_Player)
        //    {
        //        ui_Player = Instantiate(audioSetting.UISoundPlayer, Camera.main.transform).GetComponent<AudioSource>();
        //        AudioPlayer.Instance.UiPlayer = ui_Player;
        //    }
        //    else if (!ui_Player.isActiveAndEnabled)
        //    {
        //        ui_Player.gameObject.SetActive(true);
        //        ui_Player.enabled = true;
        //    }
        //    ui_Player.clip = audioClip;
        //    ui_Player.Play();
        //}

    }
   
}