using UnityEngine;
using UnityEngine.UI;

public class ShowInterstitialScript : MonoBehaviour
{
	private GameObject InitText;

	private GameObject LoadButton;

	private GameObject LoadText;

	private GameObject ShowButton;

	private GameObject ShowText;

	public static string INTERSTITIAL_INSTANCE_ID = "0";

	private void Start()
	{
		UnityEngine.Debug.Log("unity-script: ShowInterstitialScript Start called");
		LoadButton = GameObject.Find("LoadInterstitial");
		LoadText = GameObject.Find("LoadInterstitialText");
		LoadText.GetComponent<Text>().color = Color.blue;
		ShowButton = GameObject.Find("ShowInterstitial");
		ShowText = GameObject.Find("ShowInterstitialText");
		ShowText.GetComponent<Text>().color = Color.red;
	}

	private void Update()
	{
	}

	public void LoadInterstitialButtonClicked()
	{
		//UnityEngine.Debug.Log("unity-script: LoadInterstitialButtonClicked");
		//IronSource.Agent.loadInterstitial();
	}

	public void ShowInterstitialButtonClicked()
	{
		//UnityEngine.Debug.Log("unity-script: ShowInterstitialButtonClicked");
		//if (IronSource.Agent.isInterstitialReady())
		//{
		//	IronSource.Agent.showInterstitial();
		//}
		//else
		//{
		//	UnityEngine.Debug.Log("unity-script: IronSource.Agent.isInterstitialReady - False");
		//}
	}

	private void LoadDemandOnlyInterstitial()
	{
		//UnityEngine.Debug.Log("unity-script: LoadDemandOnlyInterstitialButtonClicked");
		//IronSource.Agent.loadISDemandOnlyInterstitial(INTERSTITIAL_INSTANCE_ID);
	}

	private void ShowDemandOnlyInterstitial()
	{
		//UnityEngine.Debug.Log("unity-script: ShowDemandOnlyInterstitialButtonClicked");
		//if (IronSource.Agent.isISDemandOnlyInterstitialReady(INTERSTITIAL_INSTANCE_ID))
		//{
		//	IronSource.Agent.showISDemandOnlyInterstitial(INTERSTITIAL_INSTANCE_ID);
		//}
		//else
		//{
		//	UnityEngine.Debug.Log("unity-script: IronSource.Agent.isISDemandOnlyInterstitialReady - False");
		//}
	}


}
