using UnityEngine;
using System.Collections;

public class AdjustSceneryCollider : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
		GetComponent<BoxCollider> ().size = new Vector3 (0.4f, 0.4f, 0.4f);
		GetComponent<BoxCollider> ().center = new Vector3 (0, 0.2f, 0);

	}
	
	// Update is called once per frame
	void Update () {
	
		Quaternion new_rotation = Quaternion.Euler (Vector3.zero);
		transform.rotation = new_rotation;

	}

}
