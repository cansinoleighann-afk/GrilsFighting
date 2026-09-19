using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AwCon
{
    /// <summary>
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    /// </summary>
    public sealed class ActionController
    {
        private readonly BBBCharacterController _player;
        private readonly PlayerRuntimeData _data;
        private readonly PlayerSO _config;
        private readonly InputPipeline _input;

        private int _index;

        // 已修复编码乱码的注释。
        private const int DefaultPriority = 25;

        public ActionController(BBBCharacterController player)
        {
            _player = player;
            _data = player.RuntimeData;
            _config = player.Config;
            _input = player.InputPipeline;
            _index = 0;
        }

        public void Update()
        {
            if (_data == null || _config == null || _input == null) return;
            if (_config.Action == null) return;

            if (_data.Arbitration.BlockAction) return;

            if (!_data.WantsToAction) return;

            // 已修复编码乱码的注释。
            _input.ConsumeActionPressed();

            var clip = _config.Action.GetClip(_index);
            _index = (_index + 1) % ActionSO.ActionCount;

            if (clip == null) return;

            // 已修复编码乱码的注释。
            var req = new ActionRequest(clip, DefaultPriority, 0.15f, true);
            _player.RequestOverride(in req, flushImmediately: true);
        }
    }
}
