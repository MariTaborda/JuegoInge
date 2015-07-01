using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapLoader {

	string FilePath;
	Texture2D loadedImage;
	int size_x;
	int size_y;

	float grayscale_zero;
	int level_zero;
	int level_count;
	float level_difference;
	int[,] Levels;

	public void openFile(int size_x, int size_y, string path) {

		initSizes (size_x, size_y);

		FilePath = Application.dataPath + path;
		if (System.IO.File.Exists (FilePath)) {
			byte[] bytes = System.IO.File.ReadAllBytes (FilePath);
			loadedImage = new Texture2D (1, 1);
			loadedImage.LoadImage (bytes);
		} 
		else {
			Debug.LogError("Unable to open file: "+path);
		}

	}

	public List<WaterFlowDirection[,]> getWFDMapList (int chunk_side_length) {
		
		List<WaterFlowDirection[,]> flow_directions = new List<WaterFlowDirection[,]> ();
		
		if (size_x % chunk_side_length != 0 || size_x != size_y) {
			Debug.LogError("Error 1A");
			return flow_directions;
		}
		
		int side_chunks = size_x / chunk_side_length;
		WaterFlowDirection[,] chDirs = getFlowDirections ();
		
		for(int i = 0; i < side_chunks; ++i) {
			for(int j = 0; j < side_chunks; ++j) {
				
				WaterFlowDirection[,] chunk_directions = new WaterFlowDirection[chunk_side_length, chunk_side_length];
				for(int m = 0; m < chunk_side_length; ++m) {
					for(int n = 0; n < chunk_side_length; ++n) {
						
						chunk_directions[m, n] = chDirs[i * chunk_side_length + m, j * chunk_side_length + n];
						
					}
				}
				flow_directions.Add (chunk_directions);
				
			}
		}
		
		return flow_directions;
		
	}

	public List<TileType[,]> getTileTypeMapList (int chunk_side_length) {
	
		List<TileType[,]> tile_types = new List<TileType[,]> ();
		
		if (size_x % chunk_side_length != 0 || size_x != size_y) {
			Debug.LogError("Error 1A");
			return tile_types;
		}
		
		int side_chunks = size_x / chunk_side_length;
		TileType[,] chTypes = getTileTypes ();

		for(int i = 0; i < side_chunks; ++i) {
			for(int j = 0; j < side_chunks; ++j) {
				
				TileType[,] chunk_types = new TileType[chunk_side_length, chunk_side_length];
				for(int m = 0; m < chunk_side_length; ++m) {
					for(int n = 0; n < chunk_side_length; ++n) {
						
						chunk_types[m, n] = chTypes[i * chunk_side_length + m, j * chunk_side_length + n];
						
					}
				}
				tile_types.Add (chunk_types);
				
			}
		}
		
		return tile_types;

	}

	private TileType[,] getTileTypes () {
	
		Color[] pix = loadedImage.GetPixels();
		TileType[,] types = new TileType[size_y, size_x];
		for (int i = 0; i < size_y; ++i) {
			for(int j = 0; j < size_x; ++j) {
				types[i, j] = getTypeFromColor(pix[i * size_y + j]);
			}
		}

		return types;

	}

	private WaterFlowDirection[,] getFlowDirections () {
		
		Color[] pix = loadedImage.GetPixels();
		WaterFlowDirection[,] dirs = new WaterFlowDirection[size_y, size_x];
		for (int i = 0; i < size_y; ++i) {
			for(int j = 0; j < size_x; ++j) {
				dirs[i, j] = getFlowDirectionFromColor(pix[i * size_y + j]);
			}
		}
		
		return dirs;
		
	}

	private TileType getTypeFromColor(Color color) {

		Vector3 rgb = convertColorToStandard (new Vector3 (color.r, color.g, color.b));
	
		if(rgb.x == 133 && rgb.y == 86 && rgb.z == 86) {
			// Grayish red
			return TileType.empty;
		}

		if(rgb.x == 186 && rgb.y == 77 && rgb.z == 187) {
			// Pink
		}

		if(rgb.x == 0 && rgb.y == 136 && rgb.z == 45) {
			// Bluish green
			return TileType.high_zone;
		}

		if(rgb.x == 129 && rgb.y == 187 && rgb.z == 77) {
			// Yellowish green
			return TileType.medium_zone;
		}

		if(rgb.x == 167 && rgb.y == 166 && rgb.z == 90) {
			// Brownish green
			return TileType.low_zone;
		}

		if(rgb.x == 145 && rgb.y == 110 && rgb.z == 74) {
			// Brown
			return TileType.dirt;
		}

		if(rgb.x == 89 && rgb.y == 105 && rgb.z == 114) {
			// Gray
			return TileType.rocky;
		}

		if(rgb.x == 232 && rgb.y == 211 && rgb.z == 125) {
			// Yellow
			return TileType.beach;
		}

		return TileType.high_zone;
	
	}

	private WaterFlowDirection getFlowDirectionFromColor(Color color) {
		
		Vector3 rgb = convertColorToStandard (new Vector3 (color.r, color.g, color.b));
		
		if(rgb.x == 0 && rgb.y == 0 && rgb.z == 255) {
			// blue
			return WaterFlowDirection.N;
		}
		
		if(rgb.x == 0 && rgb.y == 255 && rgb.z == 0) {
			// green
			return WaterFlowDirection.NE;
		}
		
		if(rgb.x == 255 && rgb.y == 0 && rgb.z == 0) {
			// red
			return WaterFlowDirection.E;
		}
		
		if(rgb.x == 255 && rgb.y == 255 && rgb.z == 0) {
			// yellow
			return WaterFlowDirection.SE;
		}
		
		if(rgb.x == 255 && rgb.y == 0 && rgb.z == 255) {
			// pink
			return WaterFlowDirection.S;
		}
		
		if(rgb.x == 127 && rgb.y == 0 && rgb.z == 255) {
			// purple
			return WaterFlowDirection.SW;
		}
		
		if(rgb.x == 255 && rgb.y == 127 && rgb.z == 86) {
			// orange
			return WaterFlowDirection.W;
		}
		
		if(rgb.x == 0 && rgb.y == 255 && rgb.z == 255) {
			// cyan
			return WaterFlowDirection.NW;
		}
		
		return WaterFlowDirection.none;
		
	}

	private Vector3 convertColorFromStandard(Vector3 color_standard) {
		return new Vector3(color_standard.x/255.0f, color_standard.y/255.0f, color_standard.z/255.0f);
	}

	private Vector3 convertColorToStandard(Vector3 color_norm) {
		return new Vector3( (int) (color_norm.x*255.0f), (int) (color_norm.y*255.0f), (int) (color_norm.z*255.0f) );
	}

	public List<int[,]> getLevelMapList (int level_count, float grayscale_zero, int chunk_side_length, out int chunks_x, out int chunks_y) {

		setLevels(level_count, grayscale_zero);
		List<int[,]> result = new List<int[,]> ();

		if (size_x % chunk_side_length != 0 || size_x != size_y) {
			Debug.LogError("Error 1A");
			chunks_x = 0;
			chunks_y = 0;
			return result;
		}

		int side_chunks = size_x / chunk_side_length;

		for(int i = 0; i < side_chunks; ++i) {
			for(int j = 0; j < side_chunks; ++j) {

				int[,] chunk_levels = new int[chunk_side_length, chunk_side_length];
				for(int m = 0; m < chunk_side_length; ++m) {
					for(int n = 0; n < chunk_side_length; ++n) {
						
						chunk_levels[m, n] = Levels[i * chunk_side_length + m, j * chunk_side_length + n];
						
					}
				}
				result.Add (chunk_levels);

			}
		}

		chunks_x = side_chunks;
		chunks_y = side_chunks;

		return result;

	}

	private void setLevels(int level_count, float grayscale_zero) {

		initAttributes (level_count, grayscale_zero);

		Color[] pix = loadedImage.GetPixels();
		Levels = new int[size_y, size_x];
		for (int i = 0; i < size_y; ++i) {
			for(int j = 0; j < size_x; ++j) {
				Levels[i, j] = getLevelFromGrayscale(pix[i * size_y + j].grayscale);
			}
		}

	}

	private int getLevelFromGrayscale(float grayscale_value) {
		int absolute_level = (int) (grayscale_value * level_count); 
		return absolute_level - level_zero;
	}


	private void initSizes(int size_x, int size_y) {
		this.size_x = size_x;
		this.size_y = size_y;
	}

	private void initAttributes(int level_count, float grayscale_zero) {
		this.level_count = level_count;
		this.grayscale_zero = grayscale_zero;
		this.level_zero = (int)(grayscale_zero * level_count);
		this.level_difference = 1f / level_count;
	}



}
