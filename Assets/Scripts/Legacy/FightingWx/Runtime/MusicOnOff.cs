using UnityEngine;
using UnityEngine.UI;

public class MusicOnOff : MonoBehaviour
{
	private Image _image;

	public Sprite[] _sprites;

	private int _ind;

	private void Awake()
	{
		_image = GetComponent<Image>();
		_ind = DataTools.getInt(Config.PP_Music);
		_image.sprite = _sprites[_ind];
	}

	public void OnClickEvent()
	{
		if (_ind == 0)
		{
			_ind = 1;
		}
		else
		{
			_ind = 0;
		}
		DataTools.save(Config.PP_Music, _ind);
		_image.sprite = _sprites[_ind];
		Object.FindObjectOfType<GiveMeTheMusic>().ChangeVolumeImmediate();
	}
}
