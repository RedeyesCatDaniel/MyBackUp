using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
namespace LGUVirtualOffice
{

    [CreateAssetMenu(fileName = "AudioSetting", menuName = "Audio", order = 1)]
    public class AudioSetting : ScriptableObject
    {
        [Range(0, 1)]
        [Tooltip("Audio volume")]
        public float volume;
        [Tooltip("Player")]
        public GameObject Player;
        [Tooltip("enable")]
        public AudioClip onenable;
        [Tooltip("onClick")]
        public AudioClip onClick;
        [Tooltip("onDisable")]
        public AudioClip onDisable;
        [Tooltip("BGMLIst")]
        public List<AudioClip> bgm;
        private AudioSource bgmplayer;
        public void AudioInit()
        {
            bgmplayer = AudioPlayer.Instance.BgmPlayer;
            if (!bgmplayer)
            {
                GameObject bgm = Instantiate(Player);
                DontDestroyOnLoad(bgm);
                AudioPlayer.Instance.BgmPlayer =bgm.GetComponent<AudioSource>();
                bgmplayer = AudioPlayer.Instance.BgmPlayer;
                bgmplayer.playOnAwake = false;
                bgmplayer.loop = true;
                bgmplayer.volume = volume;
            }
            else if (!bgmplayer.isActiveAndEnabled)
            {
                bgmplayer.gameObject.SetActive(true);
                bgmplayer.enabled = true;
            }

            ui_Player = AudioPlayer.Instance.UiPlayer;
            if (!ui_Player)
            {
                GameObject obj= Instantiate(Player);
                DontDestroyOnLoad(obj);
                AudioPlayer.Instance.UiPlayer =obj.GetComponent<AudioSource>();
                ui_Player = AudioPlayer.Instance.UiPlayer;
                ui_Player.loop = false;
                ui_Player.playOnAwake = false;
                ui_Player.volume = volume;
            }
            else if (!ui_Player.isActiveAndEnabled)
            {
                ui_Player.gameObject.SetActive(true);
                ui_Player.enabled = true;
            }
        }
        public void ChangeBGM(AudioClip audioClip)
        {
            AudioInit();
            bgmplayer.volume = volume;
            bgmplayer.clip = audioClip;
            bgmplayer.Play();
        }
        public void ChangeBGM(int index)
        {
            if (bgm.Count > index)
            {
                ChangeBGM(bgm[index]);
            }
            else
            {
                ChangeBGM(null);
            }

        }
        public void PauseBGM()
        {

            AudioInit();
            bgmplayer.Pause();
        }
        public void UnPauseBGM()
        {
            AudioInit();
            bgmplayer.UnPause();
        }
        private AudioSource ui_Player;
        public void PlayAudio(AudioClip audioClip)
        {
            AudioInit();
            ui_Player.volume = volume;
            ui_Player.clip = audioClip;
            ui_Player.Play();
        }
        public void SetVolume(float volume)
        {
            AudioInit();
            ui_Player.volume = volume;
            bgmplayer.volume = volume;
        }
    }
}
