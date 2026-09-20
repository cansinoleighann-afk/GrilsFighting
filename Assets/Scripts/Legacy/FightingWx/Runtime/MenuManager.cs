using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
public class MenuManager : MonoBehaviour
{
	[Header("Character")]
	public Transform _playersHolder;

	public GameObject[] _characters;

	private GameObject[] _chs;

	private int _characterInd;

	private PlayerControl _playerControl;

	[Header("Orange")]
	private int _orangeCount;

	public Text _orangeCountText;

	[Header("UI")]
	public GameObject _commingSoonText;

	public GameObject _playBtn;

	public GameObject _unlockBtn;

	public Text _unlockPriceText;

	[Header("Super Start")]
	public GameObject _superStartPanel;

	[Range(1, 7)]
	public int _superStartWeapon = 1;

	public string _superStartWeaponName = "棒球棍";

	private GameObject _menuWeaponVisual;

	private GameObject _superStartTip;

	[Header("Upgrade")]
	public CanvasGroup _upgradePanel;

	[Min(0)]
	public int _upgradePriceIncrease = 5;

	[SerializeField] private float _upgradeEffectLifetime = 2f;

	[SerializeField] private Vector3 _upgradeEffectLocalPosition = new Vector3(0f, 1.2f, 0f);

	public Text _playerName;

	public UpgradeUI _healthUI;

	public UpgradeUI _weaponTecniqueUI;

	public Text _comboCount;

	public UpgradeUI _punchPowerUI;

	public UpgradeUI _kickPowerUI;

	[Header("Level")]
	public Text _levelText;

	public Button _prewLevelBtn;

	public Button _nextLevelBtn;

	private int _level;

	private int _lastLevel;

	[Header("Diffuculty")]
	public Text _diffucultyText;

	public Color[] _diffucultyColors;

