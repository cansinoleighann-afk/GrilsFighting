using UnityEngine;
using UnityEngine.Serialization;

public enum EnvironmentLightingSource
{
    Skybox,
    Gradient,
    Color
}

[CreateAssetMenu(fileName = "GlobalLightingEnvironment", menuName = "City Fighter/Lighting Environment Config")]
public class LightingEnvironmentConfig : ScriptableObject
{
    [Header("环境照明")]
    public EnvironmentLightingSource environmentLightingSource = EnvironmentLightingSource.Gradient;
    [FormerlySerializedAs("ambientSkyColor")]
    [ColorUsage(false, true)] public Color gradientSkyColor = new Color(0.55f, 0.62f, 0.72f, 1f);
    [FormerlySerializedAs("ambientEquatorColor")]
    [ColorUsage(false, true)] public Color gradientEquatorColor = new Color(0.42f, 0.42f, 0.42f, 1f);
    [FormerlySerializedAs("ambientGroundColor")]
    [ColorUsage(false, true)] public Color gradientGroundColor = new Color(0.25f, 0.25f, 0.25f, 1f);
    [ColorUsage(false, true)] public Color ambientColor = new Color(0.55f, 0.62f, 0.72f, 1f);
    [Range(0f, 4f)] public float ambientIntensity = 1f;

    [Header("天空盒与反射")]
    public Material skybox;
    [Tooltip("请指定 PersistentLighting 预制体内的 Directional Light。")]
    public Light sunSource;
    [Range(0f, 4f)] public float reflectionIntensity = 1f;

    [Header("雾")]
    public bool fogEnabled;
    public FogMode fogMode = FogMode.ExponentialSquared;
    public Color fogColor = new Color(0.65f, 0.7f, 0.75f, 1f);
    [Range(0f, 0.2f)] public float fogDensity = 0.01f;
    public float fogStartDistance;
    public float fogEndDistance = 300f;
}
