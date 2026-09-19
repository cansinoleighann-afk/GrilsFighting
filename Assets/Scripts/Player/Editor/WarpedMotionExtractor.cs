using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Animancer;
using System;

namespace AwCon
{
    // 已修复编码乱码的注释。
    public class WarpedMotionExtractor : EditorWindow
    {
        [Header("目标设置")]
        private GameObject _targetPrefab;
        private PlayerSO _targetPlayerSO;
        private UnityEngine.Object _targetAsset;
        private int _sampleRate = 60;

        private WarpedType _batchTargetType = WarpedType.Simple;

        [MenuItem("Tools/BBB-Nexus/WarpedMotionBaker v3.4.1")]
        public static void ShowWindow()
        {
            GetWindow<WarpedMotionExtractor>("变形运动提取器");
        }

        private void OnGUI()
        {
            GUILayout.Label("变形运动烘焙工具", EditorStyles.boldLabel);

            _targetPrefab = (GameObject)EditorGUILayout.ObjectField("目标预制体", _targetPrefab, typeof(GameObject), false);
            _targetPlayerSO = (PlayerSO)EditorGUILayout.ObjectField("玩家SO配置", _targetPlayerSO, typeof(PlayerSO), false);
            _targetAsset = EditorGUILayout.ObjectField(new GUIContent("目标资源", "要烘焙的资源文件"), _targetAsset, typeof(UnityEngine.Object), false);
            _sampleRate = EditorGUILayout.IntSlider("采样率", _sampleRate, 30, 120);

            GUILayout.Space(15);

            GUI.backgroundColor = new Color(0.8f, 0.8f, 1f);
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("批量烘焙设置", EditorStyles.miniBoldLabel);
            _batchTargetType = (WarpedType)EditorGUILayout.EnumPopup("变形类型", _batchTargetType);

            if (GUILayout.Button("说明"))
            {
                SetAllFieldsToType(_batchTargetType);
            }
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = Color.white;

            GUILayout.Space(20);

            bool canBake = _targetPrefab != null && (_targetAsset != null || _targetPlayerSO != null);
            GUI.backgroundColor = canBake ? new Color(0.6f, 1f, 0.6f) : Color.white;
            GUI.enabled = canBake;
            if (GUILayout.Button("说明", GUILayout.Height(40)))
            {
                BakeAllWarpedDataInSO();
            }
            GUI.enabled = true;
            GUI.backgroundColor = Color.white;

            EditorGUILayout.HelpBox("说明", MessageType.Info);
            EditorGUILayout.HelpBox("说明", MessageType.Info);
        }

        // 已修复编码乱码的注释。
        private void ScanWarpedMotionDataRecursive(object target, Action<WarpedMotionData, FieldInfo, object> onFound)
        {
            if (target == null) return;
            var type = target.GetType();
            if (!typeof(UnityEngine.Object).IsAssignableFrom(type) && !type.IsClass) return;

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                var value = field.GetValue(target);
                if (value == null) continue;

                if (field.FieldType == typeof(WarpedMotionData) || value is WarpedMotionData)
                {
                    onFound((WarpedMotionData)value, field, target);
                }
                else if (value is ScriptableObject so)
                {
                    ScanWarpedMotionDataRecursive(so, onFound);
                }
                else if (typeof(System.Collections.IEnumerable).IsAssignableFrom(field.FieldType) && field.FieldType != typeof(string))
                {
                    var enumerable = value as System.Collections.IEnumerable;
                    if (enumerable != null)
                    {
                        foreach (var item in enumerable)
                        {
                            if (item == null) continue;
                            if (item is WarpedMotionData wmd)
                            {
                                onFound(wmd, field, target);
                            }
                            else if (item is ScriptableObject itemSo)
                            {
                                ScanWarpedMotionDataRecursive(itemSo, onFound);
                            }
                        }
                    }
                }
            }
        }

        // 已修复编码乱码的注释。
        private void SetAllFieldsToType(WarpedType targetType)
        {
            var root = _targetAsset != null ? _targetAsset : (UnityEngine.Object)_targetPlayerSO;
            if (root == null) return;

            Undo.RecordObject(root, "Batch Set Warped Type");
            int count = 0;
            ScanWarpedMotionDataRecursive(root, (data, field, owner) => {
                if (data != null)
                {
                    data.Type = targetType;
                    count++;
                }
            });

            EditorUtility.SetDirty(root);
            Debug.Log($"说明");
        }

