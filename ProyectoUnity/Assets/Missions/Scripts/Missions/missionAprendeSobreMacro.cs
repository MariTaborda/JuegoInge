using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissionAprenderSobreMacro: Mission {
	
	// mission specific attributes
	public bool Macro;
	
	public MissionAprenderSobreMacro() {
		init ();
	}
	
	public override void init() {
		base.init ();
		id = 31;		// must be different from any other mission id
		setTitle ();
		setDescriptionText ();
		setHintsText ();
		setCompletionText ();
		setRewardsText ();
	}
	
	// change mission title here
	void setTitle() {
		title = "Aprende algo nuevo";
	}
	
	// change mission description section text here
	void setDescriptionText() {
		description = "Tienes idea de que es un Macroinvertebrado? No, bueno intenta descubrirlo. Primero selecciona el personaje cientifico, y en su panel, escoge el icono de color morado en forma de macroinvertebrado... macro que? ya veras lo que es. Dale clic al agua con la lupa y da clic en el boton del signo de pregunta. ";
	}
	
	// change mission hints here
	void setHintsText() {
		hints = new List<string> ();
		hints.Add ("Recuerda que el cientifico puede realizar dos aciones");
		hints.Add ("Explora todo lo que se puede hacer en los menus de cada accion");
	}
	
	// change mission completion text section here
	void setCompletionText() {
		completion_text = "¡Genial! Aprender es poder, ahora tienes un poco de conocimiento sobre lo que es un macroinvertebrado!!!!";
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
		if (GameController.gameController.missionController.missionCompleted (30)) {		// if mission with id 1 is completed
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
		
		if (Macro) {		// flag is set in ActionWaterPH
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
		
		Macro = false;
	}
	
	//This will happen on mission completion.
	public override void completeMission() {
		base.completeMission ();	// required
	}
	
}
