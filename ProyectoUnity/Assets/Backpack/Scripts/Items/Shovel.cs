using UnityEngine;
using System.Collections;

public class Shovel : BackpackItem {
	
	public Shovel() {
		image = Resources.Load("BackpackItems/pala", typeof(Sprite)) as Sprite;
		name = "Pala";
		stackable = false;
	}
	
}

