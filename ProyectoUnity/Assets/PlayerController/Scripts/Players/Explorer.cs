using UnityEngine;
using System.Collections;

public class Explorer : Player {

	public override void init() {
		base.init ();
		GetComponent<PlayerIdentifier> ().identifier = "Explorer";
	}

}
