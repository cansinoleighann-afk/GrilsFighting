using UnityEngine;

namespace AwCon
{
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    [CreateAssetMenu(fileName = "DodgingSO", menuName = "AwCon/Player/Modules/DodgingSO")]
    public class DodgingSO : ScriptableObject
    {
        [Header("动画选项设置")]
        
        [Tooltip("淡入待机动画选项")]
        public AnimPlayOptions FadeInIdleOptions;
        
        [Tooltip("淡入移动循环动画选项")]
        public AnimPlayOptions FadeInMoveLoopOptions;

        [Header("闪避动画数据")]
        
        public WarpedMotionData ForwardDodge;
        public WarpedMotionData BackwardDodge;
        public WarpedMotionData LeftDodge;
        public WarpedMotionData RightDodge;
        public WarpedMotionData ForwardLeftDodge;
        public WarpedMotionData ForwardRightDodge;
        public WarpedMotionData BackwardLeftDodge;
        public WarpedMotionData BackwardRightDodge;
    }
}