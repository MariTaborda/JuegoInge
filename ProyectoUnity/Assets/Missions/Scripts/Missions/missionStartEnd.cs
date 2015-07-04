using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissionStartEnd : Mission {
	
	// mission specific attributes

	
	public MissionStartEnd() {
		init ();
	}
	
	public override void init() {
		base.init ();
		id = 6;		// must be different from any other mission id
		setTitle ();
		setDescriptionText ();
		setHintsText ();
		setCompletionText ();
		setRewardsText ();
	}
	
	// change mission title here
	void setTitle() {
		title = "Un mundo por explorar";
	}
	
	// change mission description section text here
	void setDescriptionText() {
		description = "Hay todo un mundo por explorar, por eso ahora tienes que llevar a todo el equipo hasta la siguiente bandera. Sigue la flecha roja que esta arriba en la pantalla.";
	}
	
	// change mission hints here
	void setHintsText() {
		hints = new List<string> ();
		hints.Add ("Recuerda que el cientifico camina muy lento, haz caminos!");
		hints.Add ("Para hacer los caminos es posible que necesites cortar arboles y arbustos!");
	}
	
	// change mission completion section text here
	void setCompletionText() {
		completion_text = "Lo has logrado! Ahora estas listo para empezar tu verdadera mision! A salvar el ambiente!";
	}
	
	// change mission rewards section text here
	void setRewardsText() {
		rewards = new List<string> ();
		rewards.Add ("10 Eco Puntos");
	}
	
	/*
	Write your method for evaluating requirements for mission start here.
	return:
		true - starting requirements are met.
		false - starting requirements are not met.
 	*/
	public override bool evaluateRequirements() {
		// check if requirements for mission start are met
		if (GameController.gameController.missionController.missionCompleted (5)) {		// if mission with id 1 is completed
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
		Vector3 scientist_pos = GameController.gameController.playerController.scientist.transform.position;
		Vector3 flag_pos = new Vector3 (51.5f, 1f, -125.1f);
		if (Vector3.Distance(scientist_pos, flag_pos) < 5f) {
			return true;
		}
		return false;
	}
	
	//Write your method for redeeming mission rewards here.
	public override void redeemRewards() {
		// Add rewards to player. (items, eco points, etc.)
		PlayerController.Player_Controller.GetComponent<PlayerPoints> ().addEcologyPoints (10);
	}
	
	//This will happen on mission start.
	public override void startMission() {
		base.startMission ();	// required
		
		// initialize flag for completion evaluation
		GameController.gameController.setObjective (new Vector3(51.5f,1f,-125.1f));
	}
	
	//This will happen on mission completion.
	public override void completeMission() {
		base.completeMission ();	// required
		GameController.gameController.unsetObjective ();
	}
	
}