using UnityEngine;
using System.Collections;

public class ActionStartMacroGame : MonoBehaviour {

	GameObject player;
	bool error;
	public TogglePanelButton slave; 
	Vector3 position;
	int chunkIndexX;
	int chunkIndexY;
	int tileIndexX;
	int tileIndexY;
	
	bool position_selected;
	bool invalid_position;
	bool reached_target;
	bool planted_target;
	bool action_interrupted;


	public void startGame() {
		Application.LoadLevel("MiniJuego");  // level de juego 
	}

	public void ask(GameObject pnl,GameObject pnl2) {
		pnl.SetActive(false);
		pnl2.SetActive(true);
		//txt= GameObject.Find ("Analisis");
		//txt.GetComponent<ChangeText> ().Change();
		pnl2.transform.position = new Vector3(Screen.width/2,((Screen.height-155)/2)+155, 0);

		MissionManager mm = GameController.gameController.missionController;
		Mission actionMacro;
		if (mm != null) {	// if mission manager was retrieved succesfully
			actionMacro = mm.getMissionById (31); // get mission with id = 2
			if (actionMacro != null && actionMacro.started) {	// if mission exists and has been started
				((MissionAprenderSobreMacro)actionMacro).Macro = true;	// set flag for mission
			}
		}

		
	}

	public void analize(GameObject pnl,GameObject pnl2) {
		pnl.SetActive(false);
		pnl2.SetActive(true);
		//txt= GameObject.Find ("Analisis");
		//txt.GetComponent<ChangeText> ().Change();
		pnl2.transform.position = new Vector3(Screen.width/2,((Screen.height-155)/2)+155, 0);
		
	}


}
