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
