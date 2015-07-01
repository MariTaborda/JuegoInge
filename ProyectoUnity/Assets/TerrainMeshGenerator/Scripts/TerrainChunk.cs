﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum LevelMapOrientation { N, NE, E, SE, S, SW, W, NW };

public enum WaterFlowDirection { none, N, NE, E, SE, S, SW, W, NW };

public enum TileDivisionConfiguration { topLeft_to_bottomRight, bottomLeft_to_topRight };

public enum TileVertices { topLeft, topRight, bottomRight, bottomLeft };

public enum TileBrightness { normal, dark, light };

public enum TextureType { grass_blue, grass_green, grass_yellow, dirt, rock, sand };

// Type of scenery object
public enum SceneryType { empty, tree1, tree2, tree3, tree4, tree5, bush1, bush2, bush3, bush4, bush5, piedra1, piedra2, casa1, granjita, zonarural, zonarural2, zonaruralsingranjas, zonaciudad, zonaciudad2, zonaciudad3, alcatraz, bigHouse1, fuente, mercado, molino, police, residencial, stores, templo, treePink, vereda};

public enum PathType { empty, path1, bridge };

//public enum FlagType { empty, checkpoint, arrowE, arrowNW, arrowNE, arrowW, arrowSW, arrowSE }; 


public enum FlagType { empty, checkpoint, trash1 }; 

public enum TileType { empty, high_zone, medium_zone, low_zone, beach, dirt, rocky, urban_area, rural_area };

public class TerrainChunk {
	
	/*

	Tile vertex indexes:

	  v[0]       v[1]
		+---------+
		|		  |
		|		  |
		|		  |
		|		  |
		+---------+
	  v[3]       v[2]

		Tile vertices are ordered clockwise and start at top-left corner of tile.
		Use TileVertices as vertex indexer (use "v[TileVertices.topLeft]" instead of "v[0]").


	Tile division configurations:

		topLeft_to_bottomRight:					bottomLeft_to_topRight:

			 +---------+							  +---------+
			 |`-.	   |							  |	     .-`|
			 |  `-.	   |							  |	   .-`  |
			 |	  `-.  |							  |	 .-`    |
			 |	  	`-.|							  |.-`	    |
			 +---------+							  +---------+

		Individual tiles are divided into 2 triangles.
		Default configuration is topLeft_to_bottomRight.
		

	 */

	private int chunk_index_x;
	private int chunk_index_y;
	private int world_chunks_x;
	private int world_chunks_y;

	private Vector3 origin;
	private int total_tile_count;
	private int side_tile_count;
	private float tile_size;
	private float slope_height;
	private Vector3 light_direction = Vector3.left;

	// level map: associates a height level for every tile	
	private int[,] level_map;
	private List<int[]> neighbor_level_maps; 

	// tile type map: associates a TileType for every tile
	private TileType[,] tile_type_map;

	// surface level: Y position of water surface
	private float surface_level = -0.01f;

	// triangle division configuration: associates a division configuration for every tile
	private TileDivisionConfiguration[,] tri_div_conf_map;

	// brightness map: associates a brigthness level for every triangle
	private TileBrightness[,,] brightness_map;

	// number of individual textures in the terrain texture
	private int num_textures_x;
	private int num_textures_y;
	private Vector2[,,] texture_coords; 

	// vars for generating terrain mesh
	private Material[] materials;
	private Vector3[] vertices;
	private int[] triangles;
	private Vector2[] uv;

	// associates a water flow direction for every tile
	private WaterFlowDirection[,] flow_map;

	// vars for generating water surface mesh
	private Material[] ws_materials;
	private Vector3[] ws_vertices;
	private int[] ws_triangles;
	private Vector2[] ws_uv;
	private Mesh water_surface_mesh;

	private Mesh mesh;
	private MeshFilter mesh_filter;
	private MeshRenderer mesh_renderer;

	// vars for generating pollution mesh
	private Material[] pollution_materials;
	private Vector3[] pollution_vertices;
	private int[] pollution_triangles;
	private Vector2[] pollution_uv;
	private Mesh pollution_mesh;

	// scenery map: associates a scenery value with every cell
	private int[,] scenery_map;
	private float scenery_density = 0.3f;

	private PathType[,] path_map;
	private FlagType[,] flag_map;

	private int[,] whotspot_map;
	private float whotspot_density = 0.1f;

	// stores scenery objects which are rendered and correspond to this terrain chunk 
	private Stack<GameObject> reserved_sc_objs;

	private Stack<GameObject> reserved_path_objs;
	private Stack<GameObject> reserved_flag_objs;  

	private Stack<GameObject> reserved_wh_objs;

	private float[,] acidity_map;
	private float[,] oxigenation_map;
	private float[,] turbidity_map;
	private float[,] pollution_map;
	private float pollution_propagation_rate = 10;
	private bool isPolluted = false;
	private bool pollutionChanged = false;
	private List<Vector2> activePollutionTiles;

	bool isValid = false;
	
	public TerrainChunk(int index_x, int index_y, int side_chunks_x, int side_chunks_y, Vector3 origin, int side_tile_count, float tile_size, float slope_height, ref int[,] level_map, TileType[,] tile_type_map, WaterFlowDirection[,] wfd_map, int num_textures_x, int num_textures_y,  ref List<int[]> neighbor_level_maps, MeshFilter mesh_filter, int zona) {
		setAttributes (index_x, index_y, side_chunks_x, side_chunks_y, origin, side_tile_count, tile_size, slope_height, ref level_map, tile_type_map, wfd_map, num_textures_x, num_textures_y, ref neighbor_level_maps, mesh_filter, zona);
		generateMesh ();
	}
	
	public TerrainChunk() {
		isValid = false;
	}

	// set initial attributes. happens only once on object initialization.
	public void setAttributes(int index_x, int index_y, int side_chunks_x, int side_chunks_y, Vector3 origin, int side_tile_count, float tile_size, float slope_height, ref int[,] level_map, TileType[,] tile_type_map, WaterFlowDirection[,] wfd_map, int textures_x, int textures_y,  ref List<int[]> neighbor_level_maps, MeshFilter mesh_filter, int zona) {
		
		if (level_map.Length == side_tile_count * side_tile_count) {
			this.chunk_index_x = index_x;
			this.chunk_index_y = index_y;
			this.world_chunks_x = side_chunks_x;
			this.world_chunks_y = side_chunks_y;
			this.origin = origin;
			this.mesh_filter = mesh_filter;
			this.total_tile_count = side_tile_count * side_tile_count;
			this.side_tile_count = side_tile_count;
			this.tile_size = tile_size;
			this.slope_height = slope_height;
			this.level_map = level_map;
			this.tile_type_map = tile_type_map;
			this.neighbor_level_maps = neighbor_level_maps;
			this.tri_div_conf_map = setTriDivConfMap (level_map);
			this.brightness_map = setBrightnessMap (level_map);
			this.flow_map = wfd_map;
			this.num_textures_x = textures_x;
			this.num_textures_y = textures_y;
			if (zona == 1) { 		// rural 
				this.scenery_map = setSceneryMapRural();
			} else if (zona == 2) { // city
				this.scenery_map = setSceneryMapCity();
			} else if (zona == 3) {				//  trees 
				this.scenery_map = setSceneryMapTree();
			} else if (zona == 4) {				//  trees end 
				this.scenery_map = setSceneryMapTreeEnd();
			} else {			// default
				this.scenery_map = setSceneryMap();
			}
			this.path_map = setPathMap();
			this.flag_map = setFlagMap(); 
			this.pollution_map = setPollutionMap();
			this.whotspot_map = setWHotspotMap();
			setTextureCoords();
			isValid = true;
		} 
		else {
			string error = "Invalid level_map: ";
			if(level_map.Length != side_tile_count * side_tile_count) {
				error += " Level map must be of size [side_tile_count, side_tile_count].";
			}
			Debug.LogError (error);
		}
		
	}

