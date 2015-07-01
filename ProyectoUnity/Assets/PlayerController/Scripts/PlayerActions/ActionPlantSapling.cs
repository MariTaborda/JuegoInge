using UnityEngine;
using System.Collections;

public class ActionPlantSapling : PlayerAction {
	
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
		inv_item = "Planton";
	}
	
	public override void performAction (GameObject player, GameObject target) {
		performAction (player, 1);
	}
	
	public void performAction(GameObject player,int tree) {
		
		this.player = player;
		this.target = target;
		
		if(base.checkInventory (inv_item)) {
			
			position_selected = false;
			invalid_position = false;
			reached_target = false;
			action_interrupted = false;
			
			StartCoroutine( SelectPosition() );
			StartCoroutine( ApproachPosition(player) );
			StartCoroutine( PlantTarget(player,tree) );
			
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
	
	IEnumerator PlantTarget (GameObject player,int tree) {
		
		PlayerController pc = PlayerController.Player_Controller.GetComponent<PlayerController>();
		PlayerPoints points =PlayerController.Player_Controller.GetComponent<PlayerPoints>();
		
		while(!reached_target && !action_interrupted) {
			yield return null;
		}
		
		if (reached_target) {
			int[,] sceneryTypes = GenerateTerrain.TerrainGenerator.getChunkSceneryMap(chunkIndexX,chunkIndexY);
			
			sceneryTypes[tileIndexX,tileIndexY] = tree;
			
			GenerateTerrain.TerrainGenerator.UpdateChunk(chunkIndexX,chunkIndexY);
			
			PlayerItems items = pc.getCurrentPlayerItems ();
			bool result = items.reduceItem (new Sapling (),1);
			pc.UpdateBackpackUI();
			
			points.addEcologyPoints(5);
			
			if (MissionPlantTree.singleton != null && MissionPlantTree.singleton.started) {
				MissionPlantTree.singleton.onPlantTree();
			}
		}
		
	}
	
}
