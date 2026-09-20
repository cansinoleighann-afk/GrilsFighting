using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GamePlayMenu : MonoBehaviour
{
	public UIFader _uIfader;

	public GameObject[] _menus;

	[HideInInspector]
	public GamePlayManager _gamePlayManager;

	private GameObject _buttonClicked;

	private void Awake()
	{
		_uIfader.gameObject.SetActive(value: true);
		_uIfader.Fade(UIFader.FADE.FadeIn, 1f, 0.3f);
	}

	public void HUDMenu()
	{
		if (!MenuIsAlreadyOpen(0) || MenuIsAlreadyOpen(1))
		{
			_gamePlayManager._makeNoise.PlaySFX("Pick");
			CloseAllMenus();
			OpenMenu(0);
			OpenMenu(4);
			_gamePlayManager._hUDScript.ClearInfoTexts();
			_gamePlayManager._playerControl.CancelInactiveInvoke();
			_gamePlayManager._playerControl._inactive = true;
			_gamePlayManager._playerControl._waitForInput = true;
			Time.timeScale = 1f;
		}
	}

	public void PauseMenu()
	{
		Debug.Log(">>>>>>>>>>>>>>>>>>>>PauseMenu");
		if (!MenuIsAlreadyOpen(1) && !_gamePlayManager._gameOver)
		{
			_gamePlayManager._makeNoise.PlaySFX("Pick");
			CloseAllMenus();
			OpenMenu(1);
			Time.timeScale = 0f;
		}
	}

	public void GameOverMenu()
	{
		if (!MenuIsAlreadyOpen(2))
		{
			CloseAllMenus();
			OpenMenu(2);
		}
	}

	public void LevelCompletedMenu()
	{
		if (!MenuIsAlreadyOpen(3))
		{
			CloseAllMenus();
			OpenMenu(3);
		}
	}

	private void OpenMenu(int ind)
	{
		_menus[ind].SetActive(value: true);
	}

	private void CloseAllMenus()
	{
		GameObject[] menus = _menus;
		foreach (GameObject gameObject in menus)
		{
			gameObject.SetActive(value: false);
		}
	}
  

   

    private bool MenuIsAlreadyOpen(int ind)
	{
		if (_menus[ind].activeSelf)
		{
			return true;
		}
		return false;
	}

	public void ReviveForOrange()
	{
		System.Collections.Generic.Dictionary<string, string> eventData = new System.Collections.Generic.Dictionary<string, string>();
		_gamePlayManager._totalOranges -= _gamePlayManager._levelOrangeCount / 2;
		if (_gamePlayManager._totalOranges < 0)
		{
			_gamePlayManager._totalOranges = 0;
		}
		DataTools.save(Config.PP_Orange_Count, _gamePlayManager._totalOranges);
		_gamePlayManager.RevivePlayer();
	}

	public void WatchVideoToRevive()
	{
		System.Collections.Generic.Dictionary<string, string> eventData = new System.Collections.Generic.Dictionary<string, string>();
		eventData.Add("liucheng", "点击视频复活");
		eventData.Add("state", "revive_video");
		eventData.Add("level", _gamePlayManager._currentLevel.ToString());
		eventData.Add("attempt", GamePlayManager.GetUpcomingLevelAttempt(_gamePlayManager._currentLevel).ToString());
		WebSdk.self.send_AddEvent("GameStatus", eventData);
		AdCon.self.video((isCompleted) =>
		{
			if (isCompleted)
			{
				Revive();
			}
		}, "revive");
	}

	public void Revive()
	{
		_gamePlayManager.RevivePlayer();
	}

	public void WatchVideoToDoubleOrange()
	{
		System.Collections.Generic.Dictionary<string, string> eventData = new System.Collections.Generic.Dictionary<string, string>();
		eventData.Add("liucheng", "点击双倍橙子");
		eventData.Add("state", "double_orange");
		eventData.Add("level", _gamePlayManager._currentLevel.ToString());
		eventData.Add("attempt", GamePlayManager.GetUpcomingLevelAttempt(_gamePlayManager._currentLevel).ToString());
		WebSdk.self.send_AddEvent("GameStatus", eventData);
		AdCon.self.video((isCompleted) =>
		{
			if (isCompleted)
			{
				DoubleOrange();
			}
		}, "double_orange");
    }

	public void DoubleOrange()
	{
		_gamePlayManager.DoubleOranges();
	}

	public void GoToMenu(GameObject go)
	{
        _buttonClicked = go;
        LoadScene("Menu");
		AdCon.self.interAd.show();
    }

	public void RestartLevel(GameObject go)
	{
        _buttonClicked = go;
        //SceneManager.LoadScene("RestartLevelExecute");
        LoadScene("Level");
		AdCon.self.interAd.show();
	}

	public void RestartLevelExecute()
	{
		LoadScene("Level");
	}

	public void NextLevel(GameObject go)
	{
        _buttonClicked = go;
        //if (!_gamePlayManager._levelCompletedScript._doubleOrangeBtn.interactable)
        //{
		NextLevelExecute();
		AdCon.self.interAd.show();
	}

	public void NextLevelExecute()
	{
		GlobalVariables._level = _gamePlayManager._currentLevel + 1;
		LoadScene("Level");
	}

	private void LoadScene(string name)
	{
		Time.timeScale = 1f;
		_gamePlayManager._makeNoise.PlaySFX("Pick");
		ButtonFlicker buttonFlicker = _buttonClicked != null ? _buttonClicked.GetComponent<ButtonFlicker>() : null;
		if (buttonFlicker != null)
		{
			buttonFlicker.StartButtonFlicker();
		}
		StartCoroutine(LoadAsync(name));
		_uIfader.gameObject.SetActive(value: true);
		_uIfader.Fade(UIFader.FADE.FadeOut, 0.5f, 0.5f);
	}

	private IEnumerator LoadAsync(string sceneName)
	{
		AsyncOperation asy = SceneManager.LoadSceneAsync(sceneName);
		while (!asy.isDone)
		{
			yield return null;
		}
	}
}
