using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissionSelectCharacter : Mission {

	// mission specific attributes
	public bool changedCharacters;
	
	public MissionSelectCharacter() {
		init ();
	}
	
	public override void init() {
		base.init ();
		id = 2;		// must be different from any other mission id
		setTitle ();
		setDescriptionText ();
		setHintsText ();
		setCompletionText ();
		setRewardsText ();
	}
	
	// change mission title here
	void setTitle() {
		title = "3 son mejor que 1";
	}
	
	// change mission description section text here
	void setDescriptionText() {
		description = "Tienes 3 personajes para usar en cualquier momento; el constructor, el explorador y el cientifico.";
		description += "\n\nCada uno tiene funciones diferentes. Adelante, pruebalos.";
	}
	
	// change mission hints here
	void setHintsText() {
		hints = new List<string> ();
		hints.Add ("Usa TAB para cambiar entre los personajes.");
		hints.Add ("Tambien puedes hacer click en los retratos de los personajes en la esquina superior izquierda.");
	}
	
	// change mission completion section text here
	void setCompletionText() {
		completion_text = "¡Bien! A menudo tendras que usar varios personajes en conjunto para cumplir tus objetivos. Ten eso en cuenta.";
	}
	
	// change mission rewards section text here
	void setRewardsText() {
		rewards = new List<string> ();
		rewards.Add ("5 Eco puntos");
		rewards.Add ("La habilidad de estar en 3 lugares a la vez");
	}
	
	/*
	Write your method for evaluating requirements for mission start here.
	return:
		true - starting requirements are met.
		false - starting requirements are not met.
 	*/
	public override bool evaluateRequirements() {
		// check if requirements for mission start are met
		if (GameController.gameController.missionController.missionCompleted (1)) {		// if mission with id 1 is completed
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
		if (changedCharacters) {		// flag is set in player controller
			return true;
		}
		return false;
	}
	
	//Write your method for redeeming mission rewards here.
	public override void redeemRewards() {
		// Add rewards to player. (items, eco points, etc.)
		PlayerController.Player_Controller.GetComponent<PlayerPoints> ().addEcologyPoints (5);
	}
	
	//This will happen on mission start.
	public override void startMission() {
		base.startMission ();	// required
		
		// initialize flag for completion evaluation
		changedCharacters = false;
	}
	
	//This will happen on mission completion.
	public override void completeMission() {
		base.completeMission ();	// required
	}

}
