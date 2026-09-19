using System;
using System.Collections.Generic;
using UnityEngine;

namespace AwCon
{
    /// <summary>
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    /// </summary>
    [CreateAssetMenu(fileName = "AudioSO", menuName = "AwCon/Player/Modules/AudioSO")]
    public sealed class AudioSO : ScriptableObject
    {
        [Serializable]
        public struct EventEntry
        {
            public PlayerSfxEvent Event;

            [Tooltip("音效剪辑数组")]
            public AudioClip[] Clips;
        }

        [Serializable]
        public struct FootstepEntry
        {
            public FootstepSurfaceType Surface;
            public FootstepActionType Action;

            [Tooltip("Terrain-aware footstep clips for this surface/action pair.")]
            public AudioClip[] Clips;
        }

        [Header("设置")]
        [SerializeField] private List<EventEntry> _entries = new List<EventEntry>();

        [Header("Footstep Surface Audio")]
        [SerializeField] private List<FootstepEntry> _footstepEntries = new List<FootstepEntry>();

        private Dictionary<PlayerSfxEvent, AudioClip[]> _cache;
        private Dictionary<int, AudioClip[]> _footstepCache;

        private void OnEnable() => BuildCache();
        private void OnValidate() => BuildCache();

        private void BuildCache()
        {
            if (_cache == null) _cache = new Dictionary<PlayerSfxEvent, AudioClip[]>();
            else _cache.Clear();

            if (_entries != null)
            {
                for (int i = 0; i < _entries.Count; i++)
                {
                    var e = _entries[i];
                    if (e.Clips == null || e.Clips.Length == 0) continue;

                    // 已修复编码乱码的注释。
                    _cache[e.Event] = e.Clips;
                }
            }

            if (_footstepCache == null) _footstepCache = new Dictionary<int, AudioClip[]>();
            else _footstepCache.Clear();

            if (_footstepEntries == null) return;
            for (int i = 0; i < _footstepEntries.Count; i++)
            {
                var entry = _footstepEntries[i];
                if (entry.Clips == null || entry.Clips.Length == 0) continue;
                _footstepCache[FootstepKey(entry.Surface, entry.Action)] = entry.Clips;
            }
        }

        public bool TryGetClips(PlayerSfxEvent evt, out AudioClip[] clips)
        {
            clips = null;
            if (_cache == null) BuildCache();
            return _cache != null && _cache.TryGetValue(evt, out clips) && clips != null && clips.Length > 0;
        }

        public bool TryPickClip(PlayerSfxEvent evt, out AudioClip clip)
        {
            clip = null;
            if (!TryGetClips(evt, out var clips)) return false;

            int idx = UnityEngine.Random.Range(0, clips.Length);
            clip = clips[idx];
            return clip != null;
        }

        public bool TryPickFootstep(FootstepSurfaceType surface, FootstepActionType action, out AudioClip clip)
        {
            clip = null;
            if (_footstepCache == null) BuildCache();
            if (_footstepCache == null || !_footstepCache.TryGetValue(FootstepKey(surface, action), out var clips) ||
                clips == null || clips.Length == 0)
                return false;

            clip = clips[UnityEngine.Random.Range(0, clips.Length)];
            return clip != null;
        }

        private static int FootstepKey(FootstepSurfaceType surface, FootstepActionType action)
        {
            return ((int)surface * 16) + (int)action;
        }
    }
}
