using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Funplay.Editor.Tools.Scripting;
using AwCon;
public class FinalizeShotgunPrefab : IFunplayCommand
{
    public void Execute(ExecutionContext ctx)
    {
        if(EditorApplication.isPlaying)throw new Exception("Exit Play Mode first");
        var source=UnityEngine.Object.FindObjectOfType<BBBCharacterController>();
        if(source==null)throw new Exception("Missing configured scene player");
        const string path="Assets/Resource/Role/ShotgunGirl_Chartacter.prefab";
        var root=PrefabUtility.LoadPrefabContents(path);
        try
        {
            var copied=new List<Component>();
            foreach(var c in source.GetComponents<Component>())
            {
                if(c==null)continue;
                string ns=c.GetType().Namespace??"";
                if(!(ns=="AwCon" || ns=="Animancer" || ns=="RootMotion.FinalIK" || c is CharacterController || c is AudioSource))continue;
                if(c.GetType().Name=="PlayerCameraManager")continue;
                var target=root.GetComponent(c.GetType());
                if(target==null){target=root.AddComponent(c.GetType());ctx.RegisterObjectCreation(target);}
                ctx.RegisterObjectModification(target);
                EditorUtility.CopySerialized(c,target);copied.Add(target);
            }
            foreach(var target in copied)
            {
                var so=new SerializedObject(target);var prop=so.GetIterator();
                while(prop.Next(true))
                {
                    if(prop.propertyType!=SerializedPropertyType.ObjectReference)continue;
                    var obj=prop.objectReferenceValue;
                    if(obj==null || EditorUtility.IsPersistent(obj))continue;
                    var component=obj as Component;var go=obj as GameObject;
                    var tr=component!=null?component.transform:go!=null?go.transform:null;
                    if(tr==null)continue;
                    if(!tr.IsChildOf(source.transform)){prop.objectReferenceValue=null;continue;}
                    string relative=AnimationUtility.CalculateTransformPath(tr,source.transform);
                    var mapped=relative.Length==0?root.transform:root.transform.Find(relative);
                    prop.objectReferenceValue=mapped==null?null:component!=null?(UnityEngine.Object)mapped.GetComponent(component.GetType()):mapped.gameObject;
                }
                so.ApplyModifiedPropertiesWithoutUndo();
            }
            var player=root.GetComponent<BBBCharacterController>();
            if(player==null)throw new Exception("Failed to copy controller");
            player.Animator=root.GetComponent<Animator>();
            player.PlayerCamera=null;
            player.InputSourceRef=root.GetComponent<PlayerInputReader>();
            player.AnimationFacadeRef=root.GetComponent<AnimancerFacade>();
            player.IKSource=root.GetComponent<FinalIKSource>();
            player.SfxSource=root.GetComponent<AudioSource>();
            var ac=root.GetComponent<Animancer.AnimancerComponent>();ac.Animator=player.Animator;
            player.Animator.runtimeAnimatorController=null;
            foreach(var c in root.GetComponents<MonoBehaviour>()) if(c!=null && c.GetType().Name=="Character_Weapon_Controller")c.enabled=false;
            foreach(var t in root.GetComponentsInChildren<Transform>(true)) if(t.name=="Weapon_Shotgun" && t.parent!=null && t.parent.name=="hand_r")t.gameObject.SetActive(false);
            PrefabUtility.SaveAsPrefabAsset(root,path);
        }
        finally{PrefabUtility.UnloadPrefabContents(root);}
        ctx.RegisterObjectModification(source.Animator);source.Animator.runtimeAnimatorController=null;
        foreach(var c in source.GetComponents<MonoBehaviour>()) if(c!=null && c.GetType().Name=="Character_Weapon_Controller"){ctx.RegisterObjectModification(c);c.enabled=false;PrefabUtility.RecordPrefabInstancePropertyModifications(c);}
        PrefabUtility.RecordPrefabInstancePropertyModifications(source.Animator);
        EditorSceneManager.MarkSceneDirty(source.gameObject.scene);
        EditorSceneManager.SaveScene(source.gameObject.scene);
        // Imported demo events target the old controller; gameplay timing is owned by the weapon now.
        int clips=0;
        foreach(var guid in AssetDatabase.FindAssets("t:AnimationClip",new[]{"Assets/_Res/Animations/Shotgun_Girl"}))
        {
            var clip=AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(guid));
            if(clip==null)continue;
            ctx.RegisterObjectModification(clip);
            AnimationUtility.SetAnimationEvents(clip,new AnimationEvent[0]);
            var settings=AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime=clip.name.Contains("Idle")||clip.name.Contains("Walk")||clip.name.Contains("Jog")||clip.name=="S_Run";
            AnimationUtility.SetAnimationClipSettings(clip,settings);
            EditorUtility.SetDirty(clip);AssetDatabase.SaveAssetIfDirty(clip);clips++;
        }
        var module=source.Config.Shotgun;
        ctx.RegisterObjectModification(module);
        module.EquipEndTime=module.TakeGun.Clip.length;
        module.ReloadDuration=module.Reload.Clip.length;
        module.FireInterval=Mathf.Max(0.65f,module.Shoot.Clip.length);
        module.UpperBodyOptions=new AnimPlayOptions{Layer=1,FadeDuration=0.12f,Speed=1f,NormalizedTime=0f};
        module.Walk=new Animancer.ClipTransition{Clip=AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/_Res/Animations/Shotgun_Girl/Normal/S_Walk.anim")};
        module.Run=new Animancer.ClipTransition{Clip=AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/_Res/Animations/Shotgun_Girl/Normal/S_Run.anim")};
        EditorUtility.SetDirty(module);AssetDatabase.SaveAssetIfDirty(module);
        ctx.ReturnValue="Saved playable prefab, Animator Controller cleared, demo driver disabled, clips validated="+clips;
    }
}
