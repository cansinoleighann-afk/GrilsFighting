namespace AwCon
{
    /// <summary>
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    /// </summary>
    public class EojIntentProcessor
    {
        private readonly PlayerRuntimeData _data;
        private readonly InputPipeline _input;

        public EojIntentProcessor(PlayerRuntimeData data, InputPipeline inputPipeline)
        {
            _data = data;
            _input = inputPipeline;
        }

        /// <summary>
        // 已修复编码乱码的注释。
        /// </summary>
        public void Update(in ProcessedInputData input)
        {
            // 已修复编码乱码的注释。
            // 已修复编码乱码的注释。
            if (input.Expression1Pressed)
            {
                _data.FacialEventRequest = PlayerFacialEvent.QuickExpression1;
                _data.WantsExpression1 = true;
                _input?.ConsumeExpression1Pressed();
            }

            if (input.Expression2Pressed)
            {
                _data.FacialEventRequest = PlayerFacialEvent.QuickExpression2;
                _data.WantsExpression2 = true;
                _input?.ConsumeExpression2Pressed();
            }

            if (input.Expression3Pressed)
            {
                _data.FacialEventRequest = PlayerFacialEvent.QuickExpression3;
                _data.WantsExpression3 = true;
                _input?.ConsumeExpression3Pressed();
            }

            if (input.Expression4Pressed)
            {
                _data.FacialEventRequest = PlayerFacialEvent.QuickExpression4;
                _data.WantsExpression4 = true;
                _input?.ConsumeExpression4Pressed();
            }
        }
    }
}
