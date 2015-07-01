using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissionManager {

	public List<Mission> missions;

	public MissionManager () {
		loadMissions ();	// <- load new missions in the loadMissions method
	}

	// This method will be triggered automatically every few seconds
	public void manageMissions() {
		if (GameController.gameController.missionsActive) {
			for (int i = 0; i < missions.Count; ++i) {
				
				if(!missions[i].completed) {
					if(missions[i].started) {
						if(missions[i].evaluateConditions ()) {
							if(!GameController.gameController.missionUI.panelsActive()) {
								missions[i].completeMission ();
							}
						}
					}
					else {
						if(missions[i].startsAutomatically && missions[i].evaluateRequirements ()) {
							if(!GameController.gameController.missionUI.panelsActive()) {
								missions[i].startMission ();
							}
						}
					}
				}
				
			}
		}
	}

	// get a mission by id
	public Mission getMissionById(int mission_id) {
		for (int i = 0; i < missions.Count; ++i) {
			if(missions[i].id == mission_id) {
				return missions[i];
			}
		}
		return null;
	}

	// checks if mission with id = mission_id is completed. returns null if no such mission was found.
	public bool missionCompleted(int mission_id) {
		Mission mission = getMissionById (mission_id);
		if(mission != null && mission.completed) {
			return true;
		}
		return false;
	}

	// Add desired missions to the manager here
	void loadMissions() {

		GameObject holder = GameController.gameController.loadedMissionsHolder;		// will keep loaded mission instances attached to an object in the hierarchy
		missions = new List<Mission> ();

		// add new missions like so


		missions.Add (
			holder.AddComponent<MissionStart> ()
		);

		missions.Add (
			holder.AddComponent<MissionMoving> ()
		);

		missions.Add (
			holder.AddComponent<MissionSelectCharacter> ()
		);

		missions.Add (
			holder.AddComponent<MissionBuilder> ()
		);

		missions.Add (
			holder.AddComponent<MissionExplorer> ()
		);

		missions.Add (
			holder.AddComponent<MissionRoads> ()
		);

		missions.Add (
			holder.AddComponent<MissionChopTree> ()
		);

		missions.Add (
			holder.AddComponent<MissionPlantTree> ()
		);

		missions.Add (
			holder.AddComponent<MissionBuildBridge> () // id 30
		);

		missions.Add (
			holder.AddComponent<MissionCaptureMacro> () // id 31

		);

		missions.Add (
			holder.AddComponent<MissionBuildBridgeBack> () // id 55
			);




	}

}
