using Animancer;
using UnityEngine;

namespace AwCon
{
    [CreateAssetMenu(fileName = "WeaponAnimationProfile", menuName = "AwCon/Items/Weapon Animation Profile")]
    public class WeaponAnimationProfileSO : ScriptableObject
    {
        [Header("Animancer 动画")]
        public ClipTransition TakeGun;
        public ClipTransition PutGun;
        public ClipTransition Idle;
        public ClipTransition AimIdle;
        public ClipTransition Shoot;
        public ClipTransition Reload;
        public ClipTransition CrouchShoot;
        public ClipTransition CrouchReload;
        public ClipTransition Walk;
        public ClipTransition Run;
        public ClipTransition CrouchIdle;
        public ClipTransition CrouchWalk;
        public ClipTransition CrouchRun;

        [Header("播放选项")]
        public AnimPlayOptions UpperBodyOptions = AnimPlayOptions.UpperBodyDefault;
        public float EquipEndTime = 0.65f;
        public float ReloadDuration = 1.35f;
        public float FireInterval = 0.65f;
    }
}
