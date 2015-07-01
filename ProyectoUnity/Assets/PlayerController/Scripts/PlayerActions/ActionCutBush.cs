using UnityEngine;
using System.Collections;

public class ActionCutBush : PlayerAction {
	
	PlayerPoints points =PlayerController.Player_Controller.GetComponent<PlayerPoints>();

	public void Start() {
		action_name = "Cortar Arbusto";
		target_tag = "SceneryBush";
		inv_item = "Machete";
	}

	public override void performAction(GameObject player, GameObject target) {
		
		this.player = player;
		this.target = target;

		if(base.checkInventory (inv_item)) {
			
			reached_target = false;
			destroyed_target = false;
			action_interrupted = false;
			
			StartCoroutine( ApproachPosition(player, target.transform.position) );
			StartCoroutine( DestroyTarget(player, target) );
			
		}
		else {
			Debug.Log("Player is missing item: "+inv_item);
		}
		
	}
	
	public override IEnumerator DestroyTarget (GameObject player, GameObject target) {
		
		// espere mientras la condicion sea verdadera
		while(!reached_target && !action_interrupted) {
			yield return null;
		}
		
		if (reached_target) {
			GenerateTerrain.TerrainGenerator.destroySceneryObject (target);
			destroyed_target = true;
			points.reduceEcologyPoints(5);
		}
		
	}
	
}

