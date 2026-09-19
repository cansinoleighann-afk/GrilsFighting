using UnityEngine;

namespace AwCon
{
    /// <summary>
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    /// </summary>
    public class LODArbiter
    {
        private readonly BBBCharacterController _player;
        private readonly PlayerRuntimeData _data;
        private readonly PlayerSO _config;

        private float _timeSinceLastArbitration;
        private CharacterLOD _lastEnforcedLOD = CharacterLOD.High;

        public LODArbiter(BBBCharacterController player)
        {
            _player = player;
            _data = player.RuntimeData;
            _config = player.Config;

            // 已修复编码乱码的注释。
            if (_config != null && _config.Core != null)
            {
                _timeSinceLastArbitration = Random.Range(0f, _config.Core.LODCheckInterval);
            }
        }

        public void Arbitrate()
        {
            if (_data.CameraTransform == null || _config == null || _config.Core == null) return;

            _timeSinceLastArbitration += Time.deltaTime;
            if (_timeSinceLastArbitration < _config.Core.LODCheckInterval) return;

            _timeSinceLastArbitration = 0f;
            ExecuteArbitrationLogic();
        }

        private void ExecuteArbitrationLogic()
        {
            float sqrDist = (_player.transform.position - _data.CameraTransform.position).sqrMagnitude;

            float medDistSqr = _config.Core.MediumLODDistance * _config.Core.MediumLODDistance;
            float lowDistSqr = _config.Core.LowLODDistance * _config.Core.LowLODDistance;

            CharacterLOD targetLOD = CharacterLOD.High;

            if (sqrDist > lowDistSqr)
            {
                targetLOD = CharacterLOD.Low;
            }
            else if (sqrDist > medDistSqr)
            {
                targetLOD = CharacterLOD.Medium;
            }

            if (targetLOD != _lastEnforcedLOD)
            {
                _lastEnforcedLOD = targetLOD;
                EnforceArbitration(targetLOD);
            }
        }

        // 已修复编码乱码的注释。
        private void EnforceArbitration(CharacterLOD lod)
        {
            if (_player.Animator == null) return;

            // 已修复编码乱码的注释。
            // 已修复编码乱码的注释。
            // 已修复编码乱码的注释。
            bool isDegraded = (lod != CharacterLOD.High);
            _data.Arbitration.BlockIK = isDegraded;
            _data.Arbitration.BlockFacial = isDegraded;

            // 已修复编码乱码的注释。
            // 已修复编码乱码的注释。
            // 已修复编码乱码的注释。
            // 已修复编码乱码的注释。
            switch (lod)
            {
                case CharacterLOD.High:
                    _player.Animator.enabled = true;
                    _player.Animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                    break;

                case CharacterLOD.Medium:
                    // 已修复编码乱码的注释。
                    // 已修复编码乱码的注释。
                    _player.Animator.enabled = false;
                    break;

                case CharacterLOD.Low:
                    // 已修复编码乱码的注释。
                    _player.Animator.enabled = false;
                    _player.Animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
                    break;
            }
        }
    }
}
