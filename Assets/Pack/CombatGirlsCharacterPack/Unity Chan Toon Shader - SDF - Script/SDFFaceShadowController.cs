using UnityEngine;
namespace CombatGirlsCharacterPack
{
    [ExecuteAlways]
    public class SDFFaceShadowController : MonoBehaviour
    {
        public enum BoneAxis { Forward, Backward, Right, Left, Up, Down }
        public Transform headBone;
        public Renderer[] faceRenderers;
        public BoneAxis headForwardAxis = BoneAxis.Forward;
        public BoneAxis headRightAxis = BoneAxis.Right;
        public bool forceSDFShadow = true;
        public bool enableSDFShadow = true;
        public bool forceScriptVectors = true;
        public string useSDFShadowProp = "_UseSDFShadow";
        public string useScriptVectorsProp = "_UseScriptVectors";
        public string faceForwardProp = "_FaceForward";
        public string faceRightProp = "_FaceRight";
        private MaterialPropertyBlock propBlock;
        private void OnEnable() { Apply(); }
        private void LateUpdate() { Apply(); }
        private void OnValidate() { Apply(); }
        private void Apply()
        {
            if (headBone == null || faceRenderers == null) return;
            Vector3 forward = GetAxisVector(headBone, headForwardAxis).normalized;
            Vector3 right = GetAxisVector(headBone, headRightAxis).normalized;
            if (forward.sqrMagnitude < .000001f) forward = transform.forward;
            if (right.sqrMagnitude < .000001f) right = transform.right;
            foreach (var r in faceRenderers) if (r != null)
            {
                if (propBlock == null) propBlock = new MaterialPropertyBlock();
                r.GetPropertyBlock(propBlock);
                if (forceSDFShadow) propBlock.SetFloat(useSDFShadowProp, enableSDFShadow ? 1 : 0);
                if (forceScriptVectors) propBlock.SetFloat(useScriptVectorsProp, 1);
                propBlock.SetVector(faceForwardProp, forward);
                propBlock.SetVector(faceRightProp, right);
                r.SetPropertyBlock(propBlock);
            }
        }
        private static Vector3 GetAxisVector(Transform t, BoneAxis a)
        {
            switch (a) { case BoneAxis.Backward:return -t.forward; case BoneAxis.Right:return t.right; case BoneAxis.Left:return -t.right; case BoneAxis.Up:return t.up; case BoneAxis.Down:return -t.up; default:return t.forward; }
        }
    }
}
