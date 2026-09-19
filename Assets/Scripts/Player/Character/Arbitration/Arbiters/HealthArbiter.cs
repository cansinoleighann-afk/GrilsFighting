using UnityEngine;

namespace AwCon
{
    /// <summary>
    // 已修复编码乱码的注释。
    /// </summary>
    public class HealthArbiter
    {
        private readonly BBBCharacterController _player;
        private readonly PlayerRuntimeData _data;

        // 已修复编码乱码的注释。
        private DamageRequest[] _damageQueue = new DamageRequest[16];
        private int _head = 0;
        private int _tail = 0;

        public HealthArbiter(BBBCharacterController player)
        {
            _player = player;
            _data = player.RuntimeData;
        }

        /// <summary>
        // 已修复编码乱码的注释。
        /// </summary>
        internal void Enqueue(in DamageRequest request)
        {
            if (_data.IsDead) return; // 已修复编码乱码的注释。

            _damageQueue[_tail] = request;
            _tail = (_tail + 1) % _damageQueue.Length;

            //Debug.Log($"Damage enqueue amount {request.Amount} hp {_data.CurrentHealth}", _player);
        }

        /// <summary>
        // 已修复编码乱码的注释。
        /// </summary>
        public void Arbitrate()
        {
            if (_data.IsDead || _head == _tail) return;

            while (_head != _tail)
            {
                ref var req = ref _damageQueue[_head];

                float before = _data.CurrentHealth;
                // 已修复编码乱码的注释。
                _data.CurrentHealth -= req.Amount;

                //Debug.Log($"Damage apply amount {req.Amount} hp {before} -> {_data.CurrentHealth}", _player);

                _head = (_head + 1) % _damageQueue.Length;
            }

            // 已修复编码乱码的注释。
            if (_data.CurrentHealth <= 0)
            {
                _data.CurrentHealth = 0;
                _data.IsDead = true;

                _data.Arbitration.IsDead = true;
                _data.Arbitration.BlockInput = true;
                _data.Arbitration.BlockUpperBody = true;
                _data.Arbitration.BlockFacial = true;
                _data.Arbitration.BlockIK = true;
                _data.Arbitration.BlockInventory = true;

                //Debug.Log("Death trigger", _player);

                var death = _player.StateRegistry.GetState<PlayerDeathState>();
                _player.StateMachine.ChangeState(death);
            }
        }
    }
}
