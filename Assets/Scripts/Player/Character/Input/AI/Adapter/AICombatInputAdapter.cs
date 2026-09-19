using UnityEngine;

namespace AwCon
{
    [DisallowMultipleComponent]
    public class AICombatInputAdapter : InputSourceBase
    {
        [Header("导航传感器")]
        public NavigatorSensorBase _navigatorSensor;

        // 已修复编码乱码的注释。
        [SubclassSelector]
        [SerializeReference]
        public IAITacticalBrain _brain;

        [Header("战术配置")]
        [Tooltip("AI战术大脑配置")]
        public AITacticalBrainConfigSO TacticalConfig;

        private bool _lastAttackIntent;
        private bool _lastAimIntent;
        private bool _lastJumpIntent;
        private bool _lastDodgeIntent;
        private bool _lastRollIntent;

        public NavigatorSensorBase NavigatorSensor => _navigatorSensor;
        public IAITacticalBrain Brain => _brain;

        protected override void Awake()
        {
            if (_navigatorSensor == null)
                _navigatorSensor = GetComponent<NavigatorSensorBase>();

            if (_brain == null)
            {
                Debug.LogError($"说明", this);
                enabled = false;
                return;
            }

            // 已修复编码乱码的注释。
            _brain.Initialize(this.transform, TacticalConfig);

            if (TacticalConfig != null)
                InjectConfigIfSupported(_brain, TacticalConfig);
        }

        private static void InjectConfigIfSupported(IAITacticalBrain brain, AITacticalBrainConfigSO config)
        {
            if (brain is MeleeRusherBrain melee)
            {
                var field = typeof(MeleeRusherBrain).GetField("_config",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (field == null)
                {
                    Debug.LogWarning("MeleeRusherBrain _config field missing");
                    return;
                }

                field.SetValue(melee, config);
            }
        }

        public override void FetchRawInput(ref RawInputData rawData)
        {
            if (_navigatorSensor == null || _brain == null)
            {
                ClearIntent(ref rawData);
                return;
            }

            ref readonly var context = ref _navigatorSensor.GetCurrentContext();
            ref readonly var intent = ref _brain.EvaluateTactics(in context);

            rawData.MoveAxis = intent.MovementInput;
            rawData.LookAxis = intent.LookInput;

            bool currentAimIntent = intent.WantsToAim;
            rawData.AimHeld = currentAimIntent;

            bool currentAttackIntent = intent.WantsToAttack;
            rawData.Expression1Held = currentAttackIntent;
            rawData.Expression1JustPressed = currentAttackIntent && !_lastAttackIntent;

            // 已修复编码乱码的注释。
            bool currentJumpIntent = intent.WantsToJump;
            rawData.JumpHeld = currentJumpIntent;
            rawData.JumpJustPressed = currentJumpIntent && !_lastJumpIntent;

            // 已修复编码乱码的注释。
            bool currentDodgeIntent = intent.WantsToDodge;
            rawData.DodgeHeld = currentDodgeIntent;
            rawData.DodgeJustPressed = currentDodgeIntent && !_lastDodgeIntent;

            // 已修复编码乱码的注释。
            bool currentRollIntent = intent.WantsToRoll;
            rawData.RollHeld = currentRollIntent;
            rawData.RollJustPressed = currentRollIntent && !_lastRollIntent;

            _lastAttackIntent = currentAttackIntent;
            _lastAimIntent = currentAimIntent;
            _lastJumpIntent = currentJumpIntent;
            _lastDodgeIntent = currentDodgeIntent;
            _lastRollIntent = currentRollIntent;

            // 已修复编码乱码的注释。
        }

        private void ClearIntent(ref RawInputData rawData)
        {
            rawData.MoveAxis = Vector2.zero;
            rawData.LookAxis = Vector2.zero;
            rawData.AimHeld = false;
            rawData.Expression1Held = false;
            rawData.Expression1JustPressed = false;
            rawData.JumpHeld = false;
            rawData.JumpJustPressed = false;
            rawData.DodgeHeld = false;
            rawData.DodgeJustPressed = false;
            rawData.RollHeld = false;
            rawData.RollJustPressed = false;
            _lastAttackIntent = false;
            _lastAimIntent = false;
            _lastJumpIntent = false;
            _lastDodgeIntent = false;
            _lastRollIntent = false;
        }
    }
}