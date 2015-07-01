using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissionMoving : Mission {

	// mission specific attributes
	Vector3 builder_initial_position;
	Vector3 explorer_initial_position;
	Vector3 scientist_initial_position;

	public MissionMoving() {
		init ();
	}

	public override void init() {
		base.init ();
		id = 1;		// must be different from any other mission id
		setTitle ();
		setDescriptionText ();
		setHintsText ();
		setCompletionText ();
		setRewardsText ();
	}

	// change mission title here
	void setTitle() {
		title = "Un pequeño paso para el hombre...";
	}

	// change mission description section text here
	void setDescriptionText() {
		description = "Para empezar a jugar, muevete dando click donde te gustaria ir. ¿Facil no?";
	}

	// change mission hints here
	void setHintsText() {
		hints = new List<string> ();
		hints.Add ("Te puedes mover a las posiciones en el terreno que no contienen objetos.");
	}

	// change mission completion text section here
	void setCompletionText() {
		completion_text = "¡Lo has logrado! Solo lo tendras que hacer un par de cientos de veces mas para ganar el nivel.";
	}

	// change mission rewards section text here
	void setRewardsText() {
		rewards = new List<string> ();
		rewards.Add ("5 Eco puntos");
		rewards.Add ("Una nueva pala");
	}
		
	/*
	Write your method for evaluating requirements for mission start here.
	return:
		true - starting requirements are met.
		false - starting requirements are not met.
 	*/
	public override bool evaluateRequirements() {
		// check if requirements for mission start are met
		if (GameController.gameController.missionController.missionCompleted (0)) {		// if mission with id 0 is completed
			return true;
		}
		return false;
	}

	/*
	Write your method for evaluating conditions for mission completion here.
	return:
		true - completion conditions are met.
		false - completion conditions are not met.
 	*/
	public override bool evaluateConditions() {

		Vector3 new_builder_pos = GameController.gameController.playerController.builder.transform.position;
		Vector3 new_explorer_pos = GameController.gameController.playerController.explorer.transform.position;
		Vector3 new_scientist_pos = GameController.gameController.playerController.scientist.transform.position;

		// check if players have moved since mission started
		if (new_builder_pos != builder_initial_position || new_explorer_pos != explorer_initial_position || new_scientist_pos != scientist_initial_position) {
			return true;
		}

		return false;
	}
	
	//Write your method for redeeming mission rewards here.
	public override void redeemRewards() {
		GameController.gameController.playerController.giveItemsToBuilder (new Shovel (), 1);	// put a shovel in builders inventory
		PlayerController.Player_Controller.GetComponent<PlayerPoints> ().addEcologyPoints (5);
	}
	
	//This will happen on mission start.
	public override void startMission() {
		base.startMission ();	// required

		// set initial player positions
		builder_initial_position = GameController.gameController.playerController.builder.transform.position;
		explorer_initial_position = GameController.gameController.playerController.explorer.transform.position;
		scientist_initial_position = GameController.gameController.playerController.scientist.transform.position;
	}

	//This will happen on mission completion.
	public override void completeMission() {
		base.completeMission ();	// required
	}

}
