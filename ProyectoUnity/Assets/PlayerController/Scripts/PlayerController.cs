using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using System.Runtime.Serialization.Formatters.Binary;

public class PlayerController : MonoBehaviour {

	public const int BUILDER = 0;
	public const int SCIENTIST = 1;
	public const int EXPLORER = 2;

	public static GameObject Player_Controller;

	public GameObject builderPrefab;
	public GameObject scientistPrefab;
	public GameObject explorerPrefab;
	
	public Sprite builderImage;
	public Sprite scientistImage;
	public Sprite explorerImage;

	public GameObject currentCharacterButton;
	public GameObject character2Button;
	public GameObject character3Button;
	
	private GameObject[] characters;
	private Sprite[] characterImages;
	private int current;

	public GameObject builder;
	public GameObject scientist;
	public GameObject explorer;
	
	private Player cbuilder;
	private Player cexplorer;
	private Player cscientist;

	public UpdateBackpackContents updateBackpack;
	public SetActionsMiniMenu setActions;

	[HideInInspector]
	public bool gettingMousePositionOnWorld = false;

	[HideInInspector]
	public Vector3 pointInWorld;
	[HideInInspector]
	public int? chunkIndexX = null;
	[HideInInspector]
	public int? chunkIndexY = null;
	[HideInInspector]
	public int? tileIndexX  = null;
	[HideInInspector]
	public int? tileIndexY  = null;

	[HideInInspector]
	PlayerDataPackage dataPackage;

	// object initialization
	public void init() {

		Player_Controller = gameObject;
		
		builder   = Instantiate (builderPrefab,   new Vector3(79.3f, 1f, -86.5f), Quaternion.identity) as GameObject;
		scientist = Instantiate (scientistPrefab, new Vector3(79.0f, 1f, -87.7f), Quaternion.identity) as GameObject;
		explorer  = Instantiate (explorerPrefab,  new Vector3(78.0f, 1f, -86.7f), Quaternion.identity) as GameObject;

		characters = new GameObject[]{builder, scientist, explorer};
		characterImages = new Sprite[]{builderImage, scientistImage, explorerImage};
		current = BUILDER;

		builder.transform.Find("AnimatedSprite").gameObject.GetComponent<CameraFace>().m_Camera = GameController.gameController.mainCamera;
		explorer.transform.Find("AnimatedSprite").gameObject.GetComponent<CameraFace>().m_Camera = GameController.gameController.mainCamera;
		scientist.transform.Find("AnimatedSprite").gameObject.GetComponent<CameraFace>().m_Camera = GameController.gameController.mainCamera;

		builder.GetComponent<PlayerMovement> ().init(builder);
		scientist.GetComponent<PlayerMovement> ().init(scientist);
		explorer.GetComponent<PlayerMovement> ().init(explorer);

		cbuilder = builder.GetComponent<Builder> ();
		cexplorer = explorer.GetComponent<Explorer> ();
		cscientist = scientist.GetComponent<Scientist> ();

		cbuilder.init ();
		cexplorer.init ();
		cscientist.init ();

		setItems ();
		setPlayerActions ();

		changeCurrentCharacter(current);

		if (PersistentData.Data.loadPersistentData) {
			// load previous player state
			dataPackage = PersistentData.Data.SerializableData.Player;

			loadInventoriesFromData();

			builder.transform.position = new Vector3(dataPackage.builderPosition_x, dataPackage.builderPosition_y+0.2f, dataPackage.builderPosition_z);
			explorer.transform.position = new Vector3(dataPackage.explorerPosition_x, dataPackage.explorerPosition_y+0.2f, dataPackage.explorerPosition_z);
			scientist.transform.position = new Vector3(dataPackage.scientistPosition_x, dataPackage.scientistPosition_y+0.2f, dataPackage.scientistPosition_z);

			builder.GetComponent<PlayerMovement> ().destinationPosition = builder.transform.position;
			builder.GetComponent<PlayerMovement> ().reachedDestination = true;
			builder.GetComponent<PlayerMovement> ().movementInterrupted = false;
			explorer.GetComponent<PlayerMovement> ().destinationPosition = explorer.transform.position;
			explorer.GetComponent<PlayerMovement> ().reachedDestination = true;
			explorer.GetComponent<PlayerMovement> ().movementInterrupted = false;
			scientist.GetComponent<PlayerMovement> ().destinationPosition = scientist.transform.position;
			scientist.GetComponent<PlayerMovement> ().reachedDestination = true;
			scientist.GetComponent<PlayerMovement> ().movementInterrupted = false;

			MoveCamera mc = Camera.main.GetComponent<MoveCamera> ();
			switch (dataPackage.currentCharacter) {
				case BUILDER:
					mc.target = builder;
					changeCurrentCharacter(BUILDER);
					break;
				case EXPLORER:
					mc.target = explorer;
					changeCurrentCharacter(EXPLORER);
					break;
				case SCIENTIST:
					mc.target = scientist;
					changeCurrentCharacter(SCIENTIST);
					break;
				default:
					mc.target = builder;
					changeCurrentCharacter(BUILDER);
					break;
			}
			mc.following = true;
			mc.gameObject.transform.position = new Vector3(
				dataPackage.cam_x, 
				dataPackage.cam_y, 
				dataPackage.cam_z
			);

		} 
		else {
			dataPackage = new PlayerDataPackage ();
			PersistentData.Data.SerializableData.Player = dataPackage;
		}

	}

