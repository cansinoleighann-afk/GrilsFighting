using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using AwCon.Fight;
using AwCon.UI;

namespace AwCon.EditorTools
{
    /// <summary>Creates the small playable migration baseline from known source assets.</summary>
    public static class FightMigrationBuilder
    {
        const string Config = "Assets/_Res/Fight/Config";
        const string Pages = "Assets/Resource/UI/Pages";

        [MenuItem("AwCon/Fight/Rebuild Migration Baseline")]
        public static void Build()
        {
            Folder("Assets/_Res/Fight"); Folder(Config); Folder("Assets/Resource/UI"); Folder(Pages);
            ClearLegacyAnimationEvents();
            var punch = Asset<CombatMoveSO>(Config + "/Move_Punch.asset"); SetMove(punch, "FacePunchRight1", 14, false);
            var kick = Asset<CombatMoveSO>(Config + "/Move_Kick.asset"); SetMove(kick, "FaceKickRight1", 20, true);
            var player = Asset<FighterDefinitionSO>(Config + "/PlayerTwinKunai.asset"); SetFighter(player, "TwinKunai Girl", "Assets/_Res/Role/TwinKunai_Girl/Prefab/TwinKunai_FullBody.prefab", 140, 4.8f, punch, kick);
            var enemy = Asset<FighterDefinitionSO>(Config + "/EnemyHumanoidBot.asset"); SetFighter(enemy, "Humanoid Bot", "Assets/_Res/Role/Humanoid_Bot/Prefab/Humanoid_F_Rifle.prefab", 75, 3.8f, punch, kick);
            var boss = Asset<FighterDefinitionSO>(Config + "/BossDualKatana.asset"); SetFighter(boss, "DualKatana Girl", "Assets/_Res/Role/DualKatana_Girl/Prefab/Dual_Katana_Girl.prefab", 220, 4.2f, punch, kick);
            var level = Asset<FightLevelSO>(Config + "/Level01.asset"); level.player = player; level.enemy = enemy; level.boss = boss; level.environmentPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/levels/Level1.prefab"); level.playerSpawnPosition = new Vector3(-2, 0, 0); level.enemySpawnPosition = new Vector3(2, 0, -1); level.waves = new[] { new FightLevelSO.Wave { enemies = 2 }, new FightLevelSO.Wave { enemies = 1, boss = true } }; EditorUtility.SetDirty(level);
            var splash = SavePage("PF_SplashScreen", MakePage("Splash", "少女格斗", new Color(.08f,.12f,.24f), false));
            var menu = SavePage("PF_MainMenu", MakePage("Menu", "少女格斗", new Color(.1f,.18f,.3f), true));
            var hud = SavePage("PF_FightHUD", MakeHud());
            BuildLegacySplash(); BuildLegacyMenu(); BuildLegacyArena(); BuildArena("Assets/_Scenes/Test/PlayerAnimationTest.unity", level, hud); BuildArena("Assets/_Scenes/Test/LevelTest.unity", level, hud);
            EditorBuildSettings.scenes = new[] { Scene("Assets/_Scenes/SplashScene.unity"), Scene("Assets/_Scenes/Game/Menu.unity"), Scene("Assets/_Scenes/Game/Level.unity"), Scene("Assets/_Scenes/Test/PlayerAnimationTest.unity"), Scene("Assets/_Scenes/Test/LevelTest.unity") };
            AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
            Debug.Log("Fight migration baseline rebuilt.");
        }

        static void ClearLegacyAnimationEvents()
        {
            foreach (var guid in AssetDatabase.FindAssets("t:AnimationClip", new[] { "Assets/_Res/Animations/FightingWx" }))
            {
                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(guid));
                if (clip == null || AnimationUtility.GetAnimationEvents(clip).Length == 0) continue;
                AnimationUtility.SetAnimationEvents(clip, new AnimationEvent[0]);
                EditorUtility.SetDirty(clip);
            }
        }

