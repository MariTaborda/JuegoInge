﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerItems : MonoBehaviour {

	[HideInInspector]
	public List<BackpackItem> items;

	[HideInInspector]
	public List<int> quantities;

	[HideInInspector]
	public int Count;

	void Awake () {
		items = new List<BackpackItem>();
		quantities = new List<int> ();
	}

	public void reset() {
		items = new List<BackpackItem>();
		quantities = new List<int> ();
		Count = 0;
	}

	public void addItem(BackpackItem item, int amount){
		bool inBackpack = false;

		if (item.stackable) {

			for (int i = 0; i < items.Count; ++i) {
				if (items [i].name == item.name) {
					inBackpack = true;
					quantities [i] += amount;
					break;
				}
			}

			if (!inBackpack) {
				items.Add (item);
				quantities.Add (amount);
			}

		} else {

			for (int i = 0; i < amount; ++i) {
				items.Add (item);
				quantities.Add (1);
			}

		}
	}

	// Devuelve falso si no fue posible reducir la cantidad de item porque no esta en la mochila o no hay suficiente
	public bool reduceItem(BackpackItem item, int amount) {
		bool canReduce = false;
		
		for (int i = 0; i < items.Count; ++i) {
			if (items[i].name == item.name) {
				if (amount < quantities[i]) {
					// Hay una mayor cantidad del item de la que se quiere reducir
					canReduce = true;
					quantities[i] -= amount;
				} else if (amount == quantities[i]) {
					// Se reduce la cantidad total del item, item se borra del inventario
					canReduce = true;
					items.RemoveAt(i);
					quantities.RemoveAt(i);
				}
				break;
			}
		}

		return canReduce;
	}

	public bool itemIsInInventory(string item_name) {
		for (int i = 0; i < items.Count; ++i) {
			if (items[i].name == item_name) {
				return true;
				break;
			}
		}
		return false;
	}

	public bool itemInBackpack(BackpackItem item) {
		bool result = false;
		
		for (int i = 0; i < items.Count; ++i) {
			if (items[i].name == item.name) {
				result = true;
				break;
			}
		}
		
		return result;
	}

	public int count() {
		return items.Count;
	}

	public BackpackItem getItemByPosition (int position) {
		return items [position];
	}

	public int getAmountByPosition (int position) {
		return quantities [position];
	}

	public int getItemAmount(BackpackItem item){
		int result = 0;
		
		for (int i = 0; i < items.Count; ++i) {
			if (items[i].name == item.name) {
				result = quantities [i];
				break;
			}
		}

		return result;
	}

}
