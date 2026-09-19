using Animancer;
using UnityEngine;

namespace AwCon
{
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    [CreateAssetMenu(fileName = "AimingSO", menuName = "AwCon/Player/Modules/AimingSO")]
    public class AimingSO : ScriptableObject
    {
        [Header("灵敏度设置")]
        
        [Tooltip("瞄准灵敏度")]
        public float AimSensitivity = 1f;
        
        [Header("移动速度设置")]
        
        [Tooltip("瞄准时行走速度")]
        public float AimWalkSpeed = 1.5f;
        
        [Tooltip("瞄准时慢跑速度")]
        public float AimJogSpeed = 2.5f;
        
        [Tooltip("瞄准时冲刺速度")]
        public float AimSprintSpeed = 5.0f;
        
        [Header("平滑时间设置")]
        
        [Tooltip("瞄准旋转平滑时间")]
        public float AimRotationSmoothTime = 0.05f;

        [Tooltip("瞄准X轴动画混合平滑时间")]
        public float AimXAnimBlendSmoothTime = 0.2f;
        
        [Tooltip("瞄准Y轴动画混合平滑时间")]
        public float AimYAnimBlendSmoothTime = 0.2f;
        
        [Tooltip("瞄准IK追踪平滑时间")]
        public float AimIkChaseSmoothTime = 0.1f;

        [Header("动画混合器设置")]

        [Tooltip("瞄准行走动画混合器")]
        public MixerTransition2D AimWalkMixer;

        [Tooltip("瞄准慢跑动画混合器")]
        public MixerTransition2D AimJogMixer;

        [Tooltip("瞄准冲刺动画混合器")]
        public MixerTransition2D AimSprintMixer;
    }
}