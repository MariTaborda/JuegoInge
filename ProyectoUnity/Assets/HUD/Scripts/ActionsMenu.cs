using UnityEngine;
using System.Collections;

public class ActionsMenu : MonoBehaviour {

	public static GameObject panel;
	public static ActionsMenu actions_menu;
	public static ShowMenu activator;
	public static bool MouseOver;

	public void init() {
		actions_menu = this;
		panel = gameObject;
	}

	public void disable() {
		gameObject.SetActive (false);
	}

}
