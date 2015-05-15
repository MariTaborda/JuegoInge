using UnityEngine;
using System.Collections;

public class Machete : BackpackItem {
	
	public Machete() {
		image = Resources.Load("BackpackItems/machete", typeof(Sprite)) as Sprite;
		name = "Machete";
		stackable = false;
	}
	
}
