using System;
using UnityEngine;

namespace AwCon
{
    /// <summary>
    // 已修复编码乱码的注释。
    // 已修复编码乱码的注释。
    /// </summary>
    public abstract class AnimationFacadeBase : MonoBehaviour, IAnimationFacade
    {
        /// <summary>
        // 已修复编码乱码的注释。
        /// </summary>
        public abstract void PlayClip(AnimationClip clip, AnimPlayOptions options);

        /// <summary>
        // 已修复编码乱码的注释。
        /// </summary>
        public abstract void PlayTransition(object transitionObj, AnimPlayOptions options);

        /// <summary>
        // 已修复编码乱码的注释。
        /// </summary>
        public abstract void SetMixerParameter(Vector2 parameter, int layerIndex = 0);

        /// <summary>
        // 已修复编码乱码的注释。
        /// </summary>
        public abstract void SetOnEndCallback(Action onEndAction, int layerIndex = 0);

        /// <summary>
        // 已修复编码乱码的注释。
        /// </summary>
        public abstract void ClearOnEndCallback(int layerIndex = 0);

        /// <summary>
        // 已修复编码乱码的注释。
        /// </summary>
        public abstract void SetOverrideOnEndCallback(Action onEndAction);

        /// <summary>
        // 已修复编码乱码的注释。
        /// </summary>
        public abstract void ClearOverrideOnEndCallback();

        /// <summary>
        // 已修复编码乱码的注释。
        /// </summary>
        public abstract void SetLayerWeight(int layerIndex, float weight, float fadeDuration = 0f);

        /// <summary>
        // 已修复编码乱码的注释。
        /// </summary>
        public abstract void SetLayerMask(int layerIndex, AvatarMask mask);

        /// <summary>
        // 已修复编码乱码的注释。
        /// </summary>
        public abstract void AddCallback(float normalizedTime, Action callback, int layerIndex = 0);

        /// <summary>
        // 已修复编码乱码的注释。
        /// </summary>
        public abstract float CurrentTime { get; }

        /// <summary>
        // 已修复编码乱码的注释。
        /// </summary>
        public abstract float CurrentNormalizedTime { get; }

        /// <summary>
        // 已修复编码乱码的注释。
        /// </summary>
        public abstract float GetLayerTime(int layerIndex);

        /// <summary>
        // 已修复编码乱码的注释。
        /// </summary>
        public abstract float GetLayerNormalizedTime(int layerIndex);

        /// <summary>
        // 已修复编码乱码的注释。
        /// </summary>
        public abstract void PlayFullBodyAction(AnimationClip clip, float fadeDuration = 0.2f);

        /// <summary>
        // 已修复编码乱码的注释。
        /// </summary>
        public abstract void StopFullBodyAction();
    }
}
