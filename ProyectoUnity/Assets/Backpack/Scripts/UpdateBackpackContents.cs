using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UpdateBackpackContents : MonoBehaviour {

	public GameObject button;

	private PlayerController playerController;
	
	void Awake () {
		playerController = GameObject.Find("Player Controller").GetComponent<PlayerController>();
	}

	public void UpdateUI(){
		PlayerItems items = playerController.getCurrentPlayerItems ();
		RectTransform rectTrans = GetComponent<RectTransform> ();

		int numberOfButtons = transform.childCount;
		int numberOfItems = items.count();

		// Se agregan botones si no hay suficientes
		if ( numberOfButtons < numberOfItems ) {
			for (int i = numberOfButtons; i < numberOfItems; ++i ) {
				int row = i/6;
				int column = i%6;

				GameObject newButton = Instantiate(button, new Vector2(65*column + 5, -65*row - 5), Quaternion.Euler(Vector3.zero)) as GameObject;
				newButton.transform.SetParent(transform, false);
			}
		}

		// Se colocan los items en los botones
		int butt = 0;
		foreach (Transform child in transform)
		{
			// Se modifica cada boton para que tenga la info de un item
			GameObject theButton = child.gameObject;

			if ( butt < numberOfItems ) {

				BackpackItem item = items.getItemByPosition(butt);
				int amount = items.getAmountByPosition(butt);
				theButton.SetActive(true);

				Component[] childText;
				childText = theButton.GetComponentsInChildren<Text>();

				// Se setea la imagen del boton
				Image image = theButton.GetComponentsInChildren<Image>()[1];
				image.sprite = item.image;

				// Se setea la cantidad del item
				Text text = theButton.GetComponentsInChildren<Text>()[0];
				if (item.stackable){
					text.text = amount.ToString();
				} else {
					text.text = "";
				}

			} else {
				// Se deshabilita el boton
				theButton.SetActive(false);
			}

			++butt;
		}

		// Se modifica la altura del panel segun la cantidad de items que hay
		Vector2 sizeDelta = rectTrans.sizeDelta;
		int rows = numberOfItems/6;
		if (numberOfItems%6 != 0) {
			++rows;
		}
		if (rows <= 2) {
			int height = 65*2 + 5;
			rectTrans.sizeDelta = new Vector2 (sizeDelta.x, height);
		} else {
			int height = 65*rows + 5;
			rectTrans.sizeDelta = new Vector2 (sizeDelta.x, height);
		}

	}
}
