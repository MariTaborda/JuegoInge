using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissionCollectTrash : Mission {
	// mission specific attributes
	public bool TrashOK;
	[HideInInspector]
	public Vector3 pointInWorld;

	public MissionCollectTrash() {
		init ();
	}
	
	public override void init() {
		base.init ();
		id = 33;		// must be different from any other mission id
		setTitle ();
		setDescriptionText ();
		setHintsText ();
		setCompletionText ();
		setRewardsText ();
	}
	
	// change mission title here
	void setTitle() {
		title = "Hay que limpiar un poco";
	}
	
	// change mission description section text here
	void setDescriptionText() {
		description = "Encontraste basura... Hora de limpiar!";
	}
	
	// change mission hints here
	void setHintsText() {
		hints = new List<string> ();
		hints.Add ("Recuerda que debemos cuidar el ambiente");
		hints.Add ("Debemos recoger la basura siempre");
	}
	
	// change mission completion text section here
	void setCompletionText() {
		completion_text = "¡Excelente! Ayudaste un poco al medio ambiente recogiendo basura!";
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
		if (GameController.gameController.missionController.missionCompleted (32)) {		// if mission with id 1 is completed
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
			if (chunkIndexX == 5 && chunkIndexY == 12) {
				if (tileIndexX == 9 && tileIndexY == 9) {

					// que le salga un cuadro de ir a jugar a la escena
					// mandar a la escena
					//Application.LoadLevel("TrashRecollection");

					return true; 
				}
			}

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
		
		TrashOK = false;
	}
	
	//This will happen on mission completion.
	public override void completeMission() {
		base.completeMission ();	// required
	}

}
