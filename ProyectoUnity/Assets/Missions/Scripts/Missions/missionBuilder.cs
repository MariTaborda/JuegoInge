using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissionBuilder : Mission {
	
	// mission specific attributes
	public int saplingAmount;
	
	public MissionBuilder() {
		init ();
	}
	
	public override void init() {
		base.init ();
		id = 3;		// must be different from any other mission id
		setTitle ();
		setDescriptionText ();
		setHintsText ();
		setCompletionText ();
		setRewardsText ();
	}
	
	// change mission title here
	void setTitle() {
		title = "Un arbol a la vez";
	}
	
	// change mission description section text here
	void setDescriptionText() {
		description = "Seleciona el personaje del constructor, y el en panel que esta abajo, al lado del mapa, selecciona un arbol.";
		description += "\n\nEste panel sirve para sembrar nuevos arboles! Pruebalo.";
	}
	
	// change mission hints here
	void setHintsText() {
		hints = new List<string> ();
		hints.Add ("Despues de seleccionar el arbol en el panel, debes seleccionar la posicion en el mundo en que lo quieres colocar.");
		hints.Add ("Solo puedes colocar arboles donde no hayan otros objetos.");
	}
	
	// change mission completion section text here
	void setCompletionText() {
		completion_text = "¡Bien! Por cada arbol que siembres ganas mas Eco Puntos!";
	}
	
	// change mission rewards section text here
	void setRewardsText() {
		rewards = new List<string> ();
		rewards.Add ("5 Eco puntos");
		rewards.Add ("Un aire mas limpio");
	}
	
	/*
	Write your method for evaluating requirements for mission start here.
	return:
		true - starting requirements are met.
		false - starting requirements are not met.
 	*/
	public override bool evaluateRequirements() {
		// check if requirements for mission start are met
		if (GameController.gameController.missionController.missionCompleted (2)) {		// if mission with id 2 is completed
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
		int newSaplingAmount = GameController.gameController.playerController.builder.GetComponent<PlayerItems> ().getItemAmount (new Sapling());
		if (newSaplingAmount < saplingAmount) {
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
		saplingAmount = GameController.gameController.playerController.builder.GetComponent<PlayerItems> ().getItemAmount (new Sapling());
	}
	
	//This will happen on mission completion.
	public override void completeMission() {
		base.completeMission ();	// required
	}
	
}
