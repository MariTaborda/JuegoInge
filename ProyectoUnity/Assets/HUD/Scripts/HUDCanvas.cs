using UnityEngine;
using System.Collections;

public class HUDCanvas : MonoBehaviour {

	public static GameObject hud_canvas;
	public static GameObject ActionsMenu;

	public void init() {

		hud_canvas = gameObject;

		ActionsMenu = transform.Find ("ActionsMenu").gameObject;
		ActionsMenu.GetComponent<ActionsMenu> ().init ();
		ActionsMenu.GetComponent<ActionsMenu> ().disable ();

	}

	// Use this for initialization
	void Start () {
		init ();
	}

}
