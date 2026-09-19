using UnityEngine;
using Animancer;

namespace AwCon
{
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    [CreateAssetMenu(fileName = "CoreSO", menuName = "AwCon/Player/Modules/CoreSO")]
    public class CoreSO : ScriptableObject
    {
    // 已修复编码乱码的注释。
        [Tooltip("最大生命值")]
        public float MaxHealth = 100f;

        [Tooltip("死亡动画")]
        public AnimationClip DeathAnim;

    [Header("LOD设置")]

        [Tooltip("LOD检查间隔")]
        public float LODCheckInterval = 0.5f;

        [Tooltip("中等LOD距离")]
        public float MediumLODDistance = 15f;

        [Tooltip("低LOD距离")]
        public float LowLODDistance = 30f;


    [Header("视角设置")]
        
        [Tooltip("视角灵敏度(X, Y)")]
        public Vector2 LookSensitivity = new Vector2(150f, 150f);

        [Tooltip("俯仰角限制(最小, 最大)")]
        public Vector2 PitchLimits = new Vector2(-70f, 70f);

        [Tooltip("旋转平滑时间")]
        public float RotationSmoothTime = 0.12f;

        [Header("移动速度设置")]
        
        [Tooltip("行走速度")]
        public float WalkSpeed = 2f;
        
        [Tooltip("慢跑速度")]
        public float JogSpeed = 4f;
        
        [Tooltip("冲刺速度")]
        public float SprintSpeed = 7f;

        [Header("物理设置")]
        
        [Tooltip("重力加速度")]
        public float Gravity = -20f;
        
        [Tooltip("反弹力")]
        public float ReboundForce = -1f;
        
        [Range(0f, 1f)]
        [Tooltip("空中控制系数")]
        public float AirControl = 0.5f;
        
        [Tooltip("移动速度平滑时间")]
        public float MoveSpeedSmoothTime = 0.15f;

        [Header("动画混合设置")]
        
        [Tooltip("X轴动画混合平滑时间")]
        public float XAnimBlendSmoothTime = 0.2f;
        
        [Tooltip("Y轴动画混合平滑时间")]
        public float YAnimBlendSmoothTime = 0.2f;
        

    [Header("体力设置")]
        
        [Tooltip("最大体力值")]
        public float MaxStamina = 1000f;
        
        [Tooltip("体力消耗速率")]
        public float StaminaDrainRate = 20f;
        
        [Tooltip("体力恢复速率")]
        public float StaminaRegenRate = 15f;
        
        [Range(0.5f, 2.0f)]
        [Tooltip("行走时体力恢复倍率")]
        public float WalkStaminaRegenMult = 1.5f;
        
        [Range(0f, 1f)]
        [Tooltip("体力恢复阈值")]
        public float StaminaRecoverThreshold = 0.2f;
        

    [Header("遮罩设置")]
        
        [Tooltip("上半身动画遮罩")]
        public AvatarMask UpperBodyMask;

        [Header("面部遮罩")]
        
        [Tooltip("面部动画遮罩")]
        public AvatarMask FacialMask;
        
        

    [Header("坠落设置")]
        
        [Range(0, 4)]
        [Tooltip("坠落高度等级阈值")]
        public int FallHeightLevelThreshold = 1;

        [Tooltip("坠落垂直速度阈值")]
        public float FallVerticalVelocityThreshold = -5f;
        

    }
}