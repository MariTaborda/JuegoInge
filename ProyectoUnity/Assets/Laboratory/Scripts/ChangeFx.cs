using UnityEngine;
using System.Collections;
using UnityEngine.UI; 

public class ChangeFx : MonoBehaviour {

	// Use this for initialization
	string funcion;
	public InputField inputFileText; 
	public Text prueba;
	// Update is called once per frame
	void Update () {
		funcion = inputFileText.text;
		prueba.text = "prueba: "+funcion; 
	}

	void guardar() {
		//funcion = inputFileText.text;
		//prueba.text = funcion; 
	}

	string ActualFuntion() {
		int hola = 0; 
		return funcion;
	}
}
