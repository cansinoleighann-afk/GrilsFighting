using UnityEngine;

public class GlobalVariables
{
	public static GameObject _player;

	public static int _level;

	// Menu 中激励视频成功后设置；由 GamePlayManager 在进入关卡时消费。
	public static int _superStartWeapon;

	public static GameObject _superStartPanel;
}
