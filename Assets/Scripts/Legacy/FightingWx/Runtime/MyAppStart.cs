using UnityEngine;

public class MyAppStart : MonoBehaviour
{

	private void OnApplicationPause(bool isPaused)
	{
		UnityEngine.Debug.Log("unity-script: OnApplicationPause = " + isPaused);

	}
}