	// generates terrain and water surface meshes
	public void generateMesh() {
		
		if (!isValid) {
			Debug.LogError("TerrainChunk is invalid. Attributes not set.");
			return;		
		}
		
		// Terrain mesh data
		mesh = new Mesh ();
		mesh.Clear ();
		mesh.name = "TerrainChunk_Mesh";
		
		List<int>[] textured_tris = new List<int>[3];
		textured_tris[(int)TileBrightness.normal] = new List<int>();
		textured_tris[(int)TileBrightness.dark] = new List<int>();
		textured_tris[(int)TileBrightness.light] = new List<int>();
		
		vertices = new Vector3[4 * total_tile_count];
		triangles = new int[6 * total_tile_count];
		uv = new Vector2[4 * total_tile_count];
		
		// Water surface mesh data
		water_surface_mesh = new Mesh ();
		water_surface_mesh.Clear ();
		water_surface_mesh.name = "TC_WaterSurface_Mesh";
		
		List<int>[] ws_flowing_tris = new List<int>[9];
		ws_flowing_tris[(int)WaterFlowDirection.none] = new List<int>();
		ws_flowing_tris[(int)WaterFlowDirection.N] = new List<int>();
		ws_flowing_tris[(int)WaterFlowDirection.NE] = new List<int>();
		ws_flowing_tris[(int)WaterFlowDirection.E] = new List<int>();	
		ws_flowing_tris[(int)WaterFlowDirection.SE] = new List<int>();
		ws_flowing_tris[(int)WaterFlowDirection.S] = new List<int>();
		ws_flowing_tris[(int)WaterFlowDirection.SW] = new List<int>();
		ws_flowing_tris[(int)WaterFlowDirection.W] = new List<int>();
		ws_flowing_tris[(int)WaterFlowDirection.NW] = new List<int>();
		
		ws_vertices = new Vector3[4 * total_tile_count];
		ws_triangles = new int[6 * total_tile_count];
		ws_uv = new Vector2[4 * total_tile_count];

		// Pollution mesh data
		pollution_mesh = new Mesh ();
		pollution_mesh.Clear ();
		pollution_mesh.name = "TC_Pollution_Mesh";
		
		pollution_vertices = new Vector3[4 * total_tile_count];
		pollution_triangles = new int[6 * total_tile_count];
		pollution_uv = new Vector2[4 * total_tile_count];
		
		// Prepare mesh data for every tile
		for(int i = 0; i < total_tile_count; ++i) {
			
			int lmap_index_y = i / side_tile_count;
			int lmap_index_x = i % side_tile_count;
			
			int v0_level = getMax( getVertexLevels( lmap_index_x, lmap_index_y, TileVertices.topLeft ) );
			int v1_level = getMax( getVertexLevels( lmap_index_x, lmap_index_y, TileVertices.topRight ) );
			int v2_level = getMax( getVertexLevels( lmap_index_x, lmap_index_y, TileVertices.bottomRight ) );
			int v3_level = getMax( getVertexLevels( lmap_index_x, lmap_index_y, TileVertices.bottomLeft ) );
			
			float origin_x = lmap_index_x * tile_size;
			float origin_y = lmap_index_y * tile_size;
			
			int tile_level = getTileLevel(lmap_index_x, lmap_index_y);
			float pollution_level = getPollutionLevel(lmap_index_x, lmap_index_y);
			
			if(tile_level < surface_level) {		
				
				// set water surface vertices
				setWaterSurfaceVertices(i, 
				                        new Vector3( origin_x, 				origin_y, 				surface_level), 
				                        new Vector3( origin_x + tile_size, 	origin_y, 				surface_level), 
				                        new Vector3( origin_x + tile_size, 	origin_y + tile_size, 	surface_level), 
				                        new Vector3( origin_x, 				origin_y + tile_size, 	surface_level)
				                        );
				
				// set water surface triangles
				setWaterSurfaceTriangles(lmap_index_x, lmap_index_y, i);
				
				// set ws triangles by direction of water flow
				ws_flowing_tris[(int) flow_map[lmap_index_y, lmap_index_x]].Add (ws_triangles[6*i + 0]);
				ws_flowing_tris[(int) flow_map[lmap_index_y, lmap_index_x]].Add (ws_triangles[6*i + 1]);
				ws_flowing_tris[(int) flow_map[lmap_index_y, lmap_index_x]].Add (ws_triangles[6*i + 2]);
				
				ws_flowing_tris[(int) flow_map[lmap_index_y, lmap_index_x]].Add (ws_triangles[6*i + 3]);
				ws_flowing_tris[(int) flow_map[lmap_index_y, lmap_index_x]].Add (ws_triangles[6*i + 4]);
				ws_flowing_tris[(int) flow_map[lmap_index_y, lmap_index_x]].Add (ws_triangles[6*i + 5]);
				
				// set water surface uvs
				ws_uv [4*i + (int)TileVertices.topLeft] 		= new Vector2(0, 0);
				ws_uv [4*i + (int)TileVertices.topRight] 		= new Vector2(1, 0);
				ws_uv [4*i + (int)TileVertices.bottomRight] 	= new Vector2(1, 1);
				ws_uv [4*i + (int)TileVertices.bottomLeft] 		= new Vector2(0, 1);

				if(pollution_level > 0f) {

					// set pollution vertices
					setPollutionVertices(i, 
					                     new Vector3( origin_x, 				origin_y, 				surface_level+0.01f), 
					                     new Vector3( origin_x + tile_size, 	origin_y, 				surface_level+0.01f), 
					                     new Vector3( origin_x + tile_size, 	origin_y + tile_size, 	surface_level+0.01f), 
					                     new Vector3( origin_x, 				origin_y + tile_size, 	surface_level+0.01f)
					                     );
					
					// set pollution triangles
					setPollutionTriangles(lmap_index_x, lmap_index_y, i);
					
					// set pollution uvs
					pollution_uv [4*i + (int)TileVertices.topLeft] 		= new Vector2(0, 0);
					pollution_uv [4*i + (int)TileVertices.topRight] 		= new Vector2(1, 0);
					pollution_uv [4*i + (int)TileVertices.bottomRight] 	= new Vector2(1, 1);
					pollution_uv [4*i + (int)TileVertices.bottomLeft] 		= new Vector2(0, 1);

				}

			}
			
			// set tile vertices
			setTileVertices(i, 
			                new Vector3( origin_x, 				origin_y, 				v0_level * slope_height), 
			                new Vector3( origin_x + tile_size, 	origin_y, 				v1_level * slope_height), 
			                new Vector3( origin_x + tile_size, 	origin_y + tile_size, 	v2_level * slope_height), 
			                new Vector3( origin_x, 				origin_y + tile_size, 	v3_level * slope_height)
			                );
			
			// set triangles
			setTileTriangles(lmap_index_x, lmap_index_y, i);
			
			// set textured triangles
			// triangle 0
			textured_tris[(int) brightness_map[lmap_index_y, lmap_index_x, 0]].Add(triangles[6*i + 0]);
			textured_tris[(int) brightness_map[lmap_index_y, lmap_index_x, 0]].Add(triangles[6*i + 1]);
			textured_tris[(int) brightness_map[lmap_index_y, lmap_index_x, 0]].Add(triangles[6*i + 2]);
			
			//triangle 1
			textured_tris[(int) brightness_map[lmap_index_y, lmap_index_x, 1]].Add(triangles[6*i + 3]);
			textured_tris[(int) brightness_map[lmap_index_y, lmap_index_x, 1]].Add(triangles[6*i + 4]);
			textured_tris[(int) brightness_map[lmap_index_y, lmap_index_x, 1]].Add(triangles[6*i + 5]);
			
			// set UVs
			int verts_below_zero = 0;
			if(v0_level < 0) {
				verts_below_zero++;
			}
			if(v1_level < 0) {
				verts_below_zero++;
			}
			if(v2_level < 0) {
				verts_below_zero++;
			}
			if(v3_level < 0) {
				verts_below_zero++;
			}
			
			Vector2 texture_type_indexes;

			if(tile_type_map[lmap_index_y, lmap_index_x] == TileType.dirt) {
				texture_type_indexes = getTextureRowIndexes(TextureType.dirt);
			} else if(tile_type_map[lmap_index_y, lmap_index_x] == TileType.high_zone) {
				texture_type_indexes = getTextureRowIndexes(TextureType.grass_blue);
			} else if(tile_type_map[lmap_index_y, lmap_index_x] == TileType.medium_zone) {
				texture_type_indexes = getTextureRowIndexes(TextureType.grass_green);
			} else if(tile_type_map[lmap_index_y, lmap_index_x] == TileType.low_zone) {
				texture_type_indexes = getTextureRowIndexes(TextureType.grass_yellow);
			} else if(tile_type_map[lmap_index_y, lmap_index_x] == TileType.rocky) {
				texture_type_indexes = getTextureRowIndexes(TextureType.rock);
			} else if(tile_type_map[lmap_index_y, lmap_index_x] == TileType.beach) {
				texture_type_indexes = getTextureRowIndexes(TextureType.sand);
			} else {
				// Otherwise set texture as grass.
				texture_type_indexes = getTextureRowIndexes(TextureType.grass_green);
			}

			if(verts_below_zero > 1) {
				// If more than 1 vertex has height below zero, set texture as dirt.
				texture_type_indexes = getTextureRowIndexes(TextureType.rock);
			}

			
			int t_index_y = (int) UnityEngine.Random.Range(texture_type_indexes.x, texture_type_indexes.y+1);
			int t_index_x = (int) UnityEngine.Random.Range(0, num_textures_x);
			
			uv [4*i + (int)TileVertices.topLeft] 		= texture_coords[t_index_y, t_index_x, (int) TileVertices.topLeft];
			uv [4*i + (int)TileVertices.topRight] 		= texture_coords[t_index_y, t_index_x, (int) TileVertices.topRight];
			uv [4*i + (int)TileVertices.bottomRight] 	= texture_coords[t_index_y, t_index_x, (int) TileVertices.bottomRight];
			uv [4*i + (int)TileVertices.bottomLeft] 	= texture_coords[t_index_y, t_index_x, (int) TileVertices.bottomLeft];
			
			//Debug.Log("Ind: "+i+", v0: "+vertices[4*i + (int)TileVertices.topLeft]+", v1: "+vertices[4*i + (int)TileVertices.topRight]+", v2: "+vertices[4*i + (int)TileVertices.bottomRight]+", v3: "+vertices[4*i + (int)TileVertices.bottomLeft]+", tri conf: "+tri_div_conf_map[lmap_index_y, lmap_index_x]+", tri-0: "+brightness_map[lmap_index_y, lmap_index_x, 0]+", tri-1: "+brightness_map[lmap_index_y, lmap_index_x, 1]);
			
		}
		
		
		// Assign terrain mesh data
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		
		mesh.subMeshCount = 3;
		
		setSubmeshTriangles(ref mesh, ref textured_tris, (int) TileBrightness.normal);
		setSubmeshTriangles(ref mesh, ref textured_tris, (int) TileBrightness.dark);
		setSubmeshTriangles(ref mesh, ref textured_tris, (int) TileBrightness.light);
		
		mesh.uv = uv;
		
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize ();
		
		
		// Assign water surface mesh data
		water_surface_mesh.vertices = ws_vertices;
		water_surface_mesh.triangles = ws_triangles;
		
		water_surface_mesh.subMeshCount = 9;

		setSubmeshTriangles(ref water_surface_mesh, ref ws_flowing_tris, (int) WaterFlowDirection.none);
		setSubmeshTriangles(ref water_surface_mesh, ref ws_flowing_tris, (int) WaterFlowDirection.N);
		setSubmeshTriangles(ref water_surface_mesh, ref ws_flowing_tris, (int) WaterFlowDirection.NE);
		setSubmeshTriangles(ref water_surface_mesh, ref ws_flowing_tris, (int) WaterFlowDirection.E);
		setSubmeshTriangles(ref water_surface_mesh, ref ws_flowing_tris, (int) WaterFlowDirection.SE);
		setSubmeshTriangles(ref water_surface_mesh, ref ws_flowing_tris, (int) WaterFlowDirection.S);
		setSubmeshTriangles(ref water_surface_mesh, ref ws_flowing_tris, (int) WaterFlowDirection.SW);
		setSubmeshTriangles(ref water_surface_mesh, ref ws_flowing_tris, (int) WaterFlowDirection.W);
		setSubmeshTriangles(ref water_surface_mesh, ref ws_flowing_tris, (int) WaterFlowDirection.NW);
		
		water_surface_mesh.uv = ws_uv;
		
		water_surface_mesh.RecalculateNormals();
		water_surface_mesh.RecalculateBounds();
		water_surface_mesh.Optimize ();


		// Assign pollution mesh data
		pollution_mesh.vertices = pollution_vertices;
		pollution_mesh.triangles = pollution_triangles;
		
		pollution_mesh.uv = pollution_uv;
		
		pollution_mesh.RecalculateNormals();
		pollution_mesh.RecalculateBounds();
		pollution_mesh.Optimize ();
	}

	public void addToPollutionMesh(int tile_index_x, int tile_index_y) {
	
		float pollution_level = getPollutionLevel (tile_index_x, tile_index_y);
		if (tileIsPolluted (tile_index_x, tile_index_y)) {

			float origin_x = tile_index_x * tile_size;
			float origin_y = tile_index_y * tile_size;
			int i = tile_index_y * side_tile_count + tile_index_x;
			
			isPolluted = true;
			
			// set pollution vertices
			setPollutionVertices(i, 
			                     new Vector3( origin_x, 				origin_y, 				surface_level+0.01f), 
			                     new Vector3( origin_x + tile_size, 	origin_y, 				surface_level+0.01f), 
			                     new Vector3( origin_x + tile_size, 	origin_y + tile_size, 	surface_level+0.01f), 
			                     new Vector3( origin_x, 				origin_y + tile_size, 	surface_level+0.01f)
			                     );
			
			// set pollution triangles
			setPollutionTriangles(tile_index_x, tile_index_y, i);
			
			// set pollution uvs
			pollution_uv [4*i + (int)TileVertices.topLeft] 		= new Vector2(0, 0);
			pollution_uv [4*i + (int)TileVertices.topRight] 	= new Vector2(1, 0);
			pollution_uv [4*i + (int)TileVertices.bottomRight] 	= new Vector2(1, 1);
			pollution_uv [4*i + (int)TileVertices.bottomLeft] 	= new Vector2(0, 1);

		}

	}

	public void refreshPollutionMesh() {

		// Assign pollution mesh data
		pollution_mesh.vertices = pollution_vertices;
		pollution_mesh.triangles = pollution_triangles;
		pollution_mesh.uv = pollution_uv;
		pollution_mesh.RecalculateNormals();
		pollution_mesh.RecalculateBounds();
		pollution_mesh.Optimize ();

	}

	public void setChunkPollution(int tile_index_x, int tile_index_y, float acidity, float oxigenation, float turbidity) {
		float pollution = calculatePollution (acidity, oxigenation, turbidity);
		if (pollution_map [tile_index_y, tile_index_x] < pollution) {
			acidity_map [tile_index_y, tile_index_x] = acidity;
			oxigenation_map [tile_index_y, tile_index_x] = oxigenation;
			turbidity_map [tile_index_y, tile_index_x] = turbidity;
			pollution_map [tile_index_y, tile_index_x] = pollution;

			activePollutionTiles.Add (new Vector2(tile_index_x, tile_index_y));
			addToPollutionMesh(tile_index_x, tile_index_y);
			refreshPollutionMesh ();
		}
	}

