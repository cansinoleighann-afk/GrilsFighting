using UnityEngine;

public class JoystickSetup : MonoBehaviour
{
	private const int StaticJoystickIndex = 1;

	public GameObject[] _joysticks;

	private void OnEnable()
	{
		if (_joysticks == null || _joysticks.Length <= StaticJoystickIndex)
		{
			Debug.LogError("JoystickSetup requires a static joystick at index 1.");
			return;
		}

		GameObject[] joysticks = _joysticks;
		foreach (GameObject gameObject in joysticks)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(value: false);
			}
		}
		DataTools.save(Config.PP_DynmJoy, StaticJoystickIndex);
		_joysticks[StaticJoystickIndex].SetActive(value: true);
	}
}
