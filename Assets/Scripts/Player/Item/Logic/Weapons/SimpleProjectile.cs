using System.Collections.Generic;
using UnityEngine;

namespace AwCon
{
    /// <summary>
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    /// </summary>
    public sealed class SimpleProjectile : MonoBehaviour, IPoolable
    {
        [Header("设置")]
        [Min(0f)] public float lifeTime = 5f;

        [Header("碰撞设置")]
        [Tooltip("碰撞检测层级")]
        public LayerMask HitLayers = ~0;

        [Tooltip("命中特效预制体")]
        public GameObject hitVFXPrefab;

        [Tooltip("命中音效")]
        public AudioClip hitSound;

        [Header("伤害设置")]
        [Tooltip("伤害范围半径")]
        [Min(0f)]
        public float DamageRadius = 0f;

        [Tooltip("伤害数值")]
        [Min(0f)]
        public float DamageAmount = 10f;

        [Tooltip("伤害检测层级")]
        public LayerMask DamageLayers = ~0;

        [Tooltip("伤害查询触发器交互模式")]
        public QueryTriggerInteraction DamageQueryTrigger = QueryTriggerInteraction.Collide;

        [Header("销毁设置")]
        [Tooltip("命中后是否销毁")]
        public bool DespawnOnHit = true;

        [Header("设置")]
        public bool debug;

        private float _despawnAt;
        private bool _hitProcessed;

        private readonly HashSet<int> _damagedTargetIds = new HashSet<int>();

        // 已修复编码乱码的注释。
        // 已修复编码乱码的注释。
        private Collider[] _overlapBuffer = new Collider[32];

        // 已修复编码乱码的注释。
        // 已修复编码乱码的注释。
        private Collider[] _cachedColliders;

        private Rigidbody _cachedRb;

        private bool UsePool => SimpleObjectPoolSystem.Shared != null;

        #region Pool
        public void OnSpawned()
        {
            _hitProcessed = false;
            _despawnAt = lifeTime > 0f ? (Time.time + lifeTime) : 0f;

            _damagedTargetIds.Clear();

            CacheComponents();

            // 已修复编码乱码的注释。
            if (_cachedColliders != null)
            {
                for (int i = 0; i < _cachedColliders.Length; i++)
                {
                    if (_cachedColliders[i] != null) _cachedColliders[i].enabled = true;
                }
            }

            // 已修复编码乱码的注释。
            if (_cachedRb != null)
            {
                _cachedRb.velocity = Vector3.zero;
                _cachedRb.angularVelocity = Vector3.zero;
            }
        }

        public void OnDespawned()
        {
            _hitProcessed = false;
            _despawnAt = 0f;

            _damagedTargetIds.Clear();

            CacheComponents();

            if (_cachedRb != null)
            {
                _cachedRb.velocity = Vector3.zero;
                _cachedRb.angularVelocity = Vector3.zero;
            }
        }
        #endregion

        private void CacheComponents()
        {
            if (_cachedRb == null) _cachedRb = GetComponent<Rigidbody>();

            // 已修复编码乱码的注释。
            // 已修复编码乱码的注释。
            if (_cachedColliders == null || _cachedColliders.Length == 0)
            {
                _cachedColliders = GetComponentsInChildren<Collider>(true);
#if UNITY_EDITOR
                // Debug.Log("[GC-RISK FIXED] SimpleProjectile cached Collider[] to avoid per-spawn GetComponentsInChildren allocations.", this);
#endif
            }
        }

        private void OnEnable()
        {
            // 已修复编码乱码的注释。
            if (_despawnAt <= 0f)
                _despawnAt = lifeTime > 0f ? (Time.time + lifeTime) : 0f;
        }

