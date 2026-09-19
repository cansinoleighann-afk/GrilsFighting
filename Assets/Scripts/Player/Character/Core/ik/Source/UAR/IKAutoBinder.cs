using UnityEngine;
#if BBBNEXUS_HAS_UAR
using UnityEngine.Animations.Rigging;
#endif
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AwCon
{
    /// <summary>
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    /// </summary>
    [ExecuteInEditMode]
    public class IKAutoBinder : MonoBehaviour
    {
#if BBBNEXUS_HAS_UAR
        [Header("角色设置")]
        [Tooltip("目标角色Animator")]
        public Animator TargetCharacter;

        [Header("IK源设置")]
        [Tooltip("Unity动画绑定源脚本")]
        public UnityAnimationRiggingSource SourceScript;

        [Space(10)]
        [Header("IK约束器")]
        public TwoBoneIKConstraint LeftHandIK;
        public TwoBoneIKConstraint RightHandIK;
        public MultiAimConstraint HeadAim;

#if UNITY_EDITOR
        [ContextMenu("Auto Bind Bones & Setup Rig")]
        public void BindToCharacter()
        {
            // 已修复编码乱码的注释。
            if (TargetCharacter == null)
            {
                TargetCharacter = GetComponentInParent<Animator>();
                if (TargetCharacter == null)
                {
                    Debug.LogError($"说明", this);
                    return;
                }
                Debug.Log($"说明");
            }

            // 已修复编码乱码的注释。
            RigBuilder characterRigBuilder = TargetCharacter.GetComponent<RigBuilder>();
            if (characterRigBuilder == null)
            {
                characterRigBuilder = TargetCharacter.gameObject.AddComponent<RigBuilder>();
                Debug.Log($"说明");
            }

            // 已修复编码乱码的注释。
            List<Object> objectsToRecord = new List<Object>
            {
                TargetCharacter.gameObject,
                characterRigBuilder
            };
            if (LeftHandIK != null) objectsToRecord.Add(LeftHandIK);
            if (RightHandIK != null) objectsToRecord.Add(RightHandIK);
            if (HeadAim != null) objectsToRecord.Add(HeadAim);
            if (SourceScript != null) objectsToRecord.Add(SourceScript);
            Undo.RecordObjects(objectsToRecord.ToArray(), "Bind IK System");

            // 已修复编码乱码的注释。
            var rigLayers = GetComponentsInChildren<Rig>(true);
            characterRigBuilder.layers.Clear();
            foreach (var rig in rigLayers)
            {
                characterRigBuilder.layers.Add(new RigLayer(rig, true));
            }
            EditorUtility.SetDirty(characterRigBuilder);
            Debug.Log($"说明");

            // 已修复编码乱码的注释。
            if (LeftHandIK != null)
            {
                var data = LeftHandIK.data;
                data.root = TargetCharacter.GetBoneTransform(HumanBodyBones.LeftUpperArm);
                data.mid = TargetCharacter.GetBoneTransform(HumanBodyBones.LeftLowerArm);
                data.tip = TargetCharacter.GetBoneTransform(HumanBodyBones.LeftHand);
                LeftHandIK.data = data;
                EditorUtility.SetDirty(LeftHandIK);
                Debug.Log($"说明"None"}");
            }
            else Debug.LogWarning("说明");

            // 已修复编码乱码的注释。
            if (RightHandIK != null)
            {
                var data = RightHandIK.data;
                data.root = TargetCharacter.GetBoneTransform(HumanBodyBones.RightUpperArm);
                data.mid = TargetCharacter.GetBoneTransform(HumanBodyBones.RightLowerArm);
                data.tip = TargetCharacter.GetBoneTransform(HumanBodyBones.RightHand);
                RightHandIK.data = data;
                EditorUtility.SetDirty(RightHandIK);
                Debug.Log($"说明"None"}");
            }
            else Debug.LogWarning("说明");

            // 已修复编码乱码的注释。
            if (HeadAim != null)
            {
                var data = HeadAim.data;
                data.constrainedObject = TargetCharacter.GetBoneTransform(HumanBodyBones.Head);
                HeadAim.data = data;
                EditorUtility.SetDirty(HeadAim);
                Debug.Log($"说明"None"}");
            }
            else Debug.LogWarning("说明");

            // 已修复编码乱码的注释。
            if (SourceScript != null)
            {
                SerializedObject so = new SerializedObject(SourceScript);
                so.Update();

                // 已修复编码乱码的注释。
                TrySetObjectReference(so, "_leftHandIK", LeftHandIK);
                TrySetObjectReference(so, "_rightHandIK", RightHandIK);
                TrySetObjectReference(so, "_headLookAtIK", HeadAim);

                // 已修复编码乱码的注释。
                if (LeftHandIK != null && LeftHandIK.data.target != null)
                    TrySetObjectReference(so, "_leftHandTarget", LeftHandIK.data.target);

                if (RightHandIK != null && RightHandIK.data.target != null)
                    TrySetObjectReference(so, "_rightHandTarget", RightHandIK.data.target);

                if (HeadAim != null && HeadAim.data.sourceObjects.Count > 0)
                    TrySetObjectReference(so, "_lookAtTarget", HeadAim.data.sourceObjects[0].transform);

                if (so.ApplyModifiedProperties())
                {
                    Debug.Log($"说明");
                }
            }
            else Debug.LogWarning("说明");

            // 已修复编码乱码的注释。
            characterRigBuilder.Build();

            Debug.Log($"说明", this);
        }

        private bool TrySetObjectReference(SerializedObject so, string propertyName, Object value)
        {
            var property = so.FindProperty(propertyName);
            if (property != null)
            {
                property.objectReferenceValue = value;
                return true;
            }
            else
            {
                Debug.LogWarning($"说明");
                return false;
            }
        }
#endif
#endif
    }

    // 已修复编码乱码的注释。
#if UNITY_EDITOR
    [CustomEditor(typeof(IKAutoBinder))]
    public class IKAutoBinderEditor : Editor
    {
#if BBBNEXUS_HAS_UAR
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            IKAutoBinder binder = (IKAutoBinder)target;

            GUILayout.Space(10);
            GUI.backgroundColor = new Color(0.6f, 1f, 0.6f);
            if (GUILayout.Button("Auto Bind Bones & Setup Rig", GUILayout.Height(35)))
            {
                binder.BindToCharacter();
            }
            GUI.backgroundColor = Color.white;
        }
#endif
    }


#endif
}