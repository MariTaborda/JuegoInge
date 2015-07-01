using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissionStart : Mission {

	public MissionStart() {
		init ();
	}

	public override void init() {
		base.init ();
		id = 0;		// must be different from any other mission id
		setTitle ();
		setDescriptionText ();
		setHintsText ();
		setCompletionText ();
		setRewardsText ();
	}

	// change mission title here
	void setTitle() {
		title = "Bienvenido a Los Siete Manantiales!";
	}

	// change mission description section text here
	void setDescriptionText() {
		description = "En esta aventura lideras a un equipo de expertos en un viaje que los llevará desde lo más profundo de la selva, pasando por rapidos rios, hasta pueblos rurales y cuidades donde la contaminación del agua es alarmante. Tu objetivo es la exploración de zonas contaminadas y la búsqueda de posibles soluciones.";
	}

	// change mission hints here
	void setHintsText() {
		hints = new List<string> ();
		hints.Add ("Dale click a la X en la esquina superior derecha de esta ventana para cerrarla.");
	}

	// change mission completion text section here
	void setCompletionText() {
		completion_text = "Empiezas con 50 eco puntos. Estos puntos los puedes usar para comprar mas materiales y mejores herramientas para cumplir con tu mision!";
	}

	// change mission rewards section text here
	void setRewardsText() {
		rewards = new List<string> ();
		rewards.Add ("50 Eco puntos");
		rewards.Add ("Un futuro mejor :)");
	}
		
	/*
	Write your method for evaluating requirements for mission start here.
	return:
		true - starting requirements are met.
		false - starting requirements are not met.
 	*/
	public override bool evaluateRequirements() {
		// check if requirements for mission start are met
		return true;	// there are no requirements for this mission
	}

	/*
	Write your method for evaluating conditions for mission completion here.
	return:
		true - completion conditions are met.
		false - completion conditions are not met.
 	*/
	public override bool evaluateConditions() {
		return true;
	}
	
	//Write your method for redeeming mission rewards here.
	public override void redeemRewards() {
		PlayerController.Player_Controller.GetComponent<PlayerPoints> ().addEcologyPoints (50);
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
