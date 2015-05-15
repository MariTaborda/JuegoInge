using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DebugPanel : MonoBehaviour {

	public static string output;
	public static GameObject Me;
	public static GameObject Console;

	void Start() {
		Me = gameObject;
		Console = Me.transform.Find ("Console").gameObject;
	}

	public static void update() {
		//show output
		Console.GetComponent<Text> ().text = output;
		//Debug.Log (output);
	}

	public static void print(string output) {
		DebugPanel.output = output;
		update ();
	}

}
