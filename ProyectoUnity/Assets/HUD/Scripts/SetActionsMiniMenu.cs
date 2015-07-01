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
		GameObject button4 = transform.GetChild(4).gameObject;
		GameObject button5 = transform.GetChild(5).gameObject;
		GameObject button6 = transform.GetChild(6).gameObject;
		GameObject button7 = transform.GetChild(7).gameObject;
		GameObject button8 = transform.GetChild(8).gameObject;
		GameObject button9 = transform.GetChild(9).gameObject;
		GameObject button10 = transform.GetChild(10).gameObject;
		GameObject button11 = transform.GetChild(11).gameObject;
		GameObject button12 = transform.GetChild(12).gameObject;
		GameObject button13 = transform.GetChild(13).gameObject;

		if (playerController.currentCharacterIsBuilder ()) {
			button0.SetActive(false);
			
			button1.SetActive(false);

			button2.SetActive(false);

			button3.SetActive(false);
			
			//Sembrar arbol delgado
			button4.SetActive(true);
			button4.GetComponentsInChildren<Text>()[0].text = "";
			button4.GetComponent<Button>().onClick.RemoveAllListeners();
			button4.GetComponent<Button>().onClick.AddListener(() => GameController.gameController.playerActionsHolder.GetComponent<ActionPlantSapling> ().performAction(playerController.currentCharacter(),1));

			//Sembrar Arbol Grueso
			button5.SetActive(true);
			button5.GetComponentsInChildren<Text>()[0].text = "";
			button5.GetComponent<Button>().onClick.RemoveAllListeners();
			button5.GetComponent<Button>().onClick.AddListener(() => GameController.gameController.playerActionsHolder.GetComponent<ActionPlantSapling> ().performAction(playerController.currentCharacter(),2));
			
			//Sembrar Palmera Grande
			button6.SetActive(true);
			button6.GetComponentsInChildren<Text>()[0].text = "";
			button6.GetComponent<Button>().onClick.RemoveAllListeners();
			button6.GetComponent<Button>().onClick.AddListener(() => GameController.gameController.playerActionsHolder.GetComponent<ActionPlantSapling> ().performAction(playerController.currentCharacter(),3));
			
			//Sembrar Palmera Pequeña
			button7.SetActive(true);
			button7.GetComponentsInChildren<Text>()[0].text = "";
			button7.GetComponent<Button>().onClick.RemoveAllListeners();
			button7.GetComponent<Button>().onClick.AddListener(() => GameController.gameController.playerActionsHolder.GetComponent<ActionPlantSapling> ().performAction(playerController.currentCharacter(),4));
			
			//Sembrar Arbusto Grande
			button8.SetActive(true);
			button8.GetComponentsInChildren<Text>()[0].text = "";
			button8.GetComponent<Button>().onClick.RemoveAllListeners();
			button8.GetComponent<Button>().onClick.AddListener(() => GameController.gameController.playerActionsHolder.GetComponent<ActionPlantSapling> ().performAction(playerController.currentCharacter(),5));
			
			//Sembrar Arbusto Pequeño
			button9.SetActive(true);
			button9.GetComponentsInChildren<Text>()[0].text = "";
			button9.GetComponent<Button>().onClick.RemoveAllListeners();
			button9.GetComponent<Button>().onClick.AddListener(() => GameController.gameController.playerActionsHolder.GetComponent<ActionPlantSapling> ().performAction(playerController.currentCharacter(),6));
			
			//Sembrar Arbusto Super Pequeño
			button10.SetActive(true);
			button10.GetComponentsInChildren<Text>()[0].text = "";
			button10.GetComponent<Button>().onClick.RemoveAllListeners();
			button10.GetComponent<Button>().onClick.AddListener(() => GameController.gameController.playerActionsHolder.GetComponent<ActionPlantSapling> ().performAction(playerController.currentCharacter(),7));
			
			//Sembrar Arbusto Grande
			button11.SetActive(true);
			button11.GetComponentsInChildren<Text>()[0].text = "";
			button11.GetComponent<Button>().onClick.RemoveAllListeners();
			button11.GetComponent<Button>().onClick.AddListener(() => GameController.gameController.playerActionsHolder.GetComponent<ActionPlantSapling> ().performAction(playerController.currentCharacter(),8));
			
			//Sembrar Palmera Gorda
			button12.SetActive(true);
			button12.GetComponentsInChildren<Text>()[0].text = "";
			button12.GetComponent<Button>().onClick.RemoveAllListeners();
			button12.GetComponent<Button>().onClick.AddListener(() => GameController.gameController.playerActionsHolder.GetComponent<ActionPlantSapling> ().performAction(playerController.currentCharacter(),9));
			
			//Construir Puente
			button13.SetActive(true);
			button13.GetComponentsInChildren<Text>()[0].text = "";
			button13.GetComponent<Button>().onClick.RemoveAllListeners();
			button13.GetComponent<Button>().onClick.AddListener(() => GameController.gameController.playerActionsHolder.GetComponent<ActionCreateBridge> ().performAction(playerController.currentCharacter(), null));
			
		} else if (playerController.currentCharacterIsScientist ()) {
			button0.SetActive(true);
			button0.GetComponentsInChildren<Text>()[0].text = "";
			button0.GetComponent<Button>().onClick.RemoveAllListeners();
			button0.GetComponent<Button>().onClick.AddListener(() => GameController.gameController.playerActionsHolder.GetComponent<ActionCollectWaterSample> ().performAction(playerController.currentCharacter(), null));


			button1.SetActive(true);
			button1.GetComponentsInChildren<Text>()[0].text = "";
			button1.GetComponent<Button>().onClick.RemoveAllListeners();
			button1.GetComponent<Button>().onClick.AddListener(() => GameController.gameController.playerActionsHolder.GetComponent<ActionCollectMacro> ().performAction(playerController.currentCharacter(), null));


			button2.SetActive(false);
			
			button3.SetActive(false);
			button4.SetActive(false);
			button5.SetActive(false);
			button6.SetActive(false);
			button7.SetActive(false);
			button8.SetActive(false);
			button9.SetActive(false);
			button10.SetActive(false);
			button11.SetActive(false);
			button12.SetActive(false);
			button13.SetActive(false);
			
		} else {
			button0.SetActive(false);	

			button1.SetActive(false);	

			//Crear Camino
			button2.SetActive(true);
			button2.GetComponentsInChildren<Text>()[0].text = "";
			button2.GetComponent<Button>().onClick.RemoveAllListeners();
			button2.GetComponent<Button>().onClick.AddListener(() => GameController.gameController.playerActionsHolder.GetComponent<ActionCreateRoad> ().performAction(playerController.currentCharacter(), null));
			
			//Borrar Camino
			button3.SetActive(true);
			button3.GetComponentsInChildren<Text>()[0].text = "";
			button3.GetComponent<Button>().onClick.RemoveAllListeners();
			button3.GetComponent<Button>().onClick.AddListener(() => GameController.gameController.playerActionsHolder.GetComponent<ActionDestroyRoad> ().performAction(playerController.currentCharacter(), null));
			
			button4.SetActive(false);
			button5.SetActive(false);
			button6.SetActive(false);
			button7.SetActive(false);
			button8.SetActive(false);
			button9.SetActive(false);
			button10.SetActive(false);
			button11.SetActive(false);
			button12.SetActive(false);
			button13.SetActive(false);
		}

	}
}
