using UnityEngine;
using System.Collections;

public class TogglePanelButton : MonoBehaviour {
	
	public void TogglePanel (GameObject panel) {

		if (!panel.activeSelf) {
			panel.GetComponent <RectTransform> ().SetAsLastSibling();

			//if((!panel.CompareTag("CardPanel")) && (!panel.CompareTag("creds"))){
				//panel.transform.position = new Vector3(Screen.width/2,((Screen.height-155)/2)+155, 0);
			//}
		} 

		panel.SetActive (!panel.activeSelf);

	}

}