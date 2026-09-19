using UnityEngine;

#if BBBNEXUS_HAS_FINALIK
using RootMotion.FinalIK;
#endif

namespace AwCon
{
    // 已修复编码乱码的注释。
    public class FinalIKSource : PlayerIKSourceBase
    {
#if BBBNEXUS_HAS_FINALIK
        [Header("FinalIK组件")]
        [SerializeField] private FullBodyBipedIK _fbbik;
        [SerializeField] private AimIK _aimIK;

        [Header("瞄准目标设置")]
        [Tooltip("瞄准目标代理")]
        [SerializeField] private Transform _aimTargetProxy;

        [Tooltip("瞄准轴心回退")]
        [SerializeField] private Transform _aimPivotFallback;

        private void Awake()
        {
            EnsureAimTargetProxy();
            EnsureAimPivotFallback();

            // 已修复编码乱码的注释。
            if (_aimIK != null)
            {
                if (_aimIK.solver.target == null) _aimIK.solver.target = _aimTargetProxy;

                // 已修复编码乱码的注释。
                if (_aimIK.solver.transform == null) _aimIK.solver.transform = _aimPivotFallback;
            }
        }

        private void EnsureAimTargetProxy()
        {
            if (_aimTargetProxy != null) return;

            var go = new GameObject("AimTarget_Proxy");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = Vector3.forward * 5f;
            go.transform.localRotation = Quaternion.identity;
            _aimTargetProxy = go.transform;
        }

        private void EnsureAimPivotFallback()
        {
            if (_aimPivotFallback != null) return;

            _aimPivotFallback = transform;
        }

        // 已修复编码乱码的注释。
        public override void SetIKTarget(IKTarget target, Transform targetTransform, float weight)
        {
            switch (target)
            {
                case IKTarget.LeftHand:
                    if (_fbbik != null)
                    {
                        // 已修复编码乱码的注释。
                        _fbbik.solver.leftHandEffector.target = targetTransform;
                        if (targetTransform != null)
                        {
                            _fbbik.solver.leftHandEffector.position = targetTransform.position;
                            //_fbbik.solver.leftHandEffector.rotation = targetTransform.rotation;
                        }
                        _fbbik.solver.leftHandEffector.positionWeight = weight;
                       // _fbbik.solver.leftHandEffector.rotationWeight = weight;
                    }
                    break;

                case IKTarget.RightHand:
                    if (_fbbik != null)
                    {
                        _fbbik.solver.rightHandEffector.target = targetTransform;
                        if (targetTransform != null)
                        {
                            _fbbik.solver.rightHandEffector.position = targetTransform.position;
                            //_fbbik.solver.rightHandEffector.rotation = targetTransform.rotation;
                        }
                        _fbbik.solver.rightHandEffector.positionWeight = weight;
                        //_fbbik.solver.rightHandEffector.rotationWeight = weight;
                    }
                    break;

                case IKTarget.AimReference:
                    if (_aimIK != null)
                    {
                        // 已修复编码乱码的注释。
                        // 已修复编码乱码的注释。
                        EnsureAimTargetProxy();
                        EnsureAimPivotFallback();

                        // 已修复编码乱码的注释。
                        // 已修复编码乱码的注释。
                        _aimIK.solver.transform = targetTransform != null ? targetTransform : _aimPivotFallback;

                        // 已修复编码乱码的注释。
                        if (_aimIK.solver.target == null) _aimIK.solver.target = _aimTargetProxy;

                        if (!_aimIK.enabled) _aimIK.enabled = true;
                    }
                    break;
            }
        }

        // 已修复编码乱码的注释。
        public override void SetIKTarget(IKTarget target, Vector3 position, Quaternion rotation, float weight)
        {
            switch (target)
            {
                case IKTarget.HeadLook:
                    if (_aimIK != null)
                    {
                        EnsureAimTargetProxy();
                        EnsureAimPivotFallback();

                        // 已修复编码乱码的注释。
                        if (_aimIK.solver.transform == null) _aimIK.solver.transform = _aimPivotFallback;

                        // 已修复编码乱码的注释。
                        _aimTargetProxy.position = position;
                        _aimTargetProxy.rotation = rotation;

                        if (_aimIK.solver.target == null) _aimIK.solver.target = _aimTargetProxy;

                        // 已修复编码乱码的注释。
                        _aimIK.solver.IKPosition = position;
                        _aimIK.solver.IKPositionWeight = weight;
                    }
                    break;

                case IKTarget.LeftHand:
                    if (_fbbik != null)
                    {
                        _fbbik.solver.leftHandEffector.target = null;
                        _fbbik.solver.leftHandEffector.position = position;
                        //_fbbik.solver.leftHandEffector.rotation = rotation;
                        _fbbik.solver.leftHandEffector.positionWeight = weight;
                        //_fbbik.solver.leftHandEffector.rotationWeight = weight;
                    }
                    break;

                case IKTarget.RightHand:
                    if (_fbbik != null)
                    {
                        _fbbik.solver.rightHandEffector.target = null;
                        _fbbik.solver.rightHandEffector.position = position;
                        //_fbbik.solver.rightHandEffector.rotation = rotation;
                        _fbbik.solver.rightHandEffector.positionWeight = weight;
                       // _fbbik.solver.rightHandEffector.rotationWeight = weight;
                    }
                    break;
            }
        }

        // 已修复编码乱码的注释。
        public override void UpdateIKWeight(IKTarget target, float weight)
        {
            switch (target)
            {
                case IKTarget.LeftHand:
                    if (_fbbik != null)
                    {
                        _fbbik.solver.leftHandEffector.positionWeight = weight;
                        //_fbbik.solver.leftHandEffector.rotationWeight = weight;
                    }
                    break;

                case IKTarget.RightHand:
                    if (_fbbik != null)
                    {
                        _fbbik.solver.rightHandEffector.positionWeight = weight;
                        //_fbbik.solver.rightHandEffector.rotationWeight = weight;
                    }
                    break;

                case IKTarget.AimReference:
                    if (_aimIK != null)
                    {
                        // 已修复编码乱码的注释。
                        // 已修复编码乱码的注释。
                        // 已修复编码乱码的注释。
                        EnsureAimPivotFallback();
                        if (_aimIK.solver.transform == null) _aimIK.solver.transform = _aimPivotFallback;
                    }
                    break;

                case IKTarget.HeadLook:
                    if (_aimIK != null)
                    {
                        _aimIK.solver.IKPositionWeight = weight;
                    }
                    break;
            }
        }

        public override void EnableAllIK()
        {
            if (_fbbik != null) _fbbik.enabled = true;
            if (_aimIK != null) _aimIK.enabled = true;
        }

        public override void DisableAllIK()
        {
            if (_fbbik != null) _fbbik.enabled = false;
            if (_aimIK != null) _aimIK.enabled = false;
        }
#else
        // 已修复编码乱码的注释。
        // 已修复编码乱码的注释。
        // 已修复编码乱码的注释。
        // 已修复编码乱码的注释。
        public override void SetIKTarget(IKTarget target, Transform targetTransform, float weight) { }
        public override void SetIKTarget(IKTarget target, Vector3 position, Quaternion rotation, float weight) { }
        public override void UpdateIKWeight(IKTarget target, float weight) { }
        public override void EnableAllIK() { }
        public override void DisableAllIK() { }
#endif
    }
}