using UnityEngine;
using System.Collections;

public class SceneryObject : MonoBehaviour {

	public int chunk_index_x;
	public int chunk_index_y;
	public int scenery_index_x;
	public int scenery_index_y;

	public void init(int chunk_ind_x, int chunk_ind_y, int scenery_ind_x, int scenery_ind_y) {
		chunk_index_x = chunk_ind_x;
		chunk_index_y = chunk_ind_y;
		scenery_index_x = scenery_ind_x;
		scenery_index_y = scenery_ind_y;
	}

}
