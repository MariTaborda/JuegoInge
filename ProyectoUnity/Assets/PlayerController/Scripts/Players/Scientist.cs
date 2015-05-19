using UnityEngine;
using System.Collections;

public class Scientist : Player {

	public override void init() {
		base.init ();
		GetComponent<PlayerIdentifier> ().identifier = "Scientist";
	}

}
