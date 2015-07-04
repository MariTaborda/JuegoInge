using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissionRecolectarO2 : Mission {
	
	// mission specific attributes
	public bool O2analisis;
	
	public MissionRecolectarO2() {
		init ();
	}
	
	public override void init() {
		base.init ();
		id = 36;		// must be different from any other mission id
		setTitle ();
		setDescriptionText ();
		setHintsText ();
		setCompletionText ();
		setRewardsText ();
	}
	
	// change mission title here
	void setTitle() {
		title = "Nuevo Tipo de Analisis";
	}
	
	// change mission description section text here
	void setDescriptionText() {
		description = "Ayuda al cientifico con su primer analisis de O2!!";
	}
	
	// change mission hints here
	void setHintsText() {
		hints = new List<string> ();
		hints.Add ("Recuerda que puedes realizar anlisis con el icono de la probeta en el menu del cientifico.");
		hints.Add ("No te confundas de analisis, recuerda que el analisis de O2 y el de PH son diferentes");
	}
	
	// change mission completion text section here
	void setCompletionText() {
		completion_text = "¡Lo has logrado! No fue tan dificil verdad?";
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
		if (GameController.gameController.missionController.missionCompleted (35)) {		// if mission with id 1 is completed
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
		
		if (O2analisis) {		// flag is set in ActionWaterO2
			return true;
		}
		return false;
	}
	
	//Write your method for redeeming mission rewards here.
	public override void redeemRewards() {
		PlayerController.Player_Controller.GetComponent<PlayerPoints> ().addEcologyPoints (5);
	}
	
	//This will happen on mission start.
	public override void startMission() {
		base.startMission ();	// required
		
		O2analisis = false;
	}
	
	//This will happen on mission completion.
	public override void completeMission() {
		base.completeMission ();	// required
	}
	
}
