using UnityEngine;
using System.Collections;

public class PlayerAction : MonoBehaviour {

	string name;
	string target_tag;
	string inv_item;

	public virtual string getName() {
		return name;
	}

	public virtual string getTargetTag() {
		return target_tag;
	}

	public virtual string getInvItem() {
		return inv_item;
	}

	public virtual void performAction(GameObject player, GameObject target) {

	}

	public bool checkInventory(string item_name) {
		bool result;
		result = GameController.gameController.playerController.currentCharacter.GetComponent<PlayerItems> ().itemIsInInventory (item_name);
		return result;
	}

}
