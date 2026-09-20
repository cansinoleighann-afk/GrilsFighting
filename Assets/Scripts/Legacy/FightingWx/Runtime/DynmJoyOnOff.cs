using UnityEngine;
using UnityEngine.UI;

public class DynmJoyOnOff : MonoBehaviour
{
	private static readonly string[] DisplayTexts = { "动态摇杆", "固定摇杆" };

	private Text _text;

	public string[] texts;

	private int _ind;

	private void Awake()
	{
		_text = GetComponentInChildren<Text>();
		_ind = DataTools.getInt(Config.PP_DynmJoy);
		_text.text = DisplayTexts[_ind];
	}

	public void OnClickEvent()
	{
		// Static joystick is the only supported control mode.
		_ind = 1;
		DataTools.save(Config.PP_DynmJoy, _ind);
		if (_text != null)
		{
			_text.text = DisplayTexts[_ind];
		}
	}
}
