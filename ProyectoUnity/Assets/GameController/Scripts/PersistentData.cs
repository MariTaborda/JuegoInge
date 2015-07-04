using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class PersistentData : MonoBehaviour {

	public static PersistentData Data;

	[HideInInspector]
	public SerializableDataStructure SerializableData;
	[HideInInspector]
	public bool loadPersistentData = false;

	void Awake() {
		if (Data == null) {
			Data = this;
			DontDestroyOnLoad (gameObject);
			SerializableData = new SerializableDataStructure ();
		} else if (Data != this) {
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void initTerrainChunkData(int terrain_chunks_x, int terrain_chunks_y) {
		SerializableData.TerrainChunks = new TerrainChunkDataPackage[terrain_chunks_y, terrain_chunks_x];
	}

	public void saveData() {
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Open (Application.persistentDataPath + "/playerData.dat", FileMode.Create);
		bf.Serialize (file, SerializableData);
		file.Close ();
	}
	
	public void loadData() {
		if (File.Exists (Application.persistentDataPath + "/playerData.dat")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/playerData.dat", FileMode.Open);
			SerializableData = (SerializableDataStructure) bf.Deserialize (file);
			file.Close ();
			loadPersistentData = true;
			Application.LoadLevel("default");
		}
	}

	public void prepareData() {
		GameController.gameController.playerController.updateDataPackage ();
		GameController.gameController.missionController.updateDataPackage ();
	}

}

[Serializable]
public class SerializableDataStructure {

	public TerrainChunkDataPackage[,] TerrainChunks;
	public PlayerDataPackage Player;
	public MissionDataPackage Missions;

}
