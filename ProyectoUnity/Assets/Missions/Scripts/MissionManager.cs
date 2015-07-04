using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;

public class MissionManager {
	
	public List<Mission> missions;
	public MissionDataPackage dataPackage;
	public bool missionsLoaded = false;

	public MissionManager () {
		loadMissions ();	// <- load new missions in the loadMissions method
		if (PersistentData.Data.loadPersistentData) {
			dataPackage = PersistentData.Data.SerializableData.Missions;
			for (int i = 0; i < dataPackage.missions_status.Count; ++i) {
				if(dataPackage.missions_status [i][0]) {
					missions [i].startMissionSilently ();
				}
				if(dataPackage.missions_status [i][1]) {
					missions [i].completeMissionSilently ();
				}
			}
		} 
		else {
			dataPackage = new MissionDataPackage ();
			PersistentData.Data.SerializableData.Missions = dataPackage;
			dataPackage.missions_status = new List<bool[]> ();
		}
		missionsLoaded = true;
	}

	// This method will be triggered automatically every few seconds
	public void manageMissions() {
		if (GameController.gameController.missionsActive && missionsLoaded) {
			for (int i = 0; i < missions.Count; ++i) {
				
				if(!missions[i].completed) {
					if(missions[i].started) {
						if(missions[i].evaluateConditions ()) {
							if(!GameController.gameController.missionUI.panelsActive()) {
								missions[i].completeMission ();
								updateDataPackage();
							}
						}
					}
					else {
						if(missions[i].startsAutomatically && missions[i].evaluateRequirements ()) {
							if(!GameController.gameController.missionUI.panelsActive()) {
								missions[i].startMission ();
								updateDataPackage();
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
			holder.AddComponent<MissionStartEnd> () // id 6
		);

		missions.Add (
			holder.AddComponent<MissionChopTree> () // id 28
			);
		
		missions.Add (
			holder.AddComponent<MissionPlantTree> () // id 29
			);


		missions.Add (
			holder.AddComponent<MissionBuildBridge> () // id 30
		);

		missions.Add (
			holder.AddComponent<MissionAprenderSobreMacro> () // id 31
			);

		missions.Add (
			holder.AddComponent<MissionCaptureMacro> () // id 32

		);


		missions.Add (
			holder.AddComponent<MissionCollectTrash> () // id 33 
			
			);

		missions.Add (
			holder.AddComponent<MissionGetToTrash> () // id 34  juan jose llega a la bandera
			
			);

		missions.Add (
			holder.AddComponent<MissionRecolectarPH> () // id 35
			);
		
		missions.Add (
			holder.AddComponent<MissionRecolectarO2> () // id 36
			);

		missions.Add (
			holder.AddComponent<MissionBuildBridgeBack> () // id 37
		);

		missions.Add (
			holder.AddComponent<MissionRecolectarPH2> () // id 38
			);

		missions.Add (
			holder.AddComponent<MissionRecolectarO22> () // id 39
			);


		missions.Add (
			holder.AddComponent<MissionTrashCrush> ()  // id 40 
			);

		missions.Add (
			holder.AddComponent<MissionBigEnding> ()  // id 41
			);



	}

	public void updateDataPackage() {
		if (dataPackage == null) {
			dataPackage = new MissionDataPackage ();
		}
		dataPackage.missions_status = new List<bool[]> ();
		for (int i = 0; i < missions.Count; ++i) {
			dataPackage.missions_status.Add (new bool[2]);
			dataPackage.missions_status[i][0] = missions[i].started;
			dataPackage.missions_status[i][1] = missions[i].completed;
		}
	}

}

// All data that needs to be saved for correct mission load must be referenced here
[Serializable]
public class MissionDataPackage {
	public List<bool[]> missions_status;
}
