using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissionTrashCrush : Mission {

	Vector3 builder_initial_position;
	bool continuar;
	[HideInInspector]
	public Vector3 pointInWorld;

	public MissionTrashCrush() {
		init ();
	}

	public override void init() {
		base.init ();
		id = 40;		// must be different from any other mission id
		continuar = false;
		setTitle ();
		setDescriptionText ();
		setHintsText ();
		setCompletionText ();
		setRewardsText ();
	}

	// change mission title here
	void setTitle() {
		title = "Clasificar la Basura es Divertido";
	}

	// change mission description section text here
	void setDescriptionText() {
		description = "Para empezar a jugar, acercate a la Bandera y dale clic en los botes de basura para comenzar a clasificarla";
	}

	// change mission hints here
	void setHintsText() {
		hints = new List<string> ();
		hints.Add ("Camina hacia la Bandera");
	}

	// change mission completion text section here
	void setCompletionText() {
		completion_text = "¡Lo has logrado!";
	}

	// change mission rewards section text here
	void setRewardsText() {
		rewards = new List<string> ();
		rewards.Add ("10 Eco puntos");
	}
		
	/*
	Write your method for evaluating requirements for mission start here.
	return:
		true - starting requirements are met.
		false - starting requirements are not met.
 	*/
	public override bool evaluateRequirements() {
		if (GameController.gameController.missionController.missionCompleted (39)) {
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
			if (chunkIndexX == 1 && chunkIndexY == 12) {
				if (tileIndexX == 16 && tileIndexY == 17) {
					
					// que le salga un cuadro de ir a jugar a la escena
					// mandar a la escena
					//Application.LoadLevel("TrashCrush");
					
					return true; 
				}
			}
			
		}
		
		return false;
	}
	
	//Write your method for redeeming mission rewards here.
	public override void redeemRewards() {
		PlayerController.Player_Controller.GetComponent<PlayerPoints> ().addEcologyPoints (10);
	}
	
	//This will happen on mission start.
	public override void startMission() {
		base.startMission ();	// required

		builder_initial_position = GameController.gameController.playerController.builder.transform.position;
	}

	//This will happen on mission completion.
	public override void completeMission() {
		base.completeMission ();	// required
	}

}
