using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MissionGetToTrash : Mission {
	// mission specific attributes
	Vector3 builder_initial_position;
	Vector3 explorer_initial_position;
	Vector3 scientist_initial_position;
	float near_distance = 0.9f;
	[HideInInspector]
	public Vector3 pointInWorld;
	
	public MissionGetToTrash() {
		init ();
	}
	
	public override void init() {
		base.init ();
		id = 34;		// must be different from any other mission id
		setTitle ();
		setDescriptionText ();
		setHintsText ();
		setCompletionText ();
		setRewardsText ();
	}
	
	// change mission title here
	void setTitle() {
		title = "Hacia el norte!";
	}
	
	// change mission description section text here
	void setDescriptionText() {
		description = "Sigue explorando el terreno, ve hacia el norte, conseguiras ver otra bandera, recuerda llegar con todos tus amigos!";
	}
	
	// change mission hints here
	void setHintsText() {
		hints = new List<string> ();
		hints.Add ("Da clic y usa las flechas para ir hacia arriba.");
	}
	
	// change mission completion text section here
	void setCompletionText() {
		completion_text = "Gran trabajo! Cada vez estamos mas cerca, lo notas en el rio?";
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
		// deberia cambiarse por el id de la mision de arboles cuando este lista 
		if (GameController.gameController.missionController.missionCompleted (33)) {		// if mission with id 36 is completed
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
		
		// si cruza a la otra orilla, (si llega a la bandera entonces okkk)
		
		Vector3 new_builder_pos = GameController.gameController.playerController.builder.transform.position;
		Vector3 new_explorer_pos = GameController.gameController.playerController.explorer.transform.position;
		Vector3 new_scientist_pos = GameController.gameController.playerController.scientist.transform.position;
		Debug.Log (new_builder_pos);
		Debug.Log (new_builder_pos.x);
		Debug.Log (new_builder_pos.y);
		Debug.Log (new_builder_pos.z);
		// check if players have moved since mission started
		
		
		Vector3 position;
		int chunkIndexX = 0;
		int chunkIndexY = 0;
		int tileIndexX = 0;
		int tileIndexY = 0;
		// SI LO HICIERAMOS POR CLIC
		
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit)) {
			int indx;
			int indz;
			TChunk chunk = GameController.gameController.terrainGenerator.getChunkTileFromPosition (hit.point, out indx, out indz);
			pointInWorld = hit.point;
			chunkIndexX = chunk.index_x;
			chunkIndexY = chunk.index_y;
			tileIndexX = indz;
			tileIndexY = indx;
			
			if ((new_builder_pos.x < 65 && new_builder_pos.x > 59) && 
			    (new_builder_pos.y < 3 && new_builder_pos.y > -2) &&
			    (new_builder_pos.z < -203 && new_builder_pos.z > -209) ) {
				
				if ((new_explorer_pos.x < 65 && new_explorer_pos.x > 59) && 
				    (new_explorer_pos.y < 3 && new_explorer_pos.y > -2) &&
				    (new_explorer_pos.z < -203 && new_explorer_pos.z > -209) ) {
					
					if ((new_scientist_pos.x < 65 && new_scientist_pos.x > 59) && 
					    (new_scientist_pos.y < 3 && new_scientist_pos.y > -2) &&
					    (new_scientist_pos.z < -203 && new_scientist_pos.z > -209) ) {
						Debug.Log("soy la bandera de la basura XD");
						return true; 
					}
					
				}
				
				
			}
			
			
		}
		return false;
	}
	
	public override void redeemRewards() {
		PlayerController.Player_Controller.GetComponent<PlayerPoints> ().addEcologyPoints (5);
	}
	
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