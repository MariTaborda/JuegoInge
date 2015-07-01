using UnityEngine;
using System.Collections;

public class ActionChopTree : PlayerAction {
	
	PlayerPoints points = PlayerController.Player_Controller.GetComponent<PlayerPoints>();
	
	public void Start() {
		action_name = "Cortar Arbol";
		target_tag = "SceneryTree";
		inv_item = "Hacha";
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
			
			// Aqui se llama a la animacion del arbol.
			tk2dSpriteAnimator anim = target.GetComponent<tk2dSpriteAnimator>();
			tk2dSprite sprite = target.GetComponent<tk2dSprite>();
			
			switch(sprite.spriteId) {
			case 1:
				anim.Play("arbol");
				break;
			case 3:
				anim.Play("palmera");
				break;
			default:
				GenerateTerrain.TerrainGenerator.destroySceneryObject (target);
				destroyed_target = true;
				break;
			}
			
			anim.AnimationCompleted = AnimationChopTreeCompletedDelegate;
			
			points.reduceEcologyPoints(5);
			
			if (MissionChopTree.singleton != null && MissionChopTree.singleton.started) {
				MissionChopTree.singleton.onDestroyTree();
			}
			
		}
		
	}
	
	void AnimationChopTreeCompletedDelegate(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip) {
		GenerateTerrain.TerrainGenerator.destroySceneryObject (target);
		destroyed_target = true;
	}
	
}