	public void spreadPollution() {
				
		List<Vector3> pollution_changes = new List<Vector3> ();

		for(int i = 0; i < side_tile_count; ++i) {
			for(int j = 0; j < side_tile_count; ++j) {
				if(tileIsPolluted(j, i)) {
					spreadPollution(j, i, pollution_changes);
				}
			}
		}

		if (pollutionChanged) {
			writebackPollutionMap (pollution_changes);
			refreshPollutionMesh ();
			pollutionChanged = false;
		}

	}

	private void writebackPollutionMap(List<Vector3> pollution_changes) {
		for (int i = 0; i < pollution_changes.Count; ++i) {
			pollution_map[(int)pollution_changes[i].y, (int)pollution_changes[i].x] = pollution_changes[i].z;
			activePollutionTiles.Add (new Vector2(pollution_changes[i].x, pollution_changes[i].y));
			addToPollutionMesh((int)pollution_changes[i].x, (int)pollution_changes[i].y);
		}
	}

	private void spreadPollution(int tile_index_x, int tile_index_y, List<Vector3> pollution_changes) {

		Vector2 new_position;
		float pollution = pollution_map [tile_index_y, tile_index_x];
		float acidity = acidity_map [tile_index_y, tile_index_x];
		float oxigenation = oxigenation_map [tile_index_y, tile_index_x];
		float turbidity = turbidity_map [tile_index_y, tile_index_x];
		List<Vector2> tiles_downstream = getTilesDownstream (tile_index_x, tile_index_y, pollution, acidity, oxigenation, turbidity);

		for (int i = 0; i < tiles_downstream.Count; ++i) {
			new_position = tiles_downstream[i];
			if (new_position.x >= 0 && level_map [(int)new_position.y, (int)new_position.x] < surface_level && pollution_map [(int)new_position.y, (int)new_position.x] < pollution) {
				setTilePollution((int)new_position.x, (int)new_position.y, acidity, oxigenation, turbidity, pollution_changes);
				pollutionChanged = true;
			}
		}

	}
	
	private List<Vector2> getTilesDownstream (int tile_index_x, int tile_index_y, float pollution, float acidity, float oxigenation, float turbidity) {

		List<Vector2> result = new List<Vector2> ();

		Vector2 position;
		bool spreadOut = false;
		if (UnityEngine.Random.Range (0, 100) < pollution_propagation_rate) {
			spreadOut = true;
		}

		switch(flow_map[tile_index_y, tile_index_x]) {
		case WaterFlowDirection.N:

			if(spreadOut) {
				position = getTileIndexesInDirection (tile_index_x, tile_index_y, WaterFlowDirection.NW);
				if(position.x >= 0) {
					result.Add(position);
				}

				position = getTileIndexesInDirection (tile_index_x, tile_index_y, WaterFlowDirection.NE);
				if(position.x >= 0) {
					result.Add(position);
				}
			}

			position = getTileIndexesInDirection (tile_index_x, tile_index_y, WaterFlowDirection.N);
			if(position.x >= 0) {
				result.Add(position);
			}
			else {
				Vector2 neighboring_chunk_indexes = getChunkIndexesInDirection(chunk_index_x, chunk_index_y, WaterFlowDirection.N);
				GameController.gameController.terrainGenerator.setChunkPollution((int)neighboring_chunk_indexes.x, (int)neighboring_chunk_indexes.y, tile_index_x, side_tile_count-1, acidity, oxigenation, turbidity);
			}

			break;
		case WaterFlowDirection.NE:

			if(spreadOut) {
				position = getTileIndexesInDirection (tile_index_x, tile_index_y, WaterFlowDirection.N);
				if(position.x >= 0) {
					result.Add(position);
				}
				else {
					Vector2 neighboring_chunk_indexes = getChunkIndexesInDirection(chunk_index_x, chunk_index_y, WaterFlowDirection.N);
					GameController.gameController.terrainGenerator.setChunkPollution((int)neighboring_chunk_indexes.x, (int)neighboring_chunk_indexes.y, tile_index_x, side_tile_count-1, acidity, oxigenation, turbidity);
				}

				position = getTileIndexesInDirection (tile_index_x, tile_index_y, WaterFlowDirection.E);
				if(position.x >= 0) {
					result.Add(position);
				}
				else {
					Vector2 neighboring_chunk_indexes = getChunkIndexesInDirection(chunk_index_x, chunk_index_y, WaterFlowDirection.E);
					GameController.gameController.terrainGenerator.setChunkPollution((int)neighboring_chunk_indexes.x, (int)neighboring_chunk_indexes.y, 0, tile_index_y, acidity, oxigenation, turbidity);
				}
			}

			position = getTileIndexesInDirection (tile_index_x, tile_index_y, WaterFlowDirection.NE);
			if(position.x >= 0) {
				result.Add(position);
			}

			break;
		case WaterFlowDirection.E:

			if(spreadOut) {
				position = getTileIndexesInDirection (tile_index_x, tile_index_y, WaterFlowDirection.NE);
				if(position.x >= 0) {
					result.Add(position);
				}

				position = getTileIndexesInDirection (tile_index_x, tile_index_y, WaterFlowDirection.SE);
				if(position.x >= 0) {
					result.Add(position);
				}
			}

			position = getTileIndexesInDirection (tile_index_x, tile_index_y, WaterFlowDirection.E);
			if(position.x >= 0) {
				result.Add(position);
			}
			else {
				Vector2 neighboring_chunk_indexes = getChunkIndexesInDirection(chunk_index_x, chunk_index_y, WaterFlowDirection.E);
				GameController.gameController.terrainGenerator.setChunkPollution((int)neighboring_chunk_indexes.x, (int)neighboring_chunk_indexes.y, 0, tile_index_y, acidity, oxigenation, turbidity);
			}

			break;
		case WaterFlowDirection.SE:

			if(spreadOut) {
				position = getTileIndexesInDirection (tile_index_x, tile_index_y, WaterFlowDirection.E);
				if(position.x >= 0) {
					result.Add(position);
				}
				else {
					Vector2 neighboring_chunk_indexes = getChunkIndexesInDirection(chunk_index_x, chunk_index_y, WaterFlowDirection.E);
					GameController.gameController.terrainGenerator.setChunkPollution((int)neighboring_chunk_indexes.x, (int)neighboring_chunk_indexes.y, 0, tile_index_y, acidity, oxigenation, turbidity);
				}

				position = getTileIndexesInDirection (tile_index_x, tile_index_y, WaterFlowDirection.S);
				if(position.x >= 0) {
					result.Add(position);
				}
				else {
					Vector2 neighboring_chunk_indexes = getChunkIndexesInDirection(chunk_index_x, chunk_index_y, WaterFlowDirection.S);
					GameController.gameController.terrainGenerator.setChunkPollution((int)neighboring_chunk_indexes.x, (int)neighboring_chunk_indexes.y, tile_index_x, 0, acidity, oxigenation, turbidity);
				}
			}

			position = getTileIndexesInDirection (tile_index_x, tile_index_y, WaterFlowDirection.SE);
			if(position.x >= 0) {
				result.Add(position);
			}

			break;
		case WaterFlowDirection.S:

			if(spreadOut) {
				position = getTileIndexesInDirection (tile_index_x, tile_index_y, WaterFlowDirection.SE);
				if(position.x >= 0) {
					result.Add(position);
				}

				position = getTileIndexesInDirection (tile_index_x, tile_index_y, WaterFlowDirection.SW);
				if(position.x >= 0) {
					result.Add(position);
				}
			}

			position = getTileIndexesInDirection (tile_index_x, tile_index_y, WaterFlowDirection.S);
			if(position.x >= 0) {
				result.Add(position);
			}
			else {
				Vector2 neighboring_chunk_indexes = getChunkIndexesInDirection(chunk_index_x, chunk_index_y, WaterFlowDirection.S);
				GameController.gameController.terrainGenerator.setChunkPollution((int)neighboring_chunk_indexes.x, (int)neighboring_chunk_indexes.y, tile_index_x, 0, acidity, oxigenation, turbidity);
			}

			break;
		case WaterFlowDirection.SW:

			if(spreadOut) {
				position = getTileIndexesInDirection (tile_index_x, tile_index_y, WaterFlowDirection.S);
				if(position.x >= 0) {
					result.Add(position);
				}
				else {
					Vector2 neighboring_chunk_indexes = getChunkIndexesInDirection(chunk_index_x, chunk_index_y, WaterFlowDirection.S);
					GameController.gameController.terrainGenerator.setChunkPollution((int)neighboring_chunk_indexes.x, (int)neighboring_chunk_indexes.y, tile_index_x, 0, acidity, oxigenation, turbidity);
				}

				position = getTileIndexesInDirection (tile_index_x, tile_index_y, WaterFlowDirection.W);
				if(position.x >= 0) {
					result.Add(position);
				}
				else {
					Vector2 neighboring_chunk_indexes = getChunkIndexesInDirection(chunk_index_x, chunk_index_y, WaterFlowDirection.W);
					GameController.gameController.terrainGenerator.setChunkPollution((int)neighboring_chunk_indexes.x, (int)neighboring_chunk_indexes.y, side_tile_count-1, tile_index_y, acidity, oxigenation, turbidity);
				}
			}

			position = getTileIndexesInDirection (tile_index_x, tile_index_y, WaterFlowDirection.SW);
			if(position.x >= 0) {
				result.Add(position);
			}

			break;
		case WaterFlowDirection.W:

			if(spreadOut) {
				position = getTileIndexesInDirection (tile_index_x, tile_index_y, WaterFlowDirection.SW);
				if(position.x >= 0) {
					result.Add(position);
				}

				position = getTileIndexesInDirection (tile_index_x, tile_index_y, WaterFlowDirection.NW);
				if(position.x >= 0) {
					result.Add(position);
				}
			}

			position = getTileIndexesInDirection (tile_index_x, tile_index_y, WaterFlowDirection.W);
			if(position.x >= 0) {
				result.Add(position);
			}
			else {
				Vector2 neighboring_chunk_indexes = getChunkIndexesInDirection(chunk_index_x, chunk_index_y, WaterFlowDirection.W);
				GameController.gameController.terrainGenerator.setChunkPollution((int)neighboring_chunk_indexes.x, (int)neighboring_chunk_indexes.y, side_tile_count-1, tile_index_y, acidity, oxigenation, turbidity);
			}

			break;
		case WaterFlowDirection.NW:

			if(spreadOut) {
				position = getTileIndexesInDirection (tile_index_x, tile_index_y, WaterFlowDirection.W);
				if(position.x >= 0) {
					result.Add(position);
				}
				else {
					Vector2 neighboring_chunk_indexes = getChunkIndexesInDirection(chunk_index_x, chunk_index_y, WaterFlowDirection.W);
					GameController.gameController.terrainGenerator.setChunkPollution((int)neighboring_chunk_indexes.x, (int)neighboring_chunk_indexes.y, side_tile_count-1, tile_index_y, acidity, oxigenation, turbidity);
				}

				position = getTileIndexesInDirection (tile_index_x, tile_index_y, WaterFlowDirection.N);
				if(position.x >= 0) {
					result.Add(position);
				}
				else {
					Vector2 neighboring_chunk_indexes = getChunkIndexesInDirection(chunk_index_x, chunk_index_y, WaterFlowDirection.N);
					GameController.gameController.terrainGenerator.setChunkPollution((int)neighboring_chunk_indexes.x, (int)neighboring_chunk_indexes.y, tile_index_x, side_tile_count-1, acidity, oxigenation, turbidity);
				}
			}

			position = getTileIndexesInDirection (tile_index_x, tile_index_y, WaterFlowDirection.NW);
			if(position.x >= 0) {
				result.Add(position);
			}

			break;
		}

		return result;

	}

