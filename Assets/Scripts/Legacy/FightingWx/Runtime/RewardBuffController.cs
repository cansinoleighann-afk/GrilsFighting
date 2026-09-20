using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>Connects the in-level rewarded-video buff buttons to the player.</summary>
public class RewardBuffController : MonoBehaviour
{
	private const string BuffRootName = "Buff";
	private const string InvincibleButtonName = "wudi";
	private const string HealButtonName = "zhiliao";
	private const string StrengthButtonName = "3power";

	[SerializeField] private float _invincibleDuration = 10f;
	[SerializeField] private float _strengthDuration = 10f;
	[SerializeField] private float _strengthMultiplier = 3f;

	private GamePlayManager _gamePlayManager;
	private Button _invincibleButton;
	private Button _healButton;
	private Button _strengthButton;
	private bool _adShowing;
	private Coroutine _invincibleRoutine;
	private Coroutine _strengthRoutine;
	private GameObject _invincibleEffect;
	private string _powerUpInfoTextOriginalText;
	private string _infoTextOwner;

	private void Awake()
	{
		_gamePlayManager = GetComponent<GamePlayManager>();
	}

	private void Start()
	{
		Transform buffRoot = GameObject.Find(BuffRootName)?.transform;
		if (buffRoot == null)
		{
			Debug.LogWarning("Reward buffs are unavailable because the Buff UI root was not found.");
			return;
		}

		_invincibleButton = ConfigureButton(buffRoot.Find(InvincibleButtonName), RequestInvincible);
		_healButton = ConfigureButton(buffRoot.Find(HealButtonName), RequestHeal);
		_strengthButton = ConfigureButton(buffRoot.Find(StrengthButtonName), RequestStrength);
		if (_gamePlayManager._powerUpScript != null && _gamePlayManager._powerUpScript._powerUpInfoText != null)
		{
			_powerUpInfoTextOriginalText = _gamePlayManager._powerUpScript._powerUpInfoText.text;
		}
	}

	private Button ConfigureButton(Transform buttonTransform, UnityAction clickAction)
	{
		if (buttonTransform == null)
		{
			Debug.LogWarning("A rewarded buff button is missing from the Buff UI.");
			return null;
		}

		Button button = buttonTransform.GetComponent<Button>() ?? buttonTransform.gameObject.AddComponent<Button>();
		button.targetGraphic = buttonTransform.GetComponent<Graphic>();
		button.onClick.RemoveListener(clickAction);
		button.onClick.AddListener(clickAction);
		return button;
	}

	private void RequestInvincible() { ShowRewardedVideo(GrantInvincible, "reward_buff_invincible"); }
	private void RequestHeal() { ShowRewardedVideo(GrantHeal, "reward_buff_heal"); }
	private void RequestStrength() { ShowRewardedVideo(GrantStrength, "reward_buff_strength"); }

	private void ShowRewardedVideo(UnityAction grantReward, string analyticsName)
	{
		if (_adShowing || !CanGrantReward) return;
		if (ShouldGrantRewardWithoutAd())
		{
			grantReward.Invoke();
			return;
		}

		_adShowing = true;
		SetButtonsInteractable(false);
		AdCon.self.video(completed =>
		{
			_adShowing = false;
			SetButtonsInteractable(true);
			if (completed) grantReward.Invoke();
		}, analyticsName);
	}

	private static bool ShouldGrantRewardWithoutAd()
	{
		return Application.isEditor ||
			Application.platform == RuntimePlatform.WindowsPlayer ||
			Application.platform == RuntimePlatform.OSXPlayer ||
			Application.platform == RuntimePlatform.LinuxPlayer;
	}

	private bool CanGrantReward => _gamePlayManager != null && !_gamePlayManager._gameOver &&
		_gamePlayManager._playerControl != null && _gamePlayManager._playerControl._characterControl != null;

	private void GrantInvincible()
	{
		if (_invincibleRoutine != null) StopCoroutine(_invincibleRoutine);
		DestroyEffect(ref _invincibleEffect);
		_invincibleRoutine = StartCoroutine(InvincibleRoutine());
	}

	private IEnumerator InvincibleRoutine()
	{
		CharacterControl character = _gamePlayManager._playerControl._characterControl;
		character._rewardInvincible = true;
		_invincibleEffect = CreateEffect("ShieldEffect", character.transform, new Color(0.2f, 0.75f, 1f));
		float remaining = _invincibleDuration;
		while (remaining > 0f)
		{
			ShowPowerUpInfoText("invincible", "无敌 " + Mathf.CeilToInt(remaining) + " 秒");
			yield return new WaitForSeconds(1f);
			remaining -= 1f;
		}
		if (character != null) character._rewardInvincible = false;
		DestroyEffect(ref _invincibleEffect);
		HidePowerUpInfoText("invincible");
		_invincibleRoutine = null;
	}

