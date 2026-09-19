using System.Collections.Generic;
using UnityEngine;

namespace CombatGirlsCharacterPack
{
    public class ObjectToggle : MonoBehaviour
    {
        [SerializeField] private List<GameObject> objects; // 已修复编码乱码的注释。
        private int currentIndex = 0; // 已修复编码乱码的注释。

        public void ToggleObjects()
        {
            if (objects.Count == 0)
                return; // 已修复编码乱码的注释。

            // 已修复编码乱码的注释。
            objects[currentIndex].SetActive(false);

            // 已修复编码乱码的注释。
            currentIndex = (currentIndex + 1) % objects.Count;

            // 已修复编码乱码的注释。
            objects[currentIndex].SetActive(true);
        }
    }
}
