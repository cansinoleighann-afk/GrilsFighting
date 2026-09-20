using System;
using System.Collections.Generic;
using Animancer;
using UnityEngine;

namespace AwCon.Fight
{
    /// <summary>Single state and movement authority. Input/AI submit intentions; AnimancerFacade owns playback.</summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class FightActor : MonoBehaviour
    {
        public FighterDefinitionSO definition;
        public bool player;
        public bool aiEnabled = true;
        public FightState State { get; private set; }
        public float Health { get; private set; }
        public bool Alive => Health > 0;
        public event Action<FightActor> Died;
        public event Action<FightActor, float> Damaged;
        public static readonly List<FightActor> Active = new List<FightActor>();
        private CharacterController motor;
        private AnimancerFacade animationFacade;
        private Animator animator;
        private AudioSource audioSource;
        private CombatMoveSO move;
        private float elapsed, duration, verticalSpeed, nextAttack, powerUntil;
        private Vector3 input, impulse;
        private readonly Collider[] hits = new Collider[48];
        private readonly HashSet<FightActor> damaged = new HashSet<FightActor>();
        private FightAction buffered;
        private int combo;
        private bool hitWindow;
        private FightActor grapple;
        private FightProp held;
        public FightProp Held => held;
        private void OnEnable() { if (!Active.Contains(this)) Active.Add(this); }
        private void OnDisable() { Active.Remove(this); ReleaseGrapple(); if (animationFacade != null) animationFacade.ClearOnEndCallback(); }
        private void Start()
        {
            motor = GetComponent<CharacterController>();
            if (definition == null || definition.modelPrefab == null || definition.animations == null) { Debug.LogError("FightActor requires a fighter definition, model, and Animancer animation set", this); enabled = false; return; }
            Health = definition.health;
            var model = Instantiate(definition.modelPrefab, transform);
            model.name = definition.displayName;
            // Imported character prefabs can carry their old gameplay, weapon, and cloth scripts.
            // This actor is the runtime authority, so the model stays visual-only.
            foreach (var behaviour in model.GetComponentsInChildren<MonoBehaviour>(true))
            {
                behaviour.enabled = false;
            }
            var fallbackShader = Shader.Find("Standard");
            if (fallbackShader != null)
            {
                foreach (var renderer in model.GetComponentsInChildren<Renderer>(true))
                {
                    var materials = renderer.materials;
                    for (var i = 0; i < materials.Length; i++)
                    {
                        var material = materials[i];
                        if (material == null || material.shader == null || material.shader.name == "Hidden/InternalErrorShader" || !material.shader.isSupported)
                        {
                            var fallback = new Material(fallbackShader);
                            if (material != null && material.HasProperty("_MainTex")) fallback.mainTexture = material.mainTexture;
                            materials[i] = fallback;
                        }
                    }
                    renderer.materials = materials;
                }
            }
            animator = model.GetComponentInChildren<Animator>();
            if (animator == null || !animator.isHuman) { Debug.LogError("Fighter model requires a Humanoid Avatar", this); enabled = false; return; }
            // Model prefabs are visuals; remove their controller and legacy clip events from the runtime playback path.
            animator.runtimeAnimatorController = null;
            animator.applyRootMotion = false;
            animator.fireEvents = false;
            var animancer = animator.GetComponent<AnimancerComponent>();
            if (animancer == null) animancer = animator.gameObject.AddComponent<AnimancerComponent>();
            animancer.Animator = animator;
            animationFacade = animator.GetComponent<AnimancerFacade>();
            if (animationFacade == null) animationFacade = animator.gameObject.AddComponent<AnimancerFacade>();
            animationFacade.enabled = true;
            audioSource = gameObject.AddComponent<AudioSource>();
            Enter(FightState.Idle, definition.animations.idle);
        }
        public void Move(Vector2 value) { input = Vector3.ClampMagnitude(new Vector3(value.x, 0, value.y), 1); }
        public bool Request(FightAction action)
        {
            if (!Alive || animationFacade == null || Time.timeScale == 0) return false;
            if (State == FightState.Attack)
            {
                if ((action == FightAction.Punch || action == FightAction.Kick) && elapsed / duration >= move.comboStart) buffered = action;
                return false;
            }
            if (State == FightState.Hit || State == FightState.Knockdown || State == FightState.Grabbed || State == FightState.Grab || State == FightState.Dodge) return false;
            switch (action)
            {
                case FightAction.Punch: case FightAction.Kick:
                    var list = action == FightAction.Punch ? definition.punches : definition.kicks;
                    var selected = State == FightState.Jump ? definition.jumpAttack : (list != null && list.Length > 0 ? list[combo % list.Length] : null);
                    if (selected == null) return false;
                    BeginAttack(selected); return true;
                case FightAction.Jump:
                    if (!motor.isGrounded) return false;
                    verticalSpeed = 7; Enter(FightState.Jump, definition.animations.jump, 0.8f); return true;
                case FightAction.Dodge:
                    impulse = transform.forward * 7; Enter(FightState.Dodge, definition.animations.dodge, 0.5f); return true;
                case FightAction.Guard: Enter(FightState.Guard, definition.animations.guard); return true;
                case FightAction.Grab: return TryGrab();
                case FightAction.Interact: return Pickup();
                case FightAction.Throw:
                    if (held == null) return false;
                    held.Launch(this, transform.forward); held = null; return true;
                case FightAction.Power:
                    if (Time.time < powerUntil + 12) return false;
                    powerUntil = Time.time + 5; return true;
            }
            return false;
        }
        public void ReleaseGuard() { if (State == FightState.Guard) Enter(FightState.Idle, definition.animations.idle); }
        private void BeginAttack(CombatMoveSO attack)
        {
            move = attack; damaged.Clear(); hitWindow = false; buffered = FightAction.None;
            Enter(FightState.Attack, attack.transition, attack.Duration, attack.speed, attack.fade);
            animationFacade.AddCallback(attack.hitStart, OpenHit);
            animationFacade.AddCallback(attack.hitEnd, CloseHit);
            if (attack.sound != null) audioSource.PlayOneShot(attack.sound);
        }
        private void OpenHit() { if (State == FightState.Attack) { hitWindow = true; ScanHit(); } }
        private void CloseHit() { hitWindow = false; }
        private void Enter(FightState state, ITransition transition, float seconds = float.PositiveInfinity, float speed = 1, float fade = 0.12f)
        {
            State = state; elapsed = 0; duration = Mathf.Max(0.05f, seconds); hitWindow = false;
            animationFacade.ClearOnEndCallback();
            if (transition != null) animationFacade.PlayTransition(transition, new AnimPlayOptions { Layer = 0, FadeDuration = fade, Speed = speed, NormalizedTime = 0 });
        }
        private void Update()
        {
            if (animationFacade == null || Time.timeScale == 0) return;
            if (!player && aiEnabled && Alive) Think();
            elapsed += Time.deltaTime;
            if (State == FightState.Attack)
            {
                if (hitWindow) ScanHit();
                if (elapsed >= duration) { var next = buffered; combo = next == FightAction.None ? 0 : combo + 1; Enter(FightState.Idle, definition.animations.idle); if (next != FightAction.None) Request(next); }
            }
            else if ((State == FightState.Hit || State == FightState.Dodge || State == FightState.Jump) && elapsed >= duration) Enter(FightState.Idle, definition.animations.idle);
            else if (State == FightState.Knockdown && elapsed >= duration) Enter(FightState.Hit, definition.animations.standUp, 0.8f);
            else if (State == FightState.Grab && elapsed >= duration) { var victim = grapple; ReleaseGrapple(); Enter(FightState.Idle, definition.animations.idle); if (victim != null) victim.Receive(this, 24, 4, true, true); }
            if (State == FightState.Idle || State == FightState.Move)
            {
                var desired = input.sqrMagnitude > 0.01f ? FightState.Move : FightState.Idle;
                if (State != desired) Enter(desired, desired == FightState.Move ? definition.animations.walk : definition.animations.idle);
                if (input.sqrMagnitude > 0.01f) transform.rotation = Quaternion.LookRotation(input);
                // Exported clips need not carry a loop flag. Explicitly replay locomotion through the facade.
                if (animationFacade.CurrentNormalizedTime >= 1) Enter(desired, desired == FightState.Move ? definition.animations.walk : definition.animations.idle);
            }
            if (motor.isGrounded && verticalSpeed < 0) verticalSpeed = -2;
            verticalSpeed += Physics.gravity.y * 2 * Time.deltaTime;
            var motion = ((State == FightState.Move || State == FightState.Jump) ? input * definition.moveSpeed : Vector3.zero) + impulse;
            motion.y = verticalSpeed;
            if (State != FightState.Grabbed && motor.enabled) motor.Move(motion * Time.deltaTime);
            impulse = Vector3.MoveTowards(impulse, Vector3.zero, 12 * Time.deltaTime);
            if (held != null) held.transform.position = transform.position + Vector3.up * 1.2f + transform.forward * 0.5f;
        }
        private void Think()
        {
            var target = NearestOpponent(100);
            if (target == null) { input = Vector3.zero; return; }
            var delta = target.transform.position - transform.position; delta.y = 0;
            if (delta.sqrMagnitude > 2) input = delta.normalized;
            else { input = Vector3.zero; if (delta.sqrMagnitude > 0.01f) transform.rotation = Quaternion.LookRotation(delta); if (Time.time >= nextAttack) { Request(FightAction.Punch); nextAttack = Time.time + definition.aiAttackInterval; } }
        }
        private FightActor NearestOpponent(float range)
        {
            FightActor result = null; float best = range * range;
            foreach (var other in Active) { if (other == this || !other.Alive || other.player == player) continue; float d = (other.transform.position - transform.position).sqrMagnitude; if (d < best) { best = d; result = other; } }
            return result;
        }
        private void ScanHit()
        {
            int count = Physics.OverlapSphereNonAlloc(transform.position + Vector3.up + transform.forward * (move.reach * 0.5f), move.radius, hits, ~0, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < count; i++)
            {
                var other = hits[i].GetComponentInParent<FightActor>();
                if (other != null && other != this && other.player != player && damaged.Add(other))
                {
                    other.Receive(this, move.damage * (Time.time < powerUntil ? 2 : 1) + (held != null ? held.damage : 0), move.knockback, move.knockdown, move.unblockable);
                    if (move.hitEffect != null) Destroy(Instantiate(move.hitEffect, other.transform.position + Vector3.up, Quaternion.identity), 2);
                }
                var prop = hits[i].GetComponent<FightProp>();
                if (prop != null && prop != held) prop.Break();
            }
        }
        public void Receive(FightActor attacker, float amount, float knockback, bool knockdown, bool unblockable = false)
        {
            if (!Alive || State == FightState.Dodge || State == FightState.Knockdown) return;
            bool blocked = State == FightState.Guard && !unblockable && Vector3.Dot(transform.forward, (attacker.transform.position - transform.position).normalized) > 0;
            Health = Mathf.Max(0, Health - Mathf.Max(0, amount) * (blocked ? 0.15f : 1));
            Damaged?.Invoke(this, amount);
            ReleaseGrapple();
            if (held != null) { held.Launch(this, transform.forward); held = null; }
            impulse = (transform.position - attacker.transform.position).normalized * knockback; impulse.y = 0;
            if (!Alive) { Enter(FightState.Dead, definition.animations.death); Died?.Invoke(this); }
            else if (!blocked) Enter(knockdown ? FightState.Knockdown : FightState.Hit, knockdown ? definition.animations.knockdown : definition.animations.hit, knockdown ? 1.8f : 0.4f);
        }
        private bool TryGrab()
        {
            var other = NearestOpponent(1.7f);
            if (other == null || other.State == FightState.Grabbed || other.State == FightState.Grab || other.State == FightState.Knockdown) return false;
            grapple = other; other.grapple = this;
            Enter(FightState.Grab, definition.animations.grab, 1.2f); other.Enter(FightState.Grabbed, other.definition.animations.grabbed);
            return true;
        }
        private void ReleaseGrapple()
        {
            if (grapple == null) return;
            var other = grapple; grapple = null; other.grapple = null;
            if (other.Alive && other.animationFacade != null && (other.State == FightState.Grabbed || other.State == FightState.Grab)) other.Enter(FightState.Idle, other.definition.animations.idle);
        }
        private bool Pickup()
        {
            if (held != null) return Request(FightAction.Throw);
            int count = Physics.OverlapSphereNonAlloc(transform.position, 2, hits);
            for (int i = 0; i < count; i++) { var prop = hits[i].GetComponent<FightProp>(); if (prop != null && prop.Pickup()) { held = prop; return true; } }
            return false;
        }
    }
}
