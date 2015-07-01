using UnityEngine;
using System.Collections;

public class Card : VersionedView 
{
	public enum CardState {Flipped,Hidden}
	public CardState state = CardState.Hidden;

	public GameObject cardView;
	public string CardType;
	public float delay = .5f;
	// Use this for initialization
	void Start () {
	
	}
	
	public override void DirtyUpdate ()
	{
		StartCoroutine(StartCardFlip());
	}

	public IEnumerator StartCardFlip()
	{
		AnimationClip clip;
		clip = cardView.GetComponent<Animation>().GetClip("Card Flip");
		if(state == CardState.Flipped)
		{
			cardView.GetComponent<Animation>()["Card Flip"].speed =1;
		}
		else
		{
			cardView.GetComponent<Animation>()["Card Flip"].speed = -1;
			cardView.GetComponent<Animation>()["Card Flip"].time = clip.length;
		}
		cardView.GetComponent<Animation>().Play();
		yield return new WaitForSeconds(clip.length +delay);
		if(state == CardState.Flipped)
		{
			Playmat.GetPlaymat().SetCardsForMatch(this);
		}
		else
		{
			Playmat.GetPlaymat().NumberOfCardsFlipped--;
		}
	}

	public void GenerateCard(string cardType)
	{
		CardType = cardType;
		cardView.GetComponent<Renderer>().material.mainTexture = Resources.Load("Graphics/" +CardType)as Texture2D;
	}

	public void Unflip()
	{
		state = CardState.Hidden;
		MarkDirty();
	}

	void OnMouseDown()
	{
		if(state == CardState.Hidden && Playmat.GetPlaymat().NumberOfCardsFlipped != 2)
		{
			Playmat.GetPlaymat().NumberOfCardsFlipped ++;
			state = CardState.Flipped;
			MarkDirty();
		}
	}
}
