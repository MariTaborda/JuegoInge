using UnityEngine;
using System.Collections;

public class ChangeScene : MonoBehaviour {

	public void changeScene(string sceneName){
		Application.LoadLevel(sceneName);
	}
	
}
