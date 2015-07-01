using UnityEngine;
using System.Collections;

public class ActionMineRock : PlayerAction {
	
	public void Start() {
		action_name = "Picar piedra";
		target_tag = "SceneryRock";
		inv_item = "Pico";
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
			StartCoroutine( FinishAction() );
			
		}
		else {
			Debug.Log("Player is missing item: "+inv_item);
		}
		
	}

	IEnumerator FinishAction () {
		while(!destroyed_target && !action_interrupted) {
			yield return null;
		}
		
		if (destroyed_target) {
			PlayerController pc = PlayerController.Player_Controller.GetComponent<PlayerController>();
			PlayerItems items = pc.getCurrentPlayerItems ();
			items.addItem (new Rock (), 3);
			pc.UpdateBackpackUI();
			performAction(player, null);		// repite la accion para crear varios caminos seguidamente
		}
		
	}
	
}

