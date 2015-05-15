using UnityEngine;
using System.Collections;

public class Container : BackpackItem {
	
	public Container() {
		image = Resources.Load("BackpackItems/contenedor9", typeof(Sprite)) as Sprite;
		name = "Contenedor";
		stackable = true;
	}
	
}
