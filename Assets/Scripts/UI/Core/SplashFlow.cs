using UnityEngine;
using UnityEngine.SceneManagement;

namespace AwCon.UI
{
    public sealed class SplashFlow : MonoBehaviour
    {
        [SerializeField] private float duration = 1.5f;
        private float elapsed;
        private void Update() { elapsed += Time.unscaledDeltaTime; if (elapsed >= duration || Input.anyKeyDown) SceneManager.LoadScene("Menu"); }
    }
}
