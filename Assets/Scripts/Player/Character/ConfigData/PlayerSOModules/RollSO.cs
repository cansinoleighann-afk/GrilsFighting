using UnityEngine;

namespace AwCon
{
    // 已修复编码乱码的注释。
    [CreateAssetMenu(fileName = "RollSO", menuName = "AwCon/Player/Modules/RollSO")]
    public class RollSO : ScriptableObject
    {
        
        [Header("动画选项设置")]
        
        [Tooltip("淡入待机动画选项")]
        public AnimPlayOptions FadeInIdleOptions;
        
        [Tooltip("淡入移动循环动画选项")]
        public AnimPlayOptions FadeInMoveLoopOptions;

        [Header("翻滚动画数据")]
        public WarpedMotionData ForwardRoll;
        public WarpedMotionData BackwardRoll;
        public WarpedMotionData LeftRoll;
        public WarpedMotionData RightRoll;
        public WarpedMotionData ForwardLeftRoll;
        public WarpedMotionData ForwardRightRoll;
        public WarpedMotionData BackwardLeftRoll;
        public WarpedMotionData BackwardRightRoll;
    }
}