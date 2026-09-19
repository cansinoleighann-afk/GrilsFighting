namespace AwCon
{
    /// <summary>
    // 已修复编码乱码的注释。
    /// </summary>
    public sealed class AudioController
    {
        private readonly BBBCharacterController _player;
        private readonly PlayerRuntimeData _data;

        public AudioController(BBBCharacterController player)
        {
            _player = player;
            _data = player.RuntimeData;
        }

        public void Update()
        {
            if (_player == null || _data == null) return;
            
            // 已修复编码乱码的注释。
            if (_data.Arbitration.BlockAudio)
            {
                _data.SfxQueue.Clear();
                return;
            }
            
            if (_player.AudioDriver == null) { _data.SfxQueue.Clear(); return; }

            int count = _data.SfxQueue.Count;
            for (int i = 0; i < count; i++)
            {
                var evt = _data.SfxQueue.Get(i);
                if (evt == PlayerSfxEvent.Land &&
                    _player.FootstepController != null &&
                    _player.AudioDriver.PlayFootstep(_player.FootstepController.ResolveSurface(), FootstepActionType.Land))
                    continue;

                _player.AudioDriver.Play(evt);
            }

            _data.SfxQueue.Clear();
        }
    }
}
