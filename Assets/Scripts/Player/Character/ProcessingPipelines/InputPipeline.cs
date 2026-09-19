using UnityEngine;

namespace AwCon
{
    /// <summary>
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    /// </summary>
    public class InputPipeline
    {
        // 已修复编码乱码的注释。
        private readonly InputSourceBase _inputSource;

        // 已修复编码乱码的注释。
        private InputData _inputData;

        // 已修复编码乱码的注释。
        private RawInputData _rawData;
        private Vector2 _bufferedMove;
        private float _lastNonZeroMoveTime;

        // 已修复编码乱码的注释。
        // 已修复编码乱码的注释。
        private readonly float _inputFlickerBuffer;
        // 已修复编码乱码的注释。
        private readonly float _actionBufferTime;

        // 已修复编码乱码的注释。
        private ulong _frameIndex;

        /// <summary>
        // 已修复编码乱码的注释。
        // 已修复编码乱码的注释。
        /// </summary>
        public InputData Current => _inputData;

        // 已修复编码乱码的注释。
        public InputPipeline(InputSourceBase inputSource)
        {
            _inputSource = inputSource;
            _inputFlickerBuffer = _inputSource.InputFlickerBuffer;
            _actionBufferTime = _inputSource.ActionBufferTime;

            // 已修复编码乱码的注释。
            _inputData = new InputData();
            _inputData.currentFrameData = new FrameInputData { FrameIndex = 0 };
            _inputData.lastFrameData = new FrameInputData { FrameIndex = 0 };

            _rawData = default;
            _bufferedMove = Vector2.zero;
            _lastNonZeroMoveTime = Time.time;
            _frameIndex = 0;
        }

        /// <summary>
        // 已修复编码乱码的注释。
        // 已修复编码乱码的注释。
        /// </summary>
        public void Update()
        {
            // 已修复编码乱码的注释。
            _inputData.lastFrameData = _inputData.currentFrameData;

            if (_inputSource != null && _inputSource.IsBlocked)
            {
                _rawData = default;
            }
            else
            {
                // 已修复编码乱码的注释。
                _inputSource.FetchRawInput(ref _rawData);
            }

            // 已修复编码乱码的注释。
            ProcessRawInput();

            _frameIndex++;
        }

        /// <summary>
        // 已修复编码乱码的注释。
        /// </summary>
        private void ProcessRawInput()
        {
            var currentFrame = new FrameInputData
            {
                FrameIndex = _frameIndex,
                Raw = _rawData,
                Processed = default
            };

            // 已修复编码乱码的注释。
            if (_rawData.MoveAxis.sqrMagnitude > 0.01f)
            {
                _bufferedMove = _rawData.MoveAxis;
                _lastNonZeroMoveTime = Time.time;
                currentFrame.Processed.Move = _rawData.MoveAxis;
            }
            else if (Time.time - _lastNonZeroMoveTime < _inputFlickerBuffer)
            {
                // 已修复编码乱码的注释。
                currentFrame.Processed.Move = _bufferedMove;
            }
            else
            {
                currentFrame.Processed.Move = Vector2.zero;
            }

            // 已修复编码乱码的注释。
            currentFrame.Processed.Look = _rawData.LookAxis;
            currentFrame.Processed.ReloadPressed = _rawData.ReloadJustPressed;
            currentFrame.Processed.CycleWeaponPressed = _rawData.CycleWeaponJustPressed;

            // 已修复编码乱码的注释。
            currentFrame.Processed.JumpHeld = _rawData.JumpHeld;
            currentFrame.Processed.DodgeHeld = _rawData.DodgeHeld;
            currentFrame.Processed.RollHeld = _rawData.RollHeld;
            currentFrame.Processed.SprintHeld = _rawData.SprintHeld;
            currentFrame.Processed.WalkHeld = _rawData.WalkHeld;
            currentFrame.Processed.AimHeld = _rawData.AimHeld;
            currentFrame.Processed.CrouchHeld = _rawData.CrouchHeld;
            currentFrame.Processed.InteractHeld = _rawData.InteractHeld;

            currentFrame.Processed.LeftMouseHeld = _rawData.LeftMouseHeld;
            currentFrame.Processed.FireHeld = _rawData.LeftMouseHeld;

            currentFrame.Processed.Expression1Held = _rawData.Expression1Held;
            currentFrame.Processed.Expression2Held = _rawData.Expression2Held;
            currentFrame.Processed.Expression3Held = _rawData.Expression3Held;
            currentFrame.Processed.Expression4Held = _rawData.Expression4Held;

            currentFrame.Processed.Number1Held = _rawData.Number1Held;
            currentFrame.Processed.Number2Held = _rawData.Number2Held;
            currentFrame.Processed.Number3Held = _rawData.Number3Held;
            currentFrame.Processed.Number4Held = _rawData.Number4Held;
            currentFrame.Processed.Number5Held = _rawData.Number5Held;

            currentFrame.Processed.ActionHeld = _rawData.ActionHeld;

            // 已修复编码乱码的注释。
            // 已修复编码乱码的注释。
            // 已修复编码乱码的注释。
            float dt = Time.deltaTime;
            var lastProc = _inputData.lastFrameData.Processed;

            float UpdateBuffer(float lastTimer, bool justPressed)
            {
                float newTimer = Mathf.Max(0f, lastTimer - dt);
                if (justPressed) newTimer = _actionBufferTime;
                return newTimer;
            }

            currentFrame.Processed.JumpBufferTimer = UpdateBuffer(lastProc.JumpBufferTimer, _rawData.JumpJustPressed);
            currentFrame.Processed.DodgeBufferTimer = UpdateBuffer(lastProc.DodgeBufferTimer, _rawData.DodgeJustPressed);
            currentFrame.Processed.RollBufferTimer = UpdateBuffer(lastProc.RollBufferTimer, _rawData.RollJustPressed);

            currentFrame.Processed.LeftMouseBufferTimer = UpdateBuffer(lastProc.LeftMouseBufferTimer, _rawData.LeftMouseJustPressed);
            currentFrame.Processed.FireBufferTimer = currentFrame.Processed.LeftMouseBufferTimer;

            currentFrame.Processed.Expression1BufferTimer = UpdateBuffer(lastProc.Expression1BufferTimer, _rawData.Expression1JustPressed);
            currentFrame.Processed.Expression2BufferTimer = UpdateBuffer(lastProc.Expression2BufferTimer, _rawData.Expression2JustPressed);
            currentFrame.Processed.Expression3BufferTimer = UpdateBuffer(lastProc.Expression3BufferTimer, _rawData.Expression3JustPressed);
            currentFrame.Processed.Expression4BufferTimer = UpdateBuffer(lastProc.Expression4BufferTimer, _rawData.Expression4JustPressed);

            currentFrame.Processed.Number1BufferTimer = UpdateBuffer(lastProc.Number1BufferTimer, _rawData.Number1JustPressed);
            currentFrame.Processed.Number2BufferTimer = UpdateBuffer(lastProc.Number2BufferTimer, _rawData.Number2JustPressed);
            currentFrame.Processed.Number3BufferTimer = UpdateBuffer(lastProc.Number3BufferTimer, _rawData.Number3JustPressed);
            currentFrame.Processed.Number4BufferTimer = UpdateBuffer(lastProc.Number4BufferTimer, _rawData.Number4JustPressed);
            currentFrame.Processed.Number5BufferTimer = UpdateBuffer(lastProc.Number5BufferTimer, _rawData.Number5JustPressed);

            currentFrame.Processed.ActionBufferTimer = UpdateBuffer(lastProc.ActionBufferTimer, _rawData.ActionJustPressed);

            // 已修复编码乱码的注释。
            _inputData.currentFrameData = currentFrame;
        }

