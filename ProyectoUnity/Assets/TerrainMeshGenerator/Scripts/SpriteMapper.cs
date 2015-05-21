using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpriteMapper {

	tk2dSprite scenery_sprite;

	public List<int> trees;
	public List<int> bushes;
	public List<int> rocks;
	public List<int> structures;

	// Sprite names:

	// trees
	public string big_tree_1 = "tree3";
	public string medium_tree_1 = "tree4";
	public string medium_tree_2 = "tree5";
	public string tropical_tree_1 = "tree1_2";
	public string tropical_tree_2 = "tree2_2";

	// bushes
	public string bush_1 = "bush1";
	public string bush_2 = "bush2";
	public string bush_3 = "bush-a";
	public string bush_4 = "bush-c";
	public string bush_5 = "bush-z";

	// rocks
	public string rock_1 = "piedra1";
	public string rock_2 = "piedra2";

	// structures
	public string casa_1 = "casa1";
	public string granjita_1 = "granjita";
	public string zona_rural_1 = "zonarural";
	public string zona_rural_2 = "zonarural2";
	public string zonaruralsingranjas = "zonaruralsingranjas";
	public string zonaciudad = "zonaciudad";
	public string zonaciudad2 = "zonaciudad2";
	public string zonaciudad3 = "zonaciudad3";
	public string alcatraz = "piedra1";
	public string bigHouse1 = "bigHouse1";
	public string fuente = "fuente";
	public string mercado = "mercado";
	public string molino = "molino";
	public string police = "police";
	public string residencial = "residencial";
	public string stores = "stores";
	public string templo = "templo";
	public string treePink = "treePink"; 
	public string vereda = "vereda";

	void initScenery() {

		trees = new List<int> ();
		bushes = new List<int> ();
		rocks = new List<int> ();
		structures = new List<int> ();

		// add trees
		//trees.Add (getSpriteIdByName(big_tree_1));
		//trees.Add (getSpriteIdByName(medium_tree_1));
		trees.Add (getSpriteIdByName(medium_tree_2));
		trees.Add (getSpriteIdByName(tropical_tree_1));
		trees.Add (getSpriteIdByName(tropical_tree_2));

		// add bushes
		bushes.Add (getSpriteIdByName(bush_1));
		bushes.Add (getSpriteIdByName(bush_2));
		bushes.Add (getSpriteIdByName(bush_3));
		bushes.Add (getSpriteIdByName(bush_4));
		bushes.Add (getSpriteIdByName(bush_5));

		// add rocks
		rocks.Add (getSpriteIdByName(rock_1));
		rocks.Add (getSpriteIdByName(rock_2));

		// add structures
		// (pending)

	}

	bool isContained(int needle, List<int> haystack) {
		for (int i = 0; i < haystack.Count; ++i) {
			if(needle == haystack[i]) {
				return true;
			}
		}
		return false;
	}

	public void init(tk2dSprite scenery_spr) {
		this.scenery_sprite = scenery_spr;
		initScenery ();
	}

	public int getRandomTreeId() {
		int index = (int) UnityEngine.Random.Range (0, trees.Count);
		return trees [index];
	}

	public int getRandomBushId() {
		int index = (int) UnityEngine.Random.Range (0, bushes.Count - 1);
		/*if ((int)UnityEngine.Random.Range (0, 100) < 99) {
			return getSpriteIdByName(bush_5);
		}*/
		return bushes [index];
	}

	public int getRandomRockId() {
		int index = (int) UnityEngine.Random.Range (0, rocks.Count);
		return rocks [index];
	}

	public int getRandomVegetation() {
		int type = UnityEngine.Random.Range (0, 100);
		if (type < 20) {			// tree
			return getRandomTreeId();
		} else if (type < 96) {		// bush
			return getRandomBushId();
		} else {					// rock
			return getRandomRockId();
		} 

	}

	public int getEmpty() {
		return -1;
	}

	public string getTagFromSpriteId(int sprite_id) {
		string tag = "Untagged";
		int type = getTypeFromSpriteId(sprite_id);
		
		if(type == 0) {
			tag = "SceneryTree";
		} else if(type == 1) {
			tag = "SceneryBush";
		}  else if(type == 2) {
			tag = "SceneryRock";
		}  
		
		return tag;
	}

	public int getTypeFromSpriteId(int sprite_id) {
		if (isContained (sprite_id, trees)) {
			return 0;	// tree
		}
		if (isContained (sprite_id, bushes)) {
			return 1;	// bush
		}
		if (isContained (sprite_id, rocks)) {
			return 2;	// rock
		}
		return -1;
	}

	public int getSpriteIdByName(string spr_name) {
		return scenery_sprite.GetSpriteIdByName (spr_name);
	}

}
