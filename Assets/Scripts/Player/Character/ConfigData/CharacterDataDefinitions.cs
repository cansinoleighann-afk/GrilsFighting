using UnityEngine;
using Animancer;
using System.Collections.Generic;

namespace AwCon
{
    // 已修复编码乱码的注释。
    /// <summary>
    // 已修复编码乱码的注释。
    /// </summary>
    public enum FootPhase { LeftFootDown, RightFootDown }

    /// <summary>
    // 已修复编码乱码的注释。
    /// </summary>
    [System.Serializable]
    public enum MotionType
    {
        InputDriven, 
        CurveDriven,   
        Mixed          
    }

    /// <summary>
    // 已修复编码乱码的注释。
    /// </summary>
    public enum WarpedType
    {
        None,           // 已修复编码乱码的注释。
        Vault,          // 已修复编码乱码的注释。
        Dodge,          // 已修复编码乱码的注释。
        Simple,         // 已修复编码乱码的注释。
        Custom          // 已修复编码乱码的注释。
    }


    // 已修复编码乱码的注释。
    /// <summary>
    // 已修复编码乱码的注释。
    /// </summary>
    [System.Serializable]
    public class MotionClipData
    {
        [Header("动画剪辑设置")]
        public ClipTransition Clip;
        public MotionType Type = MotionType.CurveDriven;

        [Header("时间设置")]
        public float TargetDuration = 0f;
        public float EndTime = 0f;

        [Header("方向设置")]
        public bool AllowBakeTargetLocalDirection;
        public Vector3 TargetLocalDirection;

        [Header("播放设置")]
        public FootPhase EndPhase = FootPhase.LeftFootDown;
        public float PlaybackSpeed = 1f;
        public AnimationCurve SpeedCurve;
        public AnimationCurve RotationCurve;
        public float RotationFinishedTime = 0f;

        public MotionClipData()
        {
            SpeedCurve = new AnimationCurve();
            RotationCurve = new AnimationCurve();
        }
    }

    /// <summary>
    // 已修复编码乱码的注释。
    /// </summary>
    [System.Serializable]
    public class WarpPointDef
    {
        [Tooltip("变形点名称")]
        public string PointName;

        [Tooltip("变形点归一化时间")]
        [Range(0f, 1f)]
        public float NormalizedTime;

        [Tooltip("变形点目标位置偏移")]
        public Vector3 TargetPositionOffset;

        [Header("烘焙设置")]
        [Tooltip("烘焙局部偏移")]
        public Vector3 BakedLocalOffset;

        [Tooltip("烘焙局部旋转")]
        public Quaternion BakedLocalRotation = Quaternion.identity;
    }

    /// <summary>
    // 已修复编码乱码的注释。
    /// </summary>
    [System.Serializable]
    public class WarpedMotionData
    {
        [Header("动画剪辑设置")]
        public ClipTransition Clip;

        [Header("时间设置")]
        public float EndTime = 0f;
        public FootPhase EndPhase = FootPhase.LeftFootDown;

        [Header("变形类型设置")]
        [Tooltip("变形类型")]
        public WarpedType Type = WarpedType.None;

        [Tooltip("变形点列表")]
        public List<WarpPointDef> WarpPoints = new List<WarpPointDef>();

        [Tooltip("手部IK权重曲线")]
        public AnimationCurve HandIKWeightCurve = new AnimationCurve();

        [Header("烘焙数据")]
        public float BakedDuration;
        public AnimationCurve LocalVelocityX = new AnimationCurve();
        public AnimationCurve LocalVelocityY = new AnimationCurve();
        public AnimationCurve LocalVelocityZ = new AnimationCurve();
        public AnimationCurve LocalRotationY = new AnimationCurve();

        [HideInInspector]
        public Vector3 TotalBakedLocalOffset;

        [Header("物理设置")]
        [Tooltip("If enabled, gravity will be applied during this warped motion. Otherwise vertical motion from gravity is ignored.")]
        public bool ApplyGravity = false;
    }


}