	public GameObject currentCharacter() {
		return characters [current];
	}
	
	public int currentCharacterIndex() {
		return current;
	}
	
	void Update () {
		if ( Input.GetKeyDown( KeyCode.Tab ) ) {
			int next = (current + 1) % 3;
			changeCurrentCharacter(next);
		}
		
		// handle left mouse button clicks
		if(Input.GetMouseButtonDown(0)) {
			if (!EventSystem.current.IsPointerOverGameObject ()) {
				// did not click on a UI element or scenery object
				//if(true) {
				if (gettingMousePositionOnWorld) {
					Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
					RaycastHit hit;
					if(Physics.Raycast(ray, out hit)) {
						int indx;
						int indz;
						TChunk chunk = GameController.gameController.terrainGenerator.getChunkTileFromPosition(hit.point, out indx, out indz);
						pointInWorld = hit.point;
						chunkIndexX = chunk.index_x;
						chunkIndexY = chunk.index_y;
						tileIndexX = indz;
						tileIndexY = indx;
						//Debug.Log ("chunkX: "+chunkIndexX+". chunkY: "+chunkIndexY+". tileX: "+tileIndexX+". tileY: "+tileIndexY + ". position: " + pointInWorld);
					}
				} else {
					currentCharacter().GetComponent<PlayerMovement> ().checkNewDestination ();
					changeCharacterOnMouseDown ();
				}
			} 
		}
	}

	void setPlayerActions() {

		//builder actions

		//explorer actions
		ActionChopTree act = GameController.gameController.playerActionsHolder.AddComponent<ActionChopTree> ();
		cexplorer.addAction (act);

		ActionCutBush acb = GameController.gameController.playerActionsHolder.AddComponent<ActionCutBush> ();
		cexplorer.addAction (acb);

		ActionMineRock amr = GameController.gameController.playerActionsHolder.AddComponent<ActionMineRock> ();
		cexplorer.addAction (amr);

		//scientist actions
	}

	void deactivateActionsMenu() {
		// se desactiva el actions menu si esta activo
		if(ActionsMenu.panel.activeSelf) {
			ActionsMenu.actions_menu.disable ();
		}
	}

	void changeCharacterOnMouseDown() {
		Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		RaycastHit hit;
		
		if( Physics.Raycast( ray, out hit, 100 ) ) {
			
			if ( hit.transform.gameObject.tag == "Player" ) {
				// se selecciono un personaje con el mouse
				if ( hit.transform.gameObject == builder ) {
					changeCurrentCharacter(BUILDER);
				} else if (hit.transform.gameObject == scientist) {
					changeCurrentCharacter(SCIENTIST);
				} else if (hit.transform.gameObject == explorer) {
					changeCurrentCharacter(EXPLORER);
				}			
			}

		}
	}
	
	public void changeCurrentCharacter(int newIndex) {
		current = newIndex;
		updateCurrentCharacterUI ();
		UpdateBackpackUI ();
		setActions.UpdateUI ();
		Camera.main.GetComponent<MoveCamera> ().target = currentCharacter();
		Camera.main.GetComponent<MoveCamera> ().following = true;
		deactivateActionsMenu ();


		// Mission condition handling 
		MissionManager mm = GameController.gameController.missionController;
		Mission changingCharacters;
		if (mm != null) {	// if mission manager was retrieved succesfully
			changingCharacters = mm.getMissionById (2); // get mission with id = 2
			if (changingCharacters != null && changingCharacters.started) {	// if mission exists and has been started
				((MissionSelectCharacter)changingCharacters).changedCharacters = true;	// set flag for mission
			}
		}

	}

	void updateCurrentCharacterUI() {
		currentCharacterButton.GetComponent<Image> ().sprite = characterImages[current];
		character2Button.GetComponent<Image> ().sprite = characterImages[(current + 1) % 3];
		character3Button.GetComponent<Image> ().sprite = characterImages[(current + 2) % 3];
	}
	
