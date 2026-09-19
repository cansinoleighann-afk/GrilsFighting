using UnityEngine;
using UnityEngine.UI;

namespace CombatGirlsCharacterPack
{
    public class AnimatorControl : MonoBehaviour
    {
        private Animator animator;
        public Toggle rootMotionToggle; // 已修复编码乱码的注释。

        private void Start()
        {
            // 已修复编码乱码的注释。
            animator = GetComponent<Animator>();

            // 已修复编码乱码的注释。
            rootMotionToggle.onValueChanged.AddListener(ToggleRootMotion);
        }

        public void ToggleRootMotion(bool enableRootMotion)
        {
            // 已修复编码乱码的注释。
            animator.applyRootMotion = enableRootMotion;
        }
    }
}
