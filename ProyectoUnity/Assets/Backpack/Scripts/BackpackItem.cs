using UnityEngine;
using System.Collections;

public class BackpackItem {

	public Sprite image = Resources.Load("BackpackItems/_empty", typeof(Sprite)) as Sprite;
	public string name = "null";
	public bool stackable = true;
	
}
