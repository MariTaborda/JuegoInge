using UnityEngine;
using System.Collections;

public class Sapling : BackpackItem {
	
	public Sapling() {
		image = Resources.Load("BackpackItems/planton", typeof(Sprite)) as Sprite;
		name = "Planton";
		stackable = true;
	}
	
}

