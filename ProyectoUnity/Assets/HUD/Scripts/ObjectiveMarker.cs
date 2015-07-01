using UnityEngine;
using System.Collections;

public class ObjectiveMarker : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 target = GameController.gameController.getObjectivePosition ();
		Vector3 screenPos = GameController.gameController.mainCamera.WorldToScreenPoint(target);
		
		if (screenPos.z > 0 && screenPos.x > 0 && screenPos.x < Screen.width && screenPos.y > 0 && screenPos.y < Screen.height) {
			// objective is inside screen boundaries
			transform.position = new Vector3(-999, -999, -999);
		} 
		else {
			// objective is out of screen boundaries

			if(screenPos.z < 0) {
				screenPos *= -1;
			}

			Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0)/2;
			screenPos -= screenCenter;

			float angle = Mathf.Atan2(screenPos.y, screenPos.x);
			angle -= 90 * Mathf.Deg2Rad;
			float cos = Mathf.Cos(angle);
			float sin = -Mathf.Sin(angle);

			screenPos = screenCenter + new Vector3(sin*150, cos*150, 0);
			float m = cos/sin;

			Vector3 screenBounds = screenCenter * 0.9f;
				
			if(cos>0) {
				screenPos = new Vector3(screenBounds.y/m, screenBounds.y, 0);
			}
			else {
				screenPos = new Vector3(-screenBounds.y/m, -screenBounds.y, 0);
			}

			if(screenPos.x > screenBounds.x) {
				screenPos = new Vector3(screenBounds.x, screenBounds.x*m, 0);
			}
			else if(screenPos.x < -screenBounds.x) {
				screenPos = new Vector3(-screenBounds.x, -screenBounds.x*m, 0);
			}

			//screenPos += screenCenter;

			transform.localPosition = screenPos;
			transform.localRotation = Quaternion.Euler(0, 0, angle*Mathf.Rad2Deg);

		}

	}

}
