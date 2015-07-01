using UnityEngine;
using System.Collections;

public abstract class PlayerAction : MonoBehaviour {

	protected GameObject player;
	protected GameObject target;

	protected string action_name;
	protected string target_tag;
	protected string inv_item;

	protected float near_distance = 0.8f;
	protected bool reached_target;
	protected bool destroyed_target;
	protected bool action_interrupted;

	public virtual string getActionName() {
		return action_name;
	}

	public virtual string getTargetTag() {
		return target_tag;
	}

	public virtual string getInvItem() {
		return inv_item;
	}

	public abstract void performAction(GameObject player, GameObject target);

	public virtual IEnumerator ApproachPosition (GameObject player, Vector3 target_position) {
		
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

	public virtual IEnumerator DestroyTarget (GameObject player, GameObject target) {
		
		// espere mientras la condicion sea verdadera
		while(!reached_target && !action_interrupted) {
			yield return null;
		}
		
		if (reached_target) {
			GenerateTerrain.TerrainGenerator.destroySceneryObject (target);
			destroyed_target = true;
		}
		
	}

	public bool checkInventory(string item_name) {
		return GameController.gameController.playerController.currentCharacter().GetComponent<PlayerItems>().itemIsInInventory (item_name);
	}

}
