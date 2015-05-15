using UnityEngine;
using System.Collections;

public class MoveCamera : MonoBehaviour {

	public float speed;
	public float camera_speed = 5f;
	private float cameraHeight;

	public bool following;
	public GameObject target;
	public Vector3 cam_offsets;

	public GameObject actionsMenu;

	void Start(){
		cameraHeight = transform.position.y;
		following = true;
		cam_offsets = new Vector3 (18, 0, 18);
	}

	void Update () {

		if (Input.GetAxis ("Mouse ScrollWheel") != 0f) {
			float camSize = Camera.main.fieldOfView;
			camSize -= Input.GetAxis ("Mouse ScrollWheel")*40f;
			camSize = Mathf.Clamp (camSize, 8, 32);
			
			GetComponent<Camera> ().fieldOfView = camSize;
			
			actionsMenu.SetActive (false);
		}

		if (following) {
		
			Vector3 new_position = new Vector3(target.transform.position.x + cam_offsets.x, cameraHeight, target.transform.position.z + cam_offsets.z);
			transform.position = Vector3.Lerp(transform.position, new_position, Time.deltaTime * camera_speed);

			if(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) ||Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {
				following = false;
			}

		} else {
		
			if(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
				transform.Translate(new Vector3(speed * Time.deltaTime,0,0));
				actionsMenu.SetActive (false);
			}

			if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
				transform.Translate(new Vector3(-speed * Time.deltaTime,0,0));
				actionsMenu.SetActive (false);
			}

			if(Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) {
				transform.Translate(new Vector3(0,0,-speed * Time.deltaTime));
				
				// Mantiene la camara siempre a la misma altura
				transform.position = new Vector3(transform.position.x,cameraHeight,transform.position.z);
				
				actionsMenu.SetActive (false);
			}

			if(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {
				transform.Translate(new Vector3(0,0,speed * Time.deltaTime));
				
				// Mantiene la camara siempre a la misma altura
				transform.position = new Vector3(transform.position.x,cameraHeight,transform.position.z);
				
				actionsMenu.SetActive (false);
			}

		}



	}
}
