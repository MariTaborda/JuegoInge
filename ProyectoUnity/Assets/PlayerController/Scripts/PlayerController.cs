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

	[HideInInspector]
	public GameObject currentCharacter;

	private GameObject character2;
	private GameObject character3;

	public UpdateBackpackContents updateBackpack;

	void Start () {
		Player_Controller = gameObject;

		builder   = Instantiate (builderPrefab,   new Vector3(145f, 0.72f, -106.5f), Quaternion.identity) as GameObject;
		scientist = Instantiate (scientistPrefab, new Vector3(13.46f, 2.1f, -5.19f), Quaternion.identity) as GameObject;
		explorer  = Instantiate (explorerPrefab,  new Vector3(13.88f, 2.1f, -5.56f), Quaternion.identity) as GameObject;

		currentCharacter = builder;
		character2 = explorer;
		character3 = scientist;

		Camera.main.GetComponent<MoveCamera> ().target = currentCharacter;
		Camera.main.GetComponent<MoveCamera> ().following = true;

		updateCurrentCharacterUI();

		setItems ();	
		updateBackpack.UpdateUI ();		

	}
	
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

		changeCharacterOnMouseDown ();
		updateCurrentCharacterUI();
	}

	void deactivateActionsMenu() {
		// se desactiva el actions menu si esta activo
		if(ActionsMenu.panel.activeSelf) {
			ActionsMenu.actions_menu.disable ();
		}
	}

	void changeCharacterOnMouseDown() {
		if( Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() ) {
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
	}

	void changeCurrentCharacter(GameObject newCurrentCharacter) {
		if (newCurrentCharacter == character2) {
			character2 = currentCharacter;
			currentCharacter = newCurrentCharacter;
		} else if (newCurrentCharacter == character3) {
			character3 = currentCharacter;
			currentCharacter = newCurrentCharacter;
		}

		updateBackpack.UpdateUI ();

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
		updateCurrentCharacterUI ();
	}

	public void setCurrentCharacter3(){
		changeCurrentCharacter (character3);
		updateCurrentCharacterUI ();
	}

	public void triggerActionFromMenu() {
	
		currentCharacter.GetComponent<ActionChopTree> ().performAction (currentCharacter, ShowMenu.clickedObject.transform.parent.gameObject);
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

	public PlayerItems getCurrentPlayerItems() {
		return currentCharacter.GetComponent<PlayerItems> ();
	}
	
	private void setItems () {
		PlayerItems builderItems = builder.GetComponent<PlayerItems> ();
		builderItems.addItem (new Shovel());
		builderItems.addItem (new Hammer());
		builderItems.addItem (new Nails(),15);
		builderItems.addItem (new Wood(),10);
		builderItems.addItem (new Sapling(),2);
		
		PlayerItems scientistItems = scientist.GetComponent<PlayerItems> ();
		scientistItems.addItem (new Container(),2);
		scientistItems.addItem (new MagnifGlass());
		scientistItems.addItem (new PHTest(),4);
		
		PlayerItems explorerItems = explorer.GetComponent<PlayerItems> ();
		explorerItems.addItem (new Pickaxe());
		explorerItems.addItem (new Machete());
		explorerItems.addItem (new Axe());
	}

}
