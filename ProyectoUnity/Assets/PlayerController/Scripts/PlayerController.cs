using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

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

	// object initialization
	public void init() {

		Player_Controller = gameObject;
		
		builder   = Instantiate (builderPrefab,   new Vector3(66.3f, 1f, -146.5f), Quaternion.identity) as GameObject;
		scientist = Instantiate (scientistPrefab, new Vector3(66.0f, 1f, -146.7f), Quaternion.identity) as GameObject;
		explorer  = Instantiate (explorerPrefab,  new Vector3(66.0f, 1f, -146.7f), Quaternion.identity) as GameObject;

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
	
	private void setItems () {
		PlayerItems builderItems = builder.GetComponent<PlayerItems> ();
		//builderItems.addItem (new Shovel(), 1);
		builderItems.addItem (new Hammer(), 1);
		builderItems.addItem (new Nails(),15);
		builderItems.addItem (new Wood(),40);
		builderItems.addItem (new Sapling(),8);
		
		PlayerItems scientistItems = scientist.GetComponent<PlayerItems> ();
		scientistItems.addItem (new Container(),2);
		scientistItems.addItem (new MagnifGlass(), 1);
		scientistItems.addItem (new PHTest(),4);
		
		PlayerItems explorerItems = explorer.GetComponent<PlayerItems> ();
		explorerItems.addItem (new Pickaxe(), 1);
		explorerItems.addItem (new Machete(), 1);
		explorerItems.addItem (new Axe(), 1);
		explorerItems.addItem (new Rock(), 40);
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
}
