using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Optional settings-button behaviour for the existing haptic preference.
/// Assign the on/off sprites in the Inspector, then bind OnClickEvent to the Button.
/// </summary>
public class HapticOnOff : MonoBehaviour
{
	private Image _image;

	public Sprite[] _sprites;

	private int _ind;

	private void Awake()
	{
		_image = GetComponent<Image>();
		_ind = DataTools.getInt(Config.PP_Haptic);
		if (_image != null && _sprites != null && _sprites.Length > _ind)
		{
			_image.sprite = _sprites[_ind];
		}
	}

	public void OnClickEvent()
	{
		_ind = _ind == 0 ? 1 : 0;
		DataTools.save(Config.PP_Haptic, _ind);
		if (_image != null && _sprites != null && _sprites.Length > _ind)
		{
			_image.sprite = _sprites[_ind];
		}
		if (_ind == 1)
		{
			MusicManage.Instance.zhen();
		}
	}
}
