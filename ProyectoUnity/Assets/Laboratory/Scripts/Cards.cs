using UnityEngine;
using System.Collections;

public class Cards : MonoBehaviour {

	public int id;
	public int bmwp;

	int id2;
	int bmwp2; 
	public void Start() {
		//Debug.Log ("Start ids ands bmwps");
		id2 = id; 
		bmwp2 = bmwp; 
	}
	// Use this for initialization
	public int getId () {
		return id2;
	}

	public int getBmwp () {
		return bmwp2;
	}

}
