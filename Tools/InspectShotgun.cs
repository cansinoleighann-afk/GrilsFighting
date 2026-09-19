using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using AwCon;
using Funplay.Editor.Tools.Scripting;
public class InspectShotgun : IFunplayCommand
{
    public void Execute(ExecutionContext ctx)
    {
        var p = UnityEngine.Object.FindObjectOfType<BBBCharacterController>();
        if(p==null){ctx.ReturnValue="missing player";return;}
        var stage = PrefabStageUtility.GetCurrentPrefabStage();
        var list=new System.Collections.Generic.List<object>();
        foreach(var t in p.GetComponentsInChildren<Transform>(true))
            if(t.name.Contains("Weapon") || t.name.Contains("Socket") || t.name=="Muzzle")
                list.Add(new {name=t.name,path=AnimationUtility.CalculateTransformPath(t,p.transform),id=t.GetInstanceID(),prefab=PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(t.gameObject),pos=t.localPosition,rot=t.localEulerAngles});
        var text = new System.Text.StringBuilder();
        foreach(var t in p.GetComponentsInChildren<Transform>(true)) if(t.name.Contains("Weapon") || t.name.Contains("Socket")) text.AppendLine(AnimationUtility.CalculateTransformPath(t,p.transform)+" prefab="+PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(t.gameObject));
        ctx.ReturnValue=text.ToString();
    }
}
