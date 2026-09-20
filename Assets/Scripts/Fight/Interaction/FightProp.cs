using UnityEngine;
namespace AwCon.Fight
{
    [RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
    public sealed class FightProp : MonoBehaviour
    {
        public float damage = 15;
        public bool breakable;
        private bool held;
        private FightActor owner;
        public bool Pickup() { if (held || owner != null || breakable) return false; held = true; GetComponent<Rigidbody>().isKinematic = true; GetComponent<Collider>().enabled = false; return true; }
        public void Launch(FightActor actor, Vector3 direction) { owner = actor; held = false; GetComponent<Collider>().enabled = true; var body = GetComponent<Rigidbody>(); body.isKinematic = false; body.velocity = direction * 9 + Vector3.up * 3; Destroy(gameObject, 5); }
        private void OnCollisionEnter(Collision collision) { if (owner == null) return; var actor = collision.collider.GetComponentInParent<FightActor>(); if (actor == null || actor == owner || actor.player == owner.player) return; actor.Receive(owner, damage, 4, true); Destroy(gameObject); }
        public void Break() { if (breakable) Destroy(gameObject); }
    }
}
