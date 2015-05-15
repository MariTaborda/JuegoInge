using UnityEngine;
using System.Collections;

public class Nails : BackpackItem {
	
	public Nails() {
		image = Resources.Load("BackpackItems/clavos", typeof(Sprite)) as Sprite;
		name = "Clavos";
		stackable = true;
	}
	
}

