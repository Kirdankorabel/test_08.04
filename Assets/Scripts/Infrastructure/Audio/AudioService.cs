using UnityEngine;

namespace Game2048.Infrastructure.Audio
{
    public class AudioService
    {
        private readonly AudioSource _source;
        private readonly AudioClip _launchClip;
        private readonly AudioClip _mergeClip;
        private readonly AudioClip _autoMergeClip;

        public AudioService()
        {
            var go = new GameObject("AudioService");
            Object.DontDestroyOnLoad(go);

            _source = go.AddComponent<AudioSource>();

            _launchClip = Resources.Load<AudioClip>("Audio/Launch");
            _mergeClip = Resources.Load<AudioClip>("Audio/Merge");
            _autoMergeClip = Resources.Load<AudioClip>("Audio/AutoMerge");
        }

        public void PlayLaunch()
        {
            if (_launchClip != null)
                _source.PlayOneShot(_launchClip);
        }

        public void PlayMerge()
        {
            if (_mergeClip != null)
                _source.PlayOneShot(_mergeClip);
        }

        public void PlayAutoMerge()
        {
            if (_autoMergeClip != null)
                _source.PlayOneShot(_autoMergeClip);
        }
    }
}
