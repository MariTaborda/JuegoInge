using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SetActionsMiniMenu : MonoBehaviour {

	public PlayerController playerController;

	public void UpdateUI () {

		GameObject button0 = transform.GetChild(0).gameObject;
		GameObject button1 = transform.GetChild(1).gameObject;
		GameObject button2 = transform.GetChild(2).gameObject;
		GameObject button3 = transform.GetChild(3).gameObject;

		if (playerController.currentCharacterIsBuilder ()) {
			button0.SetActive(true);
			button0.GetComponentsInChildren<Text>()[0].text = "Sembrar Planton";
			button0.GetComponent<Button>().onClick.RemoveAllListeners();
			button0.GetComponent<Button>().onClick.AddListener(() => GameController.gameController.playerActionsHolder.GetComponent<ActionPlantSapling> ().performAction(playerController.currentCharacter));

			button1.SetActive(true);
			button1.GetComponentsInChildren<Text>()[0].text = "Construir Puente";

			button2.SetActive(false);

			button3.SetActive(false);

		} else if (playerController.currentCharacterIsScientist ()) {
			button0.SetActive(false);
			
			button1.SetActive(false);

			button2.SetActive(false);
			
			button3.SetActive(false);
		} else {
			button0.SetActive(true);
			button0.GetComponentsInChildren<Text>()[0].text = "Crear Camino";
			button0.GetComponent<Button>().onClick.RemoveAllListeners();
			button0.GetComponent<Button>().onClick.AddListener(() => GameController.gameController.playerActionsHolder.GetComponent<ActionCreateRoad> ().performAction(playerController.currentCharacter));

			button1.SetActive(false);

			button2.SetActive(false);
			
			button3.SetActive(false);

		}

	}
}
