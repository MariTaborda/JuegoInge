using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour {

	public float speed;
	private float moveSpeed;

	private Vector3 destinationPosition;
	private Vector3 oldDestinationPosition;
	private float destinationDistance;
	private float nearDistance;

	[HideInInspector]
	public bool isMoving = false;

	[HideInInspector]
	public bool reachedDestination;

	[HideInInspector]
	public bool movementInterrupted;

	private PlayerController playerController;
	
	void Update () {

		destinationPosition = new Vector3 (destinationPosition.x, transform.position.y, destinationPosition.z);
		destinationDistance = Vector3.Distance(destinationPosition, transform.position);

		moveTowardsDestination ();

		Debug.DrawRay (transform.position, (destinationPosition - transform.position), Color.red);

		//checkNewDestination ();

	}

	// initialization
	public void init() {
		moveSpeed = speed;
		oldDestinationPosition = Vector3.zero;
		destinationPosition = transform.position;
		playerController = GameController.gameController.playerController;
		//playerController = GameObject.Find("Player Controller").GetComponent<PlayerController>();
	}

	private void moveTowardsDestination() {

		// Si la distancia entre el objeto y el objetivo final es menor que nearDistance se ha llegado al destino
		if(Vector3.Distance(oldDestinationPosition, transform.position) < nearDistance) {
			oldDestinationPosition = Vector3.zero;
			destinationPosition = transform.position;
			destinationDistance = Vector3.Distance(destinationPosition, transform.position);
		}

		// Si la distancia entre el objeto y el objetivo es menor a 0.1 se detiene
		if ( destinationDistance < 0.1f ) {

			if ( oldDestinationPosition == Vector3.zero) {
				// Se llego al destino
				destinationPosition = transform.position;
				isMoving = false;
				reachedDestination = true;
			} else {
				// Hubo una correccion de camino y se resume el destino anterior
				destinationPosition = oldDestinationPosition;
				oldDestinationPosition = Vector3.zero;
			}
		} else {
			// Movimiento en linea recta hasta el punto al que se quiere ir
			transform.position = Vector3.MoveTowards(transform.position, destinationPosition, moveSpeed * Time.deltaTime);
			
			isMoving = true;
			
		}
		
		// Se revisa si el jugador choca con algo
		if ( destinationPosition != transform.position && oldDestinationPosition == Vector3.zero ) {
			RaycastHit hit;
			if ( Physics.Raycast (transform.position, (destinationPosition - transform.position), out hit, 1f )) {
				// El jugador va a chocar con algo, entonces se corrige el curso
				oldDestinationPosition = destinationPosition;
				
				Vector3 newDestinationPosition = (destinationPosition - transform.position).normalized;
				newDestinationPosition = newDestinationPosition + transform.position;
				
				// Se elige el angulo de la correccion del curso
				while ( Physics.Raycast(transform.position, (newDestinationPosition - transform.position), out hit, 4f) && 
				       hit.transform.gameObject.layer != 8 ) {
					newDestinationPosition = RotatePointAroundPivot (newDestinationPosition, transform.position, new Vector3 (0, 1, 0));
					Debug.DrawRay (transform.position, (newDestinationPosition - transform.position), Color.green);
				}
				
				newDestinationPosition = RotatePointAroundPivot (newDestinationPosition, transform.position, new Vector3 (0, 2, 0));
				destinationPosition = newDestinationPosition;
			}
		}

		// Se detiene si entra en contacto con agua, va mas rapido en camino
		if (isMoving) {
			Debug.DrawRay (transform.position, new Vector3(0,-1,0), Color.yellow);
			RaycastHit hit;
			if ( Physics.Raycast (transform.position, new Vector3(0,-1,0), out hit, 1f )) {
				if(hit.transform.gameObject.name == "WaterSurfaceChunk(Clone)") {
					oldDestinationPosition = Vector3.zero;
					isMoving = false;
					movementInterrupted = true;
					destinationPosition = transform.position;
				} else if(hit.transform.gameObject.name == "PathPrefab(Clone)") {
					moveSpeed = 4f;
				} else {
					moveSpeed = speed;
				}
			}
		}

	}

	public void setNewDestination(Vector3 destination) {
		destinationPosition.x = destination.x;
		destinationPosition.y = destination.y + (1f*transform.lossyScale.y);	// 1f se debe reemplazar por la mitad de la altura del jugador
		destinationPosition.z = destination.z;
		reachedDestination = false;
		movementInterrupted = false;
	}

	public void setNewDestination(Vector3 destination, float nearDistance) {
		destinationPosition.x = destination.x;
		destinationPosition.y = destination.y + (1f*transform.lossyScale.y);	// 1f se debe reemplazar por la mitad de la altura del jugador
		destinationPosition.z = destination.z;
		reachedDestination = false;
		movementInterrupted = false;
		this.nearDistance = nearDistance;
	}

	private void setNewDestinationOnMouseCursor() {
	
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		
		if ( Physics.Raycast (ray, out hit) ) {
			if (hit.transform.gameObject.layer == 8) {
				// se selecciono una parte del terreno a la cual desplazarse
				setNewDestination(hit.point);
				nearDistance = 0.5f;
				
				oldDestinationPosition = Vector3.zero;
				
				if(ActionsMenu.panel.activeSelf) {
					ActionsMenu.actions_menu.disable ();
				}
				
				Camera.main.GetComponent<MoveCamera> ().following = true;
				
			}
		}

	}

	// Se elige un nuevo destino con click si el objeto es el jugador actual
	public void checkNewDestination() {
		if ( gameObject == playerController.currentCharacter ) {
			setNewDestinationOnMouseCursor();
			movementInterrupted = true;
		}
	}

	public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
		Vector3 dir = point - pivot;
		dir = Quaternion.Euler(angles) * dir;
		point = dir + pivot;
		return point;
	}

}