        // 已修复编码乱码的注释。
        private void BakeAllWarpedDataInSO()
        {
            var root = _targetAsset != null ? _targetAsset : (UnityEngine.Object)_targetPlayerSO;
            if (root == null) return;

            Undo.RecordObject(root, "Bake All Warped Motion Data");
            var allFields = new List<(WarpedMotionData, FieldInfo, object)>();
            ScanWarpedMotionDataRecursive(root, (data, field, owner) => {
                if (data != null)
                    allFields.Add((data, field, owner));
            });

            if (allFields.Count == 0) return;

            int successCount = 0;
            bool anyChange = false;

            for (int i = 0; i < allFields.Count; i++)
            {
                var (originalData, fieldInfo, ownerObj) = allFields[i];
                EditorUtility.DisplayProgressBar("说明", $"说明", (float)i / allFields.Count);

                if (originalData == null || originalData.Clip == null || originalData.Clip.Clip == null) continue;
                if (originalData.Type == WarpedType.None && (originalData.WarpPoints == null || originalData.WarpPoints.Count == 0)) continue;

                AnimationClip animClip = originalData.Clip.Clip;
                // 已修复编码乱码的注释。
                WarpedMotionData bakedData = new WarpedMotionData();
                bakedData.Clip = originalData.Clip;
                bakedData.EndTime = originalData.EndTime;
                bakedData.EndPhase = originalData.EndPhase;
                bakedData.Type = originalData.Type;
                bakedData.BakedDuration = animClip.length;
                bakedData.HandIKWeightCurve = new AnimationCurve(originalData.HandIKWeightCurve.keys);

                // 已修复编码乱码的注释。
                if (originalData.Type == WarpedType.None)
                {
                    bakedData.WarpPoints = originalData.WarpPoints.Select(wp => new WarpPointDef
                    {
                        PointName = wp.PointName,
                        NormalizedTime = wp.NormalizedTime,
                        TargetPositionOffset = wp.TargetPositionOffset
                    }).ToList();
                }
                else if (originalData.Type == WarpedType.Custom)
                {
                    bakedData.WarpPoints = originalData.WarpPoints.Select(wp => new WarpPointDef
                    {
                        PointName = wp.PointName,
                        NormalizedTime = wp.NormalizedTime,
                        TargetPositionOffset = wp.TargetPositionOffset
                    }).ToList();
                }

                // 已修复编码乱码的注释。
                if (BakeSingleWarpedData(bakedData, animClip))
                {
                    fieldInfo.SetValue(ownerObj, bakedData);
                    successCount++;
                    anyChange = true;
                }
            }

            EditorUtility.ClearProgressBar();
            if (anyChange)
            {
                EditorUtility.SetDirty(root);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("说明", $"说明", "说明");
            }
        }

