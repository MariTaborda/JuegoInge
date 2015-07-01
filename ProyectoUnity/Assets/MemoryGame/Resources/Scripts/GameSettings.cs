using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameSettings 
{
	private static GameSettings gameSetting;
	private GameSettings(){}
	public static GameSettings Instance()
	{
		if(gameSetting == null)
		{
			gameSetting = new GameSettings();
		}
		return gameSetting;
	}

	public enum GameDifficulty {Easy, Medium, Hard}
	public GameDifficulty difficulty= GameDifficulty.Easy;
	private string[] easyDifficulty = {"1","2","3","4"};
	private string[] mediumDifficulty= {"1","2","3","4","5","6","7"};
	private string[] hardDifficulty= {"1","2","3","4","5","6","7","8","9"};

	public List<string> CardTypes
	{
		get
		{
			List<string> tempList = new List<string>();
			switch(difficulty)
			{
			case GameDifficulty.Easy:
				tempList.AddRange(easyDifficulty);
				break;
			case GameDifficulty.Medium:
				tempList.AddRange(mediumDifficulty);
				break;
			case GameDifficulty.Hard:
				tempList.AddRange(hardDifficulty);
				break;
			}
			return tempList;
		}
	}
	public void SetDifficulty(GameDifficulty diff)
	{
		difficulty = diff;
	}

	public string GetRandomType()
	{
		return CardTypes[Random.Range(0,CardTypes.Count)];
	}
}
