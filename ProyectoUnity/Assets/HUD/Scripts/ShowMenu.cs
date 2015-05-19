using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class ShowMenu : MonoBehaviour {

	public GameObject panel;
	public RectTransform canvasRT;
	private RectTransform panelRT;

	private Vector2 pointerOffset;

	public static GameObject clickedObject;

	public void setShowMenu() {
		panel = ActionsMenu.actions_menu.gameObject;
		canvasRT = HUDCanvas.hud_canvas.GetComponent<RectTransform> ();
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
		actionButton.transform.GetChild(0).gameObject.GetComponent<Text> ().text = action.getName();
		actionButton.GetComponent<Button>().onClick.AddListener(() => { triggerAction(action); });
	}

	void addCloseActionsMenuButton() {
		GameObject closeButton = GameObject.Instantiate (ActionsMenu.actions_menu.close_button_prefab);
		closeButton.transform.SetParent (ActionsMenu.panel.transform, false);
		closeButton.GetComponent<Button>().onClick.AddListener(() => { deactivatePanel(); });
	}

	void restoreActionsMenu() {
		foreach (Transform child in ActionsMenu.panel.transform) {
			GameObject.Destroy(child.gameObject);
		}
		addCloseActionsMenuButton ();
	}

	void triggerAction(PlayerAction action) {
		GameController.gameController.playerController.triggerActionFromMenu (action);
	}

	void OnMouseDown() {
		
		if (!EventSystem.current.IsPointerOverGameObject ()) {

			List<PlayerAction> compatibleActions = GameController.gameController.playerController.getCurrentCharacterActionsOnObject (gameObject.transform.parent.gameObject);
			if(compatibleActions.Count > 0) {
				clickedObject = gameObject;
				restoreActionsMenu();
				setActions(compatibleActions);
				activatePanel ();
			}	

		} 

	}
}
