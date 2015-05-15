using UnityEngine;
using System.Collections;

public class ActionChopTree : MonoBehaviour {

	GameObject player;
	GameObject target;
	bool error;
	float near_distance = 0.8f;

	bool reached_target;
	bool destroyed_target;
	bool action_interrupted;

	public void performAction(GameObject player, GameObject target) {
	
		this.player = player;
		this.target = target;
		error = false;

		if (checkActors ()) {
			if(checkInventory ()) {
			
				reached_target = false;
				destroyed_target = false;
				action_interrupted = false;

				StartCoroutine( ApproachPosition(player, target.transform.position) );
				StartCoroutine( DestroyTarget(player, target) );
			
			}
			else {
				Debug.Log("Player missing pickaxe");
			}
		} 
		else {
			Debug.Log("Invalid actors");
		}

	}

	IEnumerator ApproachPosition (GameObject player, Vector3 target_position) {

		PlayerMovement pm = player.GetComponent<PlayerMovement> ();
		pm.setNewDestination (target_position, near_distance);

		// espere mientras la condicion sea verdadera
		while(!pm.reachedDestination && !pm.movementInterrupted) {
			yield return null;
		}

		if (pm.movementInterrupted) {
			action_interrupted = true;
		} else {
			reached_target = true;
		}
	}

	IEnumerator DestroyTarget (GameObject player, GameObject target) {

		// espere mientras la condicion sea verdadera
		while(!reached_target && !action_interrupted) {
			yield return null;
		}

		if (reached_target) {
			GenerateTerrain.TerrainGenerator.destroySceneryObject (target);
			destroyed_target = true;
		}

	}

	bool checkActors() {
		return true;
	}

	bool checkInventory() {
		return true;
	}

}
