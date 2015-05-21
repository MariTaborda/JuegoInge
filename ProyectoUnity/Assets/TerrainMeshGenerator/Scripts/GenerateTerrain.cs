using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GenerateTerrain : MonoBehaviour {

	public float tile_size = 0.64f;
	public float slope_factor = 0.5f;

	public int side_tile_count = 24;

	//public string heightmap_path = "/Resources/Heightmaps/mesa_heightmap_lowcontrast.jpg";
	//public string heightmap_path = "/Resources/Heightmaps/mesa_heightmap_highcontrast.jpg";
	//public string heightmap_path = "/Resources/Heightmaps/mesa_heightmap_heavyblur.jpg";
	//public string heightmap_path = "/Resources/Heightmaps/mesa_heightmap_blur.jpg";
	//public string heightmap_path = "/Resources/Heightmaps/mesa_heightmap.jpg";
	public string heightmap_path = "/Resources/Heightmaps/mesa_heightmap_big.jpg";
	public int textures_x = 3;
	public int textures_y = 3;
	public int texture_size_x = 384;
	public int texture_size_y = 384;

	public int level_count = 16;
	public float level_zero_grayscale = 0.24f; 
	

	private LevelMap[,] WorldLevelMaps;

	private float slope_height;

	private Vector3 origin;

	private float chunk_size_x;
	private float chunk_size_y;

	private int terrain_chunks_x;
	private int terrain_chunks_y;
	private TChunk[,] TerrainChunks;

	private Stack<GameObject> Scenery_Obj_Pool;
	private int scenery_stack_size = 8000;

	private Stack<GameObject> Path_Obj_Pool;
	private int path_stack_size = 1000;

	private List<TChunk> loaded_chunks2;

	private TC_HashTable loaded_chunks;


	public GameObject TerrainChunk_Prefab;
	public GameObject WaterSurfaceChunk_Prefab;
	public GameObject SceneryObject_Prefab;
	public GameObject PathObject_Prefab;

	public GameObject SceneryHolder;

	public Camera camera;

	private Vector3 last_cam_position;

	public static GenerateTerrain TerrainGenerator;
	public static GameObject SceneryObject_Ref;

	void Update() {
	
		if (last_cam_position != camera.transform.position) {

			last_cam_position = camera.transform.position;
		
			UpdateLoadedChunks();
		}

	}

	// object initialization
	public void init() {

		TerrainGenerator = gameObject.GetComponent<GenerateTerrain> ();
		SceneryObject_Ref = SceneryObject_Prefab;
		
		slope_height = tile_size * slope_factor;
		chunk_size_x = side_tile_count * tile_size;
		chunk_size_y = side_tile_count * tile_size;
		origin = gameObject.transform.position;
		
		
		MapLoader mloader = new MapLoader ();
		mloader.openFile (texture_size_x, texture_size_y, heightmap_path);
		List<int[,]> lmaps = mloader.getLevelMapList (level_count, level_zero_grayscale, side_tile_count, out terrain_chunks_x, out terrain_chunks_y);
		
		WorldLevelMaps = setWorldLevelMaps (ref lmaps, terrain_chunks_x, terrain_chunks_y);
		
		initSceneryObjPool ();
		initPathObjPool ();
		loaded_chunks = new TC_HashTable(terrain_chunks_x * terrain_chunks_y, terrain_chunks_x, terrain_chunks_x, terrain_chunks_y);
		
		last_cam_position = Vector3.zero;
		
		
		TerrainChunks = new TChunk[terrain_chunks_y, terrain_chunks_x];
		
		for (int i = 0; i < terrain_chunks_y; ++i) {
			
			for (int j = 0; j < terrain_chunks_x; ++j) {
				
				int index_x = j;
				int index_y = i;
				
				List<int[]> neighbor_level_maps = getNeighborLevelMaps(index_x, index_y, ref WorldLevelMaps);
				
				Vector3 chunk_origin = origin;
				chunk_origin.x += chunk_size_x * index_x;
				chunk_origin.z -= chunk_size_y * index_y;
				Vector3 chunk_center = new Vector3 (chunk_origin.x + chunk_size_x * 0.5f, 0, chunk_origin.z - chunk_size_y * 0.5f);
				
				GameObject new_terrain_chunk = (GameObject) Instantiate(TerrainChunk_Prefab);
				new_terrain_chunk.transform.parent = transform;
				GenerateTerrainChunk generator = new_terrain_chunk.GetComponent<GenerateTerrainChunk> ();
				generator.generate (index_x, index_y, tile_size, slope_height, side_tile_count, chunk_origin, chunk_center, ref WorldLevelMaps, terrain_chunks_x, terrain_chunks_y, textures_x, textures_y, ref neighbor_level_maps, new_terrain_chunk.GetComponent<MeshFilter> ());
				
				GameObject new_ws_chunk = (GameObject) Instantiate(WaterSurfaceChunk_Prefab);
				new_ws_chunk.transform.parent = new_terrain_chunk.transform;
				new_ws_chunk.transform.position = new_terrain_chunk.transform.position;
				generator.generateWaterSurface (new_ws_chunk);
				
				
				TChunk terrain_chunk = new TChunk(index_x, index_y, chunk_origin, chunk_center, ref new_terrain_chunk, ref new_ws_chunk, ref generator);
				TerrainChunks[index_y, index_x] = terrain_chunk;
				
			}
			
		}

	}

	public void UpdateChunk(int chunkIndexX, int chunkIndexY) {
		TChunk elem = loaded_chunks.getElem(chunkIndexX, chunkIndexY);
		elem.generator.unloadScenery(ref Scenery_Obj_Pool);
		elem.generator.unloadPaths(ref Path_Obj_Pool);
		loaded_chunks.removeElem(ref elem);

		UpdateLoadedChunks ();
	}

	public void UpdateLoadedChunks() {
		TChunk tchunk = getChunkFromPosition (last_cam_position);
		List<TChunk> tchunks = getChunksAround(ref tchunk);
		updateLoadedSceneryChunks(ref tchunks);
	}

	public TChunk getChunkTileFromPosition(Vector3 position, out int tile_index_x, out int tile_index_z) {
		
		TChunk chunk = getChunkFromPosition(position);
		chunk.generator.getTileIndexesFromPosition (new Vector3(position.x - chunk.generator.origin.x, 0, position.z - chunk.generator.origin.z), out tile_index_x, out tile_index_z);
		
		return chunk;
		
	}

	public void destroySceneryObject(GameObject obj) {
		
		SceneryObject sc_obj = obj.GetComponent<SceneryObject> ();
		TChunk chunk = loaded_chunks.getElem (sc_obj.chunk_index_x, sc_obj.chunk_index_y);
		chunk.generator.destroySceneryObject (obj, ref Scenery_Obj_Pool);
		
	}

	public int[,] getChunkSceneryMap(int chunk_index_x, int chunk_index_y) {

		TChunk chunk = loaded_chunks.getElem (chunk_index_x, chunk_index_y);
		return chunk.generator.getSceneryMap ();

	}

	public PathType[,] getChunkPathMap(int chunk_index_x, int chunk_index_y) {
		
		TChunk chunk = loaded_chunks.getElem (chunk_index_x, chunk_index_y);
		return chunk.generator.getPathMap ();
		
	}

	// load scenery and path objects in visible_chunks. This is done every frame.
	private void updateLoadedSceneryChunks(ref List<TChunk> visible_chunks) {

		List<Vector2> saved_indexes = loaded_chunks.getSavedIndexes ();

		for (int i = 0; i < saved_indexes.Count; ++i) {

			bool isVisible = false;
			for(int j = 0; j < visible_chunks.Count; ++j) {
				if(visible_chunks[j].index_x == saved_indexes[i].x && visible_chunks[j].index_y == saved_indexes[i].y) {
					isVisible = true;
					break;
				}
			}

			if(!isVisible) {
				// remove chunk from table
				TChunk elem = loaded_chunks.getElem((int)saved_indexes[i].x, (int)saved_indexes[i].y);
				elem.generator.unloadScenery(ref Scenery_Obj_Pool);
				elem.generator.unloadPaths(ref Path_Obj_Pool);
				loaded_chunks.removeElem(ref elem);
			}

		}

		for (int i = 0; i < visible_chunks.Count; ++i) {

			if(!loaded_chunks.elemIsValid(loaded_chunks.getElem(visible_chunks[i].index_x, visible_chunks[i].index_y))) {
				// add chunk to table
				TChunk elem = visible_chunks[i];
				elem.generator.loadScenery(ref Scenery_Obj_Pool);
				elem.generator.loadPaths(ref Path_Obj_Pool);
				loaded_chunks.addElem(ref elem);

			} 

		}

	}

	private bool Vector2isInList (Vector2 v2, ref List<Vector2> list) {

		for (int i = 0; i < list.Count; ++i) {
			if(list[i].x == v2.x && list[i].y == v2.y) {
				return true;
			}
		}

		return false;

	}

	public TChunk getChunkFromPosition (Vector3 position) {

		float chunk_size = tile_size * side_tile_count;

		int index_x = (int)(position.x / chunk_size);
		int index_y = (int)Mathf.Abs(position.z / chunk_size);

		if(index_x >= 0 && index_x < terrain_chunks_x && index_y < terrain_chunks_y) {
			return TerrainChunks[index_y, index_x];
		}

		TChunk invalid = new TChunk ();
		return invalid;

	}

	private List<TChunk> getChunksAround(ref TChunk chunk) {
	
		List<TChunk> chunks = new List<TChunk> ();

		addChunkToListIfExists (chunk.index_x-3, chunk.index_y, ref chunks);
		addChunkToListIfExists (chunk.index_x-2, chunk.index_y, ref chunks);
		addChunkToListIfExists (chunk.index_x-1, chunk.index_y, ref chunks);
		addChunkToListIfExists (chunk.index_x, chunk.index_y, ref chunks);
		addChunkToListIfExists (chunk.index_x+1, chunk.index_y, ref chunks);

		addChunkToListIfExists (chunk.index_x-3, chunk.index_y-1, ref chunks);
		addChunkToListIfExists (chunk.index_x-2, chunk.index_y-1, ref chunks);
		addChunkToListIfExists (chunk.index_x-1, chunk.index_y-1, ref chunks);
		addChunkToListIfExists (chunk.index_x, chunk.index_y-1, ref chunks);
		addChunkToListIfExists (chunk.index_x+1, chunk.index_y-1, ref chunks);

		addChunkToListIfExists (chunk.index_x-3, chunk.index_y+1, ref chunks);
		addChunkToListIfExists (chunk.index_x-2, chunk.index_y+1, ref chunks);
		addChunkToListIfExists (chunk.index_x-1, chunk.index_y+1, ref chunks);
		addChunkToListIfExists (chunk.index_x, chunk.index_y+1, ref chunks);
		addChunkToListIfExists (chunk.index_x+1, chunk.index_y+1, ref chunks);

		addChunkToListIfExists (chunk.index_x-3, chunk.index_y+2, ref chunks);
		addChunkToListIfExists (chunk.index_x-2, chunk.index_y+2, ref chunks);
		addChunkToListIfExists (chunk.index_x-1, chunk.index_y+2, ref chunks);
		addChunkToListIfExists (chunk.index_x, chunk.index_y+2, ref chunks);
		addChunkToListIfExists (chunk.index_x+1, chunk.index_y+2, ref chunks);

		addChunkToListIfExists (chunk.index_x-3, chunk.index_y+3, ref chunks);
		addChunkToListIfExists (chunk.index_x-2, chunk.index_y+3, ref chunks);
		addChunkToListIfExists (chunk.index_x-1, chunk.index_y+3, ref chunks);
		addChunkToListIfExists (chunk.index_x, chunk.index_y+3, ref chunks);
		addChunkToListIfExists (chunk.index_x+1, chunk.index_y+3, ref chunks);

		return chunks;

	}

	private void addChunkToListIfExists(int chunk_index_x, int chunk_index_y, ref List<TChunk> list) {
	
		if (chunk_index_x >= 0 && chunk_index_x < terrain_chunks_x) {
			if(chunk_index_y >= 0 && chunk_index_y < terrain_chunks_y) {
				list.Add(TerrainChunks[chunk_index_y, chunk_index_x]);
			}
		}

	}

	private void initSceneryObjPool() {

		Scenery_Obj_Pool = new Stack<GameObject> ();

		for (int i = 0; i < scenery_stack_size; ++i) {

			GameObject new_scenery_obj = (GameObject) Instantiate(SceneryObject_Prefab);
			new_scenery_obj.transform.parent = SceneryHolder.transform;
			new_scenery_obj.SetActive(false);
			new_scenery_obj.GetComponent<CameraFace> ().m_Camera = camera;

			Scenery_Obj_Pool.Push(new_scenery_obj);

		}

	}

	private void initPathObjPool() {
		
		Path_Obj_Pool = new Stack<GameObject> ();
		
		for (int i = 0; i < path_stack_size; ++i) {
			
			GameObject new_path_obj = (GameObject) Instantiate(PathObject_Prefab);
			new_path_obj.transform.parent = SceneryHolder.transform;
			new_path_obj.SetActive(false);
			
			Path_Obj_Pool.Push(new_path_obj);
			
		}
		
	}

	public bool tileIsSuitableForScenery(int chunk_index_x, int chunk_index_y, int tile_index_x, int tile_index_y) {
		
		TChunk chunk = loaded_chunks.getElem (chunk_index_x, chunk_index_y);
		return chunk.generator.tileIsSuitableForScenery (tile_index_x, tile_index_y);
		
	}

	private LevelMap[,] setWorldLevelMaps(ref List<int[,]> lmaps, int t_chunks_x, int t_chunks_y) {
		
		int counter = 0;
		LevelMap[,] WorldLevelMaps = new LevelMap[t_chunks_y, t_chunks_x];
		for (int i = 0; i < t_chunks_y; ++i) {
			for(int j = 0; j < t_chunks_x; ++j) {
				int[,] lmap = lmaps[counter];
				LevelMap levmap = new LevelMap();
				levmap.setLevelMap(j, i, ref lmap);
				WorldLevelMaps[i, j] = levmap;
				++counter;
			}
		}
		
		return WorldLevelMaps;
		
	}
	
	private List<int[]> getNeighborLevelMaps(int target_ind_x, int target_ind_y, ref LevelMap[,] WorldLevelMaps) {
		
		List<int[]> neighbors = new List<int[]> (8);
		for (int i = 0; i < 8; ++i) {
			neighbors.Add(new int[side_tile_count]);
		}
		
		LevelMap originalLevelMap = WorldLevelMaps [target_ind_y, target_ind_x];
		LevelMap lmap;
		LevelMapOrientation current;
		LevelMapOrientation oposite;
		
		current = LevelMapOrientation.N;
		oposite = LevelMapOrientation.S;
		lmap = getNeighborLevelMap (originalLevelMap, current, ref WorldLevelMaps);
		neighbors[(int)current] = getLevelMapBorder(ref lmap, oposite);
		
		current = LevelMapOrientation.NE;
		oposite = LevelMapOrientation.SW;
		lmap = getNeighborLevelMap (originalLevelMap, current, ref WorldLevelMaps);
		neighbors[(int)current] = getLevelMapBorder(ref lmap, oposite);
		
		current = LevelMapOrientation.E;
		oposite = LevelMapOrientation.W;
		lmap = getNeighborLevelMap (originalLevelMap, current, ref WorldLevelMaps);
		neighbors[(int)current] = getLevelMapBorder(ref lmap, oposite);
		
		current = LevelMapOrientation.SE;
		oposite = LevelMapOrientation.NW;
		lmap = getNeighborLevelMap (originalLevelMap, current, ref WorldLevelMaps);
		neighbors[(int)current] = getLevelMapBorder(ref lmap, oposite);
		
		current = LevelMapOrientation.S;
		oposite = LevelMapOrientation.N;
		lmap = getNeighborLevelMap (originalLevelMap, current, ref WorldLevelMaps);
		neighbors[(int)current] = getLevelMapBorder(ref lmap, oposite);
		
		current = LevelMapOrientation.SW;
		oposite = LevelMapOrientation.NE;
		lmap = getNeighborLevelMap (originalLevelMap, current, ref WorldLevelMaps);
		neighbors[(int)current] = getLevelMapBorder(ref lmap, oposite);
		
		current = LevelMapOrientation.W;
		oposite = LevelMapOrientation.E;
		lmap = getNeighborLevelMap (originalLevelMap, current, ref WorldLevelMaps);
		neighbors[(int)current] = getLevelMapBorder(ref lmap, oposite);
		
		current = LevelMapOrientation.NW;
		oposite = LevelMapOrientation.SE;
		lmap = getNeighborLevelMap (originalLevelMap, current, ref WorldLevelMaps);
		neighbors[(int)current] = getLevelMapBorder(ref lmap, oposite);
		
		return neighbors;
		
	}
	
	private LevelMap getNeighborLevelMap(LevelMap original, LevelMapOrientation orientation, ref LevelMap[,] WorldLevelMaps) {
		
		LevelMap result = new LevelMap();
		result.invalidate ();
		
		int n_ind_x = original.index_x;
		int n_ind_y = original.index_y;
		
		switch (orientation) {
			
		case LevelMapOrientation.N:
			if(original.index_y == 0) {
				return result;
			}
			else {
				n_ind_y -= 1;
			}
			break;
			
		case LevelMapOrientation.NE:
			if(original.index_y == 0 || original.index_x == terrain_chunks_x-1) {
				return result;
			}
			else {
				n_ind_y -= 1;
				n_ind_x += 1;
			}
			break;
			
		case LevelMapOrientation.E:
			if(original.index_x == terrain_chunks_x-1) {
				return result;
			}
			else {
				n_ind_x += 1;
			}
			break;
			
		case LevelMapOrientation.SE:
			if(original.index_y == terrain_chunks_y-1 || original.index_x == terrain_chunks_x-1) {
				return result;
			}
			else {
				n_ind_y += 1;
				n_ind_x += 1;
			}
			break;
			
		case LevelMapOrientation.S:
			if(original.index_y == terrain_chunks_y-1) {
				return result;
			}
			else {
				n_ind_y += 1;
			}
			break;
			
		case LevelMapOrientation.SW:
			if(original.index_y == terrain_chunks_y-1 || original.index_x == 0) {
				return result;
			}
			else {
				n_ind_y += 1;
				n_ind_x -= 1;
			}
			break;
			
		case LevelMapOrientation.W:
			if(original.index_x == 0) {
				return result;
			}
			else {
				n_ind_x -= 1;
			}
			break;
			
		case LevelMapOrientation.NW:
			if(original.index_y == 0 || original.index_x == 0) {
				return result;
			}
			else {
				n_ind_y -= 1;
				n_ind_x -= 1;
			}
			break;
			
		}
		
		result = WorldLevelMaps [n_ind_y, n_ind_x];
		
		return result;
		
	}
	
	private int[] getLevelMapBorder(ref LevelMap lmap, LevelMapOrientation orientation) {
		
		int[] result = new int[side_tile_count];
		
		if(lmap.index_x < 0) {
			// no neighbor
			for(int i = 0; i < side_tile_count; ++i) {
				result[i] = 0;
			}
			return result;
		}
		
		switch (orientation) {
			
		case LevelMapOrientation.N:
			for(int i = 0; i < side_tile_count; ++i) {
				result[i] = lmap.levels[0, i];
			}
			break;
			
		case LevelMapOrientation.NE:
			result[0] = lmap.levels[0, side_tile_count-1];
			break;
			
		case LevelMapOrientation.E:
			for(int i = 0; i < side_tile_count; ++i) {
				result[i] = lmap.levels[i, side_tile_count-1];
			}
			break;
			
		case LevelMapOrientation.SE:
			result[0] = lmap.levels[side_tile_count-1, side_tile_count-1];
			break;
			
		case LevelMapOrientation.S:
			for(int i = 0; i < side_tile_count; ++i) {
				result[i] = lmap.levels[side_tile_count-1, i];
			}
			break;
			
		case LevelMapOrientation.SW:
			result[0] = lmap.levels[side_tile_count-1, 0];
			break;
			
		case LevelMapOrientation.W:
			for(int i = 0; i < side_tile_count; ++i) {
				result[i] = lmap.levels[i, 0];
			}
			break;
			
		case LevelMapOrientation.NW:
			result[0] = lmap.levels[0, 0];
			break;
			
		}
		
		return result;
		
	}

}


