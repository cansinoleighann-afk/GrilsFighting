namespace AwCon
{
    /// <summary>
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    /// </summary>
    [System.Serializable]
    public struct AnimPlayOptions
    {
        /// <summary>
        // 已修复编码乱码的注释。
        /// </summary>
        public int Layer;
        /// <summary>
        // 已修复编码乱码的注释。
        // 已修复编码乱码的注释。
        /// </summary>
        public float FadeDuration;

        /// <summary>
        // 已修复编码乱码的注释。
        // 已修复编码乱码的注释。
        // 已修复编码乱码的注释。
        /// </summary>
        public float Speed;

        /// <summary>
        // 已修复编码乱码的注释。
        // 已修复编码乱码的注释。
        /// </summary>
        public float NormalizedTime;

        /// <summary>
        // 已修复编码乱码的注释。
        /// </summary>
        public bool ForcePhaseSync;

        public static AnimPlayOptions Default => new AnimPlayOptions
        {
            Layer = 0,
            FadeDuration = -1f,
            Speed = -1f,
            NormalizedTime = -1f,
            ForcePhaseSync = false
        };

        public static AnimPlayOptions UpperBodyDefault => new AnimPlayOptions
        {
            Layer = 1,
            FadeDuration = -1f,
            Speed = -1f,
            NormalizedTime = -1f,
            ForcePhaseSync = false
        };
    }
}
