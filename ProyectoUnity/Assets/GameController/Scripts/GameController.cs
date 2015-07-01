using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 *	Game Controller initializes everything. 
 */

public enum Zones {low, middle, high};

public class GameController : MonoBehaviour {

	public static GameController gameController;

	public GameObject terrainObject;
	public GameObject playerControllerObject;
	public GameObject playerActionsHolder;
	public Camera mainCamera;
	public GameObject actionsMenuObject;
	public GameObject HUDObject;
	public GameObject loadedMissionsHolder;
	public GameObject SceneryObject_Prefab;

	public bool missionsActive;
	public int level = 0;
	public Zones zone = Zones.low;
	public string coast_heightMap = "/Resources/Heightmaps/coast_heightmap_big.jpg";
	public string coast_tileTypeMap = "/Resources/TileTypeMaps/coast_ttypemap_big.png";
	public string coast_flowMap = "/Resources/FlowMaps/coast_wfdmap_big.png";
	public string middle_heightMap = "/Resources/Heightmaps/mesa_heightmap_big.jpg";
	public string middle_tileTypeMap = "/Resources/TileTypeMaps/mesa_ttypemap_big.png";
	public string middle_flowMap = "/Resources/FlowMaps/mesa_wfdmap_big.png";
	public string high_heightMap = "/Resources/Heightmaps/mesa_heightmap_big.jpg";
	public string high_tileTypeMap = "/Resources/TileTypeMaps/mesa_ttypemap_big.png";

	[HideInInspector]
	public GenerateTerrain terrainGenerator;
	[HideInInspector]
	public PlayerController playerController;
	[HideInInspector]
	public SpriteMapper spriteMapper;
	[HideInInspector]
	public HUDCanvas hudCanvas;
	[HideInInspector]
	public MissionUI missionUI;
	[HideInInspector]
	public ChangeScene sceneChanger;
	[HideInInspector]
	public MissionManager missionController;
	[HideInInspector]

	private Vector3 objective_position;
	private bool objective_is_active = false;

	private List<Vector3> waypoints;

	// Use this for initialization
	void Start () {
		TagHelper.AddTag ("SceneryTree");
		TagHelper.AddTag ("SceneryRock");
		TagHelper.AddTag ("SceneryBush");

		initGameController ();
		initSpriteMapper ();
		initTerrainGenerator ();
		initMissionController ();
		initPlayerController ();

		//Objective marker is set on scientists initial position for testing purposes. Delete later.
		setObjective (playerController.scientist.transform.position);
	
	}

	public Vector3 getObjectivePosition() {
		if (objective_is_active) {
			return objective_position;
		} 
		else {
			return Vector3.zero;
		}
	}

	public void setObjective(Vector3 pos) {
		objective_is_active = true;
		objective_position = pos;
	}

	public void setObjective(GameObject obj) {
		objective_is_active = true;
		objective_position = obj.transform.position;
	}
	
	public void unsetObjective() {
		objective_is_active = false;
		objective_position = Vector3.zero;
	}

	public void addWaypoint(Vector3 position) {
		waypoints.Add (position);
	}
	
	public Vector3 getWaypointPosition (int waypoint_index) {
		return waypoints [waypoint_index];
	}

	// GameController initialization
	void initGameController() {
		gameController = this;
		waypoints = new List<Vector3> ();
		hudCanvas = HUDObject.GetComponent<HUDCanvas> ();
		hudCanvas.init ();
		missionUI = hudCanvas.mission_UI;
		missionUI.init ();
		actionsMenuObject.GetComponent<ActionsMenu> ().init ();
		gameObject.GetComponent<ShowMenu> ().setShowMenu ();
		sceneChanger = GetComponent<ChangeScene> ();
	}

	// TerrainGenerator initialization
	void initTerrainGenerator() {
		terrainGenerator = terrainObject.GetComponent<GenerateTerrain> ();

		string heightmap;
		string flowmap;
		string ttypemap;

		switch (zone) {
			case Zones.high:
				heightmap = middle_heightMap;
				flowmap = middle_flowMap;
				ttypemap = middle_tileTypeMap;
				break;
			case Zones.middle:
				heightmap = middle_heightMap;
				flowmap = middle_flowMap;
				ttypemap = middle_tileTypeMap;
				break;
			case Zones.low:
				heightmap = coast_heightMap;
				flowmap = coast_flowMap;
				ttypemap = coast_tileTypeMap;
				break;
			default:
				heightmap = middle_heightMap;
				flowmap = middle_flowMap;
				ttypemap = middle_tileTypeMap;
				break;
		}

		terrainGenerator.init (heightmap, ttypemap, flowmap);
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

	void initMissionController () {
		missionController = new MissionManager ();

		// invoke mission managing cycle after 2 seconds, and repeat every 2 seconds.
		InvokeRepeating("ManageMissions", 2, 2);
	}

	void ManageMissions () {
		missionController.manageMissions ();
	}

}
