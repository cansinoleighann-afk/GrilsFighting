using System;
using System.Collections.Generic;
using Animancer;
using CombatGirls.WeaponControl;
using CombatGirlsCharacterPack;
using RootMotion;
using RootMotion.FinalIK;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AwCon.EditorTools
{
    /// <summary>Sets up the common AwCon player component stack on a selected humanoid character root.</summary>
    public sealed class PlayerCharacterAutoSetupWindow : EditorWindow
    {
        private const string InputMapPath = "Assets/_Res/Config/InputMap.inputactions";

        [MenuItem("AwCon/Player Setup Tool")]
        private static void Open()
        {
            GetWindow<PlayerCharacterAutoSetupWindow>("Player Setup");
        }

        private void OnGUI()
        {
            var root = Selection.activeGameObject;
            EditorGUILayout.HelpBox(
                "Select a humanoid character root, then configure it. The tool adds the standard player components, resolves humanoid bones for Final IK and face shadow, and wires safe same-root/InputMap references. Weapon, item, camera, and custom renderer bindings remain author-controlled.",
                MessageType.Info);

            using (new EditorGUI.DisabledScope(root == null))
            {
                EditorGUILayout.ObjectField("Selected Root", root, typeof(GameObject), true);
                if (GUILayout.Button("Configure Selected Character"))
                {
                    Configure(root);
                }
            }
        }

        [MenuItem("AwCon/Configure Selected Player Character", true)]
        private static bool ValidateConfigureSelected() => Selection.activeGameObject != null;

        [MenuItem("AwCon/Configure Selected Player Character")]
        private static void ConfigureSelected() => Configure(Selection.activeGameObject);

        private static void Configure(GameObject root)
        {
            if (root == null)
            {
                Debug.LogError("[Player Setup] Select a character root first.");
                return;
            }

            var animator = root.GetComponent<Animator>();
            if (animator == null || !animator.isHuman || animator.avatar == null)
            {
                Debug.LogError($"[Player Setup] '{root.name}' needs an Animator with a valid Humanoid Avatar.", root);
                return;
            }

            var undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Configure Player Character");
            try
            {
                var weaponController = GetOrAdd<Character_Weapon_Controller>(root);
                var faceShadow = GetOrAdd<SDFFaceShadowController>(root);
                var equipmentVisual = GetOrAdd<EquipmentVisualSlot>(root);
                var animancerFacade = GetOrAdd<AnimancerFacade>(root);
                var animancer = GetOrAdd<AnimancerComponent>(root);
                var characterController = GetOrAdd<CharacterController>(root);
                var fullBodyIk = GetOrAdd<FullBodyBipedIK>(root);
                var grounder = GetOrAdd<GrounderFBBIK>(root);
                var aimIk = GetOrAdd<AimIK>(root);
                var ikSource = GetOrAdd<FinalIKSource>(root);
                var audioSource = GetOrAdd<AudioSource>(root);
                var inputReader = GetOrAdd<PlayerInputReader>(root);
                var player = GetOrAdd<BBBCharacterController>(root);
                var grounding = GetOrAdd<AwConGrounding>(root);
                var cameraManager = GetOrAdd<PlayerCameraManager>(root);

                ConfigureIk(root.transform, animator, fullBodyIk, grounder, aimIk);
                ConfigureFaceShadow(faceShadow, animator);
                ConfigureInput(inputReader);
                ConfigureCrossReferences(root, animator, animancer, fullBodyIk, aimIk, ikSource, player,
                    animancerFacade, audioSource, inputReader, equipmentVisual, cameraManager);

                // Kept referenced so all standard components are intentionally created and visible to the user.
                _ = weaponController;
                _ = characterController;
                _ = grounding;

                EditorUtility.SetDirty(root);
                Selection.activeGameObject = root;
                Debug.Log($"[Player Setup] Configured '{root.name}'. Review weapon slots, equipment assets, camera targets and face renderer selection before Play Mode.", root);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, root);
            }
            finally
            {
                Undo.CollapseUndoOperations(undoGroup);
            }
        }

        private static T GetOrAdd<T>(GameObject root) where T : Component
        {
            var component = root.GetComponent<T>();
            return component != null ? component : Undo.AddComponent<T>(root);
        }

        private static void ConfigureIk(Transform root, Animator animator, FullBodyBipedIK fullBodyIk,
            GrounderFBBIK grounder, AimIK aimIk)
        {
            var references = new BipedReferences();
            if (!BipedReferences.AutoDetectReferences(ref references, root, BipedReferences.AutoDetectParams.Default) ||
                !references.isFilled || references.spine == null || references.spine.Length == 0)
            {
                throw new InvalidOperationException("Final IK could not resolve a complete humanoid biped hierarchy.");
            }

            Undo.RecordObject(fullBodyIk, "Configure Full Body Biped IK");
            fullBodyIk.SetReferences(references, null);
            EditorUtility.SetDirty(fullBodyIk);

            Undo.RecordObject(grounder, "Configure Grounder Full Body Biped");
            grounder.ik = fullBodyIk;
            grounder.enabled = false; // Matches the current player template; AwConGrounding controls its weight.
            EditorUtility.SetDirty(grounder);

            var aimBones = new List<Transform>(references.spine);
            if (references.head != null) aimBones.Add(references.head);
            if (aimBones.Count < 2 || !aimIk.solver.SetChain(aimBones.ToArray(), root))
            {
                throw new InvalidOperationException("Aim IK could not build a spine-to-head chain.");
            }

            Undo.RecordObject(aimIk, "Configure Aim IK");
            aimIk.solver.transform = references.head;
            aimIk.solver.IKPositionWeight = 0f;
            EditorUtility.SetDirty(aimIk);
        }

        private static void ConfigureFaceShadow(SDFFaceShadowController faceShadow, Animator animator)
        {
            Undo.RecordObject(faceShadow, "Configure Face Shadow Bones");
            faceShadow.headBone = animator.GetBoneTransform(HumanBodyBones.Head);

            var renderers = new List<Renderer>();
            foreach (var renderer in animator.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            {
                if (renderer.name.IndexOf("face", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    renderers.Add(renderer);
                }
            }
            faceShadow.faceRenderers = renderers.ToArray();
            EditorUtility.SetDirty(faceShadow);
        }

        private static void ConfigureInput(PlayerInputReader inputReader)
        {
            var inputAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputMapPath);
            if (inputAsset == null)
            {
                Debug.LogWarning($"[Player Setup] Input map is missing: {InputMapPath}");
                return;
            }

            Undo.RecordObject(inputReader, "Configure Player Input Actions");
            var bindings = new Dictionary<string, string>
            {
                { "moveAction", "Player/Move" }, { "lookAction", "Player/Look" },
                { "jumpAction", "Player/Jump" }, { "sprintAction", "Player/Sprint" },
                { "walkAction", "Player/walk" }, { "aimAction", "Player/Aim" },
                { "crouchAction", "Player/Crouch" }, { "dodgeAction", "Player/dodge" },
                { "rollAction", "Player/roll" }, { "lockCameraAction", "Player/Lock" },
                { "actionAction", "Player/Action" }, { "LeftMouseAction", "Player/Attack" },
                { "reloadAction", "ReloadInput" }, { "cycleWeaponAction", "CycleWeaponInput" }
            };

            var serialized = new SerializedObject(inputReader);
            foreach (var binding in bindings)
            {
                var action = inputAsset.FindAction(binding.Value, false);
                if (action == null) continue;
                var property = serialized.FindProperty(binding.Key);
                if (property != null) property.objectReferenceValue = InputActionReference.Create(action);
            }
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(inputReader);
        }

        private static void ConfigureCrossReferences(GameObject root, Animator animator, AnimancerComponent animancer,
            FullBodyBipedIK fullBodyIk, AimIK aimIk, FinalIKSource ikSource, BBBCharacterController player,
            AnimancerFacade animancerFacade, AudioSource audioSource, PlayerInputReader inputReader,
            EquipmentVisualSlot equipmentVisual, PlayerCameraManager cameraManager)
        {
            SetObjectReference(animancer, "_Animator", animator);
            SetObjectReference(ikSource, "_fbbik", fullBodyIk);
            SetObjectReference(ikSource, "_aimIK", aimIk);
            SetObjectReference(ikSource, "_aimPivotFallback", animator.GetBoneTransform(HumanBodyBones.Head));
            SetObjectReference(equipmentVisual, "_player", player);
            SetObjectReference(cameraManager, "_player", player);

            Undo.RecordObject(player, "Wire Player Components");
            player.InputSourceRef = inputReader;
            player.AnimationFacadeRef = animancerFacade;
            player.IKSource = ikSource;
            player.SfxSource = audioSource;
            player.Animator = animator;
            EditorUtility.SetDirty(player);
        }

        private static void SetObjectReference(Component component, string propertyPath, UnityEngine.Object value)
        {
            Undo.RecordObject(component, "Wire Player Component Reference");
            var serialized = new SerializedObject(component);
            var property = serialized.FindProperty(propertyPath);
            if (property == null)
            {
                Debug.LogWarning($"[Player Setup] '{component.GetType().Name}' does not expose '{propertyPath}'.", component);
                return;
            }
            property.objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(component);
        }
    }
}
