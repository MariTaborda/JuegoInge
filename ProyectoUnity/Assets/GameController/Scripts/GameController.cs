using UnityEngine;
using System.Collections;

/*
 *	Game Controller initializes everything. 
 */

public class GameController : MonoBehaviour {

	public static GameController gameController;

	public GameObject terrainObject;
	public GameObject playerControllerObject;
	public GameObject playerActionsHolder;

	[HideInInspector]
	public GenerateTerrain terrainGenerator;
	[HideInInspector]
	public PlayerController playerController;

	// Use this for initialization
	void Start () {
		initGameController ();
		initTerrainGenerator ();
		initPlayerController ();
	}

	// GameController initialization
	void initGameController() {
		gameController = this;
	}

	// TerrainGenerator initialization
	void initTerrainGenerator() {
		terrainGenerator = terrainObject.GetComponent<GenerateTerrain> ();
		terrainGenerator.init ();
	}

	// PlayerController initialization
	void initPlayerController() {
		playerController = playerControllerObject.GetComponent<PlayerController> ();
		playerController.init ();
	}

}
