using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class UI : MonoBehaviour
{
	public  List<string> levels =new List<string>();
	public enum State {Menu,InGame,Options, goLab ,LevelSelect}
	public State state = State.Menu;

	public GameObject playmat;

	IEnumerator WaitForPlaymatToLoad(string levelName)
	{
		yield return new WaitForSeconds(.5f);
		Playmat.GetPlaymat().CreateLayout(levelName);
	}

	void OnGUI()
	{
		switch(state)
		{
		case State.Menu:
			Menu();
			break;
		case State.LevelSelect:
			LevelSelect();
			break;
		case State.Options:
			Options();
			break;
		case State.goLab:
			goLab();
			break;
		case State.InGame:
			Ingame();
			break;
		}
	}

	void Menu()
	{
		GUI.Label(new Rect(Screen.width/2 - 50, 0,100,20),"Matching Tutorial");
		if(GUI.Button(new Rect(Screen.width/2 - 50,Screen.height/2,100,30),"Jugar"))
		{
			state = State.LevelSelect;
		}
		if(GUI.Button(new Rect(Screen.width/2 - 50,Screen.height/2 + 35,100,30),"Opciones"))
		{
			state = State.Options;
		}
		if(GUI.Button(new Rect(Screen.width/2 - 50,Screen.height/2 + 70,100,30),"Regresar"))
		{
			state = State.goLab;
		}
	}

	void LevelSelect()
	{
		if(GUI.Button(new Rect(Screen.width/2 - 50,0,100,30),"Random"))
		{
			state = State.InGame;
			Instantiate(playmat);
			StartCoroutine(WaitForPlaymatToLoad("Random"));
		}
		for(int i=0;i<levels.Count;i++)
		{
			if(GUI.Button(new Rect(Screen.width/2 - 50,50 + 50*i,100,30),levels[i]))
			{
				state = State.InGame;
				Instantiate(playmat);
				StartCoroutine(WaitForPlaymatToLoad(levels[i]));
			}
		}
	}
	void Ingame()
	{
		GUI.Label(new Rect(Screen.width/2 - 50, 0,100,20),"Score: " + Playmat.GetPlaymat().GetPointsString());
		if(GUI.Button(new Rect(0,0,100,30),"Quit"))
		{
			Destroy(Playmat.GetPlaymat().gameObject);
			state = State.Menu;
		}

		if(Playmat.GetPlaymat().gameWon)
		{
			if(GUI.Button(new Rect(Screen.width/2 - 50,Screen.height/2,100,30),"Play Again?"))
			{
				Destroy(Playmat.GetPlaymat().gameObject);
				Instantiate(playmat);
			}
			if(GUI.Button(new Rect(Screen.width/2 - 50,Screen.height/2 + 35,100,30),"Quit?"))
			{
				Destroy(Playmat.GetPlaymat().gameObject);
				state = State.Menu;
			}
		}
	}
	void Options()
	{
		GUI.Label(new Rect(Screen.width/2 - 50, 0,100,20),"Options");

		if(GUI.Button(new Rect(Screen.width/2 - 50,Screen.height/2,100,30),"Back"))
		{
			state = State.Menu;
		}
		Color c = GameSettings.Instance().difficulty == GameSettings.GameDifficulty.Easy ? Color.red : Color.green;
		GUI.color = c;
		if(GUI.Button(new Rect(Screen.width/2 - 50,Screen.height/2 + 35,100,30),"Easy"))
		{
			GameSettings.Instance().SetDifficulty(GameSettings.GameDifficulty.Easy);
		}
		c = GameSettings.Instance().difficulty == GameSettings.GameDifficulty.Medium ? Color.red : Color.green;
		GUI.color = c;
		if(GUI.Button(new Rect(Screen.width/2 - 50,Screen.height/2 + 70,100,30),"Medium"))
		{
			GameSettings.Instance().SetDifficulty(GameSettings.GameDifficulty.Medium);
		}
		c = GameSettings.Instance().difficulty == GameSettings.GameDifficulty.Hard ? Color.red : Color.green;
		GUI.color = c;
		if(GUI.Button(new Rect(Screen.width/2 - 50,Screen.height/2 + 105,100,30),"Hard"))
		{
			GameSettings.Instance().SetDifficulty(GameSettings.GameDifficulty.Hard);
		}
	}

	void goLab() {
		Application.LoadLevel ("laboratory"); 
	}
}
