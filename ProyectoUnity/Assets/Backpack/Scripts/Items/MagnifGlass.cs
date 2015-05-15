using UnityEngine;
using System.Collections;

public class MagnifGlass : BackpackItem {
	
	public MagnifGlass() {
		image = Resources.Load("BackpackItems/lupa", typeof(Sprite)) as Sprite;
		name = "Lupa";
		stackable = false;
	}
	
}
