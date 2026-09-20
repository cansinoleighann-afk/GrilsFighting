using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public sealed class PersistentLightingManager : MonoBehaviour
{
    // 当前关闭运行时灯光接管，测试场景只使用手动创建的灯光。
    private const bool LightingSystemEnabled = false;
    private const string ConfigResourcePath = "Lighting/GlobalLightingEnvironment";
    private const string MenuSceneName = "Menu";
    private const string RootName = "Persistent Lighting Environment";
    private static readonly int SceneLightDirectionId = Shader.PropertyToID("_BrightToonSceneLightDirection");
    private static readonly int SceneLightColorId = Shader.PropertyToID("_BrightToonSceneLightColor");
    private static readonly int CharacterLightDirectionId = Shader.PropertyToID("_BrightToonCharacterLightDirection");
    private static readonly int CharacterLightColorId = Shader.PropertyToID("_BrightToonCharacterLightColor");
    private static PersistentLightingManager _instance;
    private LightingEnvironmentConfig _config;
    private readonly Dictionary<string, Light> _persistentLights = new Dictionary<string, Light>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateBeforeFirstScene()
    {
        if (!LightingSystemEnabled)
        {
            return;
        }

        if (_instance != null)
        {
            return;
        }

        GameObject root = new GameObject(RootName);
        _instance = root.AddComponent<PersistentLightingManager>();
        DontDestroyOnLoad(root);
    }

    private void Awake()
    {
        if (!LightingSystemEnabled)
        {
            enabled = false;
            return;
        }

        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        _config = Resources.Load<LightingEnvironmentConfig>(ConfigResourcePath);
        if (_config == null)
        {
            Debug.LogWarning("未找到 Lighting/GlobalLightingEnvironment 环境配置，使用脚本默认值。");
            _config = ScriptableObject.CreateInstance<LightingEnvironmentConfig>();
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            _instance = null;
        }
    }

    private void LateUpdate()
    {
        PushLightToShader("Directional Light_S", SceneLightDirectionId, SceneLightColorId);
        PushLightToShader("Directional Light_P", CharacterLightDirectionId, CharacterLightColorId);
    }

    private void PushLightToShader(string lightName, int directionId, int colorId)
    {
        Light light;
        if (!_persistentLights.TryGetValue(lightName, out light) || light == null)
        {
            Shader.SetGlobalVector(directionId, new Vector4(0f, -1f, 0f, 0f));
            Shader.SetGlobalColor(colorId, Color.black);
            return;
        }

        Shader.SetGlobalVector(directionId, -light.transform.forward);
        Shader.SetGlobalColor(colorId, light.enabled ? light.color * light.intensity : Color.black);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == MenuSceneName)
        {
            StartCoroutine(MoveMenuLightsNextFrame(scene));
        }
        else
        {
            ApplyConfiguration();
        }
    }

    private IEnumerator MoveMenuLightsNextFrame(Scene menuScene)
    {
        yield return null;
        MoveMenuLightToPersistentRoot(menuScene, "Directional Light_P");
        MoveMenuLightToPersistentRoot(menuScene, "Directional Light_S");
        ApplyConfiguration();
    }

    private void ApplyConfiguration()
    {
        ApplyEnvironmentLighting();
        RenderSettings.ambientIntensity = _config.ambientIntensity;
        RenderSettings.reflectionIntensity = _config.reflectionIntensity;
        RenderSettings.fog = _config.fogEnabled;
        RenderSettings.fogMode = _config.fogMode;
        RenderSettings.fogColor = _config.fogColor;
        RenderSettings.fogDensity = _config.fogDensity;
        RenderSettings.fogStartDistance = _config.fogStartDistance;
        RenderSettings.fogEndDistance = _config.fogEndDistance;

        if (_config.skybox != null)
        {
            RenderSettings.skybox = _config.skybox;
        }

        ApplySunSource();
        DynamicGI.UpdateEnvironment();
    }

    private void ApplySunSource()
    {
        if (_config.sunSource == null)
        {
            RenderSettings.sun = null;
            return;
        }

        Light sunLight;
        if (!_persistentLights.TryGetValue(_config.sunSource.name, out sunLight) || sunLight == null)
        {
            Debug.LogWarning("太阳源未加载为常驻灯光，无法在运行时应用。");
            return;
        }

        RenderSettings.sun = sunLight;
    }

    private void ApplyEnvironmentLighting()
    {
        switch (_config.environmentLightingSource)
        {
            case EnvironmentLightingSource.Skybox:
                RenderSettings.ambientMode = AmbientMode.Skybox;
                break;
            case EnvironmentLightingSource.Color:
                RenderSettings.ambientMode = AmbientMode.Flat;
                RenderSettings.ambientLight = _config.ambientColor;
                break;
            default:
                RenderSettings.ambientMode = AmbientMode.Trilight;
                RenderSettings.ambientSkyColor = _config.gradientSkyColor;
                RenderSettings.ambientEquatorColor = _config.gradientEquatorColor;
                RenderSettings.ambientGroundColor = _config.gradientGroundColor;
                break;
        }
    }

    private void MoveMenuLightToPersistentRoot(Scene menuScene, string lightName)
    {
        Light persistentLight;
        if (_persistentLights.TryGetValue(lightName, out persistentLight) && persistentLight != null)
        {
            Light duplicateLight = FindLightInScene(menuScene, lightName);
            if (duplicateLight != null)
            {
                Destroy(duplicateLight.gameObject);
            }

            return;
        }

        Light menuLight = FindLightInScene(menuScene, lightName);
        if (menuLight == null)
        {
            Debug.LogError("Menu 场景缺少灯光对象：" + lightName);
            return;
        }

        DontDestroyOnLoad(menuLight.gameObject);
        menuLight.transform.SetParent(transform, true);
        _persistentLights[lightName] = menuLight;
    }

    private static Light FindLightInScene(Scene scene, string lightName)
    {
        Light[] lights = FindObjectsOfType<Light>();
        for (int index = 0; index < lights.Length; index++)
        {
            Light light = lights[index];
            if (light != null && light.gameObject.scene == scene && light.gameObject.name == lightName)
            {
                return light;
            }
        }

        return null;
    }
}
