using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LGUVirtualOffice
{
    public class AudioPlayer : Singleton<AudioPlayer>
    {
        private AudioPlayer()
        {
        }
        public AudioSource BgmPlayer;
        public AudioSource UiPlayer;
    }
}