using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class SplitLightingBinder : MonoBehaviour
{
    private const string PrefabResourcePath = "Lighting/SplitLighting";
    private const string RootName = "SplitLighting";
    private const string MenuSceneName = "Menu";
    private const string LevelSceneName = "Level";
    private const string MenuSceneLightName = "Menu_S";
    private const string MenuCharacterLightName = "Menu_P";
    private const string LevelSceneLightName = "Level_S";
    private const string LevelCharacterLightName = "Level_P";

    private static SplitLightingBinder _instance;
    private Light menuSceneLight;
    private Light menuCharacterLight;
    private Light levelSceneLight;
    private Light levelCharacterLight;
    private Light activeSceneLight;
    private Light activeCharacterLight;

    private static readonly int SceneLightDirId = Shader.PropertyToID("_SplitSceneLightDir");
    private static readonly int SceneLightColorId = Shader.PropertyToID("_SplitSceneLightColor");
    private static readonly int CharacterLightDirId = Shader.PropertyToID("_SplitCharacterLightDir");
    private static readonly int CharacterLightColorId = Shader.PropertyToID("_SplitCharacterLightColor");

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureExists()
    {
        if (FindObjectOfType<SplitLightingBinder>() != null)
        {
            return;
        }

        GameObject prefab = Resources.Load<GameObject>(PrefabResourcePath);
        if (prefab == null)
        {
            Debug.LogError("找不到灯光预制体 Resources/Lighting/SplitLighting");
            return;
        }

        GameObject instance = Instantiate(prefab);
        instance.name = RootName;
        DontDestroyOnLoad(instance);
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        gameObject.name = RootName;
        DontDestroyOnLoad(gameObject);
        CacheChildLights();
        ApplySceneLighting(SceneManager.GetActiveScene());
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplySceneLighting(scene);
    }

    private void CacheChildLights()
    {
        menuSceneLight = FindChildLight(MenuSceneLightName);
        menuCharacterLight = FindChildLight(MenuCharacterLightName);
        levelSceneLight = FindChildLight(LevelSceneLightName);
        levelCharacterLight = FindChildLight(LevelCharacterLightName);
    }

    private Light FindChildLight(string lightName)
    {
        Transform child = transform.Find(lightName);
        return child != null ? child.GetComponent<Light>() : null;
    }

    private void ApplySceneLighting(Scene scene)
    {
        CacheChildLights();

        bool useMenu = scene.name == MenuSceneName;
        bool useLevel = scene.name == LevelSceneName;

        SetLightActive(menuSceneLight, useMenu);
        SetLightActive(menuCharacterLight, useMenu);
        SetLightActive(levelSceneLight, useLevel);
        SetLightActive(levelCharacterLight, useLevel);

        if (useMenu)
        {
            activeSceneLight = menuSceneLight;
            activeCharacterLight = menuCharacterLight;
        }
        else if (useLevel)
        {
            activeSceneLight = levelSceneLight;
            activeCharacterLight = levelCharacterLight;
        }
        else
        {
            activeSceneLight = null;
            activeCharacterLight = null;
        }

        if (activeSceneLight != null)
        {
            RenderSettings.sun = activeSceneLight;
        }

        DestroyForeignSplitLights();
    }

    private static void SetLightActive(Light light, bool active)
    {
        if (light != null)
        {
            light.gameObject.SetActive(active);
        }
    }

    private void DestroyForeignSplitLights()
    {
        Light[] lights = FindObjectsOfType<Light>();
        for (int i = 0; i < lights.Length; i++)
        {
            Light light = lights[i];
            if (light == null || light.transform.IsChildOf(transform))
            {
                continue;
            }

            string lightName = light.gameObject.name;
            bool isManagedName =
                lightName == MenuSceneLightName ||
                lightName == MenuCharacterLightName ||
                lightName == LevelSceneLightName ||
                lightName == LevelCharacterLightName ||
                lightName == "Directional Light_S" ||
                lightName == "Directional Light_P";

            if (isManagedName)
            {
                Destroy(light.gameObject);
            }
        }
    }

    private void LateUpdate()
    {
        PushLight(activeSceneLight, SceneLightDirId, SceneLightColorId);
        PushLight(activeCharacterLight, CharacterLightDirId, CharacterLightColorId);
    }

    private static void PushLight(Light light, int directionId, int colorId)
    {
        if (light == null || !light.enabled || !light.gameObject.activeInHierarchy)
        {
            Shader.SetGlobalVector(directionId, new Vector4(0f, 1f, 0f, 0f));
            Shader.SetGlobalColor(colorId, Color.black);
            return;
        }

        Vector3 direction = -light.transform.forward;
        Shader.SetGlobalVector(directionId, direction);
        Shader.SetGlobalColor(colorId, light.color * light.intensity);
    }
}
