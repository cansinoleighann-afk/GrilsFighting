using UnityEngine;
using Animancer;

namespace AwCon
{
    // Shared weapon lifecycle follows the source IHoldableItem contract.
    public class HitscanWeaponBehaviour : MonoBehaviour, IHoldableItem, IHolsterableItem, IPoolable
    {
        [SerializeField] private Transform _leftHandGoal;
        [SerializeField] private Transform _muzzle;
        private BBBCharacterController _player;
        private ItemInstance _instance;
        private float _nextFireTime;
        private float _reloadEndTime;
        private bool _reloading;
        private bool _holstering;
        private ParticleSystem _flash;
        public float HolsterDuration => _item != null && _item.UnEquipAnim?.Clip != null ? _item.UnEquipAnim.Clip.length / Mathf.Max(0.01f, Mathf.Abs(_item.UnEquipAnim.Speed)) : 0f;
        private ClipTransition _loop;
        private HitscanWeaponSO _item;
        public int Ammo => _instance?.AmmoInMagazine ?? 0;
        public bool IsReloading => _reloading;
        public int ShotsFired { get; private set; }
        public void BeginHolster(float until)
        {
            _reloading = false;
            _holstering = true;
            _reloadEndTime = until;
            _player.RuntimeData.WantsLeftHandIK = false;
            _player.RuntimeData.WantsLookAtIK = false;
            Play(_item.UnEquipAnim, _item.UnEquipAnimPlayOptions);
        }

        public void Initialize(ItemInstance instanceData)
        {
            _instance = instanceData;
            _item = instanceData?.BaseData as HitscanWeaponSO;
            if (_item == null) return;
            if (_instance.AmmoInMagazine < 0) _instance.AmmoInMagazine = _item.MaxAmmo;
            if (_instance.ReserveAmmo < 0) _instance.ReserveAmmo = _item.StartingReserveAmmo;
        }

