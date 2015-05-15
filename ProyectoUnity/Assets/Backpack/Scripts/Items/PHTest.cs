using UnityEngine;
using System.Collections;

public class PHTest : BackpackItem {
	
	public PHTest() {
		image = Resources.Load("BackpackItems/papelIndicador", typeof(Sprite)) as Sprite;
		name = "Papel Indicador";
		stackable = true;
	}
	
}
