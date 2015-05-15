using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum LevelMapOrientation { N, NE, E, SE, S, SW, W, NW };

public enum WaterFlowDirection { N, NE, E, SE, S, SW, W, NW };

public enum TileDivisionConfiguration { topLeft_to_bottomRight, bottomLeft_to_topRight };

public enum TileVertices { topLeft, topRight, bottomRight, bottomLeft };

public enum TileBrightness { normal, dark, light };

public enum TextureType { grass, dirt };

// Type of scenery object
public enum SceneryType { empty, tree1, tree2, tree3, tree4, tree5, bush1, bush2, bush3, bush4, bush5, casa1, zonarural, zonaciudad, zonaruralsingranjas, fuente, mercado, police, residencial, stores, vereda};

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
	
	private Vector3 origin;
	private int total_tile_count;
	private int side_tile_count;
	private float tile_size;
	private float slope_height;
	private Vector3 light_direction = Vector3.left;

	// level map: associates a height level for every tile	
	private int[,] level_map;
	private List<int[]> neighbor_level_maps; 

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

	// scenery map: associates a sceneryType value with every cell
	private SceneryType[,] scenery_map;
	private float scenery_density = 0.5f;

	// stores scenery objects which are rendered and correspond to this terrain chunk 
	private Stack<GameObject> reserved_sc_objs;
	
	
	bool isValid = false;
	
	public TerrainChunk(Vector3 origin, int side_tile_count, float tile_size, float slope_height, ref int[,] level_map, int num_textures_x, int num_textures_y,  ref List<int[]> neighbor_level_maps, MeshFilter mesh_filter, int zona) {
		setAttributes (origin, side_tile_count, tile_size, slope_height, ref level_map, num_textures_x, num_textures_y, ref neighbor_level_maps, mesh_filter, zona);
		generateMesh ();
	}
	
	public TerrainChunk() {
		isValid = false;
	}

	// set initial attributes. happens only once on object initialization.
	public void setAttributes(Vector3 origin, int side_tile_count, float tile_size, float slope_height, ref int[,] level_map, int textures_x, int textures_y,  ref List<int[]> neighbor_level_maps, MeshFilter mesh_filter, int zona) {
		
		if (level_map.Length == side_tile_count * side_tile_count) {
			this.origin = origin;
			this.mesh_filter = mesh_filter;
			this.total_tile_count = side_tile_count * side_tile_count;
			this.side_tile_count = side_tile_count;
			this.tile_size = tile_size;
			this.slope_height = slope_height;
			this.level_map = level_map;
			this.neighbor_level_maps = neighbor_level_maps;
			this.tri_div_conf_map = setTriDivConfMap (level_map);
			this.brightness_map = setBrightnessMap (level_map);
			this.flow_map = setFlowMap();
			this.num_textures_x = textures_x;
			this.num_textures_y = textures_y;
			if (zona == 1) { 		// rural 
				this.scenery_map = setSceneryMapRural();
			} else if (zona == 2) { // city
				this.scenery_map = setSceneryMapCity();
			} else {				//  default 
				this.scenery_map = setSceneryMap();
			}
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
		
		List<int>[] ws_flowing_tris = new List<int>[8];
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
				
				// set triangles by direction of water flow
				ws_flowing_tris[(int) flow_map[lmap_index_y, lmap_index_x]].Add (ws_triangles[6*i + 0]);
				ws_flowing_tris[(int) flow_map[lmap_index_y, lmap_index_x]].Add (ws_triangles[6*i + 1]);
				ws_flowing_tris[(int) flow_map[lmap_index_y, lmap_index_x]].Add (ws_triangles[6*i + 2]);
				
				ws_flowing_tris[(int) flow_map[lmap_index_y, lmap_index_x]].Add (ws_triangles[6*i + 3]);
				ws_flowing_tris[(int) flow_map[lmap_index_y, lmap_index_x]].Add (ws_triangles[6*i + 4]);
				ws_flowing_tris[(int) flow_map[lmap_index_y, lmap_index_x]].Add (ws_triangles[6*i + 5]);
				
				// set uvs
				ws_uv [4*i + (int)TileVertices.topLeft] 		= new Vector2(0, 0);
				ws_uv [4*i + (int)TileVertices.topRight] 		= new Vector2(1, 0);
				ws_uv [4*i + (int)TileVertices.bottomRight] 	= new Vector2(1, 1);
				ws_uv [4*i + (int)TileVertices.bottomLeft] 		= new Vector2(0, 1);
				
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
			if(verts_below_zero > 1) {
				// If more than 1 vertex has height below zero, set texture as dirt.
				texture_type_indexes = getTextureRowIndexes(TextureType.dirt);
			}
			else {
				// Otherwise set texture as grass.
				texture_type_indexes = getTextureRowIndexes(TextureType.grass);
			}
			
			int t_index_y = (int) UnityEngine.Random.Range(texture_type_indexes.x, texture_type_indexes.y+1);
			int t_index_x = (int) UnityEngine.Random.Range(0, num_textures_x);
			
			uv [4*i + (int)TileVertices.topLeft] 		= texture_coords[t_index_y, t_index_x, (int) TileVertices.topLeft];
			uv [4*i + (int)TileVertices.topRight] 		= texture_coords[t_index_y, t_index_x, (int) TileVertices.topRight];
			uv [4*i + (int)TileVertices.bottomRight] 	= texture_coords[t_index_y, t_index_x, (int) TileVertices.bottomRight];
			uv [4*i + (int)TileVertices.bottomLeft] 	= texture_coords[t_index_y, t_index_x, (int) TileVertices.bottomLeft];
			
			//Debug.Log("Ind: "+i+", v0: "+vertices[4*i + (int)TileVertices.topLeft]+", v1: "+vertices[4*i + (int)TileVertices.topRight]+", v2: "+vertices[4*i + (int)TileVertices.bottomRight]+", v3: "+vertices[4*i + (int)TileVertices.bottomLeft]+", tri conf: "+tri_div_conf_map[lmap_index_y, lmap_index_x]+", tri-0: "+brightness_map[lmap_index_y, lmap_index_x, 0]+", tri-1: "+brightness_map[lmap_index_y, lmap_index_x, 1]);
			
		}
		
		
		// Assign Terrain mesh data
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
		
		
		// Assign Water surface mesh data
		water_surface_mesh.vertices = ws_vertices;
		water_surface_mesh.triangles = ws_triangles;
		
		water_surface_mesh.subMeshCount = 8;
		
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
		
	}
	
	public SceneryType[,] getSceneryMap() {
		return scenery_map;
	}

	// render all scenery objects on this terrain chunk. Stores rendered scenery objects in sc_obj_pool
	public void renderScenery(int chunk_index_x, int chunk_index_y, ref Stack<GameObject> sc_obj_pool) {
		
		for (int i = 0; i < side_tile_count; ++i) {
			for(int j = 0; j < side_tile_count; ++j) {
				
				if(scenery_map[i, j] != SceneryType.empty) {
					
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
			unloadSceneryObject(obj, ref sc_obj_pool);
		}
		
	}

	// loads/renders an individual scenery object on this chunk. recieves chunk indexes for positioning. stores rendered object in sc_obj_pool
	public void loadSceneryObject(int chunk_index_x, int chunk_index_y, int scenery_map_ind_x, int scenery_map_ind_y, ref Stack<GameObject> sc_obj_pool) {
		
		GameObject newObj = sc_obj_pool.Pop();
		newObj.SetActive(true);
		newObj.GetComponent<tk2dSprite> ().SetSprite( ((int) scenery_map[scenery_map_ind_y, scenery_map_ind_x]) - 1 );
		positionSceneryObj(ref newObj, scenery_map_ind_x, scenery_map_ind_y);
		newObj.GetComponent<SceneryObject> ().init (chunk_index_x, chunk_index_y, scenery_map_ind_x, scenery_map_ind_y);
		
		newObj.transform.Find("SceneryCollider").GetComponent<ShowMenu> ().setShowMenu();
		
		reserved_sc_objs.Push(newObj);
		
	}

	// unloads/unrenders an individual scenery object on this chunk.
	public void unloadSceneryObject(GameObject obj, ref Stack<GameObject> sc_obj_pool) {
		obj.SetActive(false);
		sc_obj_pool.Push(obj);
	}

	// associates a tiles scenery map value as empty. i.e destroys a scenery object on this chunk
	public void destroySceneryObject(GameObject obj) {
		SceneryObject sc_obj = obj.GetComponent<SceneryObject> ();
		scenery_map [sc_obj.scenery_index_y, sc_obj.scenery_index_x] = SceneryType.empty;
	}

	// positions a scenery object on the terrain
	private void positionSceneryObj(ref GameObject obj, int sc_map_index_x, int sc_map_index_y) {
		
		Vector3 position = new Vector3 (
			origin.x + sc_map_index_x * tile_size + tile_size/2, 
			level_map [sc_map_index_y, sc_map_index_x] * slope_height, 
			origin.z - sc_map_index_y * tile_size - tile_size/2
			);
		
		obj.transform.position = position;
		
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

	// gets a Vector2 which has its values set as follows:
	//		x: first row in texture image that is of type t_type
	//		y: last row in texture image that is of type t_type
	private Vector2 getTextureRowIndexes(TextureType t_type) {
		
		Vector2 indexes = Vector2.zero;
		switch(t_type) {
		case TextureType.grass:
			indexes = new Vector2(0, 1);
			break;
			
		case TextureType.dirt:
			indexes = new Vector2(2, 2);
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
		
		// remove later
		for (int i = 0; i < side_tile_count; ++i) {
			for (int j = 0; j < side_tile_count; ++j) {
				result[i, j] = (WaterFlowDirection) UnityEngine.Random.Range(0, 8);
			}		
		}
		return result;
		
	}

	// sets a scenery type for every tile
	private SceneryType[,] setSceneryMap() {
		
		reserved_sc_objs = new Stack<GameObject> ();
		SceneryType[,] result = new SceneryType[side_tile_count, side_tile_count];
		int scenery_instances = Mathf.FloorToInt(side_tile_count * side_tile_count * scenery_density);
		
		for (int i = 0; i < scenery_instances; ++i) {
			
			int rand_index_x = UnityEngine.Random.Range(0, side_tile_count);
			int rand_index_y = UnityEngine.Random.Range(0, side_tile_count);
			
			if(isSuitableForScenery(rand_index_x, rand_index_y) && (result[rand_index_y, rand_index_x] == SceneryType.empty)){ // && noHouses(rand_index_y, rand_index_x)) {
				result[rand_index_y, rand_index_x] = getRandomSceneryType();
			}
			
		}
		return result;
		
	}
	
	// deseable: que se bajen un poco mas de profundidad. 
	// que no caminen los bichitos sobre ellos. 
	private SceneryType[,] setSceneryMapRural() { 
		
		reserved_sc_objs = new Stack<GameObject> ();
		SceneryType[,] result = new SceneryType[side_tile_count, side_tile_count];
		// lugar dentro de todo el chunk (tile especifico para posicionar la zona rural)
		result[14,11] = SceneryType.zonarural;
		result[6,9] = SceneryType.zonaruralsingranjas;
		result[11,9] = SceneryType.casa1;
		result[13,12] = SceneryType.casa1;
		
		result[2,18] = SceneryType.casa1;
		result[4,16] = SceneryType.casa1;
		result[6,15] = SceneryType.casa1;
		result[8,17] = SceneryType.casa1;
		result[14,20] = SceneryType.casa1;
		result[12,23] = SceneryType.casa1;
		//result[14,14] = SceneryType.casa1;
		return result;
	}
	
	private SceneryType[,] setSceneryMapCity() { 
		
		reserved_sc_objs = new Stack<GameObject> ();
		SceneryType[,] result = new SceneryType[side_tile_count, side_tile_count];
		// lugar dentro de todo el chunk (tile especifico para posicionar la zona rural)
		result[11,11] = SceneryType.zonaciudad;
		result[11,18] = SceneryType.mercado;
		result [7,9] = SceneryType.police;
		result[6,13] = SceneryType.stores;
		result[3,12] = SceneryType.fuente;
		result[8,18] = SceneryType.vereda;
		result[16,16] = SceneryType.residencial;
		result[17,13] = SceneryType.vereda;
		result[11,13] = SceneryType.residencial;
		return result;
	}
	
	
	public bool noHouses(int rand_index_y, int rand_index_x){
		bool result = true; 
		
		// rango 3 tiles x 3 tiles por ej
		// iniciando desde tile 6-12
		for (int i = 0; i < 10; i++ ) {
			for (int j = 0; j <= 20; j++) {
				if (rand_index_y == i && rand_index_x == j) {
					result = false;
				}
			}
		}
		return result;
	}
	
	// gets a random vegetation scenery type 
	private SceneryType getRandomSceneryType() {
		
		if (UnityEngine.Random.Range (0, 10) == 7) {
			//tree
			
			if (UnityEngine.Random.Range (0, 20) == 8) {
				return (SceneryType) UnityEngine.Random.Range(1, 3);
			}
			
			if (UnityEngine.Random.Range (0, 18) == 3) {
				return SceneryType.tree5;
			}
			
			return (SceneryType) UnityEngine.Random.Range(3, 5);
			
		}
		
		if (UnityEngine.Random.Range (0, 100) == 45) {
			//return SceneryType.bush5;
		}
		
		if (UnityEngine.Random.Range (0, 2) == 1) {
			return SceneryType.empty;
		}
		
		//bush
		return (SceneryType) UnityEngine.Random.Range (6, 10);
		
		
	}

	// checks if a tile is suitable for scenery placement
	private bool isSuitableForScenery(int lmap_index_x, int lmap_index_y) {
		
		if(tileIsFlat(lmap_index_x, lmap_index_y) && !tileHeightIsLowerThanMinVert(lmap_index_x, lmap_index_y) && level_map[lmap_index_y, lmap_index_x] >= surface_level) {
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
	
	// get heights of all of a tiles vertices.
	//		returned value: sub-element 0 is height of tile 0.
	//						sub-element 1 is height of tile 1.
	//						sub-element 2 is height of tile 2.
	//						sub-element 3 is height of tile 3.
	private int[] getVertexLevels(int level_index_x, int level_index_y, TileVertices vertex_index) {
		
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
	private int getMax(int[] array) {
		
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
	
	// Get the normal to a triangle from the three corner points, a, b and c.
	private Vector3 GetNormal(Vector3 a, Vector3 b, Vector3 c) {
		// Find vectors corresponding to two of the sides of the triangle.
		Vector3 side1 = b - a;
		Vector3 side2 = c - a;
		
		// Cross the vectors to get a perpendicular vector, then normalize it.
		return Vector3.Cross(side1, side2).normalized;
	}
	
}
