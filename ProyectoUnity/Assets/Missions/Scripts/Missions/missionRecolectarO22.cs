using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissionRecolectarO22 : Mission {
	
	// mission specific attributes
	public bool O2analisis2;
	
	public MissionRecolectarO22() {
		init ();
	}
	
	public override void init() {
		base.init ();
		id = 39;		// must be different from any other mission id
		setTitle ();
		setDescriptionText ();
		setHintsText ();
		setCompletionText ();
		setRewardsText ();
	}
	
	// change mission title here
	void setTitle() {
		title = "Mas Analisis";
	}
	
	// change mission description section text here
	void setDescriptionText() {
		description = "Recuerdas como realizar analisis de O2, Si? Bueno ya sabes que hacer!!";
	}
	
	// change mission hints here
	void setHintsText() {
		hints = new List<string> ();
		hints.Add ("Recuerda que puedes realizar anlisis con el icono de la probeta en el menu del cientifico.");
		hints.Add ("No te confundas de analisis, recuerda que el analisis de O2 y el de Ph son diferentes");
	}
	
	// change mission completion text section here
	void setCompletionText() {
		completion_text = "¡Lo has logrado! Espero que no te aburras de realizar analisis, te faltan muchos mas!!";
	}
	
	// change mission rewards section text here
	void setRewardsText() {
		rewards = new List<string> ();
		rewards.Add ("50 Eco puntos");
	}
	
	/*
	Write your method for evaluating requirements for mission start here.
	return:
		true - starting requirements are met.
		false - starting requirements are not met.
 	*/
	public override bool evaluateRequirements() {
		// check if requirements for mission start are met
		if (GameController.gameController.missionController.missionCompleted (38)) {		// if mission with id 1 is completed
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
		
		if (O2analisis2) {		// flag is set in ActionWaterPH
			return true;
		}
		return false;
	}
	
	//Write your method for redeeming mission rewards here.
	public override void redeemRewards() {
		PlayerController.Player_Controller.GetComponent<PlayerPoints> ().addEcologyPoints (50);
	}
	
	//This will happen on mission start.
	public override void startMission() {
		base.startMission ();	// required
		
		O2analisis2 = false;
	}
	
	//This will happen on mission completion.
	public override void completeMission() {
		base.completeMission ();	// required
	}
	
}
