using UnityEngine;

namespace AwCon
{
    // 基础移动意图处理器 
    public class LocomotionIntentProcessor
    {
        private readonly PlayerRuntimeData _data;
        private readonly PlayerSO _config;

        public LocomotionIntentProcessor(PlayerRuntimeData data, PlayerSO config)
        {
            _data = data;
            _config = config;
        }

        public void Update(in ProcessedInputData input)
        {
            _data.IsCrouching = input.CrouchHeld && _data.IsGrounded;
            _data.IsCrouchJogging = _data.IsCrouching && input.SprintHeld;
            ProcessMovementIntent();
            ProcessLocomotionStateAndStaminaIntent(in input);
        }

        private void ProcessMovementIntent()
        {
            // 判断按键是否产生有效输入
            bool isMoving = _data.MoveInput.sqrMagnitude > 0.01f;

            if (isMoving)
            {
                // 普通移动必须以实际输出相机的水平朝向为基准。
                // Cinemachine POV 与角色权威朝向可能在不同更新阶段刷新；直接使用
                // CameraRigDriver 写入的 CameraLookDirection，避免转动镜头时角色朝旧 yaw 移动。
                Vector3 basisForward = _data.CameraLookDirection;
                basisForward.y = 0f;
                if (basisForward.sqrMagnitude < 0.0001f)
                {
                    Quaternion yawRot = Quaternion.Euler(0f, _data.AuthorityYaw, 0f);
                    basisForward = yawRot * Vector3.forward;
                }
                else
                {
                    basisForward.Normalize();
                }

                Vector3 basisRight = Vector3.Cross(Vector3.up, basisForward);
                _data.DesiredWorldMoveDir = (basisRight * _data.MoveInput.x + basisForward * _data.MoveInput.y).normalized;

                // 将摇杆输入量化为 8 个离散方向 供混合树 或 起步状态判定
                _data.QuantizedDirection = QuantizeInputDirection(_data.MoveInput);
            }
            else
            {
                _data.DesiredWorldMoveDir = Vector3.zero;
                _data.QuantizedDirection = DesiredDirection.None;
            }
        }

        private void ProcessLocomotionStateAndStaminaIntent(in ProcessedInputData input)
        {
            LocomotionState prestate = _data.CurrentLocomotionState;

            if (input.RollPressed && _data.IsGrounded)
            {
                _data.WantsToRoll = true;
            }

            if (input.DodgePressed && _data.IsGrounded)
            {
                _data.WantsToDodge = true;
            }

            // 持续性移动状态判定
            bool isMoving = _data.MoveInput.sqrMagnitude > 0.01f;

            // 体力耗尽后的恢复阈值判定
            if (_data.IsStaminaDepleted && _data.CurrentStamina > _config.Core.MaxStamina * _config.Core.StaminaRecoverThreshold)
            {
                _data.IsStaminaDepleted = false;
            }

            // 基于输入与体力 推导当前运动档位
            if (!isMoving)
            {
                _data.CurrentLocomotionState = LocomotionState.Idle;
                _data.WantToRun = false;
            }
            else if (input.SprintHeld && !_data.IsCrouching && !_data.IsStaminaDepleted && _data.CurrentStamina > 0)
            {
                _data.CurrentLocomotionState = LocomotionState.Sprint;
                _data.WantToRun = true;
            }
            else if (input.WalkHeld)
            {
                _data.CurrentLocomotionState = LocomotionState.Walk;
                _data.WantToRun = false;
            }
            else
            {
                _data.CurrentLocomotionState = LocomotionState.Jog;
                _data.WantToRun = false;
            }

            // 记录历史档位状态 供起步等状态用于动量继承判定
            if (_data.CurrentLocomotionState != prestate)
            {
                _data.LastLocomotionState = prestate;
            }
        }

        // 输入方向量化器 将连续向量切分为 8 个逻辑朝向
        private DesiredDirection QuantizeInputDirection(Vector2 input)
        {
            float threshold = 0.5f;
            bool hasForward = input.y > threshold;
            bool hasBackward = input.y < -threshold;
            bool hasRight = input.x > threshold;
            bool hasLeft = input.x < -threshold;

            if (hasForward) { if (hasLeft) return DesiredDirection.ForwardLeft; if (hasRight) return DesiredDirection.ForwardRight; return DesiredDirection.Forward; }
            if (hasBackward) { if (hasLeft) return DesiredDirection.BackwardLeft; if (hasRight) return DesiredDirection.BackwardRight; return DesiredDirection.Backward; }
            if (hasLeft) return DesiredDirection.Left;
            if (hasRight) return DesiredDirection.Right;
            return DesiredDirection.None;
        }
    }
}
