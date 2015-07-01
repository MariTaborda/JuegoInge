using UnityEngine;
using System.Collections;

public class Rock : BackpackItem {
	
	public Rock() {
		image = Resources.Load("BackpackItems/piedra", typeof(Sprite)) as Sprite;
		name = "Piedra";
		stackable = true;
	}
	
}

