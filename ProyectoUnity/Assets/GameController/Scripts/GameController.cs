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
	public Camera mainCamera;

	public GameObject SceneryObject_Prefab;

	[HideInInspector]
	public GenerateTerrain terrainGenerator;
	[HideInInspector]
	public PlayerController playerController;
	[HideInInspector]
	public SpriteMapper spriteMapper;


	// Use this for initialization
	void Start () {
		initGameController ();
		initSpriteMapper ();
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

	// SpriteMapper initialization
	void initSpriteMapper() {
		spriteMapper = new SpriteMapper ();
		spriteMapper.init (SceneryObject_Prefab.GetComponent<tk2dSprite> ());
	}

	// PlayerController initialization
	void initPlayerController() {
		playerController = playerControllerObject.GetComponent<PlayerController> ();
		playerController.init ();
	}

}
