using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Otumn.Bokya
{
    public class SoundManager : Entity
    {
        public AudioSource source;
        public AudioClip clickClip;
        public AudioClip whooshClip;
        public AudioClip wrongClip;
        public AudioClip rightClip;
        public AudioClip warningClip;

        public override void OnRequestSound(SoundRequest type, float volume)
        {
            base.OnRequestSound(type, volume);
            switch (type)
            {
                case SoundRequest.Click:
                    source.clip = clickClip;
                    break;
                case SoundRequest.Whoosh:
                    return;
                    source.clip = whooshClip;
                    break;
                case SoundRequest.Wrong:
                    source.clip = wrongClip;
                    break;
                case SoundRequest.Right:
                    source.clip = rightClip;
                    break;
                case SoundRequest.Warning:
                    return;
                    source.clip = warningClip;
                    break;
            }
            source.volume = volume * GameManager.saveManager.settingsData.Volume;
            PlaySound();
        }

        public override void OnRequestSound(AudioClip clip, float volume)
        {
            base.OnRequestSound(clip, volume);
            source.clip = clip;
            source.volume = volume * GameManager.saveManager.settingsData.Volume;
            PlaySound();
        }

        private void PlaySound()
        {
            source.Stop();
            source.Play();
        }
    }

    public enum SoundRequest
    {
        Click,
        Whoosh,
        Wrong,
        Right,
        Warning
    };
}
