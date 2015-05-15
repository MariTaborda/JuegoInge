using UnityEngine;
using System.Collections;

public class Pickaxe : BackpackItem {

	public Pickaxe() {
		image = Resources.Load("BackpackItems/pico", typeof(Sprite)) as Sprite;
		name = "Pico";
		stackable = false;
	}

}
