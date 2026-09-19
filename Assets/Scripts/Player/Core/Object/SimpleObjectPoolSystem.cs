using System;
using System.Collections.Generic;
using UnityEngine;

namespace AwCon
{
    /// <summary>
    // 已修复编码乱码的注释。
    /// </summary>
    public sealed class SimpleObjectPoolSystem : MonoBehaviour
    {
        [Serializable]
        public struct PrewarmEntry
        {
            public GameObject Prefab;
            [Min(0)] public int Count;
        }

        public static SimpleObjectPoolSystem Shared { get; private set; }

        [Header("设置")]
        [SerializeField] private List<PrewarmEntry> _prewarm = new List<PrewarmEntry>();

        // 已修复编码乱码的注释。
        private readonly Dictionary<GameObject, Queue<GameObject>> _pool = new Dictionary<GameObject, Queue<GameObject>>(16);
        private readonly Dictionary<int, GameObject> _instanceToPrefab = new Dictionary<int, GameObject>(256);

        // 已修复编码乱码的注释。
        // 已修复编码乱码的注释。
        private readonly Dictionary<int, IPoolable[]> _instancePoolablesCache = new Dictionary<int, IPoolable[]>(256);

        private void Awake()
        {
            if (Shared != null && Shared != this)
            {
                Destroy(gameObject);
                return;
            }

            Shared = this;
            PrewarmAll();
        }

        private void PrewarmAll()
        {
            if (_prewarm == null) return;
            for (int i = 0; i < _prewarm.Count; i++)
            {
                var e = _prewarm[i];
                if (e.Prefab == null || e.Count <= 0) continue;
                Prewarm(e.Prefab, e.Count);
            }
        }

        public void Prewarm(GameObject prefab, int count)
        {
            if (prefab == null || count <= 0) return;

            var q = GetOrCreateQueue(prefab);
            for (int i = 0; i < count; i++)
            {
                var inst = CreateInstance(prefab);
                InternalDespawn(inst, callCallbacks: true);
                q.Enqueue(inst);
            }
        }

        /// <summary>
        // 已修复编码乱码的注释。
        // 已修复编码乱码的注释。
        /// </summary>
        public GameObject Spawn(GameObject prefab)
        {
            if (prefab == null) return null;

            var q = GetOrCreateQueue(prefab);

            GameObject inst = null;
            while (q.Count > 0 && inst == null)
                inst = q.Dequeue();

            if (inst == null)
                inst = CreateInstance(prefab);

            InternalSpawn(inst, callCallbacks: true);
            return inst;
        }

        /// <summary>
        // 已修复编码乱码的注释。
        // 已修复编码乱码的注释。
        /// </summary>
        public bool TryDespawn(GameObject instance)
        {
            if (instance == null) return true;

            if (!_instanceToPrefab.TryGetValue(instance.GetInstanceID(), out var prefab) || prefab == null)
                return false;

            var q = GetOrCreateQueue(prefab);
            InternalDespawn(instance, callCallbacks: true);
            q.Enqueue(instance);
            return true;
        }

        /// <summary>
        // 已修复编码乱码的注释。
        /// </summary>
        public void Despawn(GameObject instance)
        {
            if (instance == null) return;

            if (!_instanceToPrefab.TryGetValue(instance.GetInstanceID(), out var prefab) || prefab == null)
            {
                Debug.LogWarning($"[SimpleObjectPoolSystem] Despawn called for non-pooled instance: {instance.name}", instance);

                return;
            }

            var q = GetOrCreateQueue(prefab);
            InternalDespawn(instance, callCallbacks: true);
            q.Enqueue(instance);
        }

        private Queue<GameObject> GetOrCreateQueue(GameObject prefab)
        {
            if (!_pool.TryGetValue(prefab, out var q) || q == null)
            {
                // 已修复编码乱码的注释。
                // 已修复编码乱码的注释。
                int initialCapacity = 0;
                if (_prewarm != null)
                {
                    for (int i = 0; i < _prewarm.Count; i++)
                    {
                        if (_prewarm[i].Prefab == prefab)
                        {
                            initialCapacity = Mathf.Max(0, _prewarm[i].Count);
                            break;
                        }
                    }
                }

                q = initialCapacity > 0 ? new Queue<GameObject>(initialCapacity) : new Queue<GameObject>();
                _pool[prefab] = q;
            }
            return q;
        }

        private GameObject CreateInstance(GameObject prefab)
        {
            var inst = Instantiate(prefab);
            int id = inst.GetInstanceID();
            _instanceToPrefab[id] = prefab;

            // 已修复编码乱码的注释。
            CachePoolables(inst);

            return inst;
        }

        private void CachePoolables(GameObject instance)
        {
            if (instance == null) return;

            int id = instance.GetInstanceID();
            if (_instancePoolablesCache.ContainsKey(id)) return;

            // 已修复编码乱码的注释。
            // 已修复编码乱码的注释。
            var poolables = instance.GetComponentsInChildren<IPoolable>(true);
            _instancePoolablesCache[id] = poolables;

        }

        private void InternalSpawn(GameObject instance, bool callCallbacks)
        {
            if (instance == null) return;

            // 已修复编码乱码的注释。
            instance.SetActive(true);

            if (callCallbacks)
            {
                int id = instance.GetInstanceID();
                if (!_instancePoolablesCache.TryGetValue(id, out var poolables) || poolables == null)
                {
                    CachePoolables(instance);
                    _instancePoolablesCache.TryGetValue(id, out poolables);
                }

                if (poolables != null)
                {
                    for (int i = 0; i < poolables.Length; i++)
                    {
                        try { poolables[i]?.OnSpawned(); } catch { }
                    }
                }
            }
        }

        private void InternalDespawn(GameObject instance, bool callCallbacks)
        {
            if (instance == null) return;

            // 已修复编码乱码的注释。
            if (callCallbacks)
            {
                int id = instance.GetInstanceID();
                if (!_instancePoolablesCache.TryGetValue(id, out var poolables) || poolables == null)
                {
                    CachePoolables(instance);
                    _instancePoolablesCache.TryGetValue(id, out poolables);
                }

                if (poolables != null)
                {
                    for (int i = 0; i < poolables.Length; i++)
                    {
                        try { poolables[i]?.OnDespawned(); } catch { }
                    }
                }
            }

            instance.SetActive(false);
        }
    }
}
