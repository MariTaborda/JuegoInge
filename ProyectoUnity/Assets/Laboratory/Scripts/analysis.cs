using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class analysis : MonoBehaviour {

	// Use this for initialization
	//Cards caddos; 
	int flag = 0; 
	GameObject espejismo; 	
	public Button VerCapturaButton; 
	public Button MacroCards; 
	public GameObject chooseBad; 
	public GameObject chooseGoodNeg; 
	public GameObject chooseGoodPos; 
	public Text countText;
	public Text realCountText;
	GameObject actualCard; 
	int[] cards = new int[22]; 
	GameObject[] gameObjects = new GameObject[5];
	public int zona; 
	int total = 0; 
	int colector = 0; 
	int realSum = 0; 
	string calidad = ""; 
	// de las 5 colecciones de macros, escoge 1 para desplegar y apartir de ella 

	public void SetCaptura(){  // 1 zona media, 2 zona baja, 3 zona alta

		espejismo = GameObject.FindGameObjectWithTag ("espejismo");
		espejismo.SetActive (false);

		//VerCapturaButton = GameObject.FindGameObjectWithTag ("botonVerCaptura");
		VerCapturaButton.interactable = false; 

		int zona = 1; 
			gameObjects = GameObject.FindGameObjectsWithTag ("captureAnalisis");
			/* los gameObjects en el editor estan acomodados de manera tal que:
		 *  Sea el orden: posicion en array, tipo de panel, tipo de zona
			gameObjects[3] = panel muy mala   		3
			gameObjects[2] = panel regular			3
			gameObjects[1] = panel mala 			3 | 2
			gameObjects[5] = panel excelente		2 | 1
			gameObjects[4] = panel buena 			1
			gameObjects[0] = panel no muy mala			1 
		*/
			//Debug.Log ("longitud" + gameObjects.Length);
			// colocamos todos en false
			for (int i = 0; i < gameObjects.Length; i ++) { 
				gameObjects [i].SetActive (false);

				Debug.Log ("desactivando objeto: ["+i+"] " + gameObjects [i]);
		
			}
			// colocamos el que nos interesa segun la zona en true 
			if (zona == 1) { // zona media
				int rand_zona = UnityEngine.Random.Range (0, 2);
				if (rand_zona == 0) { //  no muy mala
					gameObjects [0].SetActive (true);
				} else if (rand_zona == 1) { // mala
					gameObjects [1].SetActive (true);
				} else { //  muy mala
					gameObjects [3].SetActive (true);
				}
			}
			if (zona == 2) { // zona baja 
				int rand_zona = UnityEngine.Random.Range (0, 1);
				if (rand_zona == 0) { //  regular
					gameObjects [2].SetActive (true);
				} else { // no muy mala
					gameObjects [0].SetActive (true);
				}
			}

			if (zona == 3) {  // zona alta 
				int rand_zona = UnityEngine.Random.Range (0, 2);
				if (rand_zona == 0) { //  excelente
					gameObjects [5].SetActive (false);
				} else if (rand_zona == 1) { // buena
					gameObjects [4].SetActive (false);
				} else { 	// regular
					gameObjects [2].SetActive (false);
				}
			}


		
	}
	
	// inicializo todas para que esten en 0.
	public void iniciar () {
		//Debug.Log ("start en 0 las cards");
		for(int i = 0 ; i <= 21 ; i ++) {   	 
			cards[i] = 0;
			Debug.Log ("cards["+i+"]");
		}
		MacroCards.interactable = false;
	}
	// cuando de clic en boton se sumara al especifico ID. 
	// y se iran sumando.
	public void Capturado() {
		
		actualCard =  GameObject.FindGameObjectWithTag ("Card");
		Debug.Log ("el card es: "+actualCard);
		int idCorrecto;
		int id; 
		id = actualCard.GetComponent<Cards> ().getId ();
		idCorrecto = id - 1;
		Debug.Log ("el id es: "+idCorrecto);
		cards [idCorrecto] += actualCard.GetComponent<Cards> ().getBmwp ();

		// actualice interfaz cada vez que hago clic en capturado
		sumAll ();
	}

	public void sumAll() {
		total = 0; 
		for(int i = 0 ; i <= 21 ; i ++) {   	 
			total +=  cards[i];
		}
		countText.text = "Total: " + total.ToString ();
		//Debug.Log ("Total: " + total);
	}

	public void checkSum() {

		realSum = 0; 
		if (gameObjects[3].activeSelf == true) { // muy mala
			realSum = 12;
			calidad = "MuyMala";
		} 
		if (gameObjects[2].activeSelf == true) { // regular
			realSum = 79;
			calidad = "Regular";
		} 
		if (gameObjects[4].activeSelf == true) { // buena
			realSum = 102;
			calidad = "NoMuyBuena";
		} 
		if (gameObjects[5].activeSelf == true) { // excelente
			realSum = 121;
			calidad = "Buena";
		} 
		if (gameObjects[1].activeSelf == true) { // mala
			realSum = 29;
			calidad = "Mala";
		} 
		if (gameObjects[0].activeSelf == true) { // no muy mala
			realSum = 55;
			calidad = "NoMuyMala";
		} 	

		if (realSum == total) {
			realCountText.text = "Correcto! "+realSum.ToString () +"! Gran analisis! Revisa la tabla de calidad del agua con este resultado para ver la calidad de la zona!";
		} else {
			realCountText.text = "Oh Oh!, El total deberia ser: " + realSum.ToString ()+" revisa de nuevo las MacroCards!";
			countText.text = "Total: 0";
			total = 0;
			for(int i = 0 ; i <= 21 ; i ++) {   	 
				cards[i] = 0;
			}
		}


	}

	public void qualityMala() {
		Debug.Log ("hice algo!!");
		if (calidad == "Mala") {
			chooseGoodNeg.SetActive(true); 

		} else {
			chooseBad.SetActive(true); 

		}
	}

	public void qualityMuyMala() {
		Debug.Log ("hice algo!!");
		if (calidad == "MuyMala") {
			chooseGoodNeg.SetActive(true); 
		} else {
			chooseBad.SetActive(true); 
			
		}
	}

	public void qualityNoMuyMala() {
		Debug.Log ("hice algo!!");
		if (calidad == "NoMuyMala") {
			chooseGoodNeg.SetActive(true); 
		} else {
			chooseBad.SetActive(true); 
			
		}
	}

	public void qualityRegular() {
		if (calidad == "Regular") {
			chooseGoodPos.SetActive(true); 
		} else {
			chooseBad.SetActive(true); 
			
		}
	}

	public void qualityBuena() {
		if (calidad == "Buena") {
			chooseGoodPos.SetActive(true); 
		} else {
			chooseBad.SetActive(true); 
			
		}
	}

	public void qualityNoMuyBuena() {
		if (calidad == "NoMuyBuena") {
			chooseGoodPos.SetActive(true); 
		} else {
			chooseBad.SetActive(true); 
			
		}
	}




	public void StartGameMacros () {
		Application.LoadLevel ("s"); 
	}

	
}
