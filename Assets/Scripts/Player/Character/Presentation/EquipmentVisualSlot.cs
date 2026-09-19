using UnityEngine;

namespace AwCon
{
    /// <summary>Mutually exclusive authored/stowed and spawned/equipped weapon visuals.</summary>
    [DefaultExecutionOrder(10000)]
    public sealed class EquipmentVisualSlot : MonoBehaviour
    {
        [SerializeField] private BBBCharacterController _player;
        [SerializeField] private EquippableItemSO _item;
        [SerializeField] private Renderer[] _stowedRenderers;

        private void Awake()
        {
            if (_player == null) _player = GetComponentInParent<BBBCharacterController>();
        }

        private void LateUpdate()
        {
            if (_player == null || _item == null || _stowedRenderers == null) return;
            bool equipped = _player.EquipmentDriver != null &&
                _player.EquipmentDriver.CurrentItemData == _item;
            foreach (var renderer in _stowedRenderers)
            {
                // Animation clips may animate active/enabled. This render override is independent.
                if (renderer != null) renderer.forceRenderingOff = equipped;
            }
        }

        private void OnDisable()
        {
            if (_stowedRenderers == null) return;
            foreach (var renderer in _stowedRenderers)
                if (renderer != null) renderer.forceRenderingOff = false;
        }
    }
}