        // 已修复编码乱码的注释。
        // 已修复编码乱码的注释。
        // 已修复编码乱码的注释。

        public void ConsumeJumpPressed() { var f = _inputData.currentFrameData; f.Processed.JumpBufferTimer = 0f; _inputData.currentFrameData = f; }
        public void ConsumeDodgePressed() { var f = _inputData.currentFrameData; f.Processed.DodgeBufferTimer = 0f; _inputData.currentFrameData = f; }
        public void ConsumeRollPressed() { var f = _inputData.currentFrameData; f.Processed.RollBufferTimer = 0f; _inputData.currentFrameData = f; }
        public void ConsumeFirePressed() => ConsumeLeftMousePressed();
        public void ConsumeExpression1Pressed() { var f = _inputData.currentFrameData; f.Processed.Expression1BufferTimer = 0f; _inputData.currentFrameData = f; }
        public void ConsumeExpression2Pressed() { var f = _inputData.currentFrameData; f.Processed.Expression2BufferTimer = 0f; _inputData.currentFrameData = f; }
        public void ConsumeExpression3Pressed() { var f = _inputData.currentFrameData; f.Processed.Expression3BufferTimer = 0f; _inputData.currentFrameData = f; }
        public void ConsumeExpression4Pressed() { var f = _inputData.currentFrameData; f.Processed.Expression4BufferTimer = 0f; _inputData.currentFrameData = f; }
        public void ConsumeNumber1Pressed() { var f = _inputData.currentFrameData; f.Processed.Number1BufferTimer = 0f; _inputData.currentFrameData = f; }
        public void ConsumeNumber2Pressed() { var f = _inputData.currentFrameData; f.Processed.Number2BufferTimer = 0f; _inputData.currentFrameData = f; }
        public void ConsumeNumber3Pressed() { var f = _inputData.currentFrameData; f.Processed.Number3BufferTimer = 0f; _inputData.currentFrameData = f; }
        public void ConsumeNumber4Pressed() { var f = _inputData.currentFrameData; f.Processed.Number4BufferTimer = 0f; _inputData.currentFrameData = f; }
        public void ConsumeNumber5Pressed() { var f = _inputData.currentFrameData; f.Processed.Number5BufferTimer = 0f; _inputData.currentFrameData = f; }
        public void ConsumeActionPressed() { var f = _inputData.currentFrameData; f.Processed.ActionBufferTimer = 0f; _inputData.currentFrameData = f; }
        public void ConsumeLeftMousePressed() { var f = _inputData.currentFrameData; f.Processed.LeftMouseBufferTimer = 0f; f.Processed.FireBufferTimer = 0f; _inputData.currentFrameData = f; }
    }
}
