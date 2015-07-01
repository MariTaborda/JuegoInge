using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class MissionBuildBridge : Mission {
	
	// mission specific attributes
	Vector3 builder_initial_position;
	Vector3 explorer_initial_position;
	Vector3 scientist_initial_position;
	float near_distance = 0.9f;
	[HideInInspector]
	public Vector3 pointInWorld;

	public MissionBuildBridge() {
		init ();
	}
	
	public override void init() {
		base.init ();
		id = 30;		// must be different from any other mission id
		setTitle ();
		setDescriptionText ();
		setHintsText ();
		setCompletionText ();
		setRewardsText ();
	}
	
	// change mission title here
	void setTitle() {
		title = "Es hora de atravezar el mar...";
	}
	
	// change mission description section text here
	void setDescriptionText() {
		description = "Utiliza el personaje constructor y escoge la herramienta martillo para crear un puente!";
	}
	
	// change mission hints here
	void setHintsText() {
		hints = new List<string> ();
		hints.Add ("Cada vez que utilices madera se rebajara la cantidad de madera de tu mochila, usala con inteligencia!");
		hints.Add ("Nunca dejes un hombre atras! Debes llevar a todos los personajes al otro lado!");

		hints.Add ("Dale click a la X en la esquina superior derecha de esta ventana para cerrarla.");
		hints.Add ("Si no lo logras siempre puedes reiniciar tu PC.");
	}
	
	// change mission completion text section here
	void setCompletionText() {
		completion_text = "¡Lo has logrado! Ahora a lo que vinimos!";
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
		if (GameController.gameController.missionController.missionCompleted (29)) {		// if mission with id 6 is completed
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

			// codigo para la inicial 
			/*if ((new_builder_pos.x < 79 && new_builder_pos.x > 77) && 
			    (new_builder_pos.y < 2 && new_builder_pos.y > 1) &&
			    (new_builder_pos.z < -88 && new_builder_pos.z > -90) ) {
				Debug.Log("soy la bandera 1");
				return true; 
			}*/
		

			// codigo para bandera 2 de inicio arboles
			
			/*if ((new_builder_pos.x < 53 && new_builder_pos.x > 50) && 
			    (new_builder_pos.y < 2 && new_builder_pos.y > 1) &&
			    (new_builder_pos.z < -124 && new_builder_pos.z > -126) ) {
				Debug.Log("soy la bandera 2");
				return true; 
			}*/

			// codigo para bandera 3 de inicio de ruzar rio 1
			/*
			if ((new_builder_pos.x < 68 && new_builder_pos.x > 66) && 
			    (new_builder_pos.y < 1 && new_builder_pos.y > 0) &&
			    (new_builder_pos.z < -145 && new_builder_pos.z > -147) ) {
				Debug.Log("soy la bandera 3");
				return true; 
			}
			*/

			// codigo para bandera 4 (al otro lado del rio)

			if ((new_builder_pos.x < 79 && new_builder_pos.x > 75) && 
			    (new_builder_pos.y < 1 && new_builder_pos.y > 0) &&
			    (new_builder_pos.z < -158 && new_builder_pos.z > -160) ) {

				if ((new_explorer_pos.x < 79 && new_explorer_pos.x > 75) && 
				    (new_explorer_pos.y < 1 && new_explorer_pos.y > 0) &&
				    (new_explorer_pos.z < -158 && new_explorer_pos.z > -160) ) {

					if ((new_scientist_pos.x < 79 && new_scientist_pos.x > 75) && 
					    (new_scientist_pos.y < 1 && new_scientist_pos.y > 0) &&
					    (new_scientist_pos.z < -158 && new_scientist_pos.z > -160) ) {
							Debug.Log("soy la bandera 3");
							return true; 
						}

				}


			}


		}
		return false;
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
