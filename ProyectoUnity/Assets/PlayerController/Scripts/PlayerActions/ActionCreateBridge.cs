using UnityEngine;
using System.Collections;

public class ActionCreateBridge : PlayerAction {
	
	public Texture2D workingCursorTexture;
	public Texture2D loadingCursorTexture;
	
	Vector3 position;
	int chunkIndexX;
	int chunkIndexY;
	int tileIndexX;
	int tileIndexY;
	
	bool position_selected;
	bool invalid_position;
	bool created_bridge;

	PlayerController pc;

	public void Start() {
		inv_item = "Madera";
	}

	public override void performAction(GameObject player, GameObject target) {
		
		this.player = player;
		this.target = target;

		pc = PlayerController.Player_Controller.GetComponent<PlayerController>();
		
		if(base.checkInventory (inv_item)) {
			
			position_selected = false;
			invalid_position = false;
			reached_target = false;
			created_bridge = false;
			action_interrupted = false;
			
			StartCoroutine( SelectPosition() );
			StartCoroutine( ApproachPosition(player) );
			StartCoroutine( CreateTarget(player) );
			StartCoroutine( FinishAction() );
			
		}
		else {
			Debug.Log("Player is missing item: "+inv_item);
		}
		
	}
	
	IEnumerator SelectPosition () {

		pc.gettingMousePositionOnWorld = true;
		Cursor.SetCursor(workingCursorTexture, Vector2.zero, CursorMode.Auto);
		
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
		
		if (!GenerateTerrain.TerrainGenerator.tileIsSuitableForBridge (chunkIndexX, chunkIndexY, tileIndexX, tileIndexY)) {
			invalid_position = true;
		}
		
		pc.clearMousePositionsOnWorld ();
		pc.gettingMousePositionOnWorld = false;
	}
	
	IEnumerator ApproachPosition (GameObject player) {
		
		while(!position_selected) {
			yield return null;
		}
		
		if (invalid_position) {
			
			Debug.Log ("Invalid position for bridge.");
			action_interrupted = true;
			
		} else {

			Cursor.SetCursor(loadingCursorTexture, Vector2.zero, CursorMode.Auto);

			PlayerMovement pm = player.GetComponent<PlayerMovement> ();
			pm.setNewDestination (position, near_distance);
			
			while (!pm.reachedDestination && !pm.movementInterrupted) {
				yield return null;
			}
			
			if (pm.movementInterrupted) {
				if ( Vector3.Distance(player.transform.position, position) < near_distance) {
					reached_target = true;
				} else {
					action_interrupted = true;
				}
			} else {
				reached_target = true;
			}

			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

		}
	}
	
	IEnumerator CreateTarget (GameObject player) {
		
		while(!reached_target && !action_interrupted) {
			yield return null;
		}
		
		if (reached_target) {
			PathType[,] pathTypes;
			pathTypes = GenerateTerrain.TerrainGenerator.getChunkPathMap(chunkIndexX,chunkIndexY);
			
			pathTypes[tileIndexX,tileIndexY] = PathType.bridge;
			
			GenerateTerrain.TerrainGenerator.UpdateChunk(chunkIndexX,chunkIndexY);

			created_bridge = true;
		}
		
	}

	IEnumerator FinishAction () {
		while(!created_bridge && !action_interrupted) {
			yield return null;
		}

		if (created_bridge) {
			PlayerItems items = pc.getCurrentPlayerItems ();
			items.reduceItem (new Wood (), 1);
			pc.UpdateBackpackUI();
			performAction(player, null);		// repite la accion para crear varios caminos seguidamente
		}

	}
	
}
