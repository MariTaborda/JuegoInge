using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Playmat : VersionedView 
{
	public bool gameWon = false;
	public int TotalPoints =0;
	public int Points = 0;
	public int NumberOfCardsFlipped =0;

	public enum BoardState {Flipping,Comparing}
	public BoardState state = BoardState.Flipping;

	private static Playmat mat;
	public GameObject cardPrefab;
	public Layout layout;
	public GameObject board;

	public List<Card> cards =new List<Card>();

	public Card card1;
	public Card card2;

	// Use this for initialization
	void Start () 
	{
		mat = this;
	}

	public static Playmat GetPlaymat()
	{
		return mat;
	}
	public	void CreateLayout(string s)
	{
		if(s == "Random")
		{
			board = layout.GetRandomLayout();
		}
		else
		{
			board = layout.GetLayoutFromName(s);
		}
		board.transform.parent = gameObject.transform;
		CreateCardsFromLayout();
		CreateCardTypes();
	}

	void CreateCardsFromLayout()
	{
		foreach(Slot s in layout.slots)
		{
			GameObject go = GameObject.Instantiate(cardPrefab,s.transform.position,Quaternion.Euler(0,-90,270)) as GameObject;
			go.transform.parent = s.transform;
			Destroy(s.GetComponent<BoxCollider>());
			cards.Add(go.GetComponent<Card>());
		}
	}

	void CreateCardTypes()
	{
		for(int i=0;i <= cards.Count/2; i++)
		{
			Card c1 = cards[i];
			Card c2 = cards[cards.Count - 1 - i];
			string type = GameSettings.Instance().GetRandomType();
			c1.GenerateCard(type);
			c2.GenerateCard(type);
		}
		TotalPoints = cards.Count/2;
	}
	public string GetPointsString()
	{
		return Points.ToString() + "/" + TotalPoints.ToString();
	}

	public override void DirtyUpdate ()
	{
		switch(state)
		{
		case BoardState.Comparing:
			Compare();
			break;
		}
	}
	void Compare()
	{
		if(card1.CardType == card2.CardType)
		{
			NumberOfCardsFlipped =0;
			Points++;
			if(TotalPoints == Points)
			{
				gameWon = true;
			}
		}
		else
		{
			card1.Unflip();
			card2.Unflip();
		}
		card1 = null;
		card2 = null;
		state = BoardState.Flipping;
	}

	public void SetCardsForMatch(Card c)
	{
		if(card1==null)
		{
			card1 = c;
			state = BoardState.Flipping;
		}
		else
		{
			card2 = c;
			state = BoardState.Comparing;
		}
		MarkDirty();
	}
}
