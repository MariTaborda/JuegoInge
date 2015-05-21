using UnityEngine;
using System.Collections;

public class ActionChopTree : PlayerAction {

	GameObject player;
	GameObject target;
	bool error;
	float near_distance = 0.8f;

	bool reached_target;
	bool destroyed_target;
	bool action_interrupted;

	public string name = "Chop Tree";
	public string target_tag = "SceneryTree";
	public string inv_item = "Hacha";

	public void Start() {
	}

	public override string getName() {
		return name;
	}

	public override string getTargetTag() {
		return target_tag;
	}

	public override string getInvItem() {
		return inv_item;
	}

	public override void performAction(GameObject player, GameObject target) {
	
		this.player = player;
		this.target = target;
		error = false;

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
			}

			anim.AnimationCompleted = AnimationChopTreeCompletedDelegate;

		}

	}

	void AnimationChopTreeCompletedDelegate(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip) {
		GenerateTerrain.TerrainGenerator.destroySceneryObject (target);
		destroyed_target = true;
	}


}
