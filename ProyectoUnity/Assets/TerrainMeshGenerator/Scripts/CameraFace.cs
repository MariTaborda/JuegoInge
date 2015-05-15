using UnityEngine;
using System.Collections;

public class CameraFace : MonoBehaviour {

	public Camera m_Camera;

	void Update() {
		transform.LookAt(
			transform.position + m_Camera.transform.rotation * Vector3.back,
		    m_Camera.transform.rotation * Vector3.up
		);
	}


	/*var lookPos = target.position - transform.position;
	lookPos.y = 0;
	var rotation = Quaternion.LookRotation(lookPos);
	transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);*/

}