	public void onClickCharacter2Button() {
		changeCurrentCharacter ((current + 1) % 3);
	}

	public void onClickCharacter3Button() {
		changeCurrentCharacter ((current + 2) % 3);
	}
	
	public void triggerActionFromMenu(PlayerAction selected_action) {
	
		if (currentPlayerHasAction (selected_action)) {
			selected_action.performAction (currentCharacter(), ShowMenu.clickedObject.transform.parent.gameObject);
		} 
		else {
			Debug.LogError("Active character cannot perform that action.");
		}

		ActionsMenu.activator.deactivatePanel ();
	}

	public List<PlayerAction> getCurrentCharacterActionsOnObject (GameObject target) {
		List<PlayerAction> result = new List<PlayerAction> ();
		List<PlayerAction> actions = getCurrentCharacterActions ();
		for (int i = 0; i < actions.Count; ++i) {
			if(actions[i].getTargetTag() == target.tag) {
				result.Add(actions[i]);
			}
		}
		return result;
	}

	public List<PlayerAction> getCurrentCharacterActions () {
		return currentCharacter().GetComponent<Player>().getActions();
	}

	public bool currentPlayerHasAction(PlayerAction action) {
		List<PlayerAction> playerActions = getCurrentCharacterActions ();
		for (int i = 0; i < playerActions.Count; ++i) {
			if(playerActions[i].getActionName() == action.getActionName()) {
				return true;
			}
		}
		return false;
	}
	
	public PlayerItems getCurrentPlayerItems() {
		return currentCharacter().GetComponent<PlayerItems>();
	}

	private void loadInventoriesFromData() {
		PlayerItems builderItems = builder.GetComponent<PlayerItems> ();
		PlayerItems scientistItems = scientist.GetComponent<PlayerItems> ();
		PlayerItems explorerItems = explorer.GetComponent<PlayerItems> ();
		if (dataPackage.builder_inv != null) {
			loadInventoryFromData(builderItems, dataPackage.builder_inv);
		}
		if (dataPackage.explorer_inv != null) {
			loadInventoryFromData (explorerItems, dataPackage.explorer_inv);
		}
		if (dataPackage.scientist_inv != null) {
			loadInventoryFromData (scientistItems, dataPackage.scientist_inv);
		}
	}

	private void setItems () {
		PlayerItems builderItems = builder.GetComponent<PlayerItems> ();
		PlayerItems scientistItems = scientist.GetComponent<PlayerItems> ();
		PlayerItems explorerItems = explorer.GetComponent<PlayerItems> ();

		//builderItems.addItem (new Shovel(), 1);
		builderItems.addItem (new Hammer (), 1);
		builderItems.addItem (new Nails (), 15);
		builderItems.addItem (new Wood (), 40);
		builderItems.addItem (new Sapling (), 8);

		scientistItems.addItem (new Container (), 2);
		scientistItems.addItem (new MagnifGlass (), 1);
		scientistItems.addItem (new PHTest (), 4);

		explorerItems.addItem (new Pickaxe (), 1);
		explorerItems.addItem (new Machete (), 1);
		explorerItems.addItem (new Axe (), 1);
		explorerItems.addItem (new Rock (), 40);
	
	}

	public void giveItemsToBuilder(BackpackItem item, int quantity) {
		PlayerItems playerItems = builder.GetComponent<PlayerItems> ();
		playerItems.addItem (item, quantity);
		UpdateBackpackUI ();
	}

	public void giveItemsToExplorer(BackpackItem item, int quantity) {
		PlayerItems playerItems = explorer.GetComponent<PlayerItems> ();
		playerItems.addItem (item, quantity);
		UpdateBackpackUI ();
	}

	public void giveItemsToScientist(BackpackItem item, int quantity) {
		PlayerItems playerItems = scientist.GetComponent<PlayerItems> ();
		playerItems.addItem (item, quantity);
		UpdateBackpackUI ();
	}

	public void UpdateBackpackUI() {			
		updateBackpack.UpdateUI ();
	}

	public void clearMousePositionsOnWorld() {
		chunkIndexX = null;
		chunkIndexY = null;
		tileIndexX  = null;
		tileIndexY  = null;
	}

	public bool currentCharacterIsBuilder() {
		return current == BUILDER;
	}

	public bool currentCharacterIsScientist() {
		return current == SCIENTIST;
	}

	public bool currentCharacterIsExplorer() {
		return current == EXPLORER;
	}

