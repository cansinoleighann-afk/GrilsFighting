using Animancer;
using UnityEngine;

namespace AwCon.Fight
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator), typeof(AnimancerComponent), typeof(AnimancerFacade))]
    public sealed class MenuPreviewAnimancer : MonoBehaviour
    {
        [SerializeField] private ClipTransition idle;

        private Animator animator;
        private AnimancerFacade facade;

        private void Awake()
        {
            // The same legacy player prefab is also instantiated by GamePlayManager in Level.
            // Preview-only setup must never disable that runtime character controller.
            if (Object.FindObjectOfType<GamePlayManager>() != null)
            {
                enabled = false;
                return;
            }

            animator = GetComponent<Animator>();
            facade = GetComponent<AnimancerFacade>();
            foreach (var legacyCharacterControl in GetComponentsInParent<CharacterControl>(true))
                legacyCharacterControl.enabled = false;
            foreach (var legacyCharacterControl in GetComponentsInChildren<CharacterControl>(true))
                legacyCharacterControl.enabled = false;
            animator.runtimeAnimatorController = null;
            animator.applyRootMotion = false;
            animator.fireEvents = false;
        }

        private void OnEnable()
        {
            PlayIdle();
        }

        public void PlayIdle()
        {
            if (facade == null) facade = GetComponent<AnimancerFacade>();
            if (idle == null || idle.Clip == null || facade == null) return;

            facade.PlayTransition(idle, new AnimPlayOptions
            {
                Layer = 0,
                FadeDuration = 0f,
                Speed = 1f,
                NormalizedTime = 0f
            });
        }

        public void Configure(AnimationClip clip)
        {
            idle = new ClipTransition { Clip = clip };
        }
    }
}
