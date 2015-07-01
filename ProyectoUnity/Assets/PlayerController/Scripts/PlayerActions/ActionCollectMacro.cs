using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class ActionCollectMacro : PlayerAction {

	public Texture2D cursorTexture;
	private ShowMenu slave; 

	private Vector3 position;
	private int chunkIndexX;
	private int chunkIndexY;
	private int tileIndexX;
	private int tileIndexY;
	
	private bool position_selected;
	private bool invalid_position;

	[HideInInspector]
	public GameObject panel;
	[HideInInspector]
	public RectTransform canvasRT;

	private RectTransform panelRT;
	private Vector2 pointerOffset;

	public void Start() {
		near_distance = 2f;
	}

	public override void performAction(GameObject player, GameObject target) {
		
		this.player = player;
		this.target = target;

		if(true) {	
			reached_target = false;
			action_interrupted = false;
			position_selected = false;
			invalid_position = false;

			StartCoroutine( SelectPosition() );
			StartCoroutine( ApproachPosition(player) );
			StartCoroutine( AnalizeTarget(player) );
			
		}
	}
	
	IEnumerator SelectPosition () {
		
		PlayerController pc = PlayerController.Player_Controller.GetComponent<PlayerController>();
		
		pc.gettingMousePositionOnWorld = true;
		Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
		
		while(pc.chunkIndexX == null) {
			yield return null;
		}
		
		Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		
		position = pc.pointInWorld;
		chunkIndexX = (int)pc.chunkIndexX;
		chunkIndexY = (int)pc.chunkIndexY;
		tileIndexX  = (int)pc.tileIndexX;
		tileIndexY  = (int)pc.tileIndexY;
		position_selected = true;
		
		RaycastHit hit;
		Ray ray; 
		if(Input.GetMouseButtonDown(0)) {
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray, out hit)) {
				//Debug.Log("Mouse Down Hit the following object: " + hit.collider.name);
			// debo llevar acabo la accion con ese collider. 
				if (hit.collider.name != "WaterSurfaceChunk(Clone)") {
					invalid_position = true;
				}
			}
		}
		
		
		pc.clearMousePositionsOnWorld ();
		pc.gettingMousePositionOnWorld = false;
	}
	
	IEnumerator ApproachPosition (GameObject player) {
		
		while(!position_selected) {
			yield return null;
		}
		
		if (invalid_position) {
			
			Debug.Log ("Invalid position to take water sample.");
			action_interrupted = true;
			
		} else {
			// Metodo del padre
			base.ApproachPosition(player, position);
			reached_target = true;
			action_interrupted = true;

		}
	}
	
	IEnumerator AnalizeTarget (GameObject player) {
		
		PlayerController pc = PlayerController.Player_Controller.GetComponent<PlayerController>();
		
		while(!reached_target && !action_interrupted) {
			yield return null;
		}
		
		if (reached_target) { // si llegue al lugarsito donde voy a tomar la muestra
			GameController.gameController.GetComponent<ShowMenu> ().openMenuMacroSample ();
		}
		
	}
}