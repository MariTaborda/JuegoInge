using UnityEngine;
using System.Collections;

public class ActionPlantSapling : PlayerAction {

	public Texture2D cursorTexture;

	GameObject player;
	GameObject target;
	bool error;
	float near_distance = 0.9f;

	Vector3 position;
	int chunkIndexX;
	int chunkIndexY;
	int tileIndexX;
	int tileIndexY;

	bool position_selected;
	bool invalid_position;
	bool reached_target;
	bool planted_target;
	bool action_interrupted;

	public string invent_item = "Planton";
	
	public override string getInvItem() {
		return invent_item;
	}

	public void performAction(GameObject player) {
		
		this.player = player;
		this.target = target;
		error = false;

		if(base.checkInventory (invent_item)) {

			position_selected = false;
			invalid_position = false;
			reached_target = false;
			planted_target = false;
			action_interrupted = false;

			StartCoroutine( SelectPosition() );
			StartCoroutine( ApproachPosition(player) );
			StartCoroutine( PlantTarget(player) );
			
		}
		else {
			Debug.Log("Player is missing item: "+invent_item);
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
	
	IEnumerator PlantTarget (GameObject player) {

		PlayerController pc = PlayerController.Player_Controller.GetComponent<PlayerController>();

		while(!reached_target && !action_interrupted) {
			yield return null;
		}
		
		if (reached_target) {
			int[,] sceneryTypes = GenerateTerrain.TerrainGenerator.getChunkSceneryMap(chunkIndexX,chunkIndexY);

			sceneryTypes[tileIndexX,tileIndexY] = GameController.gameController.spriteMapper.getRandomTreeId();

			GenerateTerrain.TerrainGenerator.UpdateChunk(chunkIndexX,chunkIndexY);

			PlayerItems items = pc.getCurrentPlayerItems ();
			bool result = items.reduceItem (new Sapling ());
			pc.UpdateBackpackUI();

			planted_target = true;
		}
		
	}

}
