using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerPoints : MonoBehaviour {
	
	[HideInInspector]
	public int ecologicPoints;

	public Slider pointsSlider;
	public GameObject ecologicPointsUI;

	// Use this for initialization
	void Start () {
		ecologicPoints = 0;
		updatePointsUI ();
	}
	
	public void addEcologyPoints(int points) {
		ecologicPoints += points;
		if (ecologicPoints > 100) {
			ecologicPoints = 100;
		}
		updatePointsUI ();
	}
	
	public void reduceEcologyPoints(int points) {
		ecologicPoints -= points;
		if (ecologicPoints < -99) {
			ecologicPoints = -99;
		}
		updatePointsUI ();
	}
	
	void updatePointsUI() {
		pointsSlider.value = ecologicPoints;
		ecologicPointsUI.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (ecologicPoints, 0);
		ecologicPointsUI.GetComponent<Text> ().text = ecologicPoints.ToString();
		if (ecologicPoints < 0) {
			ecologicPointsUI.GetComponent<Text> ().color = Color.red;
		} else {
			ecologicPointsUI.GetComponent<Text> ().color = Color.black;
		}
	}
}
