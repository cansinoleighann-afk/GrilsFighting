
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScene : MonoBehaviour
{
	private Animator _animator;

	private AsyncOperation asy;
	private bool _sceneReady;
	private bool _entryReady;
	private bool _entering;

	private void Start()
	{
		if (!DataTools.hasData(Config.PP_Sound))
		{
			DataTools.save(Config.PP_Sound, 1);
		}
		if (!DataTools.hasData(Config.PP_Music))
		{
			DataTools.save(Config.PP_Music, 1);
		}
		if (!DataTools.hasData(Config.PP_Haptic))
		{
			DataTools.save(Config.PP_Haptic, 1);
		}
		if (!DataTools.hasData(Config.PP_Orange_Count))
		{
			DataTools.save(Config.PP_Orange_Count, 50);
		}
		if (!DataTools.hasData(Config.PP_DynmJoy))
		{
			DataTools.save(Config.PP_DynmJoy, 1);
		}
		_animator = GetComponent<Animator>();
		Application.targetFrameRate = 60;
		CallLoadScene();
	}

	public void CallLoadScene()
	{
		StartCoroutine(LoadAsync("Menu"));
	}

	private IEnumerator LoadAsync(string sceneName)
	{
		asy = SceneManager.LoadSceneAsync(sceneName);
		asy.allowSceneActivation = false;
		if (WebSdk.self != null)
		{
			WebSdk.self.login("", () =>
			{
				if (oneLoad.self == null)
				{
					_entryReady = true;
					TryEnterLoadedScene();
					return;
				}

				oneLoad.self.okFun = OnDirectPlayLoadComplete;
				oneLoad.self.show();
			}, () =>
			{
				if (DontDestory.Instance != null)
				{
					DontDestory.Instance.isFeed = true;
				}
			});
		}
		else
		{
			_entryReady = true;
		}
		while (asy.progress < 0.9f)
		{
			yield return null;
		}
		_sceneReady = true;
		TryEnterLoadedScene();
	}

	private void OnDirectPlayLoadComplete()
	{
		if (AdCon.self.ovData == null || AdCon.self.ovData.data == null)
		{
			AdCon.self.load_json_okFun = OnDirectPlayLoadComplete;
			return;
		}

		AdCon.self.load_json_okFun = null;
		_entryReady = true;
		TryEnterLoadedScene();
	}

	private void TryEnterLoadedScene()
	{
		if (_entering || !_sceneReady || !_entryReady || asy == null)
		{
			return;
		}

		_entering = true;
		bool directPlay = DontDestory.Instance != null && DontDestory.Instance.isFeed;
		if (AdCon.self.ovData.zhiWanTest == 1)
		{
			directPlay = true;
		}
		if (directPlay)
		{
			SceneManager.LoadScene("Level");
			return;
		}

		asy.allowSceneActivation = true;
	}

	public void GoToScene()
	{
		asy.allowSceneActivation = true;
	}
}
