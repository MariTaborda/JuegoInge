using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	
	List<PlayerAction> actions;

	public virtual void init() {
		actions = new List<PlayerAction> ();
	}

	public List<PlayerAction> getActions() {
		return actions;
	}

	public void addAction(PlayerAction action) {
		actions.Add (action);
	}

}
