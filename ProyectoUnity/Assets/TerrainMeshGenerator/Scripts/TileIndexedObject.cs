using UnityEngine;
using System.Collections;

public class TileIndexedObject : MonoBehaviour {

	public int chunk_index_x;
	public int chunk_index_y;
	public int tile_index_x;
	public int tile_index_y;
	public string type = "unassigned";
	
	public void init(int chunk_ind_x, int chunk_ind_y, int tile_ind_x, int tile_ind_y) {
		chunk_index_x = chunk_ind_x;
		chunk_index_y = chunk_ind_y;
		tile_index_x = tile_ind_x;
		tile_index_y = tile_ind_y;
	}

	public void initType(string type) {
		this.type = type;
	}

}