        public void OnEquipEnter(BBBCharacterController player)
        {
            _player = player;
            if (_item == null) return;
            var config = _item.AnimationProfile;
            if (config == null) return;
            _player.RuntimeData.LeftHandGoal = _leftHandGoal;
            _player.RuntimeData.WantsLeftHandIK = _leftHandGoal != null;
            _player.RuntimeData.CurrentAimReference = _muzzle;
            Play(_item.EquipAnim, _item.EquipAnimPlayOptions);
            _holstering = false;
            _nextFireTime = 0f;
            if (_flash == null && _item.MuzzleVFXPrefab != null && _muzzle != null)
            {
                _flash = Instantiate(_item.MuzzleVFXPrefab, _muzzle);
                _flash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            _reloadEndTime = Time.time + config.EquipEndTime;
            _reloading = false;
            _loop = null;
            _player.RuntimeData.WantsLeftHandIK = false;
        }

        public void OnUpdateLogic()
        {
            if (_player == null || _item == null || _holstering || _player.RuntimeData.CurrentItem != _instance) return;
            if (_player.RuntimeData.IsDead || _player.RuntimeData.Arbitration.BlockInput) { _reloading = false; return; }
            var config = _item.AnimationProfile;
            if (config == null) return;
            if (_reloadEndTime > Time.time) return;
            if (_reloading)
            {
                int count = Mathf.Min(_item.MaxAmmo - Ammo, _instance.ReserveAmmo);
                _instance.AmmoInMagazine += count;
                _instance.ReserveAmmo -= count;
                _reloading = false;
            }
            _player.RuntimeData.WantsLeftHandIK = _leftHandGoal != null;
            _player.RuntimeData.WantsLookAtIK = _player.RuntimeData.IsAiming;
            if (_player.InputPipeline.Current.currentFrameData.Processed.ReloadPressed && Ammo < _item.MaxAmmo && _instance.ReserveAmmo > 0)
            {
                _reloading = true;
                _loop = null;
                _player.RuntimeData.WantsLeftHandIK = false;
                _player.RuntimeData.WantsLookAtIK = false;
                _reloadEndTime = Time.time + config.ReloadDuration;
                Play(_player.RuntimeData.IsCrouching && config.CrouchReload?.Clip != null ? config.CrouchReload : config.Reload, config.UpperBodyOptions);
                return;
            }
            if (_player.RuntimeData.WantsToFire && (!_item.RequireAim || _player.RuntimeData.IsAiming) && Time.time >= _nextFireTime && Ammo > 0 && _muzzle != null)
            {
                _instance.AmmoInMagazine--;
                ShotsFired++;
                _loop = null;
                _nextFireTime = Time.time + Mathf.Max(0.01f, _item.FireRate);
                _reloadEndTime = _nextFireTime;
                Play(_player.RuntimeData.IsCrouching && config.CrouchShoot?.Clip != null ? config.CrouchShoot : config.Shoot, config.UpperBodyOptions);
                FirePellets();
                if (_flash != null) _flash.Play(true);
                if (_item.ShootSound != null) AudioSource.PlayClipAtPoint(_item.ShootSound, _muzzle.position);
                ApplyRecoil();
                _player.InputPipeline.ConsumeFirePressed();
            }
            else
            {
                var next = _player.RuntimeData.IsAiming ? _item.AimAnim : _item.EquipIdleAnim;
                if (_player.RuntimeData.IsCrouching && !_player.RuntimeData.IsAiming && config.CrouchIdle?.Clip != null) next = config.CrouchIdle;
                if (next != _loop)
                {
                    _loop = next;
                    Play(next, _player.RuntimeData.IsAiming ? _item.AnimPlayOptions : _item.EquipIdleAnimOptions);
                }
            }
        }

        private void FirePellets()
        {
            if (_muzzle == null) return;
            Vector3 direction = _muzzle.forward;
            Vector3 aim = _player.RuntimeData.TargetAimPoint - _muzzle.position;
            if (_player.RuntimeData.IsAiming && aim.sqrMagnitude > 0.01f) direction = aim.normalized;
            var basis = Quaternion.LookRotation(direction);
            for (int i = 0; i < _item.PelletCount; i++)
            {
                Vector2 spread = Random.insideUnitCircle * _item.SpreadDegrees;
                Vector3 ray = basis * Quaternion.Euler(spread.y, spread.x, 0f) * Vector3.forward;
                // RaycastAll is bounded by the shot frequency; select nearest non-owner hit.
                var hits = Physics.RaycastAll(_muzzle.position, ray, _item.Range, _item.HitMask, QueryTriggerInteraction.Ignore);
                RaycastHit hit = default;
                float nearest = float.PositiveInfinity;
                foreach (var candidate in hits)
                {
                    if (candidate.transform.IsChildOf(_player.transform) || candidate.distance >= nearest) continue;
                    nearest = candidate.distance;
                    hit = candidate;
                }
                if (float.IsPositiveInfinity(nearest)) continue;
                var target = hit.collider.GetComponentInParent<IDamageable>();
                if (target != null) target.RequestDamage(new DamageRequest(_item.DamagePerPellet));
            }
        }

        private void Play(ClipTransition clip, AnimPlayOptions options)
        {
            if (clip?.Clip != null) _player.AnimFacade.PlayTransition(clip, options);
        }

        private void ApplyRecoil()
        {
            var data = _player.RuntimeData;
            data.ViewPitch = Mathf.Clamp(data.ViewPitch - _item.RecoilPitchAngle - Random.Range(-_item.RecoilPitchRandomRange, _item.RecoilPitchRandomRange), _player.Config.Core.PitchLimits.x, _player.Config.Core.PitchLimits.y);
            data.ViewYaw += (Random.value > 0.5f ? 1f : -1f) * (_item.RecoilYawAngle + Random.Range(-_item.RecoilYawRandomRange, _item.RecoilYawRandomRange));
        }

        public void OnSpawned() { _reloading = false; _holstering = false; _loop = null; _nextFireTime = 0f; ShotsFired = 0; }
        public void OnDespawned() { OnForceUnequip(); _player = null; _instance = null; _item = null; }

        public void OnForceUnequip()
        {
            _reloading = false;
            _holstering = false;
            _loop = null;
            if (_flash != null) _flash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            if (_player == null) return;
            _player.RuntimeData.WantsLeftHandIK = false;
            _player.RuntimeData.LeftHandGoal = null;
            _player.RuntimeData.CurrentAimReference = null;
            _player.RuntimeData.WantsLookAtIK = false;
        }
    }
}
