using Funplay.Editor.Tools.Scripting;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using System.Collections.Generic;

public class BBBRuntimeTest : IFunplayCommand
{
    public void Execute(ExecutionContext ctx)
    {
        var player = Object.FindObjectOfType<BBBNexus.BBBCharacterController>();
        if (player == null || !Application.isPlaying) { ctx.ReturnValue = "Missing runtime"; return; }
        var keyboard = InputSystem.AddDevice<Keyboard>("BBBValidationKeyboard");
        var started = EditorApplication.timeSinceStartup;
        var samples = new List<string>();
        int phase = -1;
        EditorApplication.CallbackFunction tick = null;
        tick = () =>
        {
            if (!Application.isPlaying || player == null)
            {
                EditorApplication.update -= tick;
                InputSystem.RemoveDevice(keyboard);
                return;
            }
            double elapsed = EditorApplication.timeSinceStartup - started;
            int step = (int)elapsed;
            if (step != phase)
            {
                phase = step;
                samples.Add(step + " state=" + player.StateMachine.CurrentState.GetType().Name +
                    " locked=" + player.RuntimeData.IsAiming + " grounded=" + player.RuntimeData.IsGrounded +
                    " speed=" + player.RuntimeData.CurrentSpeed.ToString("F2") +
                    " pos=" + player.transform.position +
                    " ik=" + player.GetComponent<RootMotion.FinalIK.GrounderFBBIK>().weight.ToString("F2"));
            }
            Key[] keys = step == 1 ? new[] { Key.W } : step == 2 ? new[] { Key.W, Key.LeftShift } :
                step == 4 ? new[] { Key.Tab } : step == 6 ? new[] { Key.A } :
                step == 8 ? new[] { Key.Space } : step == 11 ? new[] { Key.Tab } : new Key[0];
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(keys));
            if (elapsed > 13)
            {
                EditorApplication.update -= tick;
                InputSystem.RemoveDevice(keyboard);
                SessionState.SetString("BBBValidation", string.Join("\n", samples));
            }
        };
        EditorApplication.update += tick;
        ctx.ReturnValue = "13-second keyboard validation started";
    }
}
