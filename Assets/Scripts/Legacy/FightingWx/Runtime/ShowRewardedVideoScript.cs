using UnityEngine;
using UnityEngine.UI;

public class ShowRewardedVideoScript : MonoBehaviour
{
	private GameObject InitText;

	private GameObject ShowButton;

	private GameObject ShowText;

	private GameObject AmountText;

	private int userTotalCredits;

	public static string REWARDED_INSTANCE_ID = "0";

	private void Start()
	{
		UnityEngine.Debug.Log("unity-script: ShowRewardedVideoScript Start called");
		ShowButton = GameObject.Find("ShowRewardedVideo");
		ShowText = GameObject.Find("ShowRewardedVideoText");
		ShowText.GetComponent<Text>().color = Color.red;
		AmountText = GameObject.Find("RVAmount");
	}

	private void Update()
	{
	}

	public void ShowRewardedVideoButtonClicked()
	{
		//UnityEngine.Debug.Log("unity-script: ShowRewardedVideoButtonClicked");
		//if (IronSource.Agent.isRewardedVideoAvailable())
		//{
		//	IronSource.Agent.showRewardedVideo();
		//}
		//else
		//{
		//	UnityEngine.Debug.Log("unity-script: IronSource.Agent.isRewardedVideoAvailable - False");
		//}
	}

	private void ShowDemandOnlyRewardedVideo()
	{
		//UnityEngine.Debug.Log("unity-script: ShowDemandOnlyRewardedVideoButtonClicked");
		//if (IronSource.Agent.isISDemandOnlyRewardedVideoAvailable(REWARDED_INSTANCE_ID))
		//{
		//	IronSource.Agent.showISDemandOnlyRewardedVideo(REWARDED_INSTANCE_ID);
		//}
		//else
		//{
		//	UnityEngine.Debug.Log("unity-script: IronSource.Agent.isISDemandOnlyRewardedVideoAvailable - False");
		//}
	}

	private void RewardedVideoAvailabilityChangedEvent(bool canShowAd)
	{
		//UnityEngine.Debug.Log("unity-script: I got RewardedVideoAvailabilityChangedEvent, value = " + canShowAd);
		//if (canShowAd)
		//{
		//	ShowText.GetComponent<Text>().color = Color.blue;
		//}
		//else
		//{
		//	ShowText.GetComponent<Text>().color = Color.red;
		//}
	}

	private void RewardedVideoAdOpenedEvent()
	{
		//UnityEngine.Debug.Log("unity-script: I got RewardedVideoAdOpenedEvent");
	}

	//private void RewardedVideoAdRewardedEvent(IronSourcePlacement ssp)
	//{
	//	UnityEngine.Debug.Log("unity-script: I got RewardedVideoAdRewardedEvent, amount = " + ssp.getRewardAmount() + " name = " + ssp.getRewardName());
	//	userTotalCredits += ssp.getRewardAmount();
	//	AmountText.GetComponent<Text>().text = string.Empty + userTotalCredits;
	//}



	private void RewardedVideoAvailabilityChangedDemandOnlyEvent(string instanceId, bool canShowAd)
	{
		//UnityEngine.Debug.Log("unity-script: I got RewardedVideoAvailabilityChangedDemandOnlyEvent for instance: " + instanceId + ", value = " + canShowAd);
		//if (canShowAd)
		//{
		//	ShowText.GetComponent<Text>().color = Color.blue;
		//}
		//else
		//{
		//	ShowText.GetComponent<Text>().color = Color.red;
		//}
	}

	//private void RewardedVideoAdRewardedDemandOnlyEvent(string instanceId, IronSourcePlacement ssp)
	//{
	//	UnityEngine.Debug.Log("unity-script: I got RewardedVideoAdRewardedDemandOnlyEvent for instance: " + instanceId + ", amount = " + ssp.getRewardAmount() + " name = " + ssp.getRewardName());
	//	userTotalCredits += ssp.getRewardAmount();
	//	AmountText.GetComponent<Text>().text = string.Empty + userTotalCredits;
	//}

}
