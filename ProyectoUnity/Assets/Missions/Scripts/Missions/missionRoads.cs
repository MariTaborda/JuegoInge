using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissionRoads : Mission {
	
	// mission specific attributes
	public int rocksAmount;
	
	public MissionRoads() {
		init ();
	}
	
	public override void init() {
		base.init ();
		id = 5;		// must be different from any other mission id
		setTitle ();
		setDescriptionText ();
		setHintsText ();
		setCompletionText ();
		setRewardsText ();
	}
	
	// change mission title here
	void setTitle() {
		title = "Un camino seguro";
	}
	
	// change mission description section text here
	void setDescriptionText() {
		description = "El cientifico puede tomar muestras de agua, pero para eso primero tienes que llegar al rio.";
		description += "\n\nEl cientifico no puede caminar rapido por el bosque porque podria regar sus muestras, entonces el explorador tiene que crear caminos para el.";
	}
	
	// change mission hints here
	void setHintsText() {
		hints = new List<string> ();
		hints.Add ("Hacer caminos con el explorador es similar a sembrar arboles con el constructor.");
		hints.Add ("Selecciona la opcion de crear caminar en el panel que esta abajo y luego selecciona en el mundo donde los quieres construir!");
	}
	
	// change mission completion section text here
	void setCompletionText() {
		completion_text = "Con los caminos que hagas vas a poder llegar a todos lados mas rapidamente!";
	}
	
	// change mission rewards section text here
	void setRewardsText() {
		rewards = new List<string> ();
		rewards.Add ("Un camino rapido");
		rewards.Add ("200 piedras para hacer mas caminos!");
	}
	
	/*
	Write your method for evaluating requirements for mission start here.
	return:
		true - starting requirements are met.
		false - starting requirements are not met.
 	*/
	public override bool evaluateRequirements() {
		// check if requirements for mission start are met
		if (GameController.gameController.missionController.missionCompleted (4)) {		// if mission with id 1 is completed
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
		int newRocksAmount = GameController.gameController.playerController.explorer.GetComponent<PlayerItems> ().getItemAmount (new Rock());
		if (newRocksAmount < rocksAmount) {
			return true;
		}
		return false;
	}
	
	//Write your method for redeeming mission rewards here.
	public override void redeemRewards() {
		// Add rewards to player. (items, eco points, etc.)
		GameController.gameController.playerController.giveItemsToExplorer (new Rock (), 200);
	}
	
	//This will happen on mission start.
	public override void startMission() {
		base.startMission ();	// required
		
		// initialize flag for completion evaluation
		rocksAmount = GameController.gameController.playerController.explorer.GetComponent<PlayerItems> ().getItemAmount (new Rock());
	}
	
	//This will happen on mission completion.
	public override void completeMission() {
		base.completeMission ();	// required
	}
	
}