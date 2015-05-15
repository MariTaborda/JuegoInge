using UnityEngine;
using System.Collections;

public class Axe : BackpackItem {
	
	public Axe() {
		image = Resources.Load("BackpackItems/hacha", typeof(Sprite)) as Sprite;
		name = "Hacha";
		stackable = false;
	}

}
