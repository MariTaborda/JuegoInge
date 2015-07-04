using UnityEngine;
using System.Collections;

public class MainMenu2 : MonoBehaviour {

	public void ShowLab () {
		PersistentData.Data.prepareData ();
		Application.LoadLevel("laboratory");
	}

}