        static EditorBuildSettingsScene Scene(string path) => new EditorBuildSettingsScene(path, true);
        static void Folder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            var slash = path.LastIndexOf('/'); AssetDatabase.CreateFolder(path.Substring(0, slash), path.Substring(slash + 1));
        }
        static T Asset<T>(string path) where T : ScriptableObject
        {
            var item = AssetDatabase.LoadAssetAtPath<T>(path);
            if (item != null) return item;
            item = ScriptableObject.CreateInstance<T>(); AssetDatabase.CreateAsset(item, path); return item;
        }
        static AnimationClip Clip(string name) => AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/_Res/Animations/FightingWx/" + name + ".anim");
        static void SetMove(CombatMoveSO move, string clip, float damage, bool knockdown)
        {
            move.clip = Clip(clip); move.speed = 1; move.fade = .1f; move.damage = damage; move.hitStart = .27f; move.hitEnd = .52f; move.comboStart = .58f; move.reach = 1.8f; move.radius = .8f; move.knockback = knockdown ? 5 : 2.5f; move.knockdown = knockdown; EditorUtility.SetDirty(move);
        }
        static void SetFighter(FighterDefinitionSO item, string name, string prefab, float health, float speed, CombatMoveSO punch, CombatMoveSO kick)
        {
            item.displayName = name; item.modelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefab); item.health = health; item.moveSpeed = speed; item.aiAttackInterval = 1.1f;
            item.idle = Clip("Idle0"); item.walk = name == "TwinKunai Girl" ? AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/_Res/Role/TwinKunai_Girl/Animations/Normal/TK_Walk.anim") : Clip("Walk"); item.hit = Clip("Hit1"); item.knockdown = Clip("KnockDown_Down"); item.standUp = Clip("StandUp"); item.death = Clip("Death"); item.jump = Clip("JumpUp"); item.dodge = Clip("EskivBodyA"); item.guard = Clip("Defend"); item.grab = Clip("Grab"); item.grabbed = Clip("Grabbed"); item.punches = new[] { punch }; item.kicks = new[] { kick }; item.jumpAttack = punch; item.groundAttack = kick; EditorUtility.SetDirty(item);
        }
        static Canvas Canvas()
        {
            var go = new GameObject("UIRoot", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(UIRoot));
            go.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = go.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution = new Vector2(1559, 720); scaler.matchWidthOrHeight = .5f;
            if (Object.FindObjectOfType<EventSystem>() == null) new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule)); return go.GetComponent<Canvas>();
        }
        static Text Label(Transform parent, string name, string text, int size, Vector2 min, Vector2 max)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text)); go.transform.SetParent(parent, false); var rect = go.GetComponent<RectTransform>(); rect.anchorMin = min; rect.anchorMax = max; rect.offsetMin = rect.offsetMax = Vector2.zero;
            var label = go.GetComponent<Text>(); label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); label.text = text; label.fontSize = size; label.alignment = TextAnchor.MiddleCenter; label.color = Color.white; return label;
        }
        static Button Button(Transform parent, string name, string text, Vector2 position, string scene)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button), typeof(SceneButton)); go.transform.SetParent(parent, false); var rect = go.GetComponent<RectTransform>(); rect.anchorMin = rect.anchorMax = new Vector2(.5f,.5f); rect.sizeDelta = new Vector2(320,80); rect.anchoredPosition = position; go.GetComponent<Image>().color = new Color(.1f,.35f,.65f,.95f); Label(go.transform,"Label",text,28,Vector2.zero,Vector2.one);
            var destination = go.GetComponent<SceneButton>(); var serial = new SerializedObject(destination); serial.FindProperty("sceneName").stringValue = scene; serial.ApplyModifiedPropertiesWithoutUndo(); UnityEventTools.AddPersistentListener(go.GetComponent<Button>().onClick, destination.Load); return go.GetComponent<Button>();
        }
        static GameObject MakePage(string id, string title, Color color, bool menu)
        {
            var root = new GameObject(id, typeof(RectTransform), typeof(CanvasGroup), typeof(UIScreen), typeof(Image)); var rect = root.GetComponent<RectTransform>(); rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = rect.offsetMax = Vector2.zero; root.GetComponent<Image>().color = color; Label(root.transform,"Title",title,54,new Vector2(.2f,.62f),new Vector2(.8f,.78f));
            if (menu) { Button(root.transform,"Play","开始战斗",new Vector2(0,-60),"Level"); Button(root.transform,"Test","动画测试",new Vector2(0,-160),"PlayerAnimationTest"); }
            var serial = new SerializedObject(root.GetComponent<UIScreen>()); serial.FindProperty("screenId").stringValue = id; serial.ApplyModifiedPropertiesWithoutUndo(); return root;
        }
        static GameObject MakeHud()
        {
            var root = new GameObject("HUD", typeof(RectTransform), typeof(CanvasGroup), typeof(UIScreen), typeof(Image)); var rect = root.GetComponent<RectTransform>(); rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = rect.offsetMax = Vector2.zero; root.GetComponent<Image>().color = Color.clear; Label(root.transform,"Status","第 1 / 2 波",28,new Vector2(.35f,.88f),new Vector2(.65f,.96f)); Label(root.transform,"PlayerHealth","玩家 140",24,new Vector2(.02f,.88f),new Vector2(.25f,.95f)); Label(root.transform,"EnemyHealth","",24,new Vector2(.75f,.88f),new Vector2(.98f,.95f)); Label(root.transform,"Controls","WASD 移动  J 拳击  K 踢击  空格 跳跃  Shift 闪避  L 抓取  E 拾取  Q 投掷  U 格挡",18,new Vector2(.08f,.02f),new Vector2(.92f,.08f));
            var serial = new SerializedObject(root.GetComponent<UIScreen>()); serial.FindProperty("screenId").stringValue = "HUD"; serial.ApplyModifiedPropertiesWithoutUndo(); return root;
        }
        static GameObject SavePage(string name, GameObject source)
        {
            var result = PrefabUtility.SaveAsPrefabAsset(source, Pages + "/" + name + ".prefab"); Object.DestroyImmediate(source); return result;
        }
        static void CameraAndArena()
        {
            var camera = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener)); camera.tag = "MainCamera"; camera.transform.position = new Vector3(0,7,-14); camera.transform.rotation = Quaternion.Euler(22,0,0); camera.GetComponent<Camera>().backgroundColor = new Color(.08f,.11f,.16f);
            var light = new GameObject("Directional Light", typeof(Light)); light.transform.rotation = Quaternion.Euler(50,-30,0); light.GetComponent<Light>().type = LightType.Directional; light.GetComponent<Light>().intensity = 1.3f;
            var floor = GameObject.CreatePrimitive(PrimitiveType.Cube); floor.name="FightFloor"; floor.transform.position=new Vector3(0,-.5f,0); floor.transform.localScale=new Vector3(22,1,9); floor.GetComponent<Renderer>().material.color=new Color(.16f,.2f,.25f);
            var crate = GameObject.CreatePrimitive(PrimitiveType.Cube); crate.name="PickupCrate"; crate.transform.position=new Vector3(0,.6f,1); crate.transform.localScale=Vector3.one*.8f; crate.AddComponent<Rigidbody>(); crate.AddComponent<FightProp>(); crate.GetComponent<Renderer>().material.color=new Color(.7f,.35f,.1f);
        }
        static void BuildArena(string path, FightLevelSO level, GameObject hud)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single); CameraAndArena(); var canvas=Canvas(); var ui=(GameObject)PrefabUtility.InstantiatePrefab(hud, canvas.transform); var go=new GameObject("FightSession",typeof(FightSession)); var session=go.GetComponent<FightSession>(); session.level=level; session.statusText=ui.transform.Find("Status").GetComponent<Text>(); session.playerHealthText=ui.transform.Find("PlayerHealth").GetComponent<Text>(); session.enemyHealthText=ui.transform.Find("EnemyHealth").GetComponent<Text>(); EditorSceneManager.SaveScene(scene,path);
        }
        static void CopyLegacyScene(string source, string destination)
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(destination) != null) AssetDatabase.MoveAssetToTrash(destination);
            if (!AssetDatabase.CopyAsset(source, destination)) throw new System.InvalidOperationException("Could not copy legacy scene: " + source);
        }
        static void BuildLegacySplash()
        {
            const string target = "Assets/_Scenes/SplashScene.unity";
            CopyLegacyScene("Assets/_Scenes/Legacy/FightingWx/SplashScene.unity", target);
        }
        static void BuildLegacyMenu()
        {
            const string target = "Assets/_Scenes/Game/Menu.unity";
            CopyLegacyScene("Assets/_Scenes/Legacy/FightingWx/Menu.unity", target);
        }
        static void BuildLegacyArena()
        {
            const string target = "Assets/_Scenes/Game/Level.unity";
            CopyLegacyScene("Assets/_Scenes/Legacy/FightingWx/Level.unity", target);
        }
        static void BuildSplash(GameObject splash)
        {
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single); var canvas=Canvas(); PrefabUtility.InstantiatePrefab(splash,canvas.transform); new GameObject("SplashFlow",typeof(SplashFlow)); EditorSceneManager.SaveScene(scene,"Assets/_Scenes/SplashScene.unity");
        }
        static void BuildMenu(GameObject menu)
        {
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single); var canvas=Canvas(); PrefabUtility.InstantiatePrefab(menu,canvas.transform); EditorSceneManager.SaveScene(scene,"Assets/_Scenes/Game/Menu.unity");
        }
    }
}
