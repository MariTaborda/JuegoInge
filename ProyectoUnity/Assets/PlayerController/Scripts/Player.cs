using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	
	protected List<PlayerAction> actions;

	public virtual void init() {
		actions = new List<PlayerAction> ();
	}

	public virtual List<PlayerAction> getActions() {
		return actions;
	}

	public virtual void addAction(PlayerAction action) {
		actions.Add (action);
	}

}
