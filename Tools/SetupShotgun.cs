using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.InputSystem;
using AwCon;
using Funplay.Editor.Tools.Scripting;
public class SetupShotgun : IFunplayCommand
{
    const string CharacterPath="Assets/Resource/Role/ShotgunGirl_Chartacter.prefab";
    const string WeaponPath="Assets/_Res/Role/Shotgun_Girl/Prefab/Prefab_Parts/Weapon_Shotgun.prefab";
    const string ItemPath="Assets/_Res/Config/AwCon/Characters/Player/Modules/ShotgunItem.asset";
    const string Hand="root/pelvis/spine_01/spine_02/spine_03/clavicle_r/upperarm_r/lowerarm_r/hand_r";
    public void Execute(ExecutionContext ctx)
    {
        if(EditorApplication.isPlaying) throw new Exception("Exit Play Mode first");
        var source=UnityEngine.Object.FindObjectOfType<BBBCharacterController>();
        if(source==null || source.Config==null || source.Config.Shotgun==null) throw new Exception("Missing scene player or Shotgun module");
        var prefab=PrefabUtility.LoadPrefabContents(CharacterPath);
        try
        {
            var weapon=prefab.transform.Find(Hand+"/Weapon_Shotgun");
            if(weapon==null) throw new Exception("Missing nested Weapon_Shotgun");
            var originalPosition=weapon.localPosition; var originalRotation=weapon.localRotation;
            var animator=prefab.GetComponent<Animator>();
            if(animator==null || animator.avatar==null) throw new Exception("Missing humanoid avatar");
            // Sample an isolated prefab preview so grip calibration never changes the scene pose.
            var pose=source.Config.Shotgun.AimIdle.Clip;
            if(pose==null) throw new Exception("Missing aim pose");
            pose.SampleAnimation(prefab, 0.1f);
            var left=animator.GetBoneTransform(HumanBodyBones.LeftHand);
            if(left==null) throw new Exception("Missing left-hand bone");
            Vector3 grip=weapon.InverseTransformPoint(left.position);
            Quaternion gripRotation=Quaternion.Inverse(weapon.rotation)*left.rotation;
            var mesh=weapon.GetComponentInChildren<MeshFilter>(true);
            if(mesh==null || mesh.sharedMesh==null) throw new Exception("Missing shotgun mesh");
            Bounds b=mesh.sharedMesh.bounds;
            Vector3 extent=b.extents;
            int axis=extent.x>extent.y ? (extent.x>extent.z?0:2) : (extent.y>extent.z?1:2);
            Vector3 localAxis=Vector3.zero; localAxis[axis]=1f;
            Vector3 worldAxis=mesh.transform.TransformDirection(localAxis);
            if(Vector3.Dot(worldAxis,prefab.transform.forward)<0f){localAxis=-localAxis;worldAxis=-worldAxis;}
            Vector3 muzzlePosition=weapon.InverseTransformPoint(mesh.transform.TransformPoint(b.center+localAxis*(extent[axis]+0.015f)));
            Quaternion muzzleRotation=Quaternion.Inverse(weapon.rotation)*Quaternion.LookRotation(worldAxis,prefab.transform.up);
            var weaponAsset=PrefabUtility.LoadPrefabContents(WeaponPath);
            try
            {
                var goal=weaponAsset.transform.Find("LeftHandGoal"); var muzzle=weaponAsset.transform.Find("Muzzle");
                var behaviour=weaponAsset.GetComponent<ShotgunBehaviour>();
                if(goal==null || muzzle==null || behaviour==null) throw new Exception("Missing previously created weapon bindings");
                ctx.RegisterObjectModification(goal); ctx.RegisterObjectModification(muzzle);
                goal.SetLocalPositionAndRotation(grip,gripRotation);
                muzzle.SetLocalPositionAndRotation(muzzlePosition,muzzleRotation);
                PrefabUtility.SaveAsPrefabAsset(weaponAsset,WeaponPath);
            }
            finally { PrefabUtility.UnloadPrefabContents(weaponAsset); }
            ctx.ReturnValue="Grip calibrated="+grip+" muzzle="+muzzlePosition;
        }
        finally { PrefabUtility.UnloadPrefabContents(prefab); }

        var item=AssetDatabase.LoadAssetAtPath<ShotgunItemSO>(ItemPath);
        if(item==null){item=ScriptableObject.CreateInstance<ShotgunItemSO>();AssetDatabase.CreateAsset(item,ItemPath);}
        ctx.RegisterObjectModification(item);
        item.DisplayName="霰弹枪"; item.ItemID="shotgun"; item.MaxAmmo=8; item.StartingReserveAmmo=40;
        item.Prefab=AssetDatabase.LoadAssetAtPath<GameObject>(WeaponPath);
        item.HoldRotationOffset=Quaternion.identity;
        item.HoldPositionOffset=Vector3.zero;
        EditorUtility.SetDirty(item); AssetDatabase.SaveAssetIfDirty(item);

        var mapAsset=ScriptableObject.CreateInstance<InputActionAsset>();
        var map=mapAsset.AddActionMap("Weapons");
        var reload=map.AddAction("Reload",InputActionType.Button,"<Keyboard>/r");
        var cycle=map.AddAction("CycleWeapon",InputActionType.Button,"<Keyboard>/q");
        const string inputPath="Assets/_Res/Config/AwCon/Characters/Player/Modules/WeaponInput.asset";
        var existing=AssetDatabase.LoadAssetAtPath<InputActionAsset>(inputPath);
        if(existing==null){AssetDatabase.CreateAsset(mapAsset,inputPath);existing=mapAsset;}else{UnityEngine.Object.DestroyImmediate(mapAsset);}
        var reloadRef=InputActionReference.Create(existing.FindAction("Weapons/Reload",true));
        var cycleRef=InputActionReference.Create(existing.FindAction("Weapons/CycleWeapon",true));
        const string reloadPath="Assets/_Res/Config/AwCon/Characters/Player/Modules/ReloadInput.asset";
        const string cyclePath="Assets/_Res/Config/AwCon/Characters/Player/Modules/CycleWeaponInput.asset";
        if(AssetDatabase.LoadAssetAtPath<InputActionReference>(reloadPath)==null)AssetDatabase.CreateAsset(reloadRef,reloadPath);else{UnityEngine.Object.DestroyImmediate(reloadRef);reloadRef=AssetDatabase.LoadAssetAtPath<InputActionReference>(reloadPath);}
        if(AssetDatabase.LoadAssetAtPath<InputActionReference>(cyclePath)==null)AssetDatabase.CreateAsset(cycleRef,cyclePath);else{UnityEngine.Object.DestroyImmediate(cycleRef);cycleRef=AssetDatabase.LoadAssetAtPath<InputActionReference>(cyclePath);}
        ctx.RegisterObjectModification(source);
        source.DefaultEquipment1=item;
        var reader=source.InputSourceRef as PlayerInputReader;
        if(reader==null)throw new Exception("Scene input is not PlayerInputReader");
        ctx.RegisterObjectModification(reader);reader.reloadAction=reloadRef;reader.cycleWeaponAction=cycleRef;
        EditorUtility.SetDirty(source);EditorUtility.SetDirty(reader);
        PrefabUtility.RecordPrefabInstancePropertyModifications(source);
        PrefabUtility.RecordPrefabInstancePropertyModifications(reader);
        EditorSceneManager.MarkSceneDirty(source.gameObject.scene);
        EditorSceneManager.SaveScene(source.gameObject.scene);
        AssetDatabase.SaveAssets();
        ctx.ReturnValue += "; default shotgun and Q/R references saved to PlayerAnimationTest";
    }
}
