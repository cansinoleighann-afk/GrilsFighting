using UnityEngine;

namespace CombatGirlsCharacterPack
{
    public class WeaponMaterialChanger : MonoBehaviour
    {
        // 已修复编码乱码的注释。
        public Material[] materials;
        // 已修复编码乱码的注释。
        public GameObject targetObject;

        // 已修复编码乱码的注释。
        private int currentMaterialIndex = 0;

        // 已修复编码乱码的注释。
        public void ChangeMaterial()
        {
            if (materials.Length == 0 || targetObject == null) return;

            // 已修复编码乱码的注释。
            MeshRenderer renderer = targetObject.GetComponent<MeshRenderer>();

            if (renderer != null)
            {
                // 已修复编码乱码的注释。
                currentMaterialIndex = (currentMaterialIndex + 1) % materials.Length;
                renderer.material = materials[currentMaterialIndex];
            }
        }
    }
}
