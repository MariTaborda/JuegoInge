using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class ShowMenu : MonoBehaviour {

	public GameObject panel;
	public GameObject panel2;
	public GameObject panel3;
	public GameObject panel4;
	public GameObject panel5;
	public int cont=0;
	public int cont1=0;
	public int cont2=0;
	public int cont3=0;

	public RectTransform canvasRT;
	private RectTransform panelRT;

	private Vector2 pointerOffset;

	public static GameObject clickedObject;

	public void setShowMenu() {
		panel = ActionsMenu.actions_menu.gameObject;
		canvasRT = GameController.gameController.hudCanvas.GetComponent<RectTransform> ();
		setPanelRT ();
	}

	void Start () {
		//setPanelRT();  
	}

	public void setPanelRT() {
		panelRT = panel.GetComponent <RectTransform> ();  
	}

	public void deactivatePanel() {
		panel.SetActive(false);
	}

	public void activatePanelOnPlayer() {
		panel.SetActive(false);
		Vector3 screenPos = Camera.main.WorldToScreenPoint (GameController.gameController.playerController.currentCharacter().transform.position);
		
		float posX = screenPos.x;
		float posY = screenPos.y;
		
		float width = panelRT.sizeDelta.x;
		float height = panelRT.sizeDelta.y;
		width = width + 30f;
		if (screenPos.y > height + 180f) {
			// El 120 es para dejar un colchon para que no salga sobre el lower menu
			posY = posY - height;
		}
		
		if (screenPos.x > width) {
			posX = posX - width;
		}
		
		panelRT.anchoredPosition = new Vector2 (posX, posY);
		
		panelRT.SetAsLastSibling ();
		panel.SetActive (true);
		
		ActionsMenu.activator = this;
		
	}

	public void activatePanel() {
		panel.SetActive(false);
		Vector3 screenPos = Camera.main.WorldToScreenPoint (transform.position);
		
		float posX = screenPos.x;
		float posY = screenPos.y;
		
		float width = panelRT.sizeDelta.x;
		float height = panelRT.sizeDelta.y;
		
		if (screenPos.y > height + 180f) {
			// El 120 es para dejar un colchon para que no salga sobre el lower menu
			posY = posY - height;
		}
		
		if (screenPos.x > width) {
			posX = posX - width;
		}

		panelRT.anchoredPosition = new Vector2 (posX, posY);
		
		panelRT.SetAsLastSibling ();
		panel.SetActive (true);

		ActionsMenu.activator = this;

	}

	void setActions(List<PlayerAction> actions) {
		for (int i = 0; i < actions.Count; ++i) {
			addActionButton(actions[i]);
		}
	}

	void addActionButton(PlayerAction action) {
		GameObject actionButton = GameObject.Instantiate (ActionsMenu.actions_menu.action_button_prefab);
		actionButton.transform.SetParent (ActionsMenu.panel.transform, false);
		actionButton.transform.GetChild(0).gameObject.GetComponent<Text> ().text = action.getActionName();
		actionButton.GetComponent<Button>().onClick.AddListener(() => { triggerAction(action); });
	}

	void addCloseActionsMenuButton() {
		GameObject closeButton = GameObject.Instantiate (ActionsMenu.actions_menu.close_button_prefab);
		closeButton.transform.SetParent (ActionsMenu.panel.transform, false);
		closeButton.GetComponent<Button>().onClick.AddListener(() => { deactivatePanel(); });
	}

	public void restoreActionsMenu() {
		foreach (Transform child in ActionsMenu.panel.transform) {
			GameObject.Destroy(child.gameObject);
		}
		addCloseActionsMenuButton ();
	}

	void triggerAction(PlayerAction action) {
		GameController.gameController.playerController.triggerActionFromMenu (action);
	}

	public void openMenu () {
		if (!EventSystem.current.IsPointerOverGameObject ()) {
			List<PlayerAction> compatibleActions = GameController.gameController.playerController.getCurrentCharacterActionsOnObject (gameObject.transform.parent.gameObject);
			if (compatibleActions.Count > 0) {
				clickedObject = gameObject;
				restoreActionsMenu ();
				setActions (compatibleActions);
				activatePanel ();
			}	
			
		}
	}

	/*public void opentxtMenu () {
		restoreActionsMenu();
		//Poner texto de 
		activatePanelOnPlayer ();

	}*/

	public void openMenuWaterSample () {

		restoreActionsMenu();
		PlayerController pc = PlayerController.Player_Controller.GetComponent<PlayerController>();

		GameObject actionButton1 = GameObject.Instantiate (ActionsMenu.actions_menu.action_button_prefab);
		actionButton1.transform.position = new Vector3(-92, 5, 0);
		if (cont == 0) {
			panel2 = GameObject.Find ("Panel resultados back");
			cont++;
		}
		panel2.SetActive(false);

		actionButton1.transform.SetParent (ActionsMenu.panel.transform, false);
		actionButton1.transform.GetChild(0).gameObject.GetComponent<Text> ().text = "PH";
		actionButton1.GetComponent<Button>().onClick.AddListener(() => GameController.gameController.playerActionsHolder.GetComponent<ActionWaterPH> ().performAction(panel, panel2, 0));
		
		GameObject actionButton2 = GameObject.Instantiate (ActionsMenu.actions_menu.action_button_prefab);

		if (cont3 == 0) {

			panel5 = GameObject.Find("PanelO2");
			cont3++;
		}
		panel5.SetActive(false);

		actionButton2.transform.SetParent (ActionsMenu.panel.transform, false);
		actionButton2.transform.GetChild(0).gameObject.GetComponent<Text> ().text = "O2";
		actionButton2.GetComponent<Button>().onClick.AddListener(() => GameController.gameController.playerActionsHolder.GetComponent<ActionWaterO2> ().performAction(panel, panel5));
		

		activatePanelOnPlayer ();

	}

	public void openMenuMacroSample () {
		restoreActionsMenu();
		panelRT.sizeDelta = new Vector2 (195, 55);
		
		GameObject askButton = GameObject.Instantiate (ActionsMenu.actions_menu.action_button_prefab);
		//askbutton.transform.position = new Vector3(-92, 5, 0);
		if (cont1 == 0) {
			panel3 = GameObject.Find ("AskPanel");
			cont1++;
		}
		panel3.SetActive(false);
		askButton.transform.position = new Vector3(-145, 5, 0);
		askButton.GetComponent<RectTransform> ().sizeDelta = new Vector2 (30,45);
		askButton.transform.SetParent (ActionsMenu.panel.transform, false);
		askButton.transform.GetChild(0).gameObject.GetComponent<Text> ().text = "?";
		
		askButton.GetComponent<Button>().onClick.AddListener(() => GameController.gameController.playerActionsHolder.GetComponent<ActionStartMacroGame> ().ask(panel, panel3));
		
		PlayerController pc = PlayerController.Player_Controller.GetComponent<PlayerController>();
		GameObject actionButton1 = GameObject.Instantiate (ActionsMenu.actions_menu.action_button_prefab);
		if (cont2 == 0) {
			panel4 = GameObject.Find ("TableMacroPanel");
			cont2++;
		}
		panel4.SetActive(false);
		actionButton1.transform.position = new Vector3(-115, 5, 0);
		actionButton1.GetComponent<RectTransform> ().sizeDelta = new Vector2 (55,45);
		actionButton1.transform.SetParent (ActionsMenu.panel.transform, false);
		actionButton1.transform.GetChild(0).gameObject.GetComponent<Text> ().text = "Analizar Macro";
		actionButton1.GetComponent<Button>().onClick.AddListener(() => GameController.gameController.playerActionsHolder.GetComponent<ActionStartMacroGame> ().analize(panel, panel4));
		
		GameObject actionButton2 = GameObject.Instantiate (ActionsMenu.actions_menu.action_button_prefab);
		actionButton2.transform.position = new Vector3(-60, 5, 0);
		actionButton2.GetComponent<RectTransform> ().sizeDelta = new Vector2 (55,45);
		actionButton2.transform.SetParent (ActionsMenu.panel.transform, false);
		actionButton2.transform.GetChild(0).gameObject.GetComponent<Text> ().text = "Capturar Macro";
		actionButton2.GetComponent<Button>().onClick.AddListener(() => GameController.gameController.playerActionsHolder.GetComponent<ActionStartMacroGame> ().startGame());
		
		activatePanelOnPlayer ();
		
	}

	void OnMouseDown() {
		openMenu ();

	}
}
