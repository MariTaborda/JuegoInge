using UnityEngine;
using System.Collections;

public class MiniJuego_BichoController : MonoBehaviour {

	//direcciones de movimiento
	public Vector2 up = new Vector2(0f,20f);
	public Vector2 down = new Vector2 (0f,-20f);
	public Vector2 left = new Vector2(-20f,0f);
	public Vector2 right = new Vector2 (20f,0f);
	public Vector2 upleft = new Vector2(-20f,20f);
	public Vector2 downleft = new Vector2 (-20f,-20f);
	public Vector2 upright = new Vector2(20f,20f);
	public Vector2 downright = new Vector2 (-20f,20f);
	//fuerza de corriente
	public Vector2 current = new Vector2 (1f,0f);
	//id del bicho
	public int id;
	//valor bmwg
	public int bmwg;
	//familia
	public int fam;
	
	
	// Use this for initialization
	void Start () {
		GetComponent<Rigidbody2D> ().AddForce (current);
		int dirNueva = (int)Random.Range (0,8);
		switch (dirNueva) {
		case 0 :
			GetComponent<Rigidbody2D> ().velocity = up;
			break;
		case 1:
			GetComponent<Rigidbody2D> ().velocity = down;
			break;
		case 2:
			GetComponent<Rigidbody2D> ().velocity = left;
			break;
		case 3:
			GetComponent<Rigidbody2D> ().velocity = right;
			break;
		case 4 :
			GetComponent<Rigidbody2D> ().velocity = upleft;
			break;
		case 5:
			GetComponent<Rigidbody2D> ().velocity = downleft;
			break;
		case 6:
			GetComponent<Rigidbody2D> ().velocity = upright;
			break;
		case 7:
			GetComponent<Rigidbody2D> ().velocity = downright;
			break;
		default	:
			break;
		}
	}
	
	
	void FixedUpdate () {
		
		
	}
	
	
	//cambia de direccion de macroinvertebrado al chocar con algun objeto
	void OnCollisionEnter2D (Collision2D collision) {
		int dirNueva = (int)Random.Range (0,8);
		switch (dirNueva) {
		case 0 :
			GetComponent<Rigidbody2D> ().velocity = up;
			break;
		case 1:
			GetComponent<Rigidbody2D> ().velocity = down;
			break;
		case 2:
			GetComponent<Rigidbody2D> ().velocity = left;
			break;
		case 3:
			GetComponent<Rigidbody2D> ().velocity = right;
			break;
		case 4 :
			GetComponent<Rigidbody2D> ().velocity = upleft;
			break;
		case 5:
			GetComponent<Rigidbody2D> ().velocity = downleft;
			break;
		case 6:
			GetComponent<Rigidbody2D> ().velocity = upright;
			break;
		case 7:
			GetComponent<Rigidbody2D> ().velocity = downright;
			break;
		default	:
			break;
		}
	}

	void setId(int x){
		id = x;
	}

	void setBmwg(int x){
		bmwg = x;
	}

	void setFam(int x){
		fam = x;
	}

	void OnTriggerEnter2D(Collider2D other){
			if (other.gameObject.tag == "Pecera") {
			print (other);
			Destroy (this.gameObject);
		}
	}

}
