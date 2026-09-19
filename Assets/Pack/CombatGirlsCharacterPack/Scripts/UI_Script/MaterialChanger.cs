using System.Collections.Generic;
using UnityEngine;

namespace CombatGirlsCharacterPack
{
    public class MaterialChanger : MonoBehaviour
    {
        [SerializeField] private List<SkinnedMeshRenderer> characterMeshRenderers; // 已修复编码乱码的注释。
        [SerializeField] private List<Material> materials; // 已修复编码乱码的注释。

        private int currentMaterialIndex = 0; // 已修复编码乱码的注释。

        public void ChangeMaterial()
        {
            if (materials.Count == 0 || characterMeshRenderers.Count == 0)
                return; // 已修复编码乱码的注释。

            // 已修复编码乱码的注释。
            foreach (SkinnedMeshRenderer renderer in characterMeshRenderers)
            {
                renderer.material = materials[currentMaterialIndex];
            }

            // 已修复编码乱码的注释。
            currentMaterialIndex = (currentMaterialIndex + 1) % materials.Count;
        }
    }
}
