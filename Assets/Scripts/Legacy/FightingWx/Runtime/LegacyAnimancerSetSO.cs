using UnityEngine;

/// <summary>
/// Per-character Animancer defaults for the legacy Level combat loop.
/// One-shot attacks and reactions continue to come from the existing attack and reaction SOs.
/// </summary>
[CreateAssetMenu(fileName = "LegacyAnimancerSet", menuName = "Fighting Wx/Legacy Animancer Set")]
public sealed class LegacyAnimancerSetSO : ScriptableObject
{
    [Header("Locomotion")]
    public AnimationClip idle;
    public AnimationClip walk;

    [Header("Legacy state fallbacks")]
    public AnimationClip jump;
    public AnimationClip guard;
    public AnimationClip knockdown;
    public AnimationClip standUp;
    public AnimationClip pickup;
}
