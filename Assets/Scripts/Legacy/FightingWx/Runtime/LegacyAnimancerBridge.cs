using System;
using AwCon;
using UnityEngine;

/// <summary>Routes the legacy CharacterControl intent API to Animancer without changing combat ownership.</summary>
[DisallowMultipleComponent]
public sealed class LegacyAnimancerBridge : MonoBehaviour
{
    [SerializeField] private LegacyAnimancerSetSO animationSet;
    [SerializeField, Min(0f)] private float fadeDuration = 0.12f;

    private AnimancerFacade facade;
    private AnimationClip idle;
    private AnimationClip walk;
    private bool moving;

    public void Initialize(Animator animator, LegacyAnimancerSetSO set, AnimationO idleSource, AnimationO walkSource)
    {
        animationSet = set;
        idle = animationSet != null && animationSet.idle != null ? animationSet.idle : idleSource != null ? idleSource._animation : null;
        walk = animationSet != null && animationSet.walk != null ? animationSet.walk : walkSource != null ? walkSource._animation : null;
        if (facade == null) facade = GetComponent<AnimancerFacade>();
        if (facade == null) facade = gameObject.AddComponent<AnimancerFacade>();
        if (animator != null)
        {
            animator.runtimeAnimatorController = null;
            animator.applyRootMotion = false;
            // Gameplay events are scheduled explicitly by CharacterControl.
            // This prevents direct playback and legacy Unity events from
            // applying damage or grab hand-offs twice.
            animator.fireEvents = false;
        }
        Play(idle, 1f, 0f);
    }

    public void SetLocomotion(float speed)
    {
        bool shouldMove = speed > 0.01f;
        if (shouldMove == moving) return;
        moving = shouldMove;
        Play(moving ? walk : idle, 1f, fadeDuration);
    }

    /// <summary>
    /// 回归待机姿态。
    /// 老 Animator 状态机是靠 "Attack/Reaction/PickUp/StandUp/... -> idles" 的
    /// exit-time 转移自动回待机的；Animancer 直接播 Clip，既没有状态图也不会
    /// 自己停掉播完的 Clip，所以必须由控制脚本显式把待机 Clip 播起来。
    /// 少了这一步，角色就会一直定格在刚才那个一次性动画的最后一帧。
    /// </summary>
    public void PlayIdle()
    {
        moving = false;
        // 已经是待机就别重播，否则每次状态回收都会把待机动画抽回第 0 帧
        if (IsShowing(idle)) return;
        Play(idle, 1f, fadeDuration);
    }

    /// <summary>
    /// 基础层当前是否正在播这条 Clip。
    /// </summary>
    public bool IsShowing(AnimationClip clip)
    {
        if (clip == null) return false;
        if (facade == null) facade = GetComponent<AnimancerFacade>();
        return facade != null && facade.CurrentClip == clip;
    }

    /// <summary>
    /// 基础层的非循环 Clip 是否已经播完。
    /// 供上层做兜底：结束回调万一没触发（被打断、淡入被抢、回调被清掉），
    /// 也不会把角色永久锁在某个动作的最后一帧上。
    /// </summary>
    public bool HasCurrentClipFinished()
    {
        if (facade == null) facade = GetComponent<AnimancerFacade>();
        return facade != null && facade.HasCurrentClipFinished();
    }

    public void Play(AnimationO source, Action onEnd = null)
    {
        if (source == null) return;
        moving = false;
        Play(source._animation, source._animationSpeed <= 0f ? 1f : source._animationSpeed, fadeDuration, onEnd);
    }

    public void PlayClip(AnimationClip clip, float speed = 1f, Action onEnd = null)
    {
        moving = false;
        Play(clip, speed <= 0f ? 1f : speed, fadeDuration, onEnd);
    }

    public void PlayState(string stateName)
    {
        if (animationSet == null) return;
        AnimationClip clip = null;
        switch (stateName)
        {
            case "Jump": clip = animationSet.jump; break;
            case "Defence": clip = animationSet.guard; break;
            case "KnockDown": clip = animationSet.knockdown; break;
            case "StandUp": clip = animationSet.standUp; break;
            case "PickUp": clip = animationSet.pickup; break;
        }
        if (clip != null) { moving = false; Play(clip, 1f, fadeDuration); }
    }

    private void Play(AnimationClip clip, float speed, float fade, Action onEnd = null)
    {
        if (clip == null || facade == null) return;
        facade.PlayClip(clip, new AnimPlayOptions { Layer = 0, FadeDuration = fade, Speed = speed, NormalizedTime = 0f });
        facade.SetOnEndCallback(onEnd);
    }
}
