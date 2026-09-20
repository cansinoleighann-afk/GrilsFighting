using System.Collections.Generic;
using UnityEngine;

namespace AwCon.UI
{
    public sealed class UIRoot : MonoBehaviour
    {
        [SerializeField] private UIScreen[] screens;
        private readonly Dictionary<string, UIScreen> registry = new Dictionary<string, UIScreen>();
        private void Awake()
        {
            foreach (var screen in screens) if (screen != null && !string.IsNullOrEmpty(screen.ScreenId)) registry[screen.ScreenId] = screen;
            foreach (var pair in registry) pair.Value.Hide();
        }
        public void Show(string id)
        {
            foreach (var pair in registry) { if (pair.Key == id) pair.Value.Show(); else pair.Value.Hide(); }
        }
    }
}
