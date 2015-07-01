using UnityEngine;
using System.Collections;

public class ActionDestroyRoad : PlayerAction {
	
	public Texture2D cursorTexture;

	Vector3 position;
	int chunkIndexX;
	int chunkIndexY;
	int tileIndexX;
	int tileIndexY;
	
	bool position_selected;
	bool invalid_position;

	public void Start() {
		near_distance = 0.9f;
		inv_item = "Pico";
	}

	public override void performAction(GameObject player, GameObject target) {
		
		this.player = player;
		this.target = target;

		if(base.checkInventory (inv_item)) {
			
			position_selected = false;
			invalid_position = false;
			reached_target = false;
			action_interrupted = false;
			
			StartCoroutine( SelectPosition() );
			StartCoroutine( ApproachPosition(player) );
			StartCoroutine( CreateTarget(player) );
			
		}
		else {
			Debug.Log("Player is missing item: "+inv_item);
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
		
		if (!GenerateTerrain.TerrainGenerator.tileContainsPath (chunkIndexX, chunkIndexY, tileIndexX, tileIndexY)) {
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
			
			Debug.Log ("Invalid position for sapling.");
			action_interrupted = true;
			
		} else {
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
		}
	}
	
	IEnumerator CreateTarget (GameObject player) {
		
		PlayerController pc = PlayerController.Player_Controller.GetComponent<PlayerController>();
		
		while(!reached_target && !action_interrupted) {
			yield return null;
		}
		
		if (reached_target) {
			
			GenerateTerrain.TerrainGenerator.destroyPathObject(chunkIndexX,chunkIndexY,tileIndexX,tileIndexY);
			GenerateTerrain.TerrainGenerator.UpdateChunk(chunkIndexX,chunkIndexY);

		}
		
	}
	
}
