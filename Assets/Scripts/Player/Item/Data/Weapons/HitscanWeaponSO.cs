using UnityEngine;

namespace AwCon
{
    [CreateAssetMenu(fileName = "HitscanWeapon", menuName = "AwCon/Items/Weapons/Hitscan")]
    public class HitscanWeaponSO : RangedWeaponSO
    {
        [Min(0)] public int StartingReserveAmmo = 40;
        [Min(1)] public int PelletCount = 8;
        [Min(0)] public float SpreadDegrees = 3f;
        [Min(1)] public float Range = 60f;
        [Min(0)] public float DamagePerPellet = 8f;
        public LayerMask HitMask = ~0;
        public bool RequireAim = true;
        public AudioClip ShootSound;
        public ParticleSystem MuzzleVFXPrefab;
        [Min(0)] public float RecoilPitchAngle = 1f;
        [Min(0)] public float RecoilYawAngle = 0.4f;
        [Min(0)] public float RecoilPitchRandomRange = 0.5f;
        [Min(0)] public float RecoilYawRandomRange = 0.5f;
    }
}
