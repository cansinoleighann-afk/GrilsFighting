using UnityEngine;
using UnityEngine.UI;

namespace CombatGirlsCharacterPack
{
    [System.Serializable]
    public class ObjectGroup
    {
        public GameObject[] objectsToToggle;
        public Button[] buttons;
    }

    public class ObjectController : MonoBehaviour
    {
        public ObjectGroup[] objectGroups;

        private void Start()
        {
            // 已修复编码乱码的注释。
            for (int groupIndex = 0; groupIndex < objectGroups.Length; groupIndex++)
            {
                ObjectGroup group = objectGroups[groupIndex];

                for (int buttonIndex = 0; buttonIndex < group.buttons.Length; buttonIndex++)
                {
                    int buttonIdx = buttonIndex; // 已修复编码乱码的注释。
                    group.buttons[buttonIdx].onClick.AddListener(() => ToggleObject(group, buttonIdx));
                }
            }
        }

        private void ToggleObject(ObjectGroup group, int buttonIndex)
        {
            // 已修复编码乱码的注释。
            if (buttonIndex >= 0 && buttonIndex < group.objectsToToggle.Length)
            {
                GameObject obj = group.objectsToToggle[buttonIndex];
                obj.SetActive(!obj.activeSelf); // 已修复编码乱码的注释。
            }
        }
    }
}
