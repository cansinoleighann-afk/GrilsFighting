using UnityEngine;

namespace AwCon
{
    // 已修复编码乱码的注释。
    public class CannonBehaviour : MonoBehaviour, IHoldableItem, IPoolable
    {
        [Header("IK设置")]
        [Tooltip("左手目标位置")]
        [SerializeField] private Transform _leftHandGoal;
        [Tooltip("枪口闪光特效")]
        [SerializeField] private ParticleSystem _muzzleFlash;
        [Tooltip("枪口位置")]
        [SerializeField] private Transform _muzzle;
        // 已修复编码乱码的注释。
        private BBBCharacterController _player;
        // 已修复编码乱码的注释。
        private ItemInstance _instance;
        // 已修复编码乱码的注释。
        private CannonSO _cannonConfig;
        // 已修复编码乱码的注释。
        private float _fireRate = 0.1f;
        // 已修复编码乱码的注释。
        private bool _isEquipping;
        private float _equipEndTime;
        // 已修复编码乱码的注释。
        private float _lastFireTime;
        // 已修复编码乱码的注释。
        private bool _wasAiming;
        // 已修复编码乱码的注释。
        private bool _ikEnableScheduled;
        private float _ikEnableTimePoint;
        private bool _ikDisableScheduled;
        private float _ikDisableTimePoint;

        // 已修复编码乱码的注释。
        public void Initialize(ItemInstance instanceData)
        {
            _instance = instanceData;
            _cannonConfig = _instance.BaseData as CannonSO;
            if (_cannonConfig != null)
            {
                float interval = _cannonConfig.ShootInterval > 0f ? _cannonConfig.ShootInterval : _cannonConfig.FireRate;
                _fireRate = Mathf.Max(0.001f, interval);
            }
        }

        // 已修复编码乱码的注释。
        public void OnEquipEnter(BBBCharacterController player)
        {
            _player = player;
            _isEquipping = true;
            if (_leftHandGoal != null && _player != null && _player.RuntimeData != null)
            {
                _player.RuntimeData.LeftHandGoal = _leftHandGoal;
                _player.RuntimeData.WantsLeftHandIK = false;
                if (_cannonConfig != null)
                {
                    _ikEnableScheduled = true;
                    _ikEnableTimePoint = Time.time + _cannonConfig.EnableIKTime;
                }
                else
                {
                    _player.RuntimeData.WantsLeftHandIK = true;
                }
            }
            
            // 已修复编码乱码的注释。
            if (_muzzle != null && _player != null && _player.RuntimeData != null)
            {
                _player.RuntimeData.CurrentAimReference = _muzzle;
            }
            
            float equipAnimDuration = _cannonConfig != null ? _cannonConfig.EquipEndTime : 0.5f;
            _equipEndTime = Time.time + equipAnimDuration;
            if (_cannonConfig != null && _cannonConfig.EquipAnim != null && _player != null)
            {
                _player.AnimFacade.PlayTransition(_cannonConfig.EquipAnim, _cannonConfig.EquipAnimPlayOptions);
            }
        }

        // 已修复编码乱码的注释。
        public void OnUpdateLogic()
        {
            if (_ikEnableScheduled && Time.time >= _ikEnableTimePoint)
            {
                if (_isEquipping)
                {
                    _ikEnableTimePoint = _equipEndTime + 0.001f;
                }
                else
                {
                    _ikEnableScheduled = false;
                    if (_player != null && _player.RuntimeData != null && _player.RuntimeData.CurrentItem == _instance)
                    {
                        _player.RuntimeData.WantsLeftHandIK = true;
                    }
                }
            }
            if (_ikDisableScheduled && Time.time >= _ikDisableTimePoint)
            {
                _ikDisableScheduled = false;
                if (_player != null && _player.RuntimeData != null)
                {
                    var current = _player.RuntimeData.CurrentItem;
                    if (current == null || current.InstanceID == _instance.InstanceID)
                    {
                        _player.RuntimeData.WantsLeftHandIK = false;
                        _player.RuntimeData.LeftHandGoal = null;
                    }
                }
            }
            if (_isEquipping)
            {
                if (Time.time >= _equipEndTime)
                {
                    _isEquipping = false;
                    if (_cannonConfig != null && _cannonConfig.EquipIdleAnim != null && _player != null)
                    {
                        _player.AnimFacade.PlayTransition(_cannonConfig.EquipIdleAnim, _cannonConfig.EquipIdleAnimOptions);
                    }
                }
                else
                {
                    return;
                }
            }
            bool isAiming = _player != null && _player.RuntimeData != null && _player.RuntimeData.IsAiming;
            if (!_isEquipping && _wasAiming != isAiming)
            {
                if (isAiming)
                {
                    if (_cannonConfig != null && _cannonConfig.AimAnim != null && _player != null)
                    {
                        _player.AnimFacade.PlayTransition(_cannonConfig.AimAnim, _cannonConfig.AnimPlayOptions);
                    }
                    if (_player != null && _player.RuntimeData != null)
                    {
                        // 已修复编码乱码的注释。
                        _player.RuntimeData.WantsLookAtIK = true;
                    }
                }
                else
                {
                    if (_cannonConfig != null && _cannonConfig.EquipIdleAnim != null && _player != null)
                    {
                        _player.AnimFacade.PlayTransition(_cannonConfig.EquipIdleAnim, _cannonConfig.EquipIdleAnimOptions);
                    }
                    if (_player != null && _player.RuntimeData != null)
                    {
                        // 已修复编码乱码的注释。
                        _player.RuntimeData.WantsLookAtIK = false;
                    }
                }
                _wasAiming = isAiming;
            }
            bool isFiring = _player != null && _player.RuntimeData != null && 
                           _player.RuntimeData.CurrentItem == _instance && 
                           _player.RuntimeData.WantsToFire;
            if (isAiming && isFiring)
            {
                TryFire();
            }
        }