	private bool tileIsPolluted(int tile_index_x, int tile_index_y) {
		if(pollution_map[tile_index_y, tile_index_x] > 0f) {
			return true;
		}
		return false;
	}

	private void setTilePollution(int tile_index_x, int tile_index_y, float acidity, float oxigenation, float turbidity, List<Vector3> pollution_changes) {
		acidity_map [tile_index_y, tile_index_x] = acidity;
		oxigenation_map [tile_index_y, tile_index_x] = oxigenation;
		turbidity_map [tile_index_y, tile_index_x] = turbidity;
		pollution_changes.Add (new Vector3(tile_index_x, tile_index_y, calculatePollution(acidity, oxigenation, turbidity)));
	}

	private Vector2 getChunkIndexesInDirection(int chunk_index_x, int chunk_index_y, WaterFlowDirection direction) {

		Vector2 result = new Vector2 (-1,-1);

		switch (direction) {
		case WaterFlowDirection.N:
			return new Vector2(chunk_index_x, chunk_index_y-1);
			break;
		case WaterFlowDirection.NE:
			return new Vector2(chunk_index_x+1, chunk_index_y-1);
			break;
		case WaterFlowDirection.E:
			return new Vector2(chunk_index_x+1, chunk_index_y);
			break;
		case WaterFlowDirection.SE:
			return new Vector2(chunk_index_x+1, chunk_index_y+1);
			break;
		case WaterFlowDirection.S:
			return new Vector2(chunk_index_x, chunk_index_y+1);
			break;
		case WaterFlowDirection.SW:
			return new Vector2(chunk_index_x-1, chunk_index_y+1);
			break;
		case WaterFlowDirection.W:
			return new Vector2(chunk_index_x-1, chunk_index_y);
			break;
		case WaterFlowDirection.NW:
			return new Vector2(chunk_index_x-1, chunk_index_y-1);
			break;
		}

		return result;

	}

	private Vector2 getTileIndexesInDirection(int tile_index_x, int tile_index_y, WaterFlowDirection direction) {
	
		Vector2 result = new Vector2 (-1,-1);

		switch(direction) {
		case WaterFlowDirection.N:
			if (tile_index_y > 0) {
				return new Vector2(tile_index_x, tile_index_y-1);
			}
			else {
				// get it from a neighboring chunk
			}
			break;
		case WaterFlowDirection.NE:
			if (tile_index_y > 0 && tile_index_x < side_tile_count-1) {
				return new Vector2(tile_index_x+1, tile_index_y-1);
			}
			else {
				// get it from a neighboring chunk
			}
			break;
		case WaterFlowDirection.E:
			if (tile_index_x < side_tile_count-1) {
				return new Vector2(tile_index_x+1, tile_index_y);
			}
			else {
				// get it from a neighboring chunk
			}
			break;
		case WaterFlowDirection.SE:
			if (tile_index_y < side_tile_count-1 && tile_index_x < side_tile_count-1) {
				return new Vector2(tile_index_x+1, tile_index_y+1);
			}
			else {
				// get it from a neighboring chunk
			}
			break;
		case WaterFlowDirection.S:
			if (tile_index_y < side_tile_count-1) {
				return new Vector2(tile_index_x, tile_index_y+1);
			}
			else {
				// get it from a neighboring chunk
			}
			break;
		case WaterFlowDirection.SW:
			if (tile_index_y < side_tile_count-1 && tile_index_x > 0) {
				return new Vector2(tile_index_x-1, tile_index_y+1);
			}
			else {
				// get it from a neighboring chunk
			}
			break;
		case WaterFlowDirection.W:
			if (tile_index_x > 0) {
				return new Vector2(tile_index_x-1, tile_index_y);
			}
			else {
				// get it from a neighboring chunk
			}
			break;
		case WaterFlowDirection.NW:
			if (tile_index_y > 0 && tile_index_x > 0) {
				return new Vector2(tile_index_x-1, tile_index_y-1);
			}
			else {
				// get it from a neighboring chunk
			}
			break;
		}

		return result;
	
	}

	public int[,] getSceneryMap() {
		return scenery_map;
	}

	public PathType[,] getPathMap() {
		return path_map;
	}

	public FlagType[,] getFlagMap() {
		return flag_map;
	}

	public float[,] getPollutionMap() {
		return pollution_map;
	}

	public int[,] getWHotspotMap() {
		return whotspot_map;
	}

	// render all scenery objects on this terrain chunk. Stores rendered scenery objects in sc_obj_pool
	public void renderScenery(int chunk_index_x, int chunk_index_y, ref Stack<GameObject> sc_obj_pool) {
		
		for (int i = 0; i < side_tile_count; ++i) {
			for(int j = 0; j < side_tile_count; ++j) {
				
				if(scenery_map[i, j] != GameController.gameController.spriteMapper.getEmpty ()) {
					
					if(sc_obj_pool.Count == 0) {
						Debug.LogError("Scenery object pool is empty.");
						return;
					}
					
					loadSceneryObject(chunk_index_x, chunk_index_y, j, i, ref sc_obj_pool);
					
				}
				
			}
		}
		
	}

	// unloads/unrenders all scenery objects on this chunk.
	public void unloadScenery(ref Stack<GameObject> sc_obj_pool) {
		
		while (reserved_sc_objs.Count > 0) {
			GameObject obj = reserved_sc_objs.Pop();
			if(obj.GetComponent<tk2dSprite>().Collection.name == "allCollection") {
				unloadSceneryObject(obj, ref sc_obj_pool);
			}
			else {
				//Debug.LogError("Not a scenery object. Collection: "+obj.GetComponent<tk2dSprite>().Collection.name);
			}

		}
		
	}

	// render all water hotspots objects on this terrain chunk. Stores rendered water hotspots objects in wh_obj_pool
	public void renderWHotspots(int chunk_index_x, int chunk_index_y, ref Stack<GameObject> wh_obj_pool) {
		
		for (int i = 0; i < side_tile_count; ++i) {
			for(int j = 0; j < side_tile_count; ++j) {
				
				if(whotspot_map[i, j] != 0) {
					
					if(wh_obj_pool.Count == 0) {
						Debug.LogError("WH object pool is empty.");
						return;
					}
					
					loadWHotspotObject(chunk_index_x, chunk_index_y, j, i, ref wh_obj_pool);
					
				}
				
			}
		}
		
	}

	// unloads/unrenders all water hotspots objects on this chunk.
	public void unloadWHotspots(ref Stack<GameObject> wh_obj_pool) {
		
		while (reserved_wh_objs.Count > 0) {
			GameObject obj = reserved_wh_objs.Pop();
			unloadWHotspotObject(obj, ref wh_obj_pool);
		}
		
	}

	// associates a tiles water hotspot map value as empty. i.e destroys a water hotspot object on this chunk
	public void destroyWHotspotObject(GameObject obj) {
		TileIndexedObject ti_obj = obj.GetComponent<TileIndexedObject> ();
		whotspot_map [ti_obj.tile_index_y, ti_obj.tile_index_x] = 0;
	}

	// loads/renders an individual water hotspot object on this chunk. recieves chunk indexes for positioning. stores rendered object in wh_obj_pool
	public void loadWHotspotObject(int chunk_index_x, int chunk_index_y, int wh_map_ind_x, int wh_map_ind_y, ref Stack<GameObject> wh_obj_pool) {
		
		GameObject newObj = wh_obj_pool.Pop ();

		newObj.SetActive (true);
		newObj.GetComponent<tk2dSprite> ().SetSprite (scenery_map [wh_map_ind_y, wh_map_ind_x]);
		positionWHotspotObj (ref newObj, wh_map_ind_x, wh_map_ind_y);
		newObj.GetComponent<TileIndexedObject> ().init (chunk_index_x, chunk_index_y, wh_map_ind_x, wh_map_ind_y);

		newObj.transform.Find ("SceneryCollider").GetComponent<ShowMenu> ().setShowMenu ();
		
		reserved_wh_objs.Push (newObj);
		
	}

	// unloads/unrenders an individual water hotspot object on this chunk.
	public void unloadWHotspotObject(GameObject obj, ref Stack<GameObject> wh_obj_pool) {

		if (checkPoolObjectCongruence (obj, wh_obj_pool)) {
			obj.SetActive(false);
			wh_obj_pool.Push(obj);
		} else {
			//error
			Debug.LogError("Invalid object type: "+obj.GetComponent<TileIndexedObject> ().type);
		}

	}

	public bool checkPoolObjectCongruence(GameObject obj, Stack<GameObject> obj_pool) {
		GameObject pool_obj = obj_pool.Peek ();
		if (pool_obj.GetComponent<TileIndexedObject> ().type == obj.GetComponent<TileIndexedObject> ().type) {
			return true;
		}
		return false;
	}

	// loads/renders an individual scenery object on this chunk. recieves chunk indexes for positioning. stores rendered object in sc_obj_pool
	public void loadSceneryObject(int chunk_index_x, int chunk_index_y, int scenery_map_ind_x, int scenery_map_ind_y, ref Stack<GameObject> sc_obj_pool) {
		
		GameObject newObj = sc_obj_pool.Pop();
		newObj.SetActive(true);
		newObj.GetComponent<tk2dSprite> ().SetSprite(scenery_map[scenery_map_ind_y, scenery_map_ind_x]);
		newObj.tag = GameController.gameController.spriteMapper.getTagFromSpriteId (scenery_map[scenery_map_ind_y, scenery_map_ind_x]);
		positionSceneryObj(ref newObj, scenery_map_ind_x, scenery_map_ind_y);
		newObj.GetComponent<TileIndexedObject> ().init (chunk_index_x, chunk_index_y, scenery_map_ind_x, scenery_map_ind_y);

		newObj.transform.Find("SceneryCollider").GetComponent<ShowMenu> ().setShowMenu();
		
		reserved_sc_objs.Push(newObj);
		
	}

	// unloads/unrenders an individual scenery object on this chunk.
	public void unloadSceneryObject(GameObject obj, ref Stack<GameObject> sc_obj_pool) {

		if (checkPoolObjectCongruence (obj, sc_obj_pool)) {
			obj.tag = "Untagged";
			obj.SetActive(false);
			sc_obj_pool.Push(obj);
		} else {
			//error
			Debug.LogError("Invalid object type: "+obj.GetComponent<TileIndexedObject> ().type);
		}

	}

	// associates a tiles scenery map value as empty. i.e destroys a scenery object on this chunk
	public void destroySceneryObject(GameObject obj) {
		TileIndexedObject ti_obj = obj.GetComponent<TileIndexedObject> ();
		scenery_map [ti_obj.tile_index_y, ti_obj.tile_index_x] = GameController.gameController.spriteMapper.getEmpty ();
	}

	private void positionObjectOnChunk(ref GameObject obj, int map_index_x, int map_index_y, out Vector3 position, out Vector3 collider_position) {

		float offset = 0f;
		
		position = new Vector3 (
			origin.x + map_index_x * tile_size + tile_size/2 + offset, 
			level_map [map_index_y, map_index_x] * slope_height + 0.01f, 
			origin.z - map_index_y * tile_size - tile_size/2 + offset
			);
		
		collider_position = new Vector3 (
			position.x-offset,
			position.y,
			position.z-offset
			);

	}

