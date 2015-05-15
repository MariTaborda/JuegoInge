using UnityEngine;
using System.Collections;

public class Hammer : BackpackItem {
	
	public Hammer() {
		image = Resources.Load("BackpackItems/martillo", typeof(Sprite)) as Sprite;
		name = "Martillo";
		stackable = false;
	}
	
}

