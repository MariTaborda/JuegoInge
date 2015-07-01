using UnityEngine;
using System.Collections;

public class ActionCreateRoad : PlayerAction {
	
	public Texture2D workingCursorTexture;
	public Texture2D loadingCursorTexture;
	
	Vector3 position;
	int chunkIndexX;
	int chunkIndexY;
	int tileIndexX;
	int tileIndexY;
	
	bool position_selected;
	bool invalid_position;
	bool created_road;

	public void Start() {
		inv_item = "Piedra";
	}

	public override void performAction(GameObject player, GameObject target) {
		
		this.player = player;
		this.target = target;

		if(base.checkInventory (inv_item)) {
			
			position_selected = false;
			invalid_position = false;
			reached_target = false;
			created_road = false;
			action_interrupted = false;
			
			StartCoroutine( SelectPosition() );
			StartCoroutine( ApproachPosition(player) );
			StartCoroutine( CreateTarget(player) );
			StartCoroutine( FinishAction() );
			
		}
		else {
			Debug.Log("Player is missing item: " + inv_item);
		}
		
	}
	
	IEnumerator SelectPosition () {
		
		PlayerController pc = PlayerController.Player_Controller.GetComponent<PlayerController>();
		
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
		
		if (!GenerateTerrain.TerrainGenerator.tileIsSuitableForScenery (chunkIndexX, chunkIndexY, tileIndexX, tileIndexY)) {
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
			
			Debug.Log ("Invalid position for road.");
			action_interrupted = true;
			
		} else {

			Cursor.SetCursor(loadingCursorTexture, Vector2.zero, CursorMode.Auto);

			PlayerMovement pm = player.GetComponent<PlayerMovement> ();
			pm.setNewDestination (position, near_distance);
			
			while (!pm.reachedDestination && !pm.movementInterrupted) {
				yield return null;
			}
			
			if (pm.movementInterrupted) {
				action_interrupted = true;
			} else {
				reached_target = true;
			}

			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

		}
	}
	
	IEnumerator CreateTarget (GameObject player) {
		
		PlayerController pc = PlayerController.Player_Controller.GetComponent<PlayerController>();
		
		while(!reached_target && !action_interrupted) {
			yield return null;
		}
		
		if (reached_target) {
			PathType[,] pathTypes;
			pathTypes = GenerateTerrain.TerrainGenerator.getChunkPathMap(chunkIndexX,chunkIndexY);
			
			pathTypes[tileIndexX,tileIndexY] = PathType.path1;
			
			GenerateTerrain.TerrainGenerator.UpdateChunk(chunkIndexX,chunkIndexY);

			created_road = true;
		}
		
	}

	IEnumerator FinishAction () {
		while(!created_road && !action_interrupted) {
			yield return null;
		}
		
		if (created_road) {
			PlayerController pc = PlayerController.Player_Controller.GetComponent<PlayerController>();
			PlayerItems items = pc.getCurrentPlayerItems ();
			items.reduceItem (new Rock (), 1);
			pc.UpdateBackpackUI();
			performAction(player, null);		// repite la accion para crear varios caminos seguidamente
		}
		
	}
	
}
