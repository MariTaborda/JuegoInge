using UnityEngine;
using System.Collections;

public class ActionsMenuMouseHandler : MonoBehaviour {

	public bool MouseOver;

	void OnMouseEnter() {
		MouseOver = true;
		Debug.Log ("mouse entered");
	}
	
	void OnMouseExit() {
		MouseOver = false;
		Debug.Log ("mouse left");
	}
	
}
