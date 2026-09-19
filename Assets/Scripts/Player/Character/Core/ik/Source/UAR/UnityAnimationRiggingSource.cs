using UnityEngine;
#if BBBNEXUS_HAS_UAR
using UnityEngine.Animations.Rigging;
#endif

namespace AwCon
{
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    public class UnityAnimationRiggingSource : PlayerIKSourceBase
    {
#if BBBNEXUS_HAS_UAR
        [Header("手部IK设置")]
        [SerializeField] private TwoBoneIKConstraint _leftHandIK;
        [SerializeField] private TwoBoneIKConstraint _rightHandIK;

        [Header("手部IK目标")]
        // 已修复编码乱码的注释。
        // 已修复编码乱码的注释。
        [SerializeField] private Transform _leftHandTarget;
        [SerializeField] private Transform _rightHandTarget;

        [Header("头部注视IK设置")]
        [SerializeField] private MultiAimConstraint _headLookAtIK;
        [SerializeField] private Transform _lookAtTarget;

        // 已修复编码乱码的注释。
        [SerializeField] private RigBuilder _rigBuilder;

        // 已修复编码乱码的注释。

        public override void SetIKTarget(IKTarget target, Transform targetTransform, float weight)
        {
            switch (target)
            {
                case IKTarget.LeftHand:
                    if (_leftHandIK != null && _leftHandTarget != null)
                    {
                        // 已修复编码乱码的注释。
                        if (targetTransform != null)
                        {
                            _leftHandTarget.position = targetTransform.position;
                            _leftHandTarget.rotation = targetTransform.rotation;
                        }

                        // 已修复编码乱码的注释。
                        _leftHandIK.weight = weight;
                    }
                    break;

                case IKTarget.RightHand:
                    if (_rightHandIK != null && _rightHandTarget != null)
                    {
                        // 已修复编码乱码的注释。
                        if (targetTransform != null)
                        {
                            _rightHandTarget.position = targetTransform.position;
                            _rightHandTarget.rotation = targetTransform.rotation;
                        }

                        // 已修复编码乱码的注释。
                        _rightHandIK.weight = weight;
                    }
                    break;
            }
        }

        public override void SetIKTarget(IKTarget target, Vector3 position, Quaternion rotation, float weight)
        {
            switch (target)
            {
                case IKTarget.LeftHand:
                    if (_leftHandIK != null && _leftHandTarget != null)
                    {
                        _leftHandTarget.position = position;
                        _leftHandTarget.rotation = rotation;
                        _leftHandIK.weight = weight;
                    }
                    break;

                case IKTarget.RightHand:
                    if (_rightHandIK != null && _rightHandTarget != null)
                    {
                        _rightHandTarget.position = position;
                        _rightHandTarget.rotation = rotation;
                        _rightHandIK.weight = weight;
                    }
                    break;

                case IKTarget.HeadLook:
                    if (_lookAtTarget != null)
                    {
                        _lookAtTarget.position = position;
                        if (_headLookAtIK != null)
                        {
                            _headLookAtIK.weight = weight;
                        }
                    }
                    break;
            }
        }

        public override void UpdateIKWeight(IKTarget target, float weight)
        {
            switch (target)
            {
                case IKTarget.LeftHand:
                    if (_leftHandIK != null) _leftHandIK.weight = weight;
                    break;

                case IKTarget.RightHand:
                    if (_rightHandIK != null) _rightHandIK.weight = weight;
                    break;

                case IKTarget.HeadLook:
                    if (_headLookAtIK != null) _headLookAtIK.weight = weight;
                    break;
            }
        }
        public override void EnableAllIK()
        {
            if (_rigBuilder != null)
            {
                _rigBuilder.enabled = true;
            }
            else
            {
                if (_leftHandIK != null) _leftHandIK.enabled = true;
                if (_rightHandIK != null) _rightHandIK.enabled = true;
                if (_headLookAtIK != null) _headLookAtIK.enabled = true;
            }
        }

        public override void DisableAllIK()
        {
            if (_rigBuilder != null)
            {
                _rigBuilder.enabled = false;
            }
            else
            {
                if (_leftHandIK != null) _leftHandIK.enabled = false;
                if (_rightHandIK != null) _rightHandIK.enabled = false;
                if (_headLookAtIK != null) _headLookAtIK.enabled = false;
            }
        }
#else
        public override void SetIKTarget(IKTarget target, Transform targetTransform, float weight) { }
        public override void SetIKTarget(IKTarget target, Vector3 position, Quaternion rotation, float weight) { }
        public override void UpdateIKWeight(IKTarget target, float weight) { }
        public override void EnableAllIK() { }
        public override void DisableAllIK() { }
#endif
    }
}