// Helper data structures

public struct TC_HashTable {
	
	public int elements;
	public int elems_per_entry;
	public int num_entries;
	public int world_size_x;
	public int world_size_y;
	public List<Vector2> indexes_saved;
	
	private List<TChunk>[] hash_table;
	private GameObject dummy1;
	private GenerateTerrainChunk dummy2;
	
	public TC_HashTable(int elements, int num_entries, int world_size_x, int world_size_y) {
		this.elements = elements;
		this.elems_per_entry = elements / num_entries + 1;
		this.num_entries = num_entries;
		this.world_size_x = world_size_x;
		this.world_size_y = world_size_y;
		this.indexes_saved = new List<Vector2> ();
		
		dummy1 = new GameObject ();
		dummy2 = dummy1.AddComponent<GenerateTerrainChunk> ();
		dummy2.isValid = false;
		
		hash_table = new List<TChunk>[num_entries];
		for (int i = 0; i < num_entries; ++i) {
			hash_table[i] = new List<TChunk> ();
		}
		
	}
	
	public void addElem(ref TChunk chunk) {
		int elem_index = chunk.index_y * world_size_x + chunk.index_x;
		List<TChunk> entry = hash_table [getEntryIndex (elem_index)];
		
		for(int i = 0; i < entry.Count; ++i) {
			if(entry[i].index_x == chunk.index_x && entry[i].index_y == chunk.index_y) {
				// already exists
				return;
			}
		}
		
		indexes_saved.Add (new Vector2(chunk.index_x, chunk.index_y));
		entry.Add (chunk);
		
	}
	
