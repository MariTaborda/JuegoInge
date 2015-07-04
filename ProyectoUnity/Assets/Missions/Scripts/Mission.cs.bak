using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mission : MonoBehaviour {

	public int id;
	public bool started = false;
	public bool completed = false;
	public bool startsAutomatically = true;
	public string title;
	public string description;
	public string completion_text;
	public List<string> hints;
	public List<string> rewards;

	public virtual void init() {
		started = false;
		completed = false;
	}

	public virtual void startMission() {
		started = true;
		callMissionStartUI ();
	}

	public virtual void completeMission() {
		completed = true;
		redeemRewards ();
		callMissionCompletedUI ();
	}

	public virtual bool evaluateRequirements() {
		return true;
	}

	public virtual bool evaluateConditions() {
		return true;
	}

	public virtual void redeemRewards() {

	}

	void callMissionStartUI() {
		GameController.gameController.missionUI.activateStartPanel (this);
	}

	void callMissionCompletedUI() {
		GameController.gameController.missionUI.activateCompletedPanel (this);
	}

}
