using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissionChopTree : Mission {

	public static MissionChopTree singleton;

	private int destroyedTrees;

	public MissionChopTree() {
		init ();
		destroyedTrees = 0;
		singleton = this;
	}
	
	public override void init() {
		base.init ();
		id = 28;		// must be different from any other mission id
		setTitle ();
		setDescriptionText ();
		setHintsText ();
		setCompletionText ();
		setRewardsText ();
	}
	
	// change mission title here
	void setTitle() {
		title = "Atraviesa el bosque!";
	}
	
	// change mission description section text here
	void setDescriptionText() {
		description = "Crea un nuevo camino entre los arboles, de manera que puedas llegar a la bandera final.";
		description += "\n\nPara esta mision utiliza al explorador y corta los arboles que se interpongan.";
		description += "\n\n¡Recuerda que por cada arbol que se corte, te restaran 5 puntos ecologicos!";
	}
	
	// change mission hints here
	void setHintsText() {
		hints = new List<string> ();
		hints.Add ("Presiona click derecho sobre el arbol que desees cortar, y selecciona la opcion de cortar arbol");
	}
	
	// change mission completion section text here
	void setCompletionText() {
		completion_text = "¡Bien Hecho! Hemos conseguido llegar hasta aqui, pero.. creo que hemos hecho algo de daño en el proceso. Vamos a repararlo.";
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
		if (GameController.gameController.missionController.missionCompleted (6)) {		
			return true;
		}
		return false;
	}
	
	public override bool evaluateConditions() {

		Vector3 new_builder_pos = GameController.gameController.playerController.builder.transform.position;
		Vector3 new_explorer_pos = GameController.gameController.playerController.explorer.transform.position;
		Vector3 new_scientist_pos = GameController.gameController.playerController.scientist.transform.position;

		Vector3 banderaFinal = new Vector3 (66, 0, -146);

		return destroyedTrees >= 1 && Vector3.Distance(new_explorer_pos, banderaFinal) <= 5 && Vector3.Distance(new_builder_pos, banderaFinal) <= 5 && Vector3.Distance(new_scientist_pos, banderaFinal) <= 5;
	}

	public void onDestroyTree() {
		destroyedTrees++;
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