	public TChunk getElem(int index_x, int index_y) {
		int elem_index = index_y * world_size_x + index_x;
		List<TChunk> entry = hash_table [getEntryIndex (elem_index)];
		
		for (int i = 0; i < entry.Count; ++i) {
			if(entry[i].index_x == index_x && entry[i].index_y == index_y) {
				return entry[i];
			}
		}
		
		return new TChunk (-1, -1, Vector3.zero, Vector3.zero, ref dummy1, ref dummy1, ref dummy2);
	}
	
	public void removeElem(ref TChunk chunk) {
		int elem_index = chunk.index_y * world_size_x + chunk.index_x;
		List<TChunk> entry = hash_table [getEntryIndex (elem_index)];
		
		for (int i = 0; i < entry.Count; ++i) {
			if(entry[i].index_x == chunk.index_x && entry[i].index_y == chunk.index_y) {
				indexes_saved.Remove (new Vector2(chunk.index_x, chunk.index_y));
				entry.RemoveAt(i);
				return;
			}
		}
		
	}
	
	public List<Vector2> getSavedIndexes() {
		return indexes_saved;
	}
	
	public int getEntryIndex(int elem_index) {
		return (int) (elem_index / elems_per_entry);
	}
	
	public bool elemIsValid(TChunk chunk) {
		if (chunk.index_x >= 0 && chunk.index_y >= 0) {
			return true;
		}
		return false;
	}
	
}

public struct TChunk {
	
	public int index_x;
	public int index_y;
	Vector3 origin;
	Vector3 center;
	
	public GameObject terrain_chunk;
	public GameObject ws_chunk;
	public GenerateTerrainChunk generator;
	
	public TChunk(int index_x, int index_y, Vector3 origin, Vector3 center, ref GameObject terrain_chunk, ref GameObject ws_chunk, ref GenerateTerrainChunk generator) {
		this.index_x = index_x;
		this.index_y = index_y;
		this.origin = origin;
		this.center = center;
		this.terrain_chunk = terrain_chunk;
		this.ws_chunk = ws_chunk;
		this.generator = generator;
	}
	
}

public struct LevelMap {
	public int[,] levels;
	public int index_x;
	public int index_y;
	public void setLevelMap(int ind_x, int ind_y, ref int[,] lmap) {
		levels = lmap;
		// chunk indexes in the world
		index_x = ind_x;
		index_y = ind_y;
	}
	public void invalidate() {
		index_x = -1;
		index_y = -1;
	}
};
