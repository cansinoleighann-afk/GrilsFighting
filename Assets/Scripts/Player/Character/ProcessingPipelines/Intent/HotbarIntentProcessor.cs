namespace AwCon
{
    // 已修复编码乱码的注释。
    public class HotbarIntentProcessor
    {
        private readonly PlayerRuntimeData _data;

        public HotbarIntentProcessor(PlayerRuntimeData data)
        {
            _data = data;
        }

        public void Update(in ProcessedInputData input)
        {
            if (input.Number1Pressed)
            {
                _data.WantsToEquipHotbarIndex = 0;
            }
            else if (input.Number2Pressed)
            {
                _data.WantsToEquipHotbarIndex = 1;
            }
            else if (input.Number3Pressed)
            {
                _data.WantsToEquipHotbarIndex = 2;
            }
            else if (input.Number4Pressed)
            {
                _data.WantsToEquipHotbarIndex = 3;
            }
            else if (input.Number5Pressed)
            {
                _data.WantsToEquipHotbarIndex = 4;
            }
        }
    }
}
