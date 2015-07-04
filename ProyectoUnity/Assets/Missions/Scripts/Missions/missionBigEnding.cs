using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class MissionBigEnding : Mission {
	
	// mission specific attributes
	Vector3 builder_initial_position;
	Vector3 explorer_initial_position;
	Vector3 scientist_initial_position;
	float near_distance = 0.9f;
	[HideInInspector]
	public Vector3 pointInWorld;

	public MissionBigEnding() {
		init ();
	}
	
	public override void init() {
		base.init ();
		id = 41;		// must be different from any other mission id
		setTitle ();
		setDescriptionText ();
		setHintsText ();
		setCompletionText ();
		setRewardsText ();
	}
	
	// change mission title here
	void setTitle() {
		title = "Lleguemos al problema!";
	}
	
	// change mission description section text here
	void setDescriptionText() {
		description = "Cerca de aqui hay una zona rural, como vos, yo tambien creo que son los causantes de la contaminacion, es hora de tomar medidas mi amigo, hacercate a la bandera cerca de la zona y vamos a negociar!! ";
	}
	
	// change mission hints here
	void setHintsText() {
		hints = new List<string> ();
		hints.Add ("Un largo viaje no lo crees? No sera en vano, vamos estamos cerca!!");
		hints.Add ("Ve con todos los personajes para lograrlo.");

		hints.Add ("Dale click a la X en la esquina superior derecha de esta ventana para cerrarla.");
	}
	
	// change mission completion text section here
	void setCompletionText() {
		completion_text = "¡Por todo tu exito a lo largo del camino, la gente del pueblo ha cambiado su parecer acerca de tirar desechos al rio, por tu esfuerzo, el rio volvera a estar limpio!!";
	}
	
	// change mission rewards section text here
	void setRewardsText() {
		rewards = new List<string> ();
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
		if (GameController.gameController.missionController.missionCompleted (40)) {		// if mission with id 6 is completed
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

			if ((new_builder_pos.x < 16 && new_builder_pos.x > 13) && 
			    (new_builder_pos.y < 1 && new_builder_pos.y > 0) &&
			    (new_builder_pos.z < -215 && new_builder_pos.z > -217) ) {

				if ((new_explorer_pos.x < 16 && new_explorer_pos.x > 13) && 
				    (new_explorer_pos.y < 1 && new_explorer_pos.y > 0) &&
				    (new_explorer_pos.z < -215 && new_explorer_pos.z > -217) ) {

					if ((new_scientist_pos.x < 16 && new_scientist_pos.x > 13) && 
					    (new_scientist_pos.y < 1 && new_scientist_pos.y > 0) &&
					    (new_scientist_pos.z < -215 && new_scientist_pos.z > -217) ) {
							Debug.Log("soy la bandera final");
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
