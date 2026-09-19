using UnityEngine;

namespace AwCon
{
    /// <summary>
    // 已修复编码乱码的注释。
    /// </summary>
    public sealed class AudioDriver
    {
        private readonly Transform _emitter;
        private readonly AudioSource _source;
        private readonly AudioSO _audio;

        public AudioDriver(Transform emitter, AudioSource source, AudioSO audio)
        {
            _emitter = emitter;
            _source = source;
            _audio = audio;
        }

        public void Play(PlayerSfxEvent evt)
        {
            if (_audio == null || _source == null) return;
            if (!_audio.TryPickClip(evt, out var clip) || clip == null) return;

            _source.PlayOneShot(clip);
        }

        public bool PlayFootstep(FootstepSurfaceType surface, FootstepActionType action)
        {
            if (_audio == null || _source == null) return false;
            if (!_audio.TryPickFootstep(surface, action, out var clip) || clip == null) return false;

            _source.PlayOneShot(clip);
            return true;
        }
    }
}