	private void positionObjectOnChunkOnWSurface(ref GameObject obj, int map_index_x, int map_index_y, out Vector3 position, out Vector3 collider_position) {
		
		float offset = 0f;
		
		position = new Vector3 (
			origin.x + map_index_x * tile_size + tile_size/2 + offset, 
			surface_level - 0.01f, 
			origin.z - map_index_y * tile_size - tile_size/2 + offset
			);
		
		collider_position = new Vector3 (
			position.x-offset,
			position.y,
			position.z-offset
			);
		
	}

	private void positionBridgeOnChunk(ref GameObject obj, int map_index_x, int map_index_y, out Vector3 position) {
		
		position = new Vector3 (
			origin.x + map_index_x * tile_size + tile_size/2, 
			surface_level + 0.03f, 
			origin.z - map_index_y * tile_size - tile_size/2
			);
	}

	// positions a scenery object on the terrain
	private void positionSceneryObj(ref GameObject obj, int map_index_x, int map_index_y) {
		Vector3 position = new Vector3 ();
		Vector3 collider_position = new Vector3 ();
		positionObjectOnChunk (ref obj, map_index_x, map_index_y, out position, out collider_position);
		obj.transform.position = position;
		obj.transform.Find ("SceneryCollider").gameObject.transform.position = collider_position;
	}

	// positions a water hotspot object on the terrain
	private void positionWHotspotObj(ref GameObject obj, int map_index_x, int map_index_y) {
		Vector3 position = new Vector3 ();
		Vector3 collider_position = new Vector3 ();
		positionObjectOnChunkOnWSurface (ref obj, map_index_x, map_index_y, out position, out collider_position);
		obj.transform.position = position;
		obj.transform.Find ("SceneryCollider").gameObject.transform.position = collider_position;
	}

	// positions a path object on the terrain
	private void positionPathObj(ref GameObject obj, int map_index_x, int map_index_y) {
		Vector3 position = new Vector3 ();
		Vector3 collider_position = new Vector3 ();

		if (path_map [map_index_y, map_index_x] != PathType.bridge) {
			positionObjectOnChunk (ref obj, map_index_x, map_index_y, out position, out collider_position);
		} else {
			positionBridgeOnChunk (ref obj, map_index_x, map_index_y, out position);
		}

		obj.transform.position = position;
	}

	// flag
	private void positionFlagObj(ref GameObject obj, int map_index_x, int map_index_y) {
		Vector3 position = new Vector3 ();
		Vector3 collider_position = new Vector3 ();
		
		positionObjectOnChunk (ref obj, map_index_x, map_index_y, out position, out collider_position);
		
		obj.transform.position = position;
	}



	// render all path objects on this terrain chunk. Stores rendered scenery objects in path_obj_pool
	public void renderPaths(int chunk_index_x, int chunk_index_y, ref Stack<GameObject> path_obj_pool) {
		
		for (int i = 0; i < side_tile_count; ++i) {
			for(int j = 0; j < side_tile_count; ++j) {
				
				if(path_map[i, j] != PathType.empty) {
					
					if(path_obj_pool.Count == 0) {
						Debug.LogError("Path object pool is empty.");
						return;
					}
					
					loadPathObject(chunk_index_x, chunk_index_y, j, i, ref path_obj_pool);
					
				}
				
			}
		}
		
	}

	// flag
	public void renderFlags(int chunk_index_x, int chunk_index_y, ref Stack<GameObject> flag_obj_pool) {
		
		for (int i = 0; i < side_tile_count; ++i) {
			for(int j = 0; j < side_tile_count; ++j) {
				
				if(flag_map[i, j] != FlagType.empty) {
					
					if(flag_obj_pool.Count == 0) {
						Debug.LogError("Flag object pool is empty.");
						return;
					}
					
					loadFlagObject(chunk_index_x, chunk_index_y, j, i, ref flag_obj_pool);
					
				}
				
			}
		}
		
	}

	// unloads/unrenders all path objects on this chunk.
	public void unloadPaths(ref Stack<GameObject> path_obj_pool) {
		
		while (reserved_path_objs.Count > 0) {
			GameObject obj = reserved_path_objs.Pop();
			unloadPathObject(obj, ref path_obj_pool);
		}
		
	}

	// flag 
	public void unloadFlags(ref Stack<GameObject> flag_obj_pool) {
		
		while (reserved_flag_objs.Count > 0) {
			GameObject obj = reserved_flag_objs.Pop();
			unloadFlagObject(obj, ref flag_obj_pool);
		}
		
	}

	// loads/renders an individual path object on this chunk. recieves chunk indexes for positioning. stores rendered object in path_obj_pool
	public void loadPathObject(int chunk_index_x, int chunk_index_y, int path_map_ind_x, int path_map_ind_y, ref Stack<GameObject> path_obj_pool) {
		
		GameObject newObj = path_obj_pool.Pop();
		newObj.SetActive(true);
		newObj.GetComponent<tk2dSprite> ().SetSprite( ((int) path_map[path_map_ind_y, path_map_ind_x]) - 1 );
		positionPathObj(ref newObj, path_map_ind_x, path_map_ind_y);
		newObj.GetComponent<TileIndexedObject> ().init (chunk_index_x, chunk_index_y, path_map_ind_x, path_map_ind_y);

		reserved_path_objs.Push(newObj);
		
	}

	// flag
	public void loadFlagObject(int chunk_index_x, int chunk_index_y, int flag_map_ind_x, int flag_map_ind_y, ref Stack<GameObject> flag_obj_pool) {
		
		GameObject newObj = flag_obj_pool.Pop();
		newObj.SetActive(true);
		newObj.GetComponent<tk2dSprite> ().SetSprite( ((int) flag_map[flag_map_ind_y, flag_map_ind_x]) - 1 );
		positionFlagObj(ref newObj, flag_map_ind_x, flag_map_ind_y);
		newObj.GetComponent<TileIndexedObject> ().init (chunk_index_x, chunk_index_y, flag_map_ind_x, flag_map_ind_y);
		
		reserved_flag_objs.Push(newObj);
		
	}

	// unloads/unrenders an individual path object on this chunk.
	public void unloadPathObject(GameObject obj, ref Stack<GameObject> path_obj_pool) {
		if (checkPoolObjectCongruence (obj, path_obj_pool)) {
			obj.SetActive (false);
			path_obj_pool.Push (obj);
		} else {
			//error
		}
	}

	//flag
	public void unloadFlagObject(GameObject obj, ref Stack<GameObject> flag_obj_pool) {
		if (checkPoolObjectCongruence (obj, flag_obj_pool)) {
			obj.SetActive (false);
			flag_obj_pool.Push (obj);
		} else {
			//error
		}
	}

	// sets all triangles of a submesh. The terrain mesh and the water surface mesh are both submeshes.
	private void setSubmeshTriangles(ref Mesh mesh, ref List<int>[] triList_bySubmeshIndex, int submesh_index = 0) {
		if (triList_bySubmeshIndex [submesh_index].Count == 0) {
			// no triangles flowing to direction in this surface chunk
			int[] no_tris = new int[] {0, 0, 0};
			mesh.SetTriangles (no_tris, submesh_index);
		} 
		else {
			mesh.SetTriangles(triList_bySubmeshIndex[submesh_index].ToArray(), submesh_index);
		}
	}
	
	// set positions of vertices in terrain tile with index tile_index
	private void setTileVertices(int tile_index, Vector3 vert0, Vector3 vert1, Vector3 vert2, Vector3 vert3) {
		vertices[4*tile_index + (int)TileVertices.topLeft] 	  		= vert0;
		vertices[4*tile_index + (int)TileVertices.topRight] 	  	= vert1;
		vertices[4*tile_index + (int)TileVertices.bottomRight] 		= vert2;
		vertices[4*tile_index + (int)TileVertices.bottomLeft]  		= vert3;
	}
	
	// set positions of vertices in water surface tile with index tile_index
	private void setWaterSurfaceVertices(int tile_index, Vector3 vert0, Vector3 vert1, Vector3 vert2, Vector3 vert3) {
		ws_vertices[4*tile_index + (int)TileVertices.topLeft] 	  		= vert0;
		ws_vertices[4*tile_index + (int)TileVertices.topRight] 	  		= vert1;
		ws_vertices[4*tile_index + (int)TileVertices.bottomRight] 		= vert2;
		ws_vertices[4*tile_index + (int)TileVertices.bottomLeft]  		= vert3;
	}

	// set positions of vertices in pollution tile with index tile_index
	private void setPollutionVertices(int tile_index, Vector3 vert0, Vector3 vert1, Vector3 vert2, Vector3 vert3) {
		pollution_vertices[4*tile_index + (int)TileVertices.topLeft] 	  		= vert0;
		pollution_vertices[4*tile_index + (int)TileVertices.topRight] 	  		= vert1;
		pollution_vertices[4*tile_index + (int)TileVertices.bottomRight] 		= vert2;
		pollution_vertices[4*tile_index + (int)TileVertices.bottomLeft]  		= vert3;
	}
	
	// set triangles in terrain tile with index tile_index, according to height level in level map entry with indexes lmap_index_x and lmap_index_y
	private void setTileTriangles(int lmap_index_x, int lmap_index_y, int tile_index) {
		
		if(tri_div_conf_map[lmap_index_y, lmap_index_x] == TileDivisionConfiguration.topLeft_to_bottomRight) {
			// triangle 0
			triangles[6*tile_index + 0] = 4*tile_index + (int)TileVertices.topLeft;
			triangles[6*tile_index + 1] = 4*tile_index + (int)TileVertices.bottomRight;
			triangles[6*tile_index + 2] = 4*tile_index + (int)TileVertices.bottomLeft;
			
			// triangle 1
			triangles[6*tile_index + 3] = 4*tile_index + (int)TileVertices.topLeft;
			triangles[6*tile_index + 4] = 4*tile_index + (int)TileVertices.topRight;
			triangles[6*tile_index + 5] = 4*tile_index + (int)TileVertices.bottomRight;
		}
		else {
			// triangle 0
			triangles[6*tile_index + 0] = 4*tile_index + (int)TileVertices.topLeft;
			triangles[6*tile_index + 1] = 4*tile_index + (int)TileVertices.topRight;
			triangles[6*tile_index + 2] = 4*tile_index + (int)TileVertices.bottomLeft;
			
			// triangle 1
			triangles[6*tile_index + 3] = 4*tile_index + (int)TileVertices.topRight;
			triangles[6*tile_index + 4] = 4*tile_index + (int)TileVertices.bottomRight;
			triangles[6*tile_index + 5] = 4*tile_index + (int)TileVertices.bottomLeft;
		}
		
	}
	
	// set triangles in terrain tile with index tile_index, according to height level in level map entry with indexes lmap_index_x and lmap_index_y
	private void setWaterSurfaceTriangles(int lmap_index_x, int lmap_index_y, int tile_index) {
		
		if(tri_div_conf_map[lmap_index_y, lmap_index_x] == TileDivisionConfiguration.topLeft_to_bottomRight) {
			// triangle 0
			ws_triangles[6*tile_index + 0] = 4*tile_index + (int)TileVertices.topLeft;
			ws_triangles[6*tile_index + 1] = 4*tile_index + (int)TileVertices.bottomRight;
			ws_triangles[6*tile_index + 2] = 4*tile_index + (int)TileVertices.bottomLeft;
			
			// triangle 1
			ws_triangles[6*tile_index + 3] = 4*tile_index + (int)TileVertices.topLeft;
			ws_triangles[6*tile_index + 4] = 4*tile_index + (int)TileVertices.topRight;
			ws_triangles[6*tile_index + 5] = 4*tile_index + (int)TileVertices.bottomRight;
		}
		else {
			// triangle 0
			ws_triangles[6*tile_index + 0] = 4*tile_index + (int)TileVertices.topLeft;
			ws_triangles[6*tile_index + 1] = 4*tile_index + (int)TileVertices.topRight;
			ws_triangles[6*tile_index + 2] = 4*tile_index + (int)TileVertices.bottomLeft;
			
			// triangle 1
			ws_triangles[6*tile_index + 3] = 4*tile_index + (int)TileVertices.topRight;
			ws_triangles[6*tile_index + 4] = 4*tile_index + (int)TileVertices.bottomRight;
			ws_triangles[6*tile_index + 5] = 4*tile_index + (int)TileVertices.bottomLeft;
		}
		
	}

