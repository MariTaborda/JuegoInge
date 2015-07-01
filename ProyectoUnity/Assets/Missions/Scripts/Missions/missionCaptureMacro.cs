using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class MissionCaptureMacro : Mission {
	
	// mission specific attributes
	Vector3 builder_initial_position;
	Vector3 explorer_initial_position;
	Vector3 scientist_initial_position;
	float near_distance = 0.9f;
	[HideInInspector]
	public Vector3 pointInWorld;

	public MissionCaptureMacro() {
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
		title = "A capturar bichitos!! digo... macroinvertebrados!";
	}
	
	// change mission description section text here
	void setDescriptionText() {
		description = "Utiliza el personaje cientifico y escoge la herramienta lupa para examinar el rio!!";
	}
	
	// change mission hints here
	void setHintsText() {
		hints = new List<string> ();
		hints.Add ("Debes dar clic en el agua con la lupa para poder analizar el agua!!");
		hints.Add ("Cuando te salga el panel da clic en capturar! Suerte");

		hints.Add ("Dale click a la X en la esquina superior derecha de esta ventana para cerrarla.");
		hints.Add ("Si no lo logras siempre puedes reiniciar tu PC.");
	}
	
	// change mission completion text section here
	void setCompletionText() {
		completion_text = "¡Lo has logrado! Sigue adelante, aun te queda mucho!!";
	}
	
	// change mission rewards section text here
	void setRewardsText() {
		rewards = new List<string> ();
		rewards.Add ("25 Eco puntos");
	}
	
	/*
	Write your method for evaluating requirements for mission start here.
	return:
		true - starting requirements are met.
		false - starting requirements are not met.
 	*/
	public override bool evaluateRequirements() {
		// check if requirements for mission start are met
		// deberia cambiarse por el id de la mision de arboles cuando este lista 
		if (GameController.gameController.missionController.missionCompleted (30)) {		// if mission with id 2 is completed
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
		/*if (carga escena cientifico) {
			return true; 
		}*/

		return true;
	}
	
	//Write your method for redeeming mission rewards here.
	//public override void redeemRewards() {
	//	GameController.gameController.playerController.giveItemsToBuilder (new Shovel (), 1);	// put a shovel in builders inventory
	//}
	
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
