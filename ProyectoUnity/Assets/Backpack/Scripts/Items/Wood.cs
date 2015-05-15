using UnityEngine;
using System.Collections;

public class Wood : BackpackItem {
	
	public Wood() {
		image = Resources.Load("BackpackItems/madera", typeof(Sprite)) as Sprite;
		name = "Madera";
		stackable = true;
	}
	
}