	private void GrantHeal()
	{
		CharacterControl character = _gamePlayManager._playerControl._characterControl;
		character.SetHealth(character._maximumHealth - character._currentHealth);
		StartCoroutine(DestroyEffectAfter(CreateEffect("TreatEffect", character.transform, new Color(0.25f, 1f, 0.35f)), 2f));
	}

	private void GrantStrength()
	{
		if (_strengthRoutine != null)
		{
			StopCoroutine(_strengthRoutine);
			EndStrengthPresentation();
		}
		_strengthRoutine = StartCoroutine(StrengthRoutine());
	}

	private IEnumerator StrengthRoutine()
	{
		CharacterControl character = _gamePlayManager._playerControl._characterControl;
		character._rewardDamageMultiplier = _strengthMultiplier;
		PlayOriginalPowerUpPresentation(character);
		yield return new WaitForSeconds(_strengthDuration);
		if (character != null) character._rewardDamageMultiplier = 1f;
		EndStrengthPresentation();
		_strengthRoutine = null;
	}

	private void PlayOriginalPowerUpPresentation(CharacterControl character)
	{
		ShowPowerUpInfoText("strength", "三倍力量");
		character.SetAnimBool("PowerUpMirror", character._direction < 0);
		character.SetAnimTrigger("PowerUp");
	}

	private void EndStrengthPresentation()
	{
		if (_gamePlayManager == null || _gamePlayManager._powerUpScript == null) return;
		HidePowerUpInfoText("strength");
		if (_gamePlayManager._playerControl != null && _gamePlayManager._playerControl._characterControl != null)
		{
			_gamePlayManager._playerControl._characterControl.PowerUpEnd();
		}
	}

	private void ShowPowerUpInfoText(string owner, string message)
	{
		if (_gamePlayManager == null || _gamePlayManager._powerUpScript == null) return;
		Text infoText = _gamePlayManager._powerUpScript._powerUpInfoText;
		if (infoText == null) return;
		if (_infoTextOwner != owner)
		{
			infoText.gameObject.SetActive(false);
			infoText.gameObject.SetActive(true);
		}
		_infoTextOwner = owner;
		infoText.text = message;
	}

	private void HidePowerUpInfoText(string owner)
	{
		if (_infoTextOwner != owner || _gamePlayManager == null || _gamePlayManager._powerUpScript == null) return;
		Text infoText = _gamePlayManager._powerUpScript._powerUpInfoText;
		if (infoText != null)
		{
			infoText.gameObject.SetActive(false);
			infoText.text = _powerUpInfoTextOriginalText;
		}
		_infoTextOwner = null;
	}

	private GameObject CreateEffect(string resourceName, Transform parent, Color color)
	{
		string resourcePath = "Effect/" + resourceName;
		GameObject prefab = Resources.Load<GameObject>(resourcePath);
		if (prefab == null)
		{
			Debug.LogWarning("Reward buff effect prefab was not found: Resources/" + resourcePath);
			return null;
		}
		GameObject effect = Instantiate(prefab, parent.position, Quaternion.identity, parent);
		effect.transform.localPosition = Vector3.zero;
		foreach (ParticleSystem particleSystem in effect.GetComponentsInChildren<ParticleSystem>())
		{
			ParticleSystem.MainModule main = particleSystem.main;
			main.startColor = color;
		}
		return effect;
	}

	private IEnumerator DestroyEffectAfter(GameObject effect, float delay)
	{
		yield return new WaitForSeconds(delay);
		if (effect != null) Destroy(effect);
	}

	private void DestroyEffect(ref GameObject effect)
	{
		if (effect != null) Destroy(effect);
		effect = null;
	}

	private void SetButtonsInteractable(bool interactable)
	{
		if (_invincibleButton != null) _invincibleButton.interactable = interactable;
		if (_healButton != null) _healButton.interactable = interactable;
		if (_strengthButton != null) _strengthButton.interactable = interactable;
	}

	private void OnDestroy()
	{
		if (_gamePlayManager == null || _gamePlayManager._playerControl == null) return;
		CharacterControl character = _gamePlayManager._playerControl._characterControl;
		if (character == null) return;
		character._rewardInvincible = false;
		character._rewardDamageMultiplier = 1f;
		EndStrengthPresentation();
	}
}
