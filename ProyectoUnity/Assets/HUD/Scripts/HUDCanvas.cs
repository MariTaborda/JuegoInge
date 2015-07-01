using UnityEngine;
using System.Collections;

public class HUDCanvas : MonoBehaviour {

	public static GameObject hud_canvas;
	public static GameObject ActionsMenu;

	public MissionUI mission_UI;
	public ObjectiveMarker objective_marker;

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
