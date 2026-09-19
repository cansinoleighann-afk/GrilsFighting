using UnityEngine;

namespace AwCon
{
    // 已修复编码乱码的注释。
    [CreateAssetMenu(fileName = "ActionSO", menuName = "AwCon/Player/Modules/ActionSO")]
    public sealed class ActionSO : ScriptableObject
    {
        [Header("交互动作动画")]
        public AnimationClip Action1;
        public AnimationClip Action2;
        public AnimationClip Action3;
        public AnimationClip Action4;
        public AnimationClip Action5;
        public AnimationClip Action6;
        public AnimationClip Action7;
        public AnimationClip Action8;

        public const int ActionCount = 8;

        public AnimationClip GetClip(int index)
        {
            return index switch
            {
                0 => Action1,
                1 => Action2,
                2 => Action3,
                3 => Action4,
                4 => Action5,
                5 => Action6,
                6 => Action7,
                7 => Action8,
                _ => null
            };
        }
    }
}