using UnityEngine;
using UnityEngine.SceneManagement;

namespace AwCon.UI
{
    public sealed class SceneButton : MonoBehaviour
    {
        [SerializeField] private string sceneName;
        public void Load() { if (!string.IsNullOrEmpty(sceneName)) SceneManager.LoadScene(sceneName); }
        public void Quit() { Application.Quit(); }
    }
}
