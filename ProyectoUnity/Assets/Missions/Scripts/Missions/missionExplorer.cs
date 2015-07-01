using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissionExplorer : Mission {
	
	// mission specific attributes
	public int ecologicPoints;
	
	public MissionExplorer() {
		init ();
	}
	
	public override void init() {
		base.init ();
		id = 4;		// must be different from any other mission id
		setTitle ();
		setDescriptionText ();
		setHintsText ();
		setCompletionText ();
		setRewardsText ();
	}
	
	// change mission title here
	void setTitle() {
		title = "Tala controlada";
	}
	
	// change mission description section text here
	void setDescriptionText() {
		description = "Seleciona el personaje del explorador, y haz click en el arbol que acabas de sembrar para cortarlo.";
	}
	
	// change mission hints here
	void setHintsText() {
		hints = new List<string> ();
		hints.Add ("Para cortar el arbol haz click en la opcion que dice: 'Cortar Arbol' o 'Cortar Arbusto'.");
		hints.Add ("Solo el explorador es capaz de cortar arboles porque es el unico que tiene un hacha.");
	}
	
	// change mission completion section text here
	void setCompletionText() {
		completion_text = "Por cada arbol que cortas pierdes Eco Puntos, pero a veces no hay otra opcion mas que cortarlo. Lo importante es cortarlos con moderacion y recordar sembrar nuevos arboles!";
	}
	
	// change mission rewards section text here
	void setRewardsText() {
		rewards = new List<string> ();
		rewards.Add ("Un camino despejado");
	}
	
	/*
	Write your method for evaluating requirements for mission start here.
	return:
		true - starting requirements are met.
		false - starting requirements are not met.
 	*/
	public override bool evaluateRequirements() {
		// check if requirements for mission start are met
		if (GameController.gameController.missionController.missionCompleted (3)) {		// if mission with id 1 is completed
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
		int newPoints = GameController.gameController.playerController.GetComponent<PlayerPoints> ().getEcologyPoints ();
		if (newPoints < ecologicPoints) {
			return true;
		}
		return false;
	}
	
	//Write your method for redeeming mission rewards here.
	public override void redeemRewards() {
		// Add rewards to player. (items, eco points, etc.)
		//PlayerController.Player_Controller.GetComponent<PlayerPoints> ().addEcologyPoints (5);
	}
	
	//This will happen on mission start.
	public override void startMission() {
		base.startMission ();	// required
		
		// initialize flag for completion evaluation
		ecologicPoints = GameController.gameController.playerController.GetComponent<PlayerPoints> ().getEcologyPoints ();
	}
	
	//This will happen on mission completion.
	public override void completeMission() {
		base.completeMission ();	// required
	}
	
}