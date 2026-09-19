using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace AwCon
{
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    public class AnimationVelocityAnalyzer : EditorWindow
    {
        // 已修复编码乱码的注释。
        private GameObject _targetPrefab;
        // 已修复编码乱码的注释。
        private AnimationClip _clip;
        // 已修复编码乱码的注释。
        private int _sampleRate = 60;

        // 已修复编码乱码的注释。
        private AnimationCurve _curveVelX = new AnimationCurve();
        private AnimationCurve _curveVelY = new AnimationCurve();
        private AnimationCurve _curveVelZ = new AnimationCurve();
        // 已修复编码乱码的注释。
        private AnimationCurve _curveSpeed = new AnimationCurve();

        // 已修复编码乱码的注释。
        private Vector2 _scrollPos;
        // 已修复编码乱码的注释。
        private float _maxSpeed = 1f;

        // 已修复编码乱码的注释。
        private float _animMaxHeight = 0f;
        // 已修复编码乱码的注释。
        private float _gravity = 9.81f;
        // 已修复编码乱码的注释。
        private float _recommendedForce = 0f;
        // 已修复编码乱码的注释。
        private float _timeToApex = 0f;

        [MenuItem("Tools/BBB-Nexus/Animation Velocity Analyzer")]
        public static void ShowWindow()
        {
            GetWindow<AnimationVelocityAnalyzer>("Root Motion Analyzer");
        }

        private void OnGUI()
        {
            GUILayout.Label("说明", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("说明", MessageType.Info);

            GUILayout.Space(10);

            // 已修复编码乱码的注释。
            _targetPrefab = (GameObject)EditorGUILayout.ObjectField("Character Prefab", _targetPrefab, typeof(GameObject), false);
            _clip = (AnimationClip)EditorGUILayout.ObjectField("Animation Clip", _clip, typeof(AnimationClip), false);

            // 已修复编码乱码的注释。
            GUILayout.BeginHorizontal();
            _sampleRate = EditorGUILayout.IntSlider("Sample Rate", _sampleRate, 30, 120);
            if (GUILayout.Button("Reset Gravity", GUILayout.Width(100))) _gravity = Mathf.Abs(Physics.gravity.y);
            GUILayout.EndHorizontal();

            _gravity = EditorGUILayout.FloatField("Gravity (g)", _gravity);

            GUILayout.Space(10);

            // 已修复编码乱码的注释。
            if (GUILayout.Button("Analyze Motion & Calculate Physics", GUILayout.Height(30)))
            {
                if (_targetPrefab && _clip) AnalyzeRootMotion();
                else EditorUtility.DisplayDialog("Error", "说明", "OK");
            }

            // 已修复编码乱码的注释。
            if (_animMaxHeight > 0.001f)
            {
                GUILayout.Space(15);
                EditorGUILayout.LabelField("说明", EditorStyles.boldLabel);

                // 已修复编码乱码的注释。
                GUI.backgroundColor = new Color(0.8f, 1f, 0.8f);
                EditorGUILayout.BeginVertical("box");

                // 已修复编码乱码的注释。
                EditorGUILayout.LabelField($"说明", $"{_animMaxHeight:F3} meters");
                // 已修复编码乱码的注释。
                EditorGUILayout.LabelField($"说明", $"{_timeToApex:F3} seconds");

                GUILayout.Space(5);
                // 已修复编码乱码的注释。
                EditorGUILayout.LabelField($"说明", $"{_recommendedForce:F2} m/s", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox($"说明", MessageType.None);

                EditorGUILayout.EndVertical();
                GUI.backgroundColor = Color.white;
            }

            // 已修复编码乱码的注释。
            GUILayout.Space(20);
            GUILayout.Label($"Velocity Curves (Max: {_maxSpeed:F2} m/s)", EditorStyles.boldLabel);

            // 已修复编码乱码的注释。
            GUILayout.BeginHorizontal();
            GUILayout.Label("X (Red)", EditorStyles.miniLabel);
            GUILayout.Label("Y (Green)", EditorStyles.miniLabel);
            GUILayout.Label("Z (Blue)", EditorStyles.miniLabel);
            GUILayout.Label("Speed (White)", EditorStyles.miniLabel);
            GUILayout.EndHorizontal();

            // 已修复编码乱码的注释。
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            DrawCurve("说明", _curveVelX, Color.red);
            DrawCurve("说明", _curveVelY, Color.green);
            DrawCurve("说明", _curveVelZ, Color.blue);
            DrawCurve("说明", _curveSpeed, Color.white);
            EditorGUILayout.EndScrollView();
        }

        // 已修复编码乱码的注释。
        private void DrawCurve(string label, AnimationCurve curve, Color color)
        {
            EditorGUILayout.LabelField(label, EditorStyles.miniBoldLabel);
            // 已修复编码乱码的注释。
            Rect rect = EditorGUILayout.GetControlRect(false, 30);
            EditorGUI.CurveField(rect, curve, color, new Rect(0, -_maxSpeed, _clip ? _clip.length : 1, _maxSpeed * 2));
        }

        // 已修复编码乱码的注释。
        private void AnalyzeRootMotion()
        {
            // 已修复编码乱码的注释。
            GameObject tempInstance = Instantiate(_targetPrefab, Vector3.zero, Quaternion.identity);
            tempInstance.hideFlags = HideFlags.HideAndDontSave;

            // 已修复编码乱码的注释。
            Animator animator = tempInstance.GetComponent<Animator>();
            if (!animator) { DestroyImmediate(tempInstance); return; }

            // 已修复编码乱码的注释。
            RuntimeAnimatorController originCtrl = animator.runtimeAnimatorController;
            if (originCtrl == null) { DestroyImmediate(tempInstance); Debug.LogError("说明"); return; }

            // 已修复编码乱码的注释。
            AnimatorOverrideController overrideCtrl = new AnimatorOverrideController(originCtrl);
            animator.runtimeAnimatorController = overrideCtrl;

            // 已修复编码乱码的注释。
            var clips = overrideCtrl.animationClips;
            if (clips.Length > 0)
            {
                var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                foreach (var c in clips) overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(c, _clip));
                overrideCtrl.ApplyOverrides(overrides);
            }

            // 已修复编码乱码的注释。
            _curveVelX = new AnimationCurve();
            _curveVelY = new AnimationCurve();
            _curveVelZ = new AnimationCurve();
            _curveSpeed = new AnimationCurve();
            _maxSpeed = 1f;

            // 已修复编码乱码的注释。
            float currentHeight = 0f;
            _animMaxHeight = 0f;
            _timeToApex = 0f;

            // 已修复编码乱码的注释。
            animator.applyRootMotion = true;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            animator.Update(0f);

            // 已修复编码乱码的注释。
            float frameRate = _sampleRate;
            float deltaTime = 1f / frameRate;
            int totalFrames = Mathf.CeilToInt(_clip.length * frameRate);

            // 已修复编码乱码的注释。
            for (int i = 0; i <= totalFrames; i++)
            {
                // 已修复编码乱码的注释。
                float time = i * deltaTime;
                animator.Update(deltaTime);
                // 已修复编码乱码的注释。
                if (i < 2) continue;

                // 已修复编码乱码的注释。
                Vector3 worldDelta = animator.deltaPosition;
                // 已修复编码乱码的注释。
                Vector3 localDelta = tempInstance.transform.InverseTransformVector(worldDelta);
                // 已修复编码乱码的注释。
                Vector3 velocity = localDelta / deltaTime;

                // 已修复编码乱码的注释。
                _curveVelX.AddKey(time, velocity.x);
                _curveVelY.AddKey(time, velocity.y);
                _curveVelZ.AddKey(time, velocity.z);

                // 已修复编码乱码的注释。
                float speed = velocity.magnitude;
                _curveSpeed.AddKey(time, speed);
                // 已修复编码乱码的注释。
                if (speed > _maxSpeed) _maxSpeed = speed;

                // 已修复编码乱码的注释。
                // 已修复编码乱码的注释。
                currentHeight += worldDelta.y;
                // 已修复编码乱码的注释。
                if (currentHeight > _animMaxHeight)
                {
                    _animMaxHeight = currentHeight;
                    _timeToApex = time;
                }
            }

            // 已修复编码乱码的注释。
            // 已修复编码乱码的注释。
            // 已修复编码乱码的注释。
            if (_animMaxHeight > 0.1f)
            {
                _recommendedForce = Mathf.Sqrt(2 * _gravity * _animMaxHeight);
            }
            else
            {
                _recommendedForce = 0f;
            }

            // 已修复编码乱码的注释。
            DestroyImmediate(tempInstance);
            Repaint();
            Debug.Log($"说明");
        }
    }
}
