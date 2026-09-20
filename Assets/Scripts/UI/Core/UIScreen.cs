using UnityEngine;

namespace AwCon.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIScreen : MonoBehaviour
    {
        [SerializeField] private string screenId;
        private CanvasGroup group;
        public string ScreenId => screenId;
        protected virtual void Awake() { group = GetComponent<CanvasGroup>(); }
        public virtual void Show() { gameObject.SetActive(true); group.alpha = 1; group.interactable = true; group.blocksRaycasts = true; }
        public virtual void Hide() { group.alpha = 0; group.interactable = false; group.blocksRaycasts = false; gameObject.SetActive(false); }
    }
}
