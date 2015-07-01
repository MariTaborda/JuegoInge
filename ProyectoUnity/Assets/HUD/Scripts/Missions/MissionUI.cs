using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MissionUI : MonoBehaviour {

	public MPanel StartPanel;
	public MPanel CompletedPanel;
	public MPanel MissionsPanel;

	public void init() {
		StartPanel.gameObject.SetActive (false);
		CompletedPanel.gameObject.SetActive (false);
		MissionsPanel.gameObject.SetActive (false);
	}

	public void activateStartPanel(Mission mission) {
		deactivateOpenPanels ();

		StartPanel.title_field.text = mission.title;
		StartPanel.description_field.text = mission.description;
		StartPanel.hints_field.text = listStrings(mission.hints);

		StartPanel.gameObject.SetActive (true);
	}

	public void activateCompletedPanel(Mission mission) {
		deactivateOpenPanels ();

		CompletedPanel.title_field.text = mission.title;
		CompletedPanel.completion_field.text = mission.completion_text;
		CompletedPanel.rewards_field.text = listStrings(mission.rewards);

		CompletedPanel.gameObject.SetActive (true);
	}

	public void activateMissionsPanel(List<Mission> active_missions) {
		MissionsPanel.gameObject.SetActive (true);
	}

	public bool panelsActive() {
		if(StartPanel.isActiveAndEnabled || CompletedPanel.isActiveAndEnabled || MissionsPanel.isActiveAndEnabled) {
			return true;
		}
		return false;
	}

	void deactivateStartPanel() {
		StartPanel.gameObject.SetActive (false);
	}
	
	void deactivateCompletedPanel() {
		CompletedPanel.gameObject.SetActive (false);
	}

	void deactivateMissionsPanel() {
		MissionsPanel.gameObject.SetActive (false);
	}

	void deactivateOpenPanels() {
		deactivateStartPanel ();
		deactivateCompletedPanel ();
		deactivateMissionsPanel ();
	}

	string listStrings (List<string> strings) {
		string result = "";
		for (int i = 0; i < strings.Count; ++i) {
			result = result + "- " + strings[i] + "\n";
		}
		return result;
	}

}
