using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissionPlantTree : Mission {

	public static MissionPlantTree singleton;

	private int plantedTrees;


	public MissionPlantTree() {
		init ();
		singleton = this;
		plantedTrees = 0;
	}
	
	public override void init() {
		base.init ();
		id = 29;		// must be different from any other mission id
		setTitle ();
		setDescriptionText ();
		setHintsText ();
		setCompletionText ();
		setRewardsText ();
	}
	
	// change mission title here
	void setTitle() {
		title = "Recupera puntos ecologicos plantando arboles!";
	}
	
	// change mission description section text here
	void setDescriptionText() {
		description = "Siembra al menos 3 arboles para reforestar lo que se destuyo en la construccion del camino anterior.";
		description += "\n\nPara esta mision utiliza al constructor.";
		description += "\n\n¡Recupera 5 puntos ecologicos por cada arbol sembrado!";
	}
	
	// change mission hints here
	void setHintsText() {
		hints = new List<string> ();
		hints.Add ("Selecciona uno de los 9 arboles de la barra inferior de la pantalla.");
		hints.Add ("Arrastra el arbol seleccionado a la zona en donde deseas plantarlo.");
	}
	
	// change mission completion section text here
	void setCompletionText() {
		completion_text = "¡Bien Hecho! Has completado la mision, regresa a la bandera con tus otros compañeros.";
	}
	
	// change mission rewards section text here
	void setRewardsText() {
		rewards = new List<string> ();
		rewards.Add ("5 Eco puntos");
	}
	
	/*
	Write your method for evaluating requirements for mission start here.
	return:
		true - starting requirements are met.
		false - starting requirements are not met.
 	*/
	public override bool evaluateRequirements() {
		// check if requirements for mission start are met
		if (GameController.gameController.missionController.missionCompleted (28)) {		
			return true;
		}
		return false;
	}
	
	public override bool evaluateConditions() {

		Vector3 new_builder_pos = GameController.gameController.playerController.builder.transform.position;
		Vector3 new_explorer_pos = GameController.gameController.playerController.explorer.transform.position;
		Vector3 new_scientist_pos = GameController.gameController.playerController.scientist.transform.position;
		
		Vector3 banderaFinal = new Vector3 (66, 0, -146);
		
		return plantedTrees >= 3 && Vector3.Distance(new_explorer_pos, banderaFinal) <= 5 && Vector3.Distance(new_builder_pos, banderaFinal) <= 5 && Vector3.Distance(new_scientist_pos, banderaFinal) <= 5;

		return true;
	}

	public void onPlantTree() {
		plantedTrees++;
	}

	//Write your method for redeeming mission rewards here.
	public override void redeemRewards() {
		// Add rewards to player. (items, eco points, etc.)
		PlayerController.Player_Controller.GetComponent<PlayerPoints> ().addEcologyPoints (5);
	}
	
	//This will happen on mission start.
	public override void startMission() {
		base.startMission ();	// required
		
	}
	
	//This will happen on mission completion.
	public override void completeMission() {
		base.completeMission ();	// required
	}
	
}