	// set triangles in terrain tile with index tile_index, according to height level in level map entry with indexes lmap_index_x and lmap_index_y
	private void setPollutionTriangles(int lmap_index_x, int lmap_index_y, int tile_index) {
		
		if(tri_div_conf_map[lmap_index_y, lmap_index_x] == TileDivisionConfiguration.topLeft_to_bottomRight) {
			// triangle 0
			pollution_triangles[6*tile_index + 0] = 4*tile_index + (int)TileVertices.topLeft;
			pollution_triangles[6*tile_index + 1] = 4*tile_index + (int)TileVertices.bottomRight;
			pollution_triangles[6*tile_index + 2] = 4*tile_index + (int)TileVertices.bottomLeft;
			
			// triangle 1
			pollution_triangles[6*tile_index + 3] = 4*tile_index + (int)TileVertices.topLeft;
			pollution_triangles[6*tile_index + 4] = 4*tile_index + (int)TileVertices.topRight;
			pollution_triangles[6*tile_index + 5] = 4*tile_index + (int)TileVertices.bottomRight;
		}
		else {
			// triangle 0
			pollution_triangles[6*tile_index + 0] = 4*tile_index + (int)TileVertices.topLeft;
			pollution_triangles[6*tile_index + 1] = 4*tile_index + (int)TileVertices.topRight;
			pollution_triangles[6*tile_index + 2] = 4*tile_index + (int)TileVertices.bottomLeft;
			
			// triangle 1
			pollution_triangles[6*tile_index + 3] = 4*tile_index + (int)TileVertices.topRight;
			pollution_triangles[6*tile_index + 4] = 4*tile_index + (int)TileVertices.bottomRight;
			pollution_triangles[6*tile_index + 5] = 4*tile_index + (int)TileVertices.bottomLeft;
		}
		
	}

	// gets a Vector2 which has its values set as follows:
	//		x: first row in texture image that is of type t_type
	//		y: last row in texture image that is of type t_type
	private Vector2 getTextureRowIndexes(TextureType t_type) {
		
		Vector2 indexes = Vector2.zero;
		switch(t_type) {
			case TextureType.grass_blue:
				indexes = new Vector2(0, 1);
				break;

			case TextureType.grass_green:
				indexes = new Vector2(2, 3);
				break;

			case TextureType.grass_yellow:
				indexes = new Vector2(4, 5);
				break;
				
			case TextureType.dirt:
				indexes = new Vector2(6, 7);
				break;

			case TextureType.rock:
				indexes = new Vector2(8, 9);
				break;

			case TextureType.sand:
				indexes = new Vector2(10, 11);
				break;
		}

		return indexes;

	}
	
	private void setTextureCoords() {
		texture_coords = new Vector2[num_textures_y, num_textures_x, 4];
		float offset_x = 1f / num_textures_x;
		float offset_y = 1f / num_textures_y;
		
		for (int i = 0; i < num_textures_y; ++i) {
			for(int j = 0; j < num_textures_x; ++j) {
				texture_coords[i, j, (int) TileVertices.topLeft] 		= new Vector2(j * offset_x, 			i * offset_y);
				texture_coords[i, j, (int) TileVertices.topRight] 		= new Vector2(j * offset_x + offset_x, 	i * offset_y);
				texture_coords[i, j, (int) TileVertices.bottomRight] 	= new Vector2(j * offset_x + offset_x, 	i * offset_y + offset_y);
				texture_coords[i, j, (int) TileVertices.bottomLeft] 	= new Vector2(j * offset_x, 			i * offset_y + offset_y);
				//Debug.Log (texture_coords[i, j, (int) TileVertices.topLeft] + ", " + texture_coords[i, j, (int) TileVertices.topRight] + ", " + texture_coords[i, j, (int) TileVertices.bottomRight] + ", " + texture_coords[i, j, (int) TileVertices.bottomLeft]);
			}
		}
		
	}

	// get terrain mesh
	public Mesh getMesh() {
		
		if (!isValid) {
			Debug.LogError("Mesh not set. Invalid TerrainChunk.");
			return new Mesh();		
		}
		
		return mesh;
		
	}

	// get water surface mesh
	public Mesh getWSMesh() {
		
		if (!isValid) {
			Debug.LogError("Mesh not set. Invalid TerrainChunk.");
			return new Mesh();		
		}
		
		return water_surface_mesh;
		
	}

	// get pollution mesh
	public Mesh getPollutionMesh() {
		
		if (!isValid) {
			Debug.LogError("Mesh not set. Invalid TerrainChunk.");
			return new Mesh();		
		}
		
		return pollution_mesh;
		
	}

	// sets brightness levels for both triangles in every tile
	private TileBrightness[,,] setBrightnessMap(int[,] level_map) {
		
		TileBrightness[,,] result = new TileBrightness[side_tile_count, side_tile_count, 2];
		
		for (int i = 0; i < side_tile_count; ++i) {
			for (int j = 0; j < side_tile_count; ++j) {
				TileBrightness[] tmp = getTileBrightness(j, i);
				result[i, j, 0] = tmp[0];
				result[i, j, 1] = tmp[1];
			}		
		}
		
		return result;
		
	}

	// sets flow directions for every tile
	private WaterFlowDirection[,] setFlowMap() {
		
		WaterFlowDirection[,] result = new WaterFlowDirection[side_tile_count, side_tile_count];
		List<Vector2> flowing_tiles = new List<Vector2> ();


		for (int i = 0; i < side_tile_count*side_tile_count; ++i) {
			if(level_map[(int)i/side_tile_count, i%side_tile_count] < surface_level) {
				flowing_tiles.Add (new Vector2(i%side_tile_count, (int)i/side_tile_count));
				break;
			}
		}



		// remove later
		/*for (int i = 0; i < side_tile_count; ++i) {
			for (int j = 0; j < side_tile_count; ++j) {
				result[i, j] = (WaterFlowDirection) UnityEngine.Random.Range(0, 8);
				if(i > j) {
					result[i, j] = WaterFlowDirection.N;
				}
				else {
					result[i, j] = WaterFlowDirection.NE;
				}
			}		
		}*/
		return result;
		
	}



	private WaterFlowDirection getFlowDirection(int origin_x, int origin_z, int destination_x, int destination_z) { 

		WaterFlowDirection result = WaterFlowDirection.N;

		if(destination_x > origin_x) {
			result = WaterFlowDirection.E;
		}

		if(destination_z > origin_z) {
			result = WaterFlowDirection.N;
		}

		if(origin_x == destination_x && origin_z == destination_z) {
			result = WaterFlowDirection.SW;
		}
		return result;

	}

	private int[,] initSceneryMap() {
		int[,] result = new int[side_tile_count, side_tile_count];
		for (int i = 0; i < side_tile_count*side_tile_count; ++i) {
			result[(int)i/side_tile_count, i%side_tile_count] = GameController.gameController.spriteMapper.getEmpty ();
		}
		return result;
	}

	// sets a scenery type for every tile
	private int[,] setSceneryMap() {
		
		reserved_sc_objs = new Stack<GameObject> ();
		int[,] result = initSceneryMap ();
		int scenery_instances = Mathf.FloorToInt(side_tile_count * side_tile_count * scenery_density);

		for (int i = 0; i < scenery_instances; ++i) {
			
			int rand_index_x = UnityEngine.Random.Range(0, side_tile_count);
			int rand_index_y = UnityEngine.Random.Range(0, side_tile_count);
			
			if(isSuitableForScenery(rand_index_x, rand_index_y) && (result[rand_index_y, rand_index_x] == GameController.gameController.spriteMapper.getEmpty ())){ // && noHouses(rand_index_y, rand_index_x)) {
				int sprite_id = GameController.gameController.spriteMapper.getRandomVegetation(tile_type_map[rand_index_y, rand_index_x]);
				if(sprite_id >= 0) {
					result[rand_index_y, rand_index_x] = sprite_id;
				}
			}
			
		}
		return result;
		
	}

	private PathType[,] setPathMap() {

		reserved_path_objs = new Stack<GameObject> ();
		PathType[,] result = new PathType[side_tile_count, side_tile_count];
		return result;

	}
	// flag
	private FlagType[,] setFlagMap() {
		
		reserved_flag_objs = new Stack<GameObject> ();
		FlagType[,] result = new FlagType[side_tile_count, side_tile_count];

		// zona inicial: start
		if (chunk_index_x == 5 && chunk_index_y == 5) {
			result [18, 2] = FlagType.checkpoint;
		}
		// zona inicial: end   zona mision arboles: start
		if (chunk_index_x == 3 && chunk_index_y == 8) {
			result [3, 8] = FlagType.checkpoint;
		}
		// zona mision arboles: end   zona analisis start
		if (chunk_index_x == 4 && chunk_index_y == 9) {
			result [13, 8] = FlagType.checkpoint;
		}
		// zona mision analisis: end zona minijuego 1: start
		if (chunk_index_x == 5 && chunk_index_y == 10) {
			result [10, 1] = FlagType.checkpoint;
		}
		// zona mision minijuego 1: end y analisis 2 start
		if (chunk_index_x == 4 && chunk_index_y == 13) {
			result [10, 1] = FlagType.checkpoint;
		}
		// zon mision analisis 2: end y mision minijuego 2 start: 
		if (chunk_index_x == 3 && chunk_index_y == 12) {
			result [20, 1] = FlagType.checkpoint;
		}
		// zona minijuego 2: end y zona final: start
		if (chunk_index_x == 1 && chunk_index_y == 12) {
			result [13, 9] = FlagType.checkpoint;
		}
		// zona final:
		if (chunk_index_x == 0 && chunk_index_y == 14) {
			result [2, 22] = FlagType.checkpoint;
		}


		return result;
		
	}

	private float calculatePollution(float acidity, float oxigenation, float turbidity) {
		return acidity + oxigenation + turbidity;
	}

	private float calculateTilePollution(int tile_index_x, int tile_index_y) {
		return calculatePollution(acidity_map[tile_index_y, tile_index_x], oxigenation_map[tile_index_y, tile_index_x], turbidity_map[tile_index_y, tile_index_x]);
	}

	private float[,] setPollutionMap() {

		activePollutionTiles = new List<Vector2> ();
		float[,] result = new float[side_tile_count, side_tile_count];
		acidity_map = new float[side_tile_count, side_tile_count];
		oxigenation_map = new float[side_tile_count, side_tile_count];
		turbidity_map = new float[side_tile_count, side_tile_count];

		/*for (int i = 0; i < side_tile_count; ++i) {
			for(int j = 0; j < side_tile_count; ++j){
				result[i, j] = 1f;
			}
		}*/

		if (chunk_index_x == 9 && chunk_index_y == 6) {
			acidity_map [21, 17] = 1f;
			oxigenation_map [21, 17] = 1f;
			turbidity_map [21, 17] = 1f;
			result [21, 17] = calculateTilePollution(17, 21);
		}

		if (chunk_index_x == 1 && chunk_index_y == 14) {
			acidity_map [18, 1] = 1f;
			oxigenation_map [18, 1] = 1f;
			turbidity_map [18, 1] = 1f;
			result [18, 1] = calculateTilePollution(1, 18);
		}

		if (chunk_index_x == 9 && chunk_index_y == 0) {
			acidity_map [22, 20] = 1f;
			oxigenation_map [22, 20] = 1f;
			turbidity_map [22, 20] = 1f;
			result [22, 20] = calculateTilePollution(20, 22);
		}

		pollutionChanged = true;

		return result;
		
	}