        private void Update()
        {
            if (_despawnAt > 0f && Time.time >= _despawnAt)
            {
                DespawnSelf();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            HandleHit(collision.collider, collision.GetContact(0).point);
        }

        private void OnTriggerEnter(Collider other)
        {
            HandleHit(other, other.ClosestPoint(transform.position));
        }

        private void HandleHit(Collider other, Vector3 hitPoint)
        {
            if (other == null) return;
            if (_hitProcessed) return;

            // 已修复编码乱码的注释。
            if (((1 << other.gameObject.layer) & HitLayers.value) == 0)
                return;

            _hitProcessed = true;

            if (debug)
                Debug.Log($"[SimpleProjectile] Hit '{other.name}' layer={other.gameObject.layer} point={hitPoint}", other);

            SpawnHitVFX(hitPoint, other);
            ApplyAreaDamage(hitPoint);

            if (DespawnOnHit)
                DespawnSelf();
        }

        private void SpawnHitVFX(Vector3 hitPoint, Collider hitCollider)
        {
            if (hitVFXPrefab != null)
            {
                var rot = Quaternion.LookRotation(-transform.forward, Vector3.up);

                GameObject vfx;
                if (SimpleObjectPoolSystem.Shared != null)
                {
                    vfx = SimpleObjectPoolSystem.Shared.Spawn(hitVFXPrefab);
                    vfx.transform.SetPositionAndRotation(hitPoint, rot);
                }
                else
                {
                    vfx = Instantiate(hitVFXPrefab, hitPoint, rot);
                }
            }

            if (hitSound != null)
                AudioSource.PlayClipAtPoint(hitSound, hitPoint);
        }

        private void ApplyAreaDamage(Vector3 center)
        {
            if (DamageRadius <= 0f || DamageAmount <= 0f) return;

            // 已修复编码乱码的注释。
            var layers = DamageLayers.value != 0 ? DamageLayers : HitLayers;

            int count = Physics.OverlapSphereNonAlloc(center, DamageRadius, _overlapBuffer, layers, DamageQueryTrigger);

            // 已修复编码乱码的注释。
            if (count == _overlapBuffer.Length)
            {
                int newSize = Mathf.Min(_overlapBuffer.Length * 2, 2048);
                if (newSize > _overlapBuffer.Length)
                {
#if UNITY_EDITOR
                    // Debug.Log($"[GC-RISK NOTE] SimpleProjectile overlap buffer resized {_overlapBuffer.Length} -> {newSize} (one-time alloc).", this);
#endif
                    _overlapBuffer = new Collider[newSize];
                    count = Physics.OverlapSphereNonAlloc(center, DamageRadius, _overlapBuffer, layers, DamageQueryTrigger);
                }
            }

            if (count <= 0) return;

            _damagedTargetIds.Clear();

            var hitCount = 0;
            var req = new DamageRequest(DamageAmount);

            for (int i = 0; i < count; i++)
            {
                var c = _overlapBuffer[i];
                if (c == null) continue;

                IDamageable d = FindDamageable(c);
                if (d == null) continue;

                int id = (d as Component) != null ? ((Component)d).GetInstanceID() : d.GetHashCode();
                if (!_damagedTargetIds.Add(id))
                    continue;

                d.RequestDamage(in req);
                hitCount++;

                if (debug)
                    Debug.Log($"[SimpleProjectile] Damage -> {d.GetType().Name} ({id})", (d as Component));
            }

            if (debug)
                Debug.Log($"[SimpleProjectile] AreaDamage radius={DamageRadius} amount={DamageAmount} targets={hitCount}");
        }

        private static IDamageable FindDamageable(Collider col)
        {
            if (col == null) return null;

            // 已修复编码乱码的注释。
            var d = col.GetComponentInParent<IDamageable>();
            if (d != null) return d;

            // 已修复编码乱码的注释。
            var rb = col.attachedRigidbody;
            if (rb != null)
            {
                d = rb.GetComponentInParent<IDamageable>();
                if (d != null) return d;
            }

            // 已修复编码乱码的注释。
            var root = col.transform.root;
            return root != null ? root.GetComponent<IDamageable>() : null;
        }

        private void DespawnSelf()
        {
            if (UsePool)
                SimpleObjectPoolSystem.Shared.Despawn(gameObject);
            else
                Destroy(gameObject);
        }
    }
}