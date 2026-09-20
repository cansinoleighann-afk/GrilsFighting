using UnityEngine;

namespace AwCon.Fight
{
    [RequireComponent(typeof(FightActor))]
    public sealed class FightPlayerInput : MonoBehaviour
    {
        private FightActor actor;
        private void Awake() { actor = GetComponent<FightActor>(); }
        private void Update()
        {
            actor.Move(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
            if (Input.GetKeyDown(KeyCode.J)) actor.Request(FightAction.Punch);
            if (Input.GetKeyDown(KeyCode.K)) actor.Request(FightAction.Kick);
            if (Input.GetKeyDown(KeyCode.Space)) actor.Request(FightAction.Jump);
            if (Input.GetKeyDown(KeyCode.LeftShift)) actor.Request(FightAction.Dodge);
            if (Input.GetKeyDown(KeyCode.L)) actor.Request(FightAction.Grab);
            if (Input.GetKeyDown(KeyCode.E)) actor.Request(FightAction.Interact);
            if (Input.GetKeyDown(KeyCode.Q)) actor.Request(FightAction.Throw);
            if (Input.GetKeyDown(KeyCode.R)) actor.Request(FightAction.Power);
            if (Input.GetKeyDown(KeyCode.U)) actor.Request(FightAction.Guard);
            if (Input.GetKeyUp(KeyCode.U)) actor.ReleaseGuard();
        }
    }
}