	public void loadInventoryFromData(PlayerItems player_inv, List<int[]> inv_data) {
		player_inv.reset ();
		for (int i = 0; i < inv_data.Count; ++i) {
			switch(inv_data[i][0]) {
			case 0:
				player_inv.addItem (new Axe (), inv_data[i][1]);
				break;
			case 1:
				player_inv.addItem (new Container (), inv_data[i][1]);
				break;
			case 2:
				player_inv.addItem (new Hammer (), inv_data[i][1]);
				break;
			case 3:
				player_inv.addItem (new MagnifGlass (), inv_data[i][1]);
				break;
			case 4:
				player_inv.addItem (new Nails (), inv_data[i][1]);
				break;
			case 5:
				player_inv.addItem (new PHTest (), inv_data[i][1]);
				break;
			case 6:
				player_inv.addItem (new Pickaxe (), inv_data[i][1]);
				break;
			case 7:
				player_inv.addItem (new Rock (), inv_data[i][1]);
				break;
			case 8:
				player_inv.addItem (new Sapling (), inv_data[i][1]);
				break;
			case 9:
				player_inv.addItem (new Shovel (), inv_data[i][1]);
				break;
			case 10:
				player_inv.addItem (new Wood (), inv_data[i][1]);
				break;
			default:
					// unrecognized item, do nothing
				break;
			}
		}
	}

	public void saveInventoryData(PlayerItems player_inv, List<int[]> inv_data) {
		inv_data = new List<int[]> ();
		for (int i = 0; i < player_inv.items.Count; ++i) {
			inv_data.Add(new int[2]);
			switch(player_inv.items[i].name) {
				case "Hacha":
					inv_data[i][0] = 0;
					break;
				case "Contenedor":
					inv_data[i][0] = 1;
					break;
				case "Martillo":
					inv_data[i][0] = 2;
					break;
				case "Lupa":
					inv_data[i][0] = 3;
					break;
				case "Clavos":
					inv_data[i][0] = 4;
					break;
				case "Papel Indicador":
					inv_data[i][0] = 5;
					break;
				case "Pico":
					inv_data[i][0] = 6;
					break;
				case "Piedra":
					inv_data[i][0] = 7;
					break;
				case "Planton":
					inv_data[i][0] = 8;
					break;
				case "Pala":
					inv_data[i][0] = 9;
					break;
				case "Madera":
					inv_data[i][0] = 10;
					break;
				default:
					inv_data[i][0] = -1;
					break;
			}
			inv_data[i][1] = player_inv.quantities[i];
		}
	}

	public void updateDataPackage() {
		if (dataPackage == null) {
			dataPackage = new PlayerDataPackage ();
		}
		if (currentCharacterIsBuilder ()) {
			dataPackage.currentCharacter = BUILDER;
		} else if (currentCharacterIsExplorer ()) {
			dataPackage.currentCharacter = EXPLORER;
		} else {
			dataPackage.currentCharacter = SCIENTIST;
		}
		dataPackage.builderPosition_x = builder.transform.position.x;
		dataPackage.builderPosition_y = builder.transform.position.y;
		dataPackage.builderPosition_z = builder.transform.position.z;
		dataPackage.explorerPosition_x = explorer.transform.position.x;
		dataPackage.explorerPosition_y = explorer.transform.position.y;
		dataPackage.explorerPosition_z = explorer.transform.position.z;
		dataPackage.scientistPosition_x = scientist.transform.position.x;
		dataPackage.scientistPosition_y = scientist.transform.position.y;
		dataPackage.scientistPosition_z = scientist.transform.position.z;
		dataPackage.cam_x = Camera.main.transform.position.x;
		dataPackage.cam_y = Camera.main.transform.position.y;
		dataPackage.cam_z = Camera.main.transform.position.z;
		PlayerItems builder_items = builder.GetComponent<PlayerItems> ();
		saveInventoryData (builder_items, dataPackage.builder_inv);
		PlayerItems explorer_items = explorer.GetComponent<PlayerItems> ();
		saveInventoryData (explorer_items, dataPackage.explorer_inv);
		PlayerItems scientist_items = scientist.GetComponent<PlayerItems> ();
		saveInventoryData (scientist_items, dataPackage.scientist_inv);
	}

}

// All data that needs to be saved for correct player load must be referenced here
[Serializable]
public class PlayerDataPackage {
	public int currentCharacter;
	public float builderPosition_x;	
	public float builderPosition_y;
	public float builderPosition_z;
	public float explorerPosition_x;
	public float explorerPosition_y;
	public float explorerPosition_z;
	public float scientistPosition_x;
	public float scientistPosition_y;
	public float scientistPosition_z;
	public float cam_x;
	public float cam_y;
	public float cam_z;
	public List<int[]> builder_inv;
	public List<int[]> explorer_inv;
	public List<int[]> scientist_inv;
}
