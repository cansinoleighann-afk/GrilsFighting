using UnityEngine;
using UnityEngine.UI;

public class GameOverScript : MonoBehaviour
{
	private string[] GetupMessages = new string[7]
	{
		"站起来，继续战斗！",
		"这么快就放弃了吗？",
		"嘿！敌人还在那儿！",
		"打不到首领吗？",
		"站起来！还有更多战斗等着你。",
		"嘿！别轻易认输！",
		"嘿！打败他们，他们不是真的！"
	};

	private string[] GetupMessagesEarly = new string[4]
	{
		"哇！这么快？",
		"你就这点本事吗？",
		"你知道可以出拳吗？",
		"下次试试踢击吧！"
	};

	public Text _getupMessageText;

	public Text _levelCompletedPercentText;

	public Text _collectedOrangesText;

	public Text _totalOrangesCountText;

	public Button _reviveForOrangeBtn;

	public Text _reviveForOrangeCountText;

	public GameObject _leftSide;

	public RectTransform _rightSide;

	public void FillValues(float percent, int collectedOranges, int totalOranges, bool secondChanceUsed, int reviveForOrangeCount)
	{
		if (percent > 15f)
		{
			_getupMessageText.text = GetupMessages[Random.Range(0, GetupMessages.Length)];
		}
		else
		{
			_getupMessageText.text = GetupMessagesEarly[Random.Range(0, GetupMessagesEarly.Length)];
		}
		_levelCompletedPercentText.text = "% " + percent.ToString("f1");
		_collectedOrangesText.text = collectedOranges.ToString();
		_totalOrangesCountText.text = totalOranges.ToString();
		if (secondChanceUsed)
		{
			_leftSide.SetActive(value: false);
			_rightSide.anchoredPosition = new Vector2(0f, 0f);
			return;
		}
		_leftSide.SetActive(value: true);
		_rightSide.anchoredPosition = new Vector2(440f, 0f);
		_reviveForOrangeCountText.text = $"复活需要\n{reviveForOrangeCount} 个橙子";
		if (totalOranges < reviveForOrangeCount)
		{
			_reviveForOrangeBtn.interactable = false;
			_reviveForOrangeBtn.GetComponent<CanvasGroup>().alpha = 0.5f;
			_reviveForOrangeBtn.transform.GetChild(0).GetComponent<Animator>().enabled = false;
		}
		else
		{
			_reviveForOrangeBtn.interactable = true;
			_reviveForOrangeBtn.GetComponent<CanvasGroup>().alpha = 1f;
			_reviveForOrangeBtn.transform.GetChild(0).GetComponent<Animator>().enabled = true;
		}
	}
}
