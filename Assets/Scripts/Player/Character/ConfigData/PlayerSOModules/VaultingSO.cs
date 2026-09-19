using UnityEngine;

namespace AwCon
{
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    [CreateAssetMenu(fileName = "VaultingSO", menuName = "AwCon/Player/Modules/VaultingSO")]
    public class VaultingSO : ScriptableObject
    {
        [Header("检测设置")]
        
        [Tooltip("障碍物检测层级")]
        public LayerMask ObstacleLayers;

        [Tooltip("前方射线长度")]
        public float VaultForwardRayLength = 1.5f;

        [Tooltip("前方射线高度")]
        public float VaultForwardRayHeight = 1.0f;

        [Tooltip("向下射线偏移")]
        public float VaultDownwardRayOffset = 0.5f;

        [Tooltip("向下射线长度")]
        public float VaultDownwardRayLength = 2.0f;

        [Space]
        [Tooltip("翻越时双手间距")]
        public float VaultHandSpread = 0.4f;

        [Tooltip("落地检测距离")]
        public float VaultLandDistance = 1.5f;

        [Tooltip("落地射线长度")]
        public float VaultLandRayLength = 3.0f;

        [Tooltip("是否需要墙后地面")]
        public bool RequireGroundBehindWall = true;

        [Header("高度设置")]
        
        [Tooltip("低翻越最小高度")]
        public float LowVaultMinHeight = 0.5f;
        
        [Tooltip("低翻越最大高度")]
        public float LowVaultMaxHeight = 1.2f;

        [Space]
        [Tooltip("高翻越最小高度")]
        public float HighVaultMinHeight = 1.2f;
        
        [Tooltip("高翻越最大高度")]
        public float HighVaultMaxHeight = 2.5f;

        [Header("动画选项设置")]
        
        [Tooltip("翻越到待机动画选项")]
        public AnimPlayOptions VaultToIdleOptions = AnimPlayOptions.Default;
        
        [Tooltip("翻越到移动动画选项")]
        public AnimPlayOptions VaultToMoveOptions = AnimPlayOptions.Default;

        [Tooltip("低翻越动画")]
        public WarpedMotionData lowVaultAnim;

        [Tooltip("高翻越动画")]
        public WarpedMotionData highVaultAnim;

        [Header("IK偏移设置")]
        [Tooltip("左手IK偏移")]
        public Vector3 LeftHandIKOffset = Vector3.zero;

        [Tooltip("右手IK偏移")]
        public Vector3 RightHandIKOffset = Vector3.zero;

        [Tooltip("手部旋转偏移(欧拉角)")]
        public Vector3 HandRotationOffsetEuler = Vector3.zero;
    }
}