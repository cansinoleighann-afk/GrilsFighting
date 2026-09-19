using UnityEngine;

namespace AwCon
{
    [CreateAssetMenu(fileName = "JumpSO", menuName = "AwCon/Player/Modules/JumpSO")]
    public class JumpSO : ScriptableObject
    {
        [Header("跳跃基础设置")]
        
        [Tooltip("跳跃力度")]
        public float JumpForce = 6f;
        
        [Tooltip("跳跃空中动画")]
        public MotionClipData JumpAirAnim;

        [Header("行走跳跃设置")]
        
        [Tooltip("行走时跳跃力度")]
        public float JumpForceWalk = 5f;
        
        [Tooltip("行走跳跃空中动画")]
        public MotionClipData JumpAirAnimWalk;

        [Header("冲刺跳跃设置")]
        
        [Tooltip("冲刺时跳跃力度")]
        public float JumpForceSprint = 7f;
        
        [Tooltip("冲刺跳跃空中动画")]
        public MotionClipData JumpAirAnimSprint;

        [Header("空手冲刺跳跃设置")]
        
        [Tooltip("空手冲刺时跳跃力度")]
        public float JumpForceSprintEmpty = 8f;
        
        [Tooltip("空手冲刺跳跃空中动画")]
        public MotionClipData JumpAirAnimSprintEmpty;

        [Header("二段跳设置")]
        
        [Tooltip("二段跳向上力度")]
        public float DoubleJumpForceUp = 6f;
        
        [Tooltip("空手冲刺二段跳向上力度")]
        public float DoubleJumpEmptyHandSprintForceUp = 8f;

        [Tooltip("二段跳向上动画")]
        public MotionClipData DoubleJumpUp;
        
        [Tooltip("二段跳淡入选项")]
        public AnimPlayOptions DoubleJumpFadeInOptions;

        [Tooltip("二段跳冲刺翻滚动画")]
        public MotionClipData DoubleJumpSprintRoll;
        
        [Tooltip("二段跳冲刺翻滚淡入选项")]
        public AnimPlayOptions DoubleJumpSprintRollFadeInOptions;
        

        [Header("落地高度等级设置")]
        
        [Tooltip("落地高度等级0阈值")]
        public float LandHeight_Level0 = 2f;
        
        [Tooltip("落地高度等级0动画选项")]
        public AnimPlayOptions LandHeight_Level0_options;
        
        [Tooltip("落地高度等级1阈值")]
        public float LandHeight_Level1 = 2f;
        
        [Tooltip("落地高度等级1动画选项")]
        public AnimPlayOptions LandHeight_Level1_options;
        
        [Tooltip("落地高度等级2阈值")]
        public float LandHeight_Level2 = 5f;
        
        [Tooltip("落地高度等级2动画选项")]
        public AnimPlayOptions LandHeight_Level2_options;
        
        [Tooltip("落地高度等级3阈值")]
        public float LandHeight_Level3 = 8f;
        
        [Tooltip("落地高度等级3动画选项")]
        public AnimPlayOptions LandHeight_Level3_options;
        
        [Tooltip("落地高度等级4阈值")]
        public float LandHeight_Level4 = 12f;
        
        [Tooltip("落地高度等级4动画选项")]
        public AnimPlayOptions LandHeight_Level4_options;

        [Header("落地到待机设置")]
        
        [Tooltip("落地到待机动画选项")]
        public AnimPlayOptions LandToIdleOptions = AnimPlayOptions.Default;

        [Header("行走慢跑落地缓冲设置")]
        
        [Tooltip("行走慢跑落地缓冲动画等级0")]
        public MotionClipData LandBuffer_WalkJog_L0;
        
        [Tooltip("行走慢跑落地到循环淡入时间等级0选项")]
        public AnimPlayOptions LandToLoopFadeInTime_WalkJog_L0ptions = AnimPlayOptions.Default;
        
        [Tooltip("行走慢跑落地缓冲动画等级1")]
        public MotionClipData LandBuffer_WalkJog_L1;
        
        [Tooltip("行走慢跑落地到循环淡入时间等级1选项")]
        public AnimPlayOptions LandToLoopFadeInTime_WalkJog_L1ptions = AnimPlayOptions.Default;
        
        [Tooltip("行走慢跑落地缓冲动画等级2")]
        public MotionClipData LandBuffer_WalkJog_L2;
        
        [Tooltip("行走慢跑落地到循环淡入时间等级2选项")]
        public AnimPlayOptions LandToLoopFadeInTime_WalkJog_L2ptions = AnimPlayOptions.Default;
        
        [Tooltip("行走慢跑落地缓冲动画等级3")]
        public MotionClipData LandBuffer_WalkJog_L3;
        
        [Tooltip("行走慢跑落地到循环淡入时间等级3选项")]
        public AnimPlayOptions LandToLoopFadeInTime_WalkJog_L3ptions = AnimPlayOptions.Default;

        [Header("冲刺落地缓冲设置")]
        
        [Tooltip("冲刺落地缓冲动画等级0")]
        public MotionClipData LandBuffer_Sprint_L0;
        
        [Tooltip("冲刺落地到循环淡入时间等级0选项")]
        public AnimPlayOptions LandToLoopFadeInTime_Sprint_L0ptions = AnimPlayOptions.Default;
        
        [Tooltip("冲刺落地缓冲动画等级1")]
        public MotionClipData LandBuffer_Sprint_L1;
        
        [Tooltip("冲刺落地到循环淡入时间等级1选项")]
        public AnimPlayOptions LandToLoopFadeInTime_Sprint_L1ptions = AnimPlayOptions.Default;
        
        [Tooltip("冲刺落地缓冲动画等级2")]
        public MotionClipData LandBuffer_Sprint_L2;
        
        [Tooltip("冲刺落地到循环淡入时间等级2选项")]
        public AnimPlayOptions LandToLoopFadeInTime_Sprint_L2ptions = AnimPlayOptions.Default;
        
        [Tooltip("冲刺落地缓冲动画等级3")]
        public MotionClipData LandBuffer_Sprint_L3;
        
        [Tooltip("冲刺落地到循环淡入时间等级3选项")]
        public AnimPlayOptions LandToLoopFadeInTime_Sprint_L3ptions = AnimPlayOptions.Default;

        [Header("超限落地设置")]
        
        [Tooltip("超限落地缓冲动画")]
        public MotionClipData LandBuffer_ExceedLimit;
        
        [Tooltip("超限落地到循环淡入选项")]
        public AnimPlayOptions LandToLoopFadeInTime_ExceedLimitOptions = AnimPlayOptions.Default;
        

    }
}