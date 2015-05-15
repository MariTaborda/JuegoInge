using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

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
		
		clickedObject = gameObject;

		ActionsMenu.activator = this;

	}

	void OnMouseDown() {
		
		if (!EventSystem.current.IsPointerOverGameObject ()) {

			activatePanel ();

			
			/*// Builds a ray from camera point of view to the mouse position
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			// Casts the ray and get the first game object hit
			if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
				clickedObject = hit.transform.gameObject;
			}*/
				
		} 

	}
}