	private int _diffuculty;
	private bool _upgradeVideoShowing;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Z))
		{
			SetOrangeCount(100);
		}
	}

	[Header("微信游戏圈")]
	[SerializeField] private RectTransform m_quan;

	[Header("Store")]
	public CanvasGroup _storeMenu;


	public GameObject _adRemoved;

	[Header("Credits")]
	public GameObject _credits;

	public Text _creditsInfoText;

	[Header("Fader")]
	public UIFader _uIfader;

	[HideInInspector]
	public MakeNoise _makeNoise;

	private void Awake()
	{
		//if ((bool)UnityEngine.Object.FindObjectOfType<AdManager>())
		//{
		//	AdManager adManager = UnityEngine.Object.FindObjectOfType<AdManager>();
		//	adManager.HideBanner();
		//}

		_makeNoise = GetComponent<MakeNoise>();
		_diffuculty = DataTools.getInt(Config.PP_Diffuculty);
		_diffucultyText.text = Config.Diffuculties[_diffuculty];
		_diffucultyText.color = _diffucultyColors[_diffuculty];
		SetLevelInfoForDiffuculty();

		_orangeCount = DataTools.getInt(Config.PP_Orange_Count);
		//_orangeCount = 99999;
		Debug.Log("UNLI ORANGE");
		UpdateOrangeCountText();

		_chs = new GameObject[_characters.Length];
		for (int i = 0; i < _characters.Length; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(_characters[i], _playersHolder.transform.position, Quaternion.Euler(0f, -90f, 0f));
			gameObject.transform.parent = _playersHolder;
			gameObject.transform.position = _playersHolder.position;
			Animator menuAnimator = gameObject.GetComponentInChildren<Animator>();
			if (menuAnimator != null)
			{
				// Menu characters are static previews. Root Motion would move the
				// cloned root away from PlayersHolder even when gameplay is disabled.
				menuAnimator.applyRootMotion = false;
			}
			Rigidbody menuBody = gameObject.GetComponent<Rigidbody>();
			if (menuBody != null)
			{
				// The gameplay prefab has a dynamic rigidbody. Keep its Menu clone
				// anchored instead of letting gravity move it out of the camera view.
				menuBody.velocity = Vector3.zero;
				menuBody.angularVelocity = Vector3.zero;
				menuBody.useGravity = false;
				menuBody.isKinematic = true;
			}
			//  gameObject.transform.rotation = Quaternion.Euler(0f, 90f, 0f);

			_chs[i] = gameObject;
		}
		if (!DataTools.hasData(Config.PP_SelectedCharacter_ID))
		{
			_characterInd = 0;
		}
		else
		{
			for (int j = 0; j < _chs.Length; j++)
			{
				if (_chs[j].GetComponent<PlayerControl>()._id == DataTools.getInt(Config.PP_SelectedCharacter_ID))
				{
					_characterInd = j;
					break;
				}
			}
		}
		ChangeCharacter();
		_storeMenu.alpha = 0f;
		_storeMenu.interactable = false;
		_storeMenu.blocksRaycasts = false;
		_credits.SetActive(value: false);
		_creditsInfoText.text = "设置\n" + Config.version;
		_uIfader.gameObject.SetActive(value: true);
		_uIfader.enabled = true;
		_uIfader.Fade(UIFader.FADE.FadeIn, 1f, 0.3f);
		if (_superStartPanel != null)
		{
			_superStartPanel.SetActive(value: false);
		}
	}

	private void Start()
	{
		/* Legacy login flow moved to SplashScene.LoadAsync. ()
		{
			oneLoad.self.okFun = enter_game;
			oneLoad.self.show();
		}, () =>
		{
			// 微信直玩资源加载完成回调由 SDK 触发。
			DontDestory.Instance.isFeed = true;
		}); */
		if (DontDestory.Instance != null && !DontDestory.Instance.isFeed)
		{
			AdCon.self.gezi.show(0);
			AdCon.self.gezi.show(1);
			AdCon.self.rank.open_quan(m_quan);
		}
	}

	private void OnDisable()
	{
		AdCon.self.rank.close_quan();
	}

	public void ChangeDifficulty(int i)
	{
		_diffuculty += i;
		if (_diffuculty < 0)
		{
			_diffuculty = Config.Diffuculties.Length - 1;
		}
		if (_diffuculty > Config.Diffuculties.Length - 1)
		{
			_diffuculty = 0;
		}
		DataTools.save(Config.PP_Diffuculty, _diffuculty);
		_diffucultyText.text = Config.Diffuculties[_diffuculty];
		_diffucultyText.color = _diffucultyColors[_diffuculty];
		_makeNoise.PlaySFX("CharSelect");
		SetLevelInfoForDiffuculty();
	}

	public void SetOrangeCount(int count)
	{
		_orangeCount += count;
		DataTools.save(Config.PP_Orange_Count, _orangeCount);
		UpdateOrangeCountText();
		_makeNoise.PlaySFX("ButtonStart");
	}

	private void UpdateOrangeCountText()
	{
		_orangeCountText.text = _orangeCount.ToString();
	}

	public void SelectLevel(int i)
	{
		Debug.Log("<<<<<<<<<<<<<<<<<<<选择关: " + i);
		_makeNoise.PlaySFX("CharSelect");
		_level += i;
		if (_level < 1)
		{
			_level = 1;
		}
		else if (_level > _lastLevel)
		{
			_level = _lastLevel;
		}
		ChangeLevel();
	}

	private void SetLevelInfoForDiffuculty()
	{
		_lastLevel = DataTools.getInt(Config.PP_CityLevel_Index + Config.LevelDiffucultyPrefix[_diffuculty]);
		//_lastLevel = 30;
		if (_lastLevel == 0)
		{
			_lastLevel = 1;
		}
		if (_lastLevel > Config.CityTotalLevel)
		{
			_lastLevel--;
		}
		_level = _lastLevel;
		ChangeLevel();
	}

	private void SetLevelButtons()
	{
		if (_level > 1)
		{
			_prewLevelBtn.interactable = true;
		}
		else
		{
			_prewLevelBtn.interactable = false;
		}
		if (_level < _lastLevel)
		{
			_nextLevelBtn.interactable = true;
		}
		else
		{
			_nextLevelBtn.interactable = false;
		}
	}

	private void ChangeLevel()
	{
		SetLevelButtons();
		_levelText.text = "第 " + _level + " 关";
	}

	public void SelectChar(int i)
	{
		Debug.Log("选择角色: " + i);
		_makeNoise.PlaySFX("CharSelect");
		_characterInd += i;
		if (_characterInd < 0)
		{
			_characterInd = _characters.Length - 1;
		}
		else if (_characterInd > _characters.Length - 1)
		{
			_characterInd = 0;
		}
		ChangeCharacter();
	}

	private void ChangeCharacter()
	{
		ClearMenuWeaponVisual();
		_playerControl = _chs[_characterInd].GetComponent<PlayerControl>();
		_unlockBtn.SetActive(value: false);
		_playBtn.SetActive(value: false);
		_commingSoonText.SetActive(value: false);
		GameObject[] chs = _chs;
		foreach (GameObject gameObject in chs)
		{
			gameObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(value: false);
			gameObject.transform.GetChild(0).GetChild(1).gameObject.SetActive(value: false);
		}
		_chs[_characterInd].transform.GetChild(0).GetChild(0).gameObject.SetActive(value: true);
		_chs[_characterInd].transform.GetChild(0).GetChild(1).gameObject.SetActive(value: true);
		if (_characterInd == _chs.Length - 1)
		{
			_commingSoonText.SetActive(value: true);
			_upgradePanel.alpha = 0f;
			return;
		}
		_playerControl.SetValues();
		ShowMenuWeaponVisualIfNeeded();
		Invoke("RefreshMenuWeaponVisual", 0f);
		FillUpgradeUI();
		if (_playerControl._unlocked)
		{
			_playBtn.SetActive(value: true);
			ActiveDeactiveUpgradeButtons(AcDe: true);
		}
		else if (DataTools.getInt(Config.PP_Character + _playerControl._id) == 0)
		{
			_unlockBtn.SetActive(value: true);
			_unlockPriceText.text = _playerControl._price.ToString();
			ActiveDeactiveUpgradeButtons(AcDe: false);
		}
		else
		{
			_playBtn.SetActive(value: true);
			ActiveDeactiveUpgradeButtons(AcDe: true);
		}
	}

	public void CloseSuperStart()
	{
		if (_superStartPanel != null)
		{
			_superStartPanel.SetActive(value: false);
		}
		BeginGame();
	}

	public void GetSuperStartWeapon()
	{
		System.Collections.Generic.Dictionary<string, string> eventData = new System.Collections.Generic.Dictionary<string, string>();
		eventData.Add("liucheng", "点击超级开局");
		eventData.Add("state", "super_start");
		eventData.Add("level", _level.ToString());
		eventData.Add("attempt", GamePlayManager.GetUpcomingLevelAttempt(_level).ToString());
		WebSdk.self.send_AddEvent("GameStatus", eventData);
		AdCon.self.video((ok) =>
		{
			if (!ok)
			{
				return;
			}

			GlobalVariables._superStartWeapon = _superStartWeapon;
			if (_superStartPanel != null)
			{
				_superStartPanel.SetActive(value: false);
			}
			ShowMenuWeaponVisualIfNeeded();
			Invoke("RefreshMenuWeaponVisual", 0f);
			ShowSuperStartTip("你已获得“" + _superStartWeaponName + "”");
			BeginGame();
		}, "super_start");
	}

	private void RefreshMenuWeaponVisual()
	{
		ShowMenuWeaponVisualIfNeeded();
	}

	private void ShowSuperStartTip(string message)
	{
		Canvas canvas = _superStartPanel.GetComponentInParent<Canvas>();
		if (canvas == null)
		{
			Debug.LogWarning(message);
			return;
		}

		if (_superStartTip == null)
		{
			_superStartTip = new GameObject("SuperStartTip", typeof(RectTransform), typeof(Image));
			_superStartTip.transform.SetParent(canvas.transform, false);
			Image background = _superStartTip.GetComponent<Image>();
			background.color = new Color(0f, 0f, 0f, 0.8f);

			RectTransform tipRect = _superStartTip.GetComponent<RectTransform>();
			tipRect.anchorMin = new Vector2(0.5f, 0.5f);
			tipRect.anchorMax = new Vector2(0.5f, 0.5f);
			tipRect.anchoredPosition = new Vector2(0f, 140f);
			tipRect.sizeDelta = new Vector2(520f, 88f);

			GameObject textObject = new GameObject("Text", typeof(RectTransform), typeof(Text));
			textObject.transform.SetParent(_superStartTip.transform, false);
			Text text = textObject.GetComponent<Text>();
			text.font = _orangeCountText.font;
			text.fontSize = 34;
			text.alignment = TextAnchor.MiddleCenter;
			text.color = Color.white;
			RectTransform textRect = textObject.GetComponent<RectTransform>();
			textRect.anchorMin = Vector2.zero;
			textRect.anchorMax = Vector2.one;
			textRect.offsetMin = Vector2.zero;
			textRect.offsetMax = Vector2.zero;
		}

		_superStartTip.GetComponentInChildren<Text>().text = message;
		_superStartTip.SetActive(value: true);
		_superStartTip.transform.SetAsLastSibling();
		CancelInvoke("HideSuperStartTip");
		Invoke("HideSuperStartTip", 1.5f);
	}

	private void HideSuperStartTip()
	{
		if (_superStartTip != null)
		{
			_superStartTip.SetActive(value: false);
		}
	}

	private void ShowMenuWeaponVisualIfNeeded()
	{
		if (GlobalVariables._superStartWeapon <= 0 || GlobalVariables._superStartWeapon >= Config.WeaponNames.Length || _playerControl == null)
		{
			return;
		}
		ClearMenuWeaponVisual();

		CharacterControl characterControl = _playerControl.GetComponent<CharacterControl>();
		GameObject weaponPrefab = Resources.Load<GameObject>("items/" + Config.WeaponNames[GlobalVariables._superStartWeapon]);
		if (characterControl == null || characterControl.RightHandWeaponHolder == null || weaponPrefab == null)
		{
			Debug.LogWarning("Super Start 武器展示配置无效。");
			return;
		}

		_menuWeaponVisual = UnityEngine.Object.Instantiate(weaponPrefab, characterControl.RightHandWeaponHolder);
		_menuWeaponVisual.transform.localPosition = Vector3.zero;
		_menuWeaponVisual.transform.localRotation = Quaternion.identity;
		SetLayerRecursively(_menuWeaponVisual, _playerControl.gameObject.layer);
		foreach (Renderer renderer in _menuWeaponVisual.GetComponentsInChildren<Renderer>(true))
		{
			renderer.enabled = true;
		}

		Weapons weapon = _menuWeaponVisual.GetComponent<Weapons>();
		if (weapon != null)
		{
			weapon.enabled = false;
		}

		Rigidbody weaponBody = _menuWeaponVisual.GetComponent<Rigidbody>();
		if (weaponBody != null)
		{
			weaponBody.isKinematic = true;
		}
	}

	private void SetLayerRecursively(GameObject gameObject, int layer)
	{
		gameObject.layer = layer;
		foreach (Transform child in gameObject.transform)
		{
			SetLayerRecursively(child.gameObject, layer);
		}
	}

	private void ClearMenuWeaponVisual()
	{
		if (_menuWeaponVisual != null)
		{
			Destroy(_menuWeaponVisual);
			_menuWeaponVisual = null;
		}
	}

	private void ActiveDeactiveUpgradeButtons(bool AcDe)
	{
		if (AcDe)
		{
			_upgradePanel.alpha = 1f;
		}
		else
		{
			_upgradePanel.alpha = 0.75f;
		}
		SetUpgradeButtonAvailable(_healthUI, AcDe);
		SetUpgradeButtonAvailable(_weaponTecniqueUI, AcDe);
		SetUpgradeButtonAvailable(_punchPowerUI, AcDe);
		SetUpgradeButtonAvailable(_kickPowerUI, AcDe);
		CheckUpgradeCounts();
	}

	private void CheckUpgradeCounts()
	{
		if (DataTools.getInt(_playerControl._id.ToString() + Config.PP_CharacterHealthUpgradeCount) >= Config.MaximumUpgradeCount)
		{
			SetUpgradeButtonAvailable(_healthUI, available: false);
		}
		if (DataTools.getInt(_playerControl._id.ToString() + Config.PP_CharacterWeaponTecniqueUpgradeCount) >= Config.MaximumUpgradeCount)
		{
			SetUpgradeButtonAvailable(_weaponTecniqueUI, available: false);
		}
		if (DataTools.getInt(_playerControl._id.ToString() + Config.PP_CharacterPunchPowerUpgradeCount) >= Config.MaximumUpgradeCount)
		{
			SetUpgradeButtonAvailable(_punchPowerUI, available: false);
		}
		if (DataTools.getInt(_playerControl._id.ToString() + Config.PP_CharacterKickPowerUpgradeCount) >= Config.MaximumUpgradeCount)
		{
			SetUpgradeButtonAvailable(_kickPowerUI, available: false);
		}
	}

	private int HealthUpgradePrice => GetUpgradePrice(_playerControl._healthUpPrice, Config.PP_CharacterHealthUpgradeCount);
	private int WeaponTecniqueUpgradePrice => GetUpgradePrice(_playerControl._weaponTecUpPrice, Config.PP_CharacterWeaponTecniqueUpgradeCount);
	private int PunchPowerUpgradePrice => GetUpgradePrice(_playerControl._punchUpgradePrice, Config.PP_CharacterPunchPowerUpgradeCount);
	private int KickPowerUpgradePrice => GetUpgradePrice(_playerControl._kickUpgradePrice, Config.PP_CharacterKickPowerUpgradeCount);

	private int GetUpgradePrice(int basePrice, string upgradeCountKey)
	{
		int upgradeCount = DataTools.getInt(_playerControl._id + upgradeCountKey);
		return (basePrice + upgradeCount * _upgradePriceIncrease) * GetCharacterQuality();
	}

	/// <summary>
	/// 从角色预制体名称的首位数字读取品质，例如 1ManA 为品质 1、3ManB 为品质 3。
	/// </summary>
	private int GetCharacterQuality()
	{
		string characterName = _playerControl.gameObject.name;
		if (!string.IsNullOrEmpty(characterName) && char.IsDigit(characterName[0]))
		{
			return Mathf.Max(1, characterName[0] - '0');
		}

		return 1;
	}

	public void UpgradeHealth()
	{
		System.Collections.Generic.Dictionary<string, string> eventData = new System.Collections.Generic.Dictionary<string, string>();
		eventData.Add("升级属性", "生命");
		eventData.Add("state", "health");
		eventData.Add("level", _level.ToString());
		eventData.Add("attempt", GamePlayManager.GetUpcomingLevelAttempt(_level).ToString());
		WebSdk.self.send_AddEvent("PowerUp", eventData);
		if (NotEnoughOranges(HealthUpgradePrice)) RequestHealthVideoUpgrade();
		else UpgradeHealthWithCurrency();
	}

	public void UpgradeWeaponTecnique()
	{
		System.Collections.Generic.Dictionary<string, string> eventData = new System.Collections.Generic.Dictionary<string, string>();
		eventData.Add("升级属性", "武器技巧");
		eventData.Add("state", "weapon_technique");
		eventData.Add("level", _level.ToString());
		eventData.Add("attempt", GamePlayManager.GetUpcomingLevelAttempt(_level).ToString());
		WebSdk.self.send_AddEvent("PowerUp", eventData);
		if (NotEnoughOranges(WeaponTecniqueUpgradePrice)) RequestWeaponTecniqueVideoUpgrade();
		else UpgradeWeaponTecniqueWithCurrency();
	}

	public void UpgradePunchPower()
	{
		System.Collections.Generic.Dictionary<string, string> eventData = new System.Collections.Generic.Dictionary<string, string>();
		eventData.Add("升级属性", "拳力");
		eventData.Add("state", "punch_power");
		eventData.Add("level", _level.ToString());
		eventData.Add("attempt", GamePlayManager.GetUpcomingLevelAttempt(_level).ToString());
		WebSdk.self.send_AddEvent("PowerUp", eventData);
		if (NotEnoughOranges(PunchPowerUpgradePrice)) RequestPunchPowerVideoUpgrade();
		else UpgradePunchPowerWithCurrency();
	}

	public void UpgradeKickPower()
	{
		System.Collections.Generic.Dictionary<string, string> eventData = new System.Collections.Generic.Dictionary<string, string>();
		eventData.Add("升级属性", "腿力");
		eventData.Add("state", "kick_power");
		eventData.Add("level", _level.ToString());
		eventData.Add("attempt", GamePlayManager.GetUpcomingLevelAttempt(_level).ToString());
		WebSdk.self.send_AddEvent("PowerUp", eventData);
		if (NotEnoughOranges(KickPowerUpgradePrice)) RequestKickPowerVideoUpgrade();
		else UpgradeKickPowerWithCurrency();
	}

	private void UpgradeHealthWithCurrency() { UpgradeHealth(false); }
	private void UpgradeWeaponTecniqueWithCurrency() { UpgradeWeaponTecnique(false); }
	private void UpgradePunchPowerWithCurrency() { UpgradePunchPower(false); }
	private void UpgradeKickPowerWithCurrency() { UpgradeKickPower(false); }

	private void RequestHealthVideoUpgrade() { RequestVideoUpgrade(UpgradeHealthWithVideo); }
	private void RequestWeaponTecniqueVideoUpgrade() { RequestVideoUpgrade(UpgradeWeaponTecniqueWithVideo); }
	private void RequestPunchPowerVideoUpgrade() { RequestVideoUpgrade(UpgradePunchPowerWithVideo); }
	private void RequestKickPowerVideoUpgrade() { RequestVideoUpgrade(UpgradeKickPowerWithVideo); }

	private void UpgradeHealthWithVideo() { UpgradeHealth(true); }
	private void UpgradeWeaponTecniqueWithVideo() { UpgradeWeaponTecnique(true); }
	private void UpgradePunchPowerWithVideo() { UpgradePunchPower(true); }
	private void UpgradeKickPowerWithVideo() { UpgradeKickPower(true); }

	private void UpgradeHealth(bool free)
	{
		UpgradePlayer(_playerControl._id + Config.PP_CharacterHealthUpgradeCount, DataTools.getInt(_playerControl._id + Config.PP_CharacterHealthUpgradeCount) + 1, HealthUpgradePrice, !free);
	}

	private void UpgradeWeaponTecnique(bool free)
	{
		UpgradePlayer(_playerControl._id + Config.PP_CharacterWeaponTecniqueUpgradeCount, DataTools.getInt(_playerControl._id + Config.PP_CharacterWeaponTecniqueUpgradeCount) + 1, WeaponTecniqueUpgradePrice, !free);
	}

	private void UpgradePunchPower(bool free)
	{
		UpgradePlayer(_playerControl._id + Config.PP_CharacterPunchPowerUpgradeCount, DataTools.getInt(_playerControl._id + Config.PP_CharacterPunchPowerUpgradeCount) + 1, PunchPowerUpgradePrice, !free);
	}

	private void UpgradeKickPower(bool free)
	{
		UpgradePlayer(_playerControl._id + Config.PP_CharacterKickPowerUpgradeCount, DataTools.getInt(_playerControl._id + Config.PP_CharacterKickPowerUpgradeCount) + 1, KickPowerUpgradePrice, !free);
	}

	private void RequestVideoUpgrade(UnityAction grantUpgrade)
	{
		if (_upgradeVideoShowing) return;
		_upgradeVideoShowing = true;
		AdCon.self.video((ok) =>
		{
			_upgradeVideoShowing = false;
			if (ok) grantUpgrade.Invoke();
		}, "视频免费升级");
	}

	private void UpgradePlayer(string key, int value, int price, bool payWithOranges)
	{
		if (payWithOranges) SetOrangeCount(-price);
		DataTools.save(key, value);
		_playerControl.SetValues();
		FillUpgradeUI();
		CheckUpgradeCounts();
		PlayUpgradeEffect();
	}

	/// <summary>
	/// 玩家升级效果播放升级特效
	/// </summary>
	private void PlayUpgradeEffect()
	{
		if (_playerControl == null) return;

		GameObject effectPrefab = Resources.Load<GameObject>("Effect/UpgradeUpEffect");
		if (effectPrefab == null)
		{
			Debug.LogWarning("Upgrade effect prefab was not found: Resources/Effect/UpgradeUpEffect.");
			return;
		}

		Transform playerTransform = _playerControl.transform;
		Vector3 effectPosition = playerTransform.TransformPoint(_upgradeEffectLocalPosition);
		GameObject effect = Instantiate(effectPrefab, effectPosition, playerTransform.rotation, playerTransform);
		SetLayerRecursively(effect, _playerControl.gameObject.layer);
		Destroy(effect, _upgradeEffectLifetime);
	}

	public void UnlockCharacter()
	{
		if (!NotEnoughOranges(_playerControl._price))
		{
			SetOrangeCount(-_playerControl._price);
			DataTools.save(Config.PP_Character + _playerControl._id, 1);
			_unlockBtn.SetActive(value: false);
			_playBtn.SetActive(value: true);
			ActiveDeactiveUpgradeButtons(AcDe: true);
		}
	}

	private void FillUpgradeUI()
	{
		ResolveUpgradeUIReferences(_healthUI);
		ResolveUpgradeUIReferences(_weaponTecniqueUI);
		ResolveUpgradeUIReferences(_punchPowerUI);
		ResolveUpgradeUIReferences(_kickPowerUI);
		_playerName.text = _playerControl._name;
		_healthUI.Value.text = _playerControl._healthU.ToString("f1");
		_healthUI.Price.text = HealthUpgradePrice.ToString();
		_healthUI.Bar.fillAmount = _playerControl._healthU / Config.MaximumPlayerHealth;
		_weaponTecniqueUI.Value.text = (_playerControl._weaponTecniqueU * 10f).ToString("f1");
		_weaponTecniqueUI.Price.text = WeaponTecniqueUpgradePrice.ToString();
		_weaponTecniqueUI.Bar.fillAmount = _playerControl._weaponTecniqueU / Config.MaximumPlayerWeaponTecnique;
		_comboCount.text = _playerControl.GetComponent<CharacterControl>()._punches.Length + _playerControl.GetComponent<CharacterControl>()._kicks.Length + " 连击";
		_punchPowerUI.Value.text = _playerControl._punchPowerU.ToString("f1");
		_punchPowerUI.Price.text = PunchPowerUpgradePrice.ToString();
		_punchPowerUI.Bar.fillAmount = _playerControl._punchPowerU / Config.MaximumPlayerPunchPower;
		_kickPowerUI.Value.text = _playerControl._kickPowerU.ToString("f1");
		_kickPowerUI.Price.text = KickPowerUpgradePrice.ToString();
		_kickPowerUI.Bar.fillAmount = _playerControl._kickPowerU / Config.MaximumPlayerKickPower;
		UpdateUpgradePaymentDisplay(_healthUI, HealthUpgradePrice);
		UpdateUpgradePaymentDisplay(_weaponTecniqueUI, WeaponTecniqueUpgradePrice);
		UpdateUpgradePaymentDisplay(_punchPowerUI, PunchPowerUpgradePrice);
		UpdateUpgradePaymentDisplay(_kickPowerUI, KickPowerUpgradePrice);
	}

	private void UpdateUpgradePaymentDisplay(UpgradeUI upgradeUI, int price)
	{
		bool useVideo = NotEnoughOranges(price);
		GameObject orange = FindUpgradeButtonChild(upgradeUI, "Orange");
		GameObject video = FindUpgradeButtonChild(upgradeUI, "Video");
		if (orange != null) orange.SetActive(!useVideo);
		if (video != null) video.SetActive(useVideo);
	}

	private void SetUpgradeButtonAvailable(UpgradeUI upgradeUI, bool available)
	{
		GameObject upgradeButton = FindUpgradeButtonChild(upgradeUI, "UpgradeButton");
		if (upgradeButton == null)
		{
			upgradeButton = FindUpgradeButtonChild(upgradeUI, "UpgradeButtonRight");
		}

		if (upgradeButton != null)
		{
			upgradeButton.SetActive(available);
		}
	}

	private GameObject FindUpgradeButtonChild(UpgradeUI upgradeUI, string childName)
	{
		foreach (Transform child in upgradeUI.GetComponentsInChildren<Transform>(includeInactive: true))
		{
			if (child.name == childName) return child.gameObject;
		}

		return null;
	}

	private void ResolveUpgradeUIReferences(UpgradeUI upgradeUI)
	{
		if (upgradeUI.Price == null)
		{
			foreach (Text text in upgradeUI.GetComponentsInChildren<Text>(includeInactive: true))
			{
				if (text.name == "Price")
				{
					upgradeUI.Price = text;
					break;
				}
			}
		}

		if (upgradeUI.Btn == null)
		{
			GameObject upgradeArrow = FindUpgradeButtonChild(upgradeUI, "UpgradeArrow");
			if (upgradeArrow != null) upgradeUI.Btn = upgradeArrow;
		}
	}

	private bool NotEnoughOranges(int price)
	{
		if (price > _orangeCount)
		{
			//OpenCloseStoreMenu();
			return true;
		}
		return false;
	}

	public void OpenCloseStoreMenu()
	{
		_makeNoise.PlaySFX("CharSelect");
		if (_storeMenu.interactable)
		{
			_storeMenu.alpha = 0f;
			_storeMenu.interactable = false;
			_storeMenu.blocksRaycasts = false;
			return;
		}
		//_adNotRemoved.SetActive(value: false);

		//if (DataTools.getInt(Config.PP_Removed_Ads) == 1)
		//{
		_adRemoved.SetActive(value: true);
		//}
		//else
		//{
		//_adNotRemoved.SetActive(value: true);
		//}
		_storeMenu.alpha = 1f;
		_storeMenu.interactable = true;
		_storeMenu.blocksRaycasts = true;
	}

	public void OpenCloseCreditMenu()
	{
		_makeNoise.PlaySFX("CharSelect");
		_credits.SetActive(!_credits.activeSelf);
		if (_credits.activeSelf)
		{
			AdCon.self.rank.close_quan();
			AdCon.self.gezi.show(2);
			AdCon.self.gezi.show(3);
			AdCon.self.gezi.hide(0);
			AdCon.self.gezi.hide(1);
		}
		else
		{
			AdCon.self.rank.open_quan(m_quan);
			AdCon.self.gezi.hide(2);
			AdCon.self.gezi.hide(3);
			AdCon.self.gezi.show(0);
			AdCon.self.gezi.show(1);
		}
	}

	public void PrivacyURL()
	{
		Application.OpenURL("https://endlessjourneyrunprivacypolicy.blogspot.com/2020/10/super-street-fighter-privacy-policy.html");
	}

	public void TermsURL()
	{
		Application.OpenURL("https://www.aryasgames.com/terms-conditions");
	}

	public void PlayGame()
	{
		GlobalVariables._superStartWeapon = 0;
		if (_superStartPanel != null)
		{
			BindSuperStartButtons();
			_superStartPanel.SetActive(value: true);
			return;
		}

		BeginGame();
	}

	private void BindSuperStartButtons()
	{
		Button[] buttons = _superStartPanel.GetComponentsInChildren<Button>(includeInactive: true);
		foreach (Button button in buttons)
		{
			if (button.name == "NoBtn")
			{
				button.onClick = new Button.ButtonClickedEvent();
				button.onClick.AddListener(CloseSuperStart);
			}
			else if (button.name == "GetBtn")
			{
				button.onClick = new Button.ButtonClickedEvent();
				button.onClick.AddListener(GetSuperStartWeapon);
			}
		}
	}

	private void BeginGame()
	{
		System.Collections.Generic.Dictionary<string, string> startEventData = new System.Collections.Generic.Dictionary<string, string>();
		startEventData.Add("state", "点击开始游戏");
		startEventData.Add("level", _level.ToString());
		startEventData.Add("attempt", GamePlayManager.GetUpcomingLevelAttempt(_level).ToString());
		WebSdk.self.send_AddEvent("StartGame", startEventData);
		AdCon.self.rank.close_quan();
		AdCon.self.gezi.hide();
		AdCon.self.interAd.show();
		_makeNoise.PlaySFX("ButtonStart");
		GlobalVariables._player = _characters[_characterInd];
		GlobalVariables._level = _level;


		DataTools.save(Config.PP_SelectedCharacter_ID, _chs[_characterInd].GetComponent<PlayerControl>()._id);
		// AnalyticsManager.SendEventInfo(Config.Diffuculties[DataTools.getInt(Config.PP_Diffuculty)]);
		if (WebSdk.self != null)
		{
			string difficulty = Config.Diffuculties[DataTools.getInt(Config.PP_Diffuculty)];
			System.Collections.Generic.Dictionary<string, string> eventData = new System.Collections.Generic.Dictionary<string, string>();
			eventData.Add("state", difficulty);
			eventData.Add("level", _level.ToString());
			eventData.Add("attempt", GamePlayManager.GetUpcomingLevelAttempt(_level).ToString());
			WebSdk.self.send_AddEvent(difficulty, eventData);
		}
		_uIfader.gameObject.SetActive(value: true);
		_uIfader.enabled = true;
		_uIfader.Fade(UIFader.FADE.FadeOut, 0.3f, 0f);
		Invoke("loadLevel", 0.5f);
	}

	private void loadLevel()
	{
		SceneManager.LoadScene("Level");
	}
}