        // 已修复编码乱码的注释。
        private void ClearPlayerIKIfOwned()
        {
            if (_player == null || _player.RuntimeData == null) return;

            _player.RuntimeData.LeftHandGoal = null;
            _player.RuntimeData.WantsLeftHandIK = false;
            _player.RuntimeData.CurrentAimReference = null;
            _player.RuntimeData.WantsLookAtIK = false;
        }

        // 已修复编码乱码的注释。
        public void OnForceUnequip()
        {
            _isEquipping = false;
            if (_muzzleFlash != null) _muzzleFlash.Stop();

            // 已修复编码乱码的注释。
            // 已修复编码乱码的注释。

            if (_cannonConfig != null)
            {
                _ikDisableScheduled = true;
                _ikDisableTimePoint = Time.time + _cannonConfig.DisableIKTime;
            }

            if (_player != null && _player.RuntimeData != null && _player.RuntimeData.CurrentItem == null)
            {
                if (_cannonConfig != null && _cannonConfig.UnEquipAnim != null)
                {
                    _player.AnimFacade.PlayTransition(_cannonConfig.UnEquipAnim, _cannonConfig.UnEquipAnimPlayOptions);
                }
            }
        }

        public void OnSpawned()
        {
            _isEquipping = false;
            _wasAiming = false;
            _ikEnableScheduled = false;
            _ikDisableScheduled = false;
            _lastFireTime = 0f;

            if (_muzzleFlash != null) _muzzleFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        public void OnDespawned()
        {
            if (_muzzleFlash != null) _muzzleFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        // 已修复编码乱码的注释。
        private void TryFire()
        {
            if (Time.time - _lastFireTime < _fireRate) return;
            _lastFireTime = Time.time;
            if (_muzzleFlash != null) _muzzleFlash.Play();
            if (_cannonConfig != null && _cannonConfig.ShootSound != null && _muzzle != null)
            {
                AudioSource.PlayClipAtPoint(_cannonConfig.ShootSound, _muzzle.position);
            }

            if (_cannonConfig != null && _cannonConfig.MuzzleVFXPrefab != null && _muzzle != null)
            {
                GameObject muzzleVFX;
                if (AwCon.SimpleObjectPoolSystem.Shared != null)
                {
                    muzzleVFX = AwCon.SimpleObjectPoolSystem.Shared.Spawn(_cannonConfig.MuzzleVFXPrefab);
                    muzzleVFX.transform.SetPositionAndRotation(_muzzle.position, _muzzle.rotation);
                    muzzleVFX.transform.SetParent(_muzzle, true);
                }
                else
                {
                    muzzleVFX = Object.Instantiate(_cannonConfig.MuzzleVFXPrefab, _muzzle.position, _muzzle.rotation);
                    muzzleVFX.transform.parent = _muzzle;
                }
            }

            ApplyRecoil();

            if (_cannonConfig != null && _cannonConfig.ProjectilePrefab != null && _muzzle != null)
            {
                GameObject proj;
                if (AwCon.SimpleObjectPoolSystem.Shared != null)
                {
                    proj = AwCon.SimpleObjectPoolSystem.Shared.Spawn(_cannonConfig.ProjectilePrefab);
                    proj.transform.SetPositionAndRotation(_muzzle.position, _muzzle.rotation);
                    proj.transform.SetParent(null, true);
                }
                else
                {
                    proj = Object.Instantiate(_cannonConfig.ProjectilePrefab, _muzzle.position, _muzzle.rotation);
                    proj.transform.parent = null;
                }

                var rb = proj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = _muzzle.forward * _cannonConfig.ProjectileSpeed;
                }

                var simple = proj.GetComponent<SimpleProjectile>();
                if (simple != null)
                {
                    simple.hitSound = _cannonConfig.ProjectileHitSound;
                }
            }
        }

        // 已修复编码乱码的注释。
        private void ApplyRecoil()
        {
            if (_player == null || _player.RuntimeData == null || _cannonConfig == null) return;
            float pitchNoise = Random.Range(-_cannonConfig.RecoilPitchRandomRange, _cannonConfig.RecoilPitchRandomRange);
            float yawNoise = Random.Range(-_cannonConfig.RecoilYawRandomRange, _cannonConfig.RecoilYawRandomRange);
            float finalPitch = _cannonConfig.RecoilPitchAngle + pitchNoise;
            float finalYaw = _cannonConfig.RecoilYawAngle + yawNoise;
            float yawSign = Random.value > 0.5f ? 1f : -1f;
            _player.RuntimeData.ViewPitch -= finalPitch;
            _player.RuntimeData.ViewYaw += yawSign * finalYaw;
            _player.RuntimeData.ViewPitch = Mathf.Clamp(
                _player.RuntimeData.ViewPitch,
                _player.Config.Core.PitchLimits.x,
                _player.Config.Core.PitchLimits.y
            );
        }
    }
}