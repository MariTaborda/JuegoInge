using UnityEngine;
using System.Collections;

public class Builder : Player {

	public override void init() {
		base.init ();
		GetComponent<PlayerIdentifier> ().identifier = "Builder";
	}

}
