using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour {

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

	private GameObject builder;
	private GameObject scientist;
	private GameObject explorer;

	private Player cbuilder;
	private Player cexplorer;
	private Player cscientist;

	[HideInInspector]
	public GameObject currentCharacter;

	private GameObject character2;
	private GameObject character3;

	public UpdateBackpackContents updateBackpack;
	public SetActionsMiniMenu setActions;			// CAMBIOS

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

	void Update () {
		if ( Input.GetKeyDown( KeyCode.Tab ) ) {
			// se cambia el personaje con Tab
			if (currentCharacter == builder) {
				changeCurrentCharacter(explorer);
			} else if (currentCharacter == scientist) {
				changeCurrentCharacter(builder);
			} else {
				changeCurrentCharacter(scientist);
			}
		}

		// handle left mouse button clicks
		if(Input.GetMouseButtonDown (0)) {
			if (!EventSystem.current.IsPointerOverGameObject ()) {
				// did not click on a UI element or scenery object

				if (gettingMousePositionOnWorld) {			// CAMBIOS

					// TILE INDEXES
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
						//Debug.Log("Chunk indexes: ("+chunk.index_x+", "+chunk.index_y+"), "+"Tile indexes: ("+indx+", "+indz+")");
					}
					// END OF TILE INDEXES

				} else {

					currentCharacter.GetComponent<PlayerMovement> ().checkNewDestination ();
					changeCharacterOnMouseDown ();

				}

			} 
		}
		
	}

	// object initialization
	public void init() {
		Player_Controller = gameObject;
		
		builder   = Instantiate (builderPrefab,   new Vector3(145f, 0.72f, -106.5f), Quaternion.identity) as GameObject;
		scientist = Instantiate (scientistPrefab, new Vector3(13.46f, 2.1f, -5.19f), Quaternion.identity) as GameObject;
		explorer  = Instantiate (explorerPrefab,  new Vector3(13.88f, 2.1f, -5.56f), Quaternion.identity) as GameObject;
		
		currentCharacter = builder;
		character2 = explorer;
		character3 = scientist;
		
		Camera.main.GetComponent<MoveCamera> ().target = currentCharacter;
		Camera.main.GetComponent<MoveCamera> ().following = true;

		setItems ();	

		updateCurrentCharacterUI();					
		UpdateBackpackUI ();
		setActions.UpdateUI ();

		builder.transform.Find ("AnimatedSprite").gameObject.GetComponent<CameraFace>().m_Camera = GameController.gameController.mainCamera;
		explorer.transform.Find ("AnimatedSprite").gameObject.GetComponent<CameraFace>().m_Camera = GameController.gameController.mainCamera;
		scientist.transform.Find ("AnimatedSprite").gameObject.GetComponent<CameraFace>().m_Camera = GameController.gameController.mainCamera;

		builder.GetComponent<PlayerMovement> ().init ();
		scientist.GetComponent<PlayerMovement> ().init ();
		explorer.GetComponent<PlayerMovement> ().init ();

		cbuilder = builder.GetComponent<Builder> ();
		cbuilder.init ();
		cexplorer = explorer.GetComponent<Explorer> ();
		cexplorer.init ();
		cscientist = scientist.GetComponent<Scientist> ();
		cscientist.init ();

		setPlayerActions ();

	}

	void setPlayerActions() {

		//builder actions
		//ActionChopTree act = GameController.gameController.playerActionsHolder.AddComponent<ActionChopTree> ();
		//cbuilder.addAction (act);

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
					changeCurrentCharacter(builder);
				} else if (hit.transform.gameObject == scientist) {
					changeCurrentCharacter(scientist);
				} else if (hit.transform.gameObject == explorer) {
					changeCurrentCharacter(explorer);
				}			
			}

		}
	}

	void changeCurrentCharacter(GameObject newCurrentCharacter) {
		if (newCurrentCharacter == character2) {
			character2 = currentCharacter;
			currentCharacter = newCurrentCharacter;
		} else if (newCurrentCharacter == character3) {
			character3 = currentCharacter;
			currentCharacter = newCurrentCharacter;
		}

		updateCurrentCharacterUI ();					// CAMBIO
		UpdateBackpackUI ();
		setActions.UpdateUI ();

		Camera.main.GetComponent<MoveCamera> ().target = currentCharacter;
		Camera.main.GetComponent<MoveCamera> ().following = true;

		deactivateActionsMenu ();

	}

	void updateCurrentCharacterUI() {
		if (currentCharacter == builder) {
			currentCharacterButton.GetComponent<Image> ().sprite = builderImage;
			if (character2 == scientist) {
				character2Button.GetComponent<Image> ().sprite = scientistImage;
				character3Button.GetComponent<Image> ().sprite = explorerImage;
			} else {
				character2Button.GetComponent<Image> ().sprite = explorerImage;
				character3Button.GetComponent<Image> ().sprite = scientistImage;
			}
		} else if (currentCharacter == scientist) {
			currentCharacterButton.GetComponent<Image> ().sprite = scientistImage;
			if (character2 == builder) {
				character2Button.GetComponent<Image> ().sprite = builderImage;
				character3Button.GetComponent<Image> ().sprite = explorerImage;
			} else {
				character2Button.GetComponent<Image> ().sprite = explorerImage;
				character3Button.GetComponent<Image> ().sprite = builderImage;
			}
		} else {
			currentCharacterButton.GetComponent<Image> ().sprite = explorerImage;
			if (character2 == builder) {
				character2Button.GetComponent<Image> ().sprite = builderImage;
				character3Button.GetComponent<Image> ().sprite = scientistImage;
			} else {
				character2Button.GetComponent<Image> ().sprite = scientistImage;
				character3Button.GetComponent<Image> ().sprite = builderImage;
			}
		}
	}

	public void setCurrentCharacter2(){
		changeCurrentCharacter (character2);
	}

	public void setCurrentCharacter3(){
		changeCurrentCharacter (character3);
	}

	public void triggerActionFromMenu(PlayerAction selected_action) {
	
		if (currentPlayerHasAction (selected_action)) {
			selected_action.performAction (currentCharacter, ShowMenu.clickedObject.transform.parent.gameObject);
		} 
		else {
			Debug.LogError("Active character cannot perform that action.");
		}

		ActionsMenu.activator.deactivatePanel ();

		/*SceneryType[,] sc_map = GenerateTerrain.TerrainGenerator.getChunkSceneryMap (0, 0);
		
		string output = "";
		for (int i = 0; i < 24; ++i) {
			output = output + "(";
			for (int j = 0; j < 24; ++j) {
				output = output + (int) sc_map[i, j] + " ";
			}
			output = output + ")\n";
		}
		
		DebugPanel.print (output);*/

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

		if ( currentCharacter.GetComponent<PlayerIdentifier> ().identifier == "Builder" ) {
			return cbuilder.getActions ();
		} else if ( currentCharacter.GetComponent<PlayerIdentifier> ().identifier == "Scientist" ) {
			return cscientist.getActions ();
		} else if ( currentCharacter.GetComponent<PlayerIdentifier> ().identifier == "Explorer" ) {
			return cexplorer.getActions ();
		}	

		return new List<PlayerAction> ();
		
	}

	bool currentPlayerHasAction(PlayerAction action) {
		List<PlayerAction> playerActions = getCurrentCharacterActions ();
		for (int i = 0; i < playerActions.Count; ++i) {
			if(playerActions[i].getName() == action.getName()) {
				return true;
			}
		}
		return false;
	}
	
	public PlayerItems getCurrentPlayerItems() {
		return currentCharacter.GetComponent<PlayerItems> ();
	}
	
	private void setItems () {
		PlayerItems builderItems = builder.GetComponent<PlayerItems> ();
		builderItems.addItem (new Shovel());
		builderItems.addItem (new Hammer());
		builderItems.addItem (new Nails(),15);
		builderItems.addItem (new Wood(),10);
		builderItems.addItem (new Sapling(),8);
		
		PlayerItems scientistItems = scientist.GetComponent<PlayerItems> ();
		scientistItems.addItem (new Container(),2);
		scientistItems.addItem (new MagnifGlass());
		scientistItems.addItem (new PHTest(),4);
		
		PlayerItems explorerItems = explorer.GetComponent<PlayerItems> ();
		explorerItems.addItem (new Pickaxe());
		explorerItems.addItem (new Machete());
		explorerItems.addItem (new Axe());
	}

	public void UpdateBackpackUI() {			
		updateBackpack.UpdateUI ();
	}

	public bool currentCharacterIsBuilder() {
		if (currentCharacter == builder) {
			return true;
		} else {
			return false;
		}
	}

	public bool currentCharacterIsScientist() {
		if (currentCharacter == scientist) {
			return true;
		} else {
			return false;
		}
	}

	public bool currentCharacterIsExplorer() {
		if (currentCharacter == explorer) {
			return true;
		} else {
			return false;
		}
	}

	public void clearMousePositionsOnWorld() {
		chunkIndexX = null;
		chunkIndexY = null;
		tileIndexX  = null;
		tileIndexY  = null;
	}

}
