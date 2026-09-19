using UnityEngine;

namespace AwCon
{
    /// <summary>
    // 已修复编码乱码的注释。
    /// </summary>
    public sealed class ActionIntentProcessor
    {
        private readonly PlayerRuntimeData _data;

        public ActionIntentProcessor(PlayerRuntimeData data)
        {
            _data = data;
        }
        
        public void Update(in ProcessedInputData input)
        {
            if (input.ActionPressed)
            {
                _data.WantsToAction = true;
            }
        }
    }
}