        // 已修复编码乱码的注释。
        private bool BakeSingleWarpedData(WarpedMotionData warpData, AnimationClip clip)
        {
            // 已修复编码乱码的注释。
            GameObject tempInstance = Instantiate(_targetPrefab, Vector3.zero, Quaternion.identity);
            tempInstance.hideFlags = HideFlags.HideAndDontSave;
            Animator animator = tempInstance.GetComponent<Animator>();
            if (!animator || animator.runtimeAnimatorController == null) { DestroyImmediate(tempInstance); return false; }

            // 已修复编码乱码的注释。
            var overrideCtrl = new AnimatorOverrideController(animator.runtimeAnimatorController);
            animator.runtimeAnimatorController = overrideCtrl;
            var clips = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            foreach (var c in overrideCtrl.animationClips) clips.Add(new KeyValuePair<AnimationClip, AnimationClip>(c, clip));
            overrideCtrl.ApplyOverrides(clips);

            animator.applyRootMotion = true;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            animator.Update(0f);

            float deltaTime = 1f / _sampleRate;
            int totalFrames = Mathf.CeilToInt(clip.length * _sampleRate);
            AnimationCurve curveX = new AnimationCurve(), curveY = new AnimationCurve(), curveZ = new AnimationCurve(), curveRotY = new AnimationCurve();
            Vector3[] absolutePositions = new Vector3[totalFrames + 1];
            Vector3 totalOffset = Vector3.zero;

            // 已修复编码乱码的注释。
            for (int i = 0; i <= totalFrames; i++)
            {
                float time = i * deltaTime;
                float normalizedTime = Mathf.Clamp01(time / clip.length);
                animator.Update(deltaTime);
                if (i < 2) { absolutePositions[i] = Vector3.zero; continue; }

                // 已修复编码乱码的注释。
                Vector3 worldDelta = animator.deltaPosition;
                Quaternion worldDeltaRot = animator.deltaRotation;
                Vector3 localDelta = tempInstance.transform.InverseTransformVector(worldDelta);
                Vector3 localVel = localDelta / deltaTime;

                float rotVelY = worldDeltaRot.eulerAngles.y;
                if (rotVelY > 180f) rotVelY -= 360f;

                // 已修复编码乱码的注释。
                curveX.AddKey(normalizedTime, localVel.x);
                curveY.AddKey(normalizedTime, localVel.y);
                curveZ.AddKey(normalizedTime, localVel.z);
                curveRotY.AddKey(normalizedTime, rotVelY / deltaTime);

                totalOffset += localDelta;
                absolutePositions[i] = totalOffset;

                // 已修复编码乱码的注释。
                tempInstance.transform.Translate(worldDelta, Space.World);
                tempInstance.transform.Rotate(worldDeltaRot.eulerAngles, Space.World);
            }

            // 已修复编码乱码的注释。
            if (warpData.Type != WarpedType.None)
            {
                if (warpData.Type != WarpedType.Custom)
                {
                    warpData.WarpPoints.Clear();

                    if (warpData.Type == WarpedType.Vault)
                    {
                        // 已修复编码乱码的注释。
                        float maxY = -999f; int apexIndex = 0;
                        for (int i = 0; i < absolutePositions.Length; i++) { if (absolutePositions[i].y > maxY) { maxY = absolutePositions[i].y; apexIndex = i; } }
                        warpData.WarpPoints.Add(new WarpPointDef { PointName = "Apex", NormalizedTime = (float)apexIndex / totalFrames, BakedLocalOffset = absolutePositions[apexIndex] });
                    }
                    else if (warpData.Type == WarpedType.Dodge)
                    {
                        // 已修复编码乱码的注释。
                        float maxXZ = -999f; int dodgeIndex = 0;
                        for (int i = 0; i < absolutePositions.Length; i++) { float dist = new Vector2(absolutePositions[i].x, absolutePositions[i].z).magnitude; if (dist > maxXZ) { maxXZ = dist; dodgeIndex = i; } }
                        warpData.WarpPoints.Add(new WarpPointDef { PointName = "MaxDodge", NormalizedTime = (float)dodgeIndex / totalFrames, BakedLocalOffset = absolutePositions[dodgeIndex] });
                    }
                }
            }

            // 已修复编码乱码的注释。
            if (!warpData.WarpPoints.Any(wp => wp.NormalizedTime >= 0.98f))
            {
                if (warpData.Type != WarpedType.Custom)
                {
                    warpData.WarpPoints.Add(new WarpPointDef { PointName = "End", NormalizedTime = 1.0f, BakedLocalOffset = totalOffset });
                }
            }

            warpData.WarpPoints = warpData.WarpPoints.OrderBy(wp => wp.NormalizedTime).ToList();

            // 已修复编码乱码的注释。
            Vector3 lastAbsPos = Vector3.zero;
            for (int k = 0; k < warpData.WarpPoints.Count; k++)
            {
                var wp = warpData.WarpPoints[k];

                // 已修复编码乱码的注释。
                if (warpData.Type == WarpedType.Custom && wp.BakedLocalOffset == Vector3.zero)
                {
                    int frameIndex = Mathf.RoundToInt(wp.NormalizedTime * totalFrames);
                    frameIndex = Mathf.Clamp(frameIndex, 0, absolutePositions.Length - 1);
                    wp.BakedLocalOffset = absolutePositions[frameIndex];
                }

                Vector3 currentAbsPos = wp.BakedLocalOffset;
                wp.BakedLocalOffset = currentAbsPos - lastAbsPos;
                warpData.WarpPoints[k] = wp;
                lastAbsPos = currentAbsPos;
            }

            // 已修复编码乱码的注释。
            warpData.LocalVelocityX = curveX;
            warpData.LocalVelocityY = curveY;
            warpData.LocalVelocityZ = curveZ;
            warpData.LocalRotationY = curveRotY;
            warpData.TotalBakedLocalOffset = totalOffset;

            DestroyImmediate(tempInstance);
            return true;
        }
    }
}