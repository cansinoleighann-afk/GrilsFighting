using Animancer;
using UnityEngine;

namespace AwCon
{
    [CreateAssetMenu(fileName = "LocomotionSO", menuName = "AwCon/Player/Modules/LocomotionSO")]
    public class LocomotionSO : ScriptableObject
    {
        [Header("移动状态切换过渡")]
        
        [Tooltip("淡入行走开始动画选项")]
        public AnimPlayOptions FadeInWalkStartOptions = AnimPlayOptions.Default;
        
        [Tooltip("淡入跑步开始动画选项")]
        public AnimPlayOptions FadeInRunStartOptions = AnimPlayOptions.Default;
        
        [Tooltip("淡入冲刺开始动画选项")]
        public AnimPlayOptions FadeInSprintStartOptions = AnimPlayOptions.Default;
        
        [Tooltip("淡入循环打断动画选项")]
        public AnimPlayOptions FadeInLoopBreakInOptions;
        
        [Space]
        [Tooltip("淡入行走循环动画选项")]
        public AnimPlayOptions FadeInWalkLoopOptions = AnimPlayOptions.Default;
        
        [Tooltip("淡入跑步循环动画选项")]
        public AnimPlayOptions FadeInRunLoopOptions = AnimPlayOptions.Default;
        
        [Tooltip("淡入冲刺循环动画选项")]
        public AnimPlayOptions FadeInSprintLoopOptions = AnimPlayOptions.Default;
        
        [Space]
        [Tooltip("淡入停止行走动画选项")]
        public AnimPlayOptions FadeInStopWalkOptions = AnimPlayOptions.Default;
        
        [Tooltip("淡入停止跑步动画选项")]
        public AnimPlayOptions FadeInStopRunOptions = AnimPlayOptions.Default;
        
        [Tooltip("淡入停止冲刺动画选项")]
        public AnimPlayOptions FadeInStopSprintOptions = AnimPlayOptions.Default;
        
        [Space]
        [Header("动作状态切换过渡")]
        
        [Tooltip("淡入跳跃动画选项")]
        public AnimPlayOptions FadeInJumpOptions = AnimPlayOptions.Default;
        
        [Tooltip("淡入坠落动画选项")]
        public AnimPlayOptions FadeInFallOptions = AnimPlayOptions.Default;
        
        [Tooltip("淡入翻越动画选项")]
        public AnimPlayOptions FadeInVaultOptions = AnimPlayOptions.Default;
        
        [Tooltip("淡入快速闪避选项")]
        public AnimPlayOptions FadeInQuickDodgeOptions = AnimPlayOptions.Default;
        
        [Tooltip("淡入移动闪避动画选项")]
        public AnimPlayOptions FadeInMoveDodgeOptions = AnimPlayOptions.Default;
        

        #region  空中状态设置

        [Header("空中状态判定")]
        
        [Tooltip("进入坠落状态的滞空时间阈值")]
        public float AirborneTimeThresholdForFall = 0.3f;
        

        #region Locomotion Animations 动画设置 - 待机 坠落 循环动画

        [Header("待机动画")]
        
        [Tooltip("待机动画")]
        public ClipTransition IdleAnim;
        
        [Header("坠落动画")]
        [Tooltip("坠落动画 ")]
        public ClipTransition FallAnim;

        [Header("前向循环移动动画")]
       
        public ClipTransition WalkLoopFwd_L;
        public ClipTransition WalkLoopFwd_R;
        
        [Space]
        public ClipTransition JogLoopFwd_L;
        public ClipTransition JogLoopFwd_R;
        
        [Space]
        public ClipTransition SprintLoopFwd_L;
        public ClipTransition SprintLoopFwd_R;

        [Header("下蹲八向移动动画")]
        [Tooltip("普通待机切换到蹲下待机时播放的一次性过渡动画。")]
        public ClipTransition IdleToCrouch;
        [Tooltip("角色保持下蹲且没有移动输入时循环播放的待机动画。")]
        public ClipTransition CrouchIdle;
        [Tooltip("蹲下待机切换回普通待机时播放的一次性过渡动画。")]
        public ClipTransition CrouchToIdle;

        [Header("下蹲八向移动循环")]
        [Tooltip("普通下蹲移动使用的八方向循环动画。")]
        public ClipTransition CrouchWalkFwd;
        public ClipTransition CrouchWalkBack;
        public ClipTransition CrouchWalkLeft;
        public ClipTransition CrouchWalkRight;
        public ClipTransition CrouchWalkFwdLeft;
        public ClipTransition CrouchWalkFwdRight;
        public ClipTransition CrouchWalkBackLeft;
        public ClipTransition CrouchWalkBackRight;
        public ClipTransition CrouchJogFwd;
        public ClipTransition CrouchJogBack;
        public ClipTransition CrouchJogLeft;
        public ClipTransition CrouchJogRight;
        public ClipTransition CrouchJogFwdLeft;
        public ClipTransition CrouchJogFwdRight;
        public ClipTransition CrouchJogBackLeft;
        public ClipTransition CrouchJogBackRight;

        [Header("移动停止动画")]
        public ClipTransition WalkStopLeft;
        public ClipTransition WalkStopRight;
        
        [Space]
        public ClipTransition RunStopLeft;
        public ClipTransition RunStopRight;
        
        [Space]
        public ClipTransition SprintStopLeft;
        public ClipTransition SprintStopRight;
        

        #region 移动开始动画

        [Header("行走起步动画（八方向）")]
        public MotionClipData WalkStartFwd;
        public MotionClipData WalkStartBack;
        public MotionClipData WalkStartLeft;
        public MotionClipData WalkStartRight;
        public MotionClipData WalkStartFwdLeft;
        public MotionClipData WalkStartFwdRight;
        public MotionClipData WalkStartBackLeft;
        public MotionClipData WalkStartBackRight;

        [Header("慢跑起步动画（八方向）")]
        public MotionClipData RunStartFwd;
        public MotionClipData RunStartBack;
        public MotionClipData RunStartLeft;
        public MotionClipData RunStartRight;
        public MotionClipData RunStartFwdLeft;
        public MotionClipData RunStartFwdRight;
        public MotionClipData RunStartBackLeft;
        public MotionClipData RunStartBackRight;

        [Header("冲刺起步动画（八方向）")]
        public MotionClipData SprintStartFwd;
        public MotionClipData SprintStartBack;
        public MotionClipData SprintStartLeft;
        public MotionClipData SprintStartRight;
        public MotionClipData SprintStartFwdLeft;
        public MotionClipData SprintStartFwdRight;
        public MotionClipData SprintStartBackLeft;
        public MotionClipData SprintStartBackRight;
        

        #endregion
        #endregion
        #endregion
    }
}