	private int[,] setWHotspotMap() {

		reserved_wh_objs = new Stack<GameObject> ();
		int[,] result = new int[side_tile_count, side_tile_count];
		
		for (int i = 0; i < side_tile_count; ++i) {
			for(int j = 0; j < side_tile_count; ++j) {
				if(tileIsOnWaterEdge(j, i)) {
					if(UnityEngine.Random.Range (0, 100) < whotspot_density * 100) {
						result[i, j] = 1;
					}
				}
			}
		}
		
		return result;
		
	}

	private void addPollutionToTile(int index_x, int index_y, float pollution) {
		if (pollution_map [index_y, index_x] >= pollution) {
			return;
		}
		pollution_map [index_y, index_x] = pollution;
		activePollutionTiles.Add (new Vector2(index_x, index_y));
		addToPollutionMesh(index_x, index_y);
		refreshPollutionMesh ();
	}

	private bool tileIsOnWaterEdge(int tile_index_x, int tile_index_y) {

		int[] vert_levels = new int[4];
		vert_levels[(int)TileVertices.topLeft] = getMax( getVertexLevels( tile_index_x, tile_index_y, TileVertices.topLeft ) );
		vert_levels[(int)TileVertices.topRight] = getMax( getVertexLevels( tile_index_x, tile_index_y, TileVertices.topRight ) );
		vert_levels[(int)TileVertices.bottomRight] = getMax( getVertexLevels( tile_index_x, tile_index_y, TileVertices.bottomRight ) );
		vert_levels[(int)TileVertices.bottomLeft] = getMax( getVertexLevels( tile_index_x, tile_index_y, TileVertices.bottomLeft ) );

		int max_vert = getMax (vert_levels);
		int min_vert = getMin (vert_levels);

		if (min_vert < surface_level && max_vert >= surface_level) {
			return true;
		}

		return false;

	}
	
	// deseable: que se bajen un poco mas de profundidad. 
	// que no caminen los bichitos sobre ellos. 
	private int[,] setSceneryMapRural() { 
		
		reserved_sc_objs = new Stack<GameObject> ();
		int[,] result = initSceneryMap ();
		// lugar dentro de todo el chunk (tile especifico para posicionar la zona rural)
		result[15,10] = GameController.gameController.spriteMapper.getSpriteIdByName(GameController.gameController.spriteMapper.zona_rural_2);
		result[6,9] = GameController.gameController.spriteMapper.getSpriteIdByName(GameController.gameController.spriteMapper.zonaruralsingranjas);
		result[3,4] = GameController.gameController.spriteMapper.getSpriteIdByName(GameController.gameController.spriteMapper.molino);
		result[12,9] = GameController.gameController.spriteMapper.getSpriteIdByName(GameController.gameController.spriteMapper.casa_1);
		
		result [6, 14] = GameController.gameController.spriteMapper.getSpriteIdByName(GameController.gameController.spriteMapper.granjita_1); //zonaruralsingranjas;
		result[15,12] = GameController.gameController.spriteMapper.getSpriteIdByName(GameController.gameController.spriteMapper.molino);
		result[2,15] = GameController.gameController.spriteMapper.getSpriteIdByName(GameController.gameController.spriteMapper.molino);
		result[13,19] = GameController.gameController.spriteMapper.getSpriteIdByName(GameController.gameController.spriteMapper.granjita_1);
		result[15,22] = GameController.gameController.spriteMapper.getSpriteIdByName(GameController.gameController.spriteMapper.casa_1);
		result[10,18] = GameController.gameController.spriteMapper.getSpriteIdByName(GameController.gameController.spriteMapper.casa_1);
		result[2,22] = GameController.gameController.spriteMapper.getSpriteIdByName(GameController.gameController.spriteMapper.zonaruralsingranjas);
		return result;
	}
	
	private int[,] setSceneryMapCity() { 
		
		reserved_sc_objs = new Stack<GameObject> ();
		int[,] result = initSceneryMap();
		// lugar dentro de todo el chunk (tile especifico para posicionar la zona rural)
		result[11,11] = GameController.gameController.spriteMapper.getSpriteIdByName(GameController.gameController.spriteMapper.alcatraz);
		result[1,11] = GameController.gameController.spriteMapper.getSpriteIdByName(GameController.gameController.spriteMapper.zonaciudad);
		result [3,20] = GameController.gameController.spriteMapper.getSpriteIdByName(GameController.gameController.spriteMapper.zonaciudad3);
		result[18,15] = GameController.gameController.spriteMapper.getSpriteIdByName(GameController.gameController.spriteMapper.bigHouse1);
		result[18,18] = GameController.gameController.spriteMapper.getSpriteIdByName(GameController.gameController.spriteMapper.vereda);
		result[13,15] = GameController.gameController.spriteMapper.getSpriteIdByName(GameController.gameController.spriteMapper.treePink);
		result[15,20] = GameController.gameController.spriteMapper.getSpriteIdByName(GameController.gameController.spriteMapper.molino);
		result[11,22] = GameController.gameController.spriteMapper.getSpriteIdByName(GameController.gameController.spriteMapper.molino);
		return result;
	}

	private int[,] setSceneryMapTree() {
		reserved_sc_objs = new Stack<GameObject> ();
		int[,] result = initSceneryMap();
		
		for (int i=1; i<24; i++) {
			for(int j=0; j<8; ++j){
				int k = UnityEngine.Random.Range(1,24);
				if(isSuitableForScenery(k, i)) {
					result [i, k] = GameController.gameController.spriteMapper.getSpriteIdByName (setSprite(k));		
				}
			}
		}

		// Despeja un poco para que se vea la bandera
		for (int i = 1; i < 3; ++i) {
			for (int j = 7; j < 10; ++j) {
				result [i,j] = GameController.gameController.spriteMapper.getEmpty();
			}
		}
		
		return result;
	}

	private int[,] setSceneryMapTreeEnd(){
		reserved_sc_objs = new Stack<GameObject> ();
		int[,] result = initSceneryMap();
		
		for (int i=1; i<15; i++) {
			for(int j=0; j<3; ++j){
				int k = UnityEngine.Random.Range(1,10);
				result [i, k] = GameController.gameController.spriteMapper.getSpriteIdByName (setSprite(k));
			}
		}
		
		return result;
	}
	
	private String setSprite(int k){
		String sprite = "";
		if (k % 2 == 0) {
			sprite = GameController.gameController.spriteMapper.medium_tree_2;	
		} else if (k % 3 == 0) {
			sprite = GameController.gameController.spriteMapper.tropical_tree_5;
		} else if (k % 5 == 0) {
			sprite = GameController.gameController.spriteMapper.tropical_tree_4;
		} else {
			sprite = GameController.gameController.spriteMapper.rock_1;
		}
		
		return sprite;
	}

	// checks if a tile is suitable for scenery placement
	public bool isSuitableForScenery(int lmap_index_x, int lmap_index_y) {
		
		if(tileIsFlat(lmap_index_x, lmap_index_y) && !tileHeightIsLowerThanMinVert(lmap_index_x, lmap_index_y) && level_map[lmap_index_y, lmap_index_x] >= surface_level) {
			return true;
		}
		
		return false;
		
	}

	// checks if a tile is suitable for bridge placement
	public bool isSuitableForBridge(int lmap_index_x, int lmap_index_y) {
		
		if( level_map[lmap_index_y, lmap_index_x] < surface_level) {
			return true;
		}
		
		return false;
		
	}

	// checks if tile is flat (all vertices have same height)
	private bool tileIsFlat(int lmap_index_x, int lmap_index_y) {
		
		if (getHighestVertLevel (lmap_index_x, lmap_index_y) == getLowestVertLevel (lmap_index_x, lmap_index_y)) {
			return true;
		}
		
		return false;
		
	}
	
	// checks if a tiles height is lower than its lowest vertex. this can happen if all surrounding tiles are of greater height
	private bool tileHeightIsLowerThanMinVert(int lmap_index_x, int lmap_index_y) {
		
		if (getLowestVertLevel (lmap_index_x, lmap_index_y) > level_map [lmap_index_y, lmap_index_x]) {
			return true;
		}
		
		return false;
		
	}
	
	// get height of highest vertex in tile 
	private int getHighestVertLevel(int lmap_index_x, int lmap_index_y) {
		
		int[] vert_levels = new int[4];
		vert_levels[0] = getMax( getVertexLevels( lmap_index_x, lmap_index_y, TileVertices.topLeft ) );
		vert_levels[1] = getMax( getVertexLevels( lmap_index_x, lmap_index_y, TileVertices.topRight ) );
		vert_levels[2] = getMax( getVertexLevels( lmap_index_x, lmap_index_y, TileVertices.bottomRight ) );
		vert_levels[3] = getMax( getVertexLevels( lmap_index_x, lmap_index_y, TileVertices.bottomLeft ) );
		
		int max = -9999999;
		for (int i = 0; i < 4; ++i) {
			if (vert_levels[i] > max) {
				max = vert_levels[i];
			}
		}
		
		return max;
		
	}
	
	// get height of lowest vertex in tile
	private int getLowestVertLevel(int lmap_index_x, int lmap_index_y) {
		
		int[] vert_levels = new int[4];
		vert_levels[0] = getMax( getVertexLevels( lmap_index_x, lmap_index_y, TileVertices.topLeft ) );
		vert_levels[1] = getMax( getVertexLevels( lmap_index_x, lmap_index_y, TileVertices.topRight ) );
		vert_levels[2] = getMax( getVertexLevels( lmap_index_x, lmap_index_y, TileVertices.bottomRight ) );
		vert_levels[3] = getMax( getVertexLevels( lmap_index_x, lmap_index_y, TileVertices.bottomLeft ) );
		
		int min = 9999999;
		for (int i = 0; i < 4; ++i) {
			if (vert_levels[i] < min) {
				min = vert_levels[i];
			}
		}
		
		return min;
		
	}
	
	// set triangle division configuration for every tile in terrain chunk
	private TileDivisionConfiguration[,] setTriDivConfMap(int[,] level_map) {
		
		TileDivisionConfiguration[,] result = new TileDivisionConfiguration[side_tile_count, side_tile_count];
		
		for (int i = 0; i < side_tile_count; ++i) {
			for (int j = 0; j < side_tile_count; ++j) {
				
				if(tileHasInvertedTriangles(j, i)) {
					result[i, j] = TileDivisionConfiguration.bottomLeft_to_topRight;
				}
				else {
					// default configuration
					result[i, j] = TileDivisionConfiguration.topLeft_to_bottomRight;
				}
				
			}		
		}
		
		return result;
		
	}
	
