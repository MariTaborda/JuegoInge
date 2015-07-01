using UnityEngine;
using System.Collections;

public class Slot : MonoBehaviour 
{

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(transform.position,new Vector3(3,2,4));

		Gizmos.color = Color.white;
		Gizmos.DrawCube(transform.position,new Vector3(3,0,4));
	}

}
