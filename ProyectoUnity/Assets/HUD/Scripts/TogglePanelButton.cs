using UnityEngine;
using System.Collections;

public class TogglePanelButton : MonoBehaviour {
	
	public void TogglePanel (GameObject panel) {

		if (!panel.activeSelf) {
			panel.GetComponent <RectTransform> ().SetAsLastSibling ();
		} 

		panel.SetActive (!panel.activeSelf);

	}

}