	// get brightness level of both triangles in a tile. 
	//		returned value: sub-element 0 is brightness of triangle 0. sub-element 1 is brightness of triangle 1.
	private TileBrightness[] getTileBrightness(int tile_index_x, int tile_index_y) {
		
		TileBrightness[] result = new TileBrightness[2] { TileBrightness.normal, TileBrightness.normal };
		
		/*
			Triangle indexing
			
				+---------+				+---------+
				|`-.   1  |				|  0   .-`|
				|   `-.   |				|   .-`   |
				|  0   `-.|				|.-`  1   |
				+---------+				+---------+

		*/
		
		int tl_vert_level = getMax( getVertexLevels( tile_index_x, tile_index_y, TileVertices.topLeft ) );
		int tr_vert_level = getMax( getVertexLevels( tile_index_x, tile_index_y, TileVertices.topRight ) );
		int br_vert_level = getMax( getVertexLevels( tile_index_x, tile_index_y, TileVertices.bottomRight ) );
		int bl_vert_level = getMax( getVertexLevels( tile_index_x, tile_index_y, TileVertices.bottomLeft ) );
		
		int[] tri0 = new int[3];
		int[] tri1 = new int[3];
		Vector3 tri0_normal = Vector3.zero;
		Vector3 tri1_normal = Vector3.zero;
		TileDivisionConfiguration tri_conf = tri_div_conf_map [tile_index_y, tile_index_x];
		if (tri_conf == TileDivisionConfiguration.topLeft_to_bottomRight) {
			tri0[0] = tl_vert_level;
			tri0[1] = br_vert_level;
			tri0[2] = bl_vert_level;
			
			tri1[0] = tl_vert_level;
			tri1[1] = tr_vert_level;
			tri1[2] = br_vert_level;
			
			tri0_normal = GetNormal(new Vector3(0,tl_vert_level,0), new Vector3(1,br_vert_level,1), new Vector3(0,bl_vert_level,1));
			tri1_normal = GetNormal(new Vector3(0,tl_vert_level,0), new Vector3(1,tr_vert_level,0), new Vector3(1,br_vert_level,1));
		} 
		else {
			tri0[0] = tl_vert_level;	
			tri0[1] = tr_vert_level;
			tri0[2] = bl_vert_level;
			
			tri1[0] = tr_vert_level;
			tri1[1] = br_vert_level;
			tri1[2] = bl_vert_level;
			
			tri0_normal = GetNormal(new Vector3(0,tl_vert_level,0), new Vector3(1,tr_vert_level,0), new Vector3(0,bl_vert_level,1));
			tri1_normal = GetNormal(new Vector3(1,tr_vert_level,0), new Vector3(1,br_vert_level,1), new Vector3(0,bl_vert_level,1));
		}
		
		if (tri0 [0] != tri0 [1] || tri0 [0] != tri0 [2]) {			// triangle 0 is sloped
			float tri0_angle_diff = Vector3.Angle (tri0_normal, this.light_direction);
			
			if (tri0_angle_diff < 90) {
				result[0] = TileBrightness.light;
			} 
			else {
				result[0] = TileBrightness.dark;
			}
		}
		
		if (tri1 [0] != tri1 [1] || tri1 [0] != tri1 [2]) {			// triangle 1 is sloped
			float tri1_angle_diff = Vector3.Angle (tri1_normal, this.light_direction);
			
			if (tri1_angle_diff < 90) {
				result[1] = TileBrightness.light;
			} 
			else {
				result[1] = TileBrightness.dark;
			}
		}
		
		return result;
		
	}
	
	// check if tile triangle division configuration is not the default configuration (top-left to bottom-right)
	private bool tileHasInvertedTriangles(int tile_index_x, int tile_index_y) {
		
		bool invertTriangles = false;
		
		int tl_vert_level = getMax( getVertexLevels( tile_index_x, tile_index_y, TileVertices.topLeft ) );
		int tr_vert_level = getMax( getVertexLevels( tile_index_x, tile_index_y, TileVertices.topRight ) );
		int br_vert_level = getMax( getVertexLevels( tile_index_x, tile_index_y, TileVertices.bottomRight ) );
		int bl_vert_level = getMax( getVertexLevels( tile_index_x, tile_index_y, TileVertices.bottomLeft ) );
		
		if( br_vert_level == bl_vert_level && br_vert_level == tr_vert_level && br_vert_level != tl_vert_level ) {
			/*
				1--0
				|  |
				0--0
			*/
			invertTriangles = true;
		}
		
		if( tl_vert_level == bl_vert_level && tl_vert_level == tr_vert_level && tl_vert_level != br_vert_level ) {
			/*
				0--0
				|  |
				0--1
			*/
			invertTriangles = true;
		}
		
		if( bl_vert_level < tl_vert_level && bl_vert_level < br_vert_level && tr_vert_level < tl_vert_level && tr_vert_level < br_vert_level ) {
			/*
				1--0
				|  |
				0--1
			*/
			invertTriangles = true;
		}
		
		if( br_vert_level < tr_vert_level && br_vert_level < bl_vert_level && tl_vert_level < tr_vert_level && tl_vert_level < bl_vert_level ) {
			/*
				0--1
				|  |
				1--0
			*/
			invertTriangles = false;
		}
		
		return invertTriangles;
		
	}
	
	// get height of a tile
	private int getTileLevel(int l_index_x, int l_index_y) {
		return level_map [l_index_y, l_index_x];
	}

	// get pollution level of a tile
	private float getPollutionLevel(int l_index_x, int l_index_y) {
		return pollution_map [l_index_y, l_index_x];
	}
	
	// get heights of all of a tiles vertices.
	//		returned value: sub-element 0 is height of tile 0.
	//						sub-element 1 is height of tile 1.
	//						sub-element 2 is height of tile 2.
	//						sub-element 3 is height of tile 3.
	public int[] getVertexLevels(int level_index_x, int level_index_y, TileVertices vertex_index) {
		
		if ((int) vertex_index < 0 || (int) vertex_index > 3) {
			Debug.LogError("vertex_index must be between 0 and 3.");
			return new int[0];		
		}
		
		int[] levels = new int[4];
		
		switch (vertex_index) {
			
		case TileVertices.topRight:
			
			// NE
			if(level_index_y - 1 >= 0 && level_index_x + 1 < side_tile_count) {
				levels [0] = level_map [level_index_y - 1, level_index_x + 1];
			}	
			else {
				if(level_index_y - 1 >= 0) {
					levels [0] = neighbor_level_maps[(int)LevelMapOrientation.E][level_index_y-1];
				}
				else {
					if(level_index_x + 1 < side_tile_count) {
						levels [0] = neighbor_level_maps[(int)LevelMapOrientation.N][level_index_x+1];
					}
					else {
						levels [0] = neighbor_level_maps[(int)LevelMapOrientation.NE][0];
					}
				}
				
			}
			
			// N
			if(level_index_y - 1 >= 0) {
				levels [1] = level_map [level_index_y - 1, level_index_x];
			}	
			else {
				levels [1] = neighbor_level_maps[(int)LevelMapOrientation.N][level_index_x];
			}
			
			// E
			if(level_index_x + 1 < side_tile_count) {
				levels [2] = level_map [level_index_y, level_index_x + 1];
			}	
			else {
				levels [2] = neighbor_level_maps[(int)LevelMapOrientation.E][level_index_y];
			}
			
			// This tile
			levels [3] = level_map [level_index_y, level_index_x];
			
			break;
			
		case TileVertices.topLeft:
			
			// NW
			if(level_index_y - 1 >= 0 && level_index_x - 1 >= 0) {
				levels [1] = level_map [level_index_y - 1, level_index_x - 1];
			}	
			else {
				if(level_index_y - 1 >= 0) {
					levels [1] = neighbor_level_maps[(int)LevelMapOrientation.W][level_index_y-1];
				}
				else {
					if(level_index_x - 1 >= 0) {
						levels [1] = neighbor_level_maps[(int)LevelMapOrientation.N][level_index_x-1];
					}
					else {
						levels [1] = neighbor_level_maps[(int)LevelMapOrientation.NW][0];
					}
				}
				
			}
			
			// N
			if(level_index_y - 1 >= 0) {
				levels [0] = level_map [level_index_y - 1, level_index_x];
			}	
			else {
				levels [0] = neighbor_level_maps[(int)LevelMapOrientation.N][level_index_x];
			}
			
			// W
			if(level_index_x - 1 >= 0) {
				levels [3] = level_map [level_index_y, level_index_x - 1];
			}	
			else {
				levels [3] = neighbor_level_maps[(int)LevelMapOrientation.W][level_index_y];
			}
			
			// This tile
			levels [2] = level_map [level_index_y, level_index_x];
			
			break;
			
		case TileVertices.bottomLeft:
			
			// SW
			if(level_index_y + 1 < side_tile_count && level_index_x - 1 >= 0) {
				levels [2] = level_map [level_index_y + 1, level_index_x - 1];
			}	
			else {
				if(level_index_y + 1 < side_tile_count) {
					levels [2] = neighbor_level_maps[(int)LevelMapOrientation.W][level_index_y+1];
				}
				else {
					if(level_index_x - 1 >= 0) {
						levels [2] = neighbor_level_maps[(int)LevelMapOrientation.S][level_index_x-1];
					}
					else {
						levels [2] = neighbor_level_maps[(int)LevelMapOrientation.SW][0];
					}
				}
			}
			
			// S
			if(level_index_y + 1 < side_tile_count) {
				levels [3] = level_map [level_index_y + 1, level_index_x];
			}	
			else {
				levels [3] = neighbor_level_maps[(int)LevelMapOrientation.S][level_index_x];
			}
			
			// W
			if(level_index_x - 1 >= 0) {
				levels [0] = level_map [level_index_y, level_index_x - 1];
			}	
			else {
				levels [0] = neighbor_level_maps[(int)LevelMapOrientation.W][level_index_y];
			}
			
			// This tile
			levels [1] = level_map [level_index_y, level_index_x];
			
			break;
			
		case TileVertices.bottomRight:
			
			// SE
			if(level_index_y + 1 < side_tile_count && level_index_x + 1 < side_tile_count) {
				levels [3] = level_map [level_index_y + 1, level_index_x + 1];
			}	
			else {
				if(level_index_y + 1 < side_tile_count) {
					levels [3] = neighbor_level_maps[(int)LevelMapOrientation.E][level_index_y+1];
				}
				else {
					if(level_index_x + 1 < side_tile_count) {
						levels [3] = neighbor_level_maps[(int)LevelMapOrientation.S][level_index_x+1];
					}
					else {
						levels [3] = neighbor_level_maps[(int)LevelMapOrientation.SE][0];
					}
				}
			}
			
			// S
			if(level_index_y + 1 < side_tile_count) {
				levels [2] = level_map [level_index_y + 1, level_index_x];
			}	
			else {
				levels [2] = neighbor_level_maps[(int)LevelMapOrientation.S][level_index_x];
			}
			
			// E
			if(level_index_x + 1 < side_tile_count) {
				levels [1] = level_map [level_index_y, level_index_x + 1];
			}	
			else {
				levels [1] = neighbor_level_maps[(int)LevelMapOrientation.E][level_index_y];
			}
			
			// This tile
			levels [0] = level_map [level_index_y, level_index_x];
			
			break;
			
		}
		
		return levels;
		
	}
	
	// Get the highest int in array
	public int getMax(int[] array) {
		
		if (array.Length == 0) {
			return 0;
		}
		
		int result = array [0];
		
		for (int i = 1; i < array.Length; ++i) {
			if (array[i] > result) {
				result = array[i];
			}
		}
		
		return result;
		
	}

	// Get the lowest int in array
	public int getMin(int[] array) {
		
		if (array.Length == 0) {
			return 0;
		}
		
		int result = array [0];
		
		for (int i = 1; i < array.Length; ++i) {
			if (array [i] < result) {
				result = array [i];
			}
		}
		
		return result;
	
	}

		
	// Get the normal to a triangle from the three corner points, a, b and c.
	private Vector3 GetNormal(Vector3 a, Vector3 b, Vector3 c) {
		// Find vectors corresponding to two of the sides of the triangle.
		Vector3 side1 = b - a;
		Vector3 side2 = c - a;
		
		// Cross the vectors to get a perpendicular vector, then normalize it.
		return Vector3.Cross(side1, side2).normalized;
	}
	
}
