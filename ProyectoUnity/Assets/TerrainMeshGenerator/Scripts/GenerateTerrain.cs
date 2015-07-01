﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GenerateTerrain : MonoBehaviour {

	public float tile_size = 0.64f;
	public float slope_factor = 0.5f;

	public int side_tile_count = 24;
	
	public int terrain_textures_x = 3;
	public int terrain_textures_y = 12;
	public int map_size_x = 384;
	public int map_size_y = 384;

	public int level_count = 16;
	public float level_zero_grayscale = 0.24f; 
	

	private LevelMap[,] WorldLevelMaps;
	private TileTypeMap[,] WorldTileTypeMaps;
	private WaterFlowDirectionMap[,] WorldWaterFlowDirectionMaps;

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

	private Stack<GameObject> Flag_Obj_Pool;
	private int flag_stack_size = 1000;


	private Stack<GameObject> WHotspot_Obj_Pool;
	private int whotspot_stack_size = 1000;

	private List<TChunk> loaded_chunks2;

	private TC_HashTable loaded_chunks;


	public GameObject TerrainChunk_Prefab;
	public GameObject WaterSurfaceChunk_Prefab;
	public GameObject PollutionChunk_Prefab;
	public GameObject SceneryObject_Prefab;
	public GameObject PathObject_Prefab;
	public GameObject FlagObject_Prefab;
	public GameObject WaterHotspotObject_Prefab;

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
	public void init(string heightMap_path, string tileTypeMap_path, string waterFlowMap_path) {

		TerrainGenerator = gameObject.GetComponent<GenerateTerrain> ();
		SceneryObject_Ref = SceneryObject_Prefab;
		
		slope_height = tile_size * slope_factor;
		chunk_size_x = side_tile_count * tile_size;
		chunk_size_y = side_tile_count * tile_size;
		origin = gameObject.transform.position;

		MapLoader mloader = new MapLoader ();
		mloader.openFile (map_size_x, map_size_y, heightMap_path);
		List<int[,]> lmaps = mloader.getLevelMapList (level_count, level_zero_grayscale, side_tile_count, out terrain_chunks_x, out terrain_chunks_y);

		mloader = new MapLoader ();
		mloader.openFile (map_size_x, map_size_y, tileTypeMap_path);
		List<TileType[,]> ttmaps = mloader.getTileTypeMapList (side_tile_count);

		mloader = new MapLoader ();
		mloader.openFile (map_size_x, map_size_y, waterFlowMap_path);
		List<WaterFlowDirection[,]> wfmaps = mloader.getWFDMapList (side_tile_count);




		WorldLevelMaps = setWorldLevelMaps (ref lmaps, terrain_chunks_x, terrain_chunks_y);
		WorldTileTypeMaps = setWorldTileTypeMaps (ref ttmaps, terrain_chunks_x, terrain_chunks_y);
		WorldWaterFlowDirectionMaps = setWorldWFDMaps (ref wfmaps, terrain_chunks_x, terrain_chunks_y);

		initSceneryObjPool ();
		initPathObjPool ();
		initFlagObjPool ();
		initWHotspotObjPool ();
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
				generator.generate (index_x, index_y, tile_size, slope_height, side_tile_count, chunk_origin, chunk_center, ref WorldLevelMaps, WorldTileTypeMaps, WorldWaterFlowDirectionMaps, terrain_chunks_x, terrain_chunks_y, terrain_textures_x, terrain_textures_y, ref neighbor_level_maps, new_terrain_chunk.GetComponent<MeshFilter> ());
				
				GameObject new_ws_chunk = (GameObject) Instantiate(WaterSurfaceChunk_Prefab);
				new_ws_chunk.GetComponent<MeshRenderer> ().sortingOrder = -1;
				new_ws_chunk.transform.parent = new_terrain_chunk.transform;
				new_ws_chunk.transform.position = new_terrain_chunk.transform.position;
				generator.generateWaterSurface (new_ws_chunk);

				GameObject new_pollution_chunk = (GameObject) Instantiate(PollutionChunk_Prefab);
				new_pollution_chunk.transform.parent = new_terrain_chunk.transform;
				new_pollution_chunk.transform.position = new_terrain_chunk.transform.position;
				generator.generatePollution (new_pollution_chunk);
				
				TChunk terrain_chunk = new TChunk(index_x, index_y, chunk_origin, chunk_center, ref new_terrain_chunk, ref new_ws_chunk, ref generator);
				TerrainChunks[index_y, index_x] = terrain_chunk;
				
			}
			
		}

	}

	public void setChunkPollution(int chunk_index_x, int chunk_index_y, int tile_index_x, int tile_index_y, float acidity, float oxigenation, float turbidity) {

		TChunk? chunk = getChunk (chunk_index_x, chunk_index_y);

		if(chunk != null && tile_index_x >= 0 && tile_index_x < side_tile_count && tile_index_y >= 0 && tile_index_y < side_tile_count) {

			TChunk tchunk = (TChunk) chunk;
			tchunk.generator.tc.setChunkPollution (tile_index_x, tile_index_y, acidity, oxigenation, turbidity);

		}

	}

	public void UpdateChunk(int chunkIndexX, int chunkIndexY) {
		TChunk elem = loaded_chunks.getElem(chunkIndexX, chunkIndexY);
		elem.generator.unloadScenery(ref Scenery_Obj_Pool);
		elem.generator.unloadPaths(ref Path_Obj_Pool);
		elem.generator.unloadFlags(ref Flag_Obj_Pool);
		elem.generator.unloadWHotspots(ref WHotspot_Obj_Pool);
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
		TileIndexedObject sc_obj = obj.GetComponent<TileIndexedObject> ();
		TChunk chunk = loaded_chunks.getElem (sc_obj.chunk_index_x, sc_obj.chunk_index_y);
		chunk.generator.destroySceneryObject (obj, ref Scenery_Obj_Pool);
		
	}

	public void destroyPathObject(int chunk_index_x, int chunk_index_y, int tile_index_x, int tile_index_y) {
		
		TChunk chunk = loaded_chunks.getElem (chunk_index_x, chunk_index_y);
		chunk.generator.destroyPathObject (tile_index_x, tile_index_y);
		
	}
	
	public bool tileContainsPath(int chunk_index_x, int chunk_index_y, int tile_index_x, int tile_index_y) {
		
		TChunk chunk = loaded_chunks.getElem (chunk_index_x, chunk_index_y);
		return chunk.generator.tileContainsPath (tile_index_x, tile_index_y);
		
	}

	// flag

	public void destroyFlagObject(int chunk_index_x, int chunk_index_y, int tile_index_x, int tile_index_y) {
		
		TChunk chunk = loaded_chunks.getElem (chunk_index_x, chunk_index_y);
		chunk.generator.destroyFlagObject (tile_index_x, tile_index_y);
		
	}
	
	public bool tileContainsFlag(int chunk_index_x, int chunk_index_y, int tile_index_x, int tile_index_y) {
		
		TChunk chunk = loaded_chunks.getElem (chunk_index_x, chunk_index_y);
		return chunk.generator.tileContainsFlag (tile_index_x, tile_index_y);
		
	}

	public int[,] getChunkSceneryMap(int chunk_index_x, int chunk_index_y) {

		TChunk chunk = loaded_chunks.getElem (chunk_index_x, chunk_index_y);
		return chunk.generator.getSceneryMap ();

	}

	public PathType[,] getChunkPathMap(int chunk_index_x, int chunk_index_y) {
		
		TChunk chunk = loaded_chunks.getElem (chunk_index_x, chunk_index_y);
		return chunk.generator.getPathMap ();
		
	}
	// flag
	public FlagType[,] getChunkFlagMap(int chunk_index_x, int chunk_index_y) {
		
		TChunk chunk = loaded_chunks.getElem (chunk_index_x, chunk_index_y);
		return chunk.generator.getFlagMap ();
		
	}

	// load scenery, path and water hotspot objects in visible_chunks. This is done every frame.
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
				elem.generator.unloadFlags(ref Flag_Obj_Pool);
				elem.generator.unloadWHotspots(ref WHotspot_Obj_Pool);
				loaded_chunks.removeElem(ref elem);
			}

		}

		for (int i = 0; i < visible_chunks.Count; ++i) {

			if(!loaded_chunks.elemIsValid(loaded_chunks.getElem(visible_chunks[i].index_x, visible_chunks[i].index_y))) {
				// add chunk to table
				TChunk elem = visible_chunks[i];
				elem.generator.loadScenery(ref Scenery_Obj_Pool);
				elem.generator.loadPaths(ref Path_Obj_Pool);
				elem.generator.loadFlags(ref Flag_Obj_Pool);
				elem.generator.loadWHotspots(ref WHotspot_Obj_Pool);
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

		if(index_x >= 0 && index_x < terrain_chunks_x && index_y >= 0 && index_y < terrain_chunks_y) {
			return TerrainChunks[index_y, index_x];
		}

		TChunk invalid = new TChunk ();
		return invalid;

	}

	public TChunk? getChunk (int index_x, int index_y) {

		if(index_x >= 0 && index_x < terrain_chunks_x && index_y >= 0 && index_y < terrain_chunks_y) {
			return TerrainChunks[index_y, index_x];
		}

		return null;

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
			new_scenery_obj.GetComponent<TileIndexedObject> ().initType("scenery");
			new_scenery_obj.GetComponent<CameraFace> ().m_Camera = camera;
			new_scenery_obj.SetActive(false);

			Scenery_Obj_Pool.Push(new_scenery_obj);

		}

	}


	// sjakjlsajasklj
	private void initPathObjPool() {
		
		Path_Obj_Pool = new Stack<GameObject> ();
		
		for (int i = 0; i < path_stack_size; ++i) {
			
			GameObject new_path_obj = (GameObject) Instantiate(PathObject_Prefab);
			new_path_obj.transform.parent = SceneryHolder.transform;
			new_path_obj.GetComponent<TileIndexedObject> ().initType("path");
			new_path_obj.SetActive(false);
			
			Path_Obj_Pool.Push(new_path_obj);
			
		}
		
	}

	private void initFlagObjPool() {
		
		Flag_Obj_Pool = new Stack<GameObject> ();
		
		for (int i = 0; i < flag_stack_size; ++i) {
			
			GameObject new_flag_obj = (GameObject) Instantiate(FlagObject_Prefab);
			new_flag_obj.transform.parent = SceneryHolder.transform;
			new_flag_obj.GetComponent<TileIndexedObject> ().initType("flag");
			new_flag_obj.GetComponent<CameraFace> ().m_Camera = camera;
			new_flag_obj.SetActive(false);
			
			Flag_Obj_Pool.Push(new_flag_obj);
			
		}
		
	}

	private void initWHotspotObjPool() {
		
		WHotspot_Obj_Pool = new Stack<GameObject> ();
		
		for (int i = 0; i < whotspot_stack_size; ++i) {
			
			GameObject new_wh_obj = (GameObject) Instantiate(WaterHotspotObject_Prefab);
			new_wh_obj.transform.parent = SceneryHolder.transform;
			new_wh_obj.GetComponent<TileIndexedObject> ().initType("whotspot");
			new_wh_obj.SetActive(false);
			
			WHotspot_Obj_Pool.Push(new_wh_obj);
			
		}
		
	}

	public bool tileIsSuitableForScenery(int chunk_index_x, int chunk_index_y, int tile_index_x, int tile_index_y) {
		
		TChunk chunk = loaded_chunks.getElem (chunk_index_x, chunk_index_y);
		return chunk.generator.tileIsSuitableForScenery (tile_index_x, tile_index_y);
		
	}

	public bool tileIsSuitableForBridge(int chunk_index_x, int chunk_index_y, int tile_index_x, int tile_index_y) {
		
		TChunk chunk = loaded_chunks.getElem (chunk_index_x, chunk_index_y);
		return chunk.generator.tileIsSuitableForBridge (tile_index_x, tile_index_y);
		
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

	private TileTypeMap[,] setWorldTileTypeMaps(ref List<TileType[,]> wttmaps, int t_chunks_x, int t_chunks_y) {
		
		int counter = 0;
		TileTypeMap[,] WorldTTypeMaps = new TileTypeMap[t_chunks_y, t_chunks_x];
		for (int i = 0; i < t_chunks_y; ++i) {
			for(int j = 0; j < t_chunks_x; ++j) {
				TileType[,] ttmap = wttmaps[counter];
				TileTypeMap map = new TileTypeMap();
				map.setTTypeMap(j, i, ref ttmap);
				WorldTTypeMaps[i, j] = map;
				++counter;
			}
		}
		
		return WorldTTypeMaps;
		
	}

	private WaterFlowDirectionMap[,] setWorldWFDMaps(ref List<WaterFlowDirection[,]> wfmaps, int t_chunks_x, int t_chunks_y) {
		
		int counter = 0;
		WaterFlowDirectionMap[,] WorldWFDMaps = new WaterFlowDirectionMap[t_chunks_y, t_chunks_x];
		for (int i = 0; i < t_chunks_y; ++i) {
			for(int j = 0; j < t_chunks_x; ++j) {
				WaterFlowDirection[,] wfmap = wfmaps[counter];
				WaterFlowDirectionMap map = new WaterFlowDirectionMap();
				map.setWFDMap(j, i, ref wfmap);
				WorldWFDMaps[i, j] = map;
				++counter;
			}
		}
		
		return WorldWFDMaps;
		
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

public struct TileTypeMap {
	public TileType[,] ttypes;
	public int index_x;
	public int index_y;
	public void setTTypeMap(int ind_x, int ind_y, ref TileType[,] ttpmap) {
		ttypes = ttpmap;
		// chunk indexes in the world
		index_x = ind_x;
		index_y = ind_y;
	}
	public void invalidate() {
		index_x = -1;
		index_y = -1;
	}
};

public struct WaterFlowDirectionMap {
	public WaterFlowDirection[,] wfdirs;
	public int index_x;
	public int index_y;
	public void setWFDMap(int ind_x, int ind_y, ref WaterFlowDirection[,] wfdmap) {
		wfdirs = wfdmap;
		// chunk indexes in the world
		index_x = ind_x;
		index_y = ind_y;
	}
	public void invalidate() {
		index_x = -1;
		index_y = -1;
	}
};
