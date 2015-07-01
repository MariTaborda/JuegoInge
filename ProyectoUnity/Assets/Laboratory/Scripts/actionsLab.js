#pragma strict

var gameObjects : GameObject[];
var index : int;  // 0 - 21
var first : int;
var last : int;
var newIndex : int;
var flag : int; 
index = 10;  
public var MacroCards : UnityEngine.UI.Button;

public var panel : GameObject;

function Start () {
    index = 10;
    first = 0;
    last = 21;
	 // cuando se active el panel, todo estara en true, 
	 // desactivamos los que no ocupamos y dejamos el index
	 // activado
    gameObjects =  GameObject.FindGameObjectsWithTag ("Card");
    //gameObjects[index].SetActive(true);
     	for(var i = 0 ; i < gameObjects.length ; i ++) {
	     	 if (i != index) {
	                  gameObjects[i].SetActive(false);
	         }
     	} 
}

function leftArrow () {
	// cuando da clic en izquierda, retrocede indice en array de objetos
	if (index != first) {
		newIndex = index-1;
		gameObjects[index].SetActive(false);
		 gameObjects[newIndex].SetActive(true);
		 index--; 
	}
	
}

function rightArrow () {
	if (index != last) {
		//Debug.Log("index:"+index+" last:"+last);
			newIndex = index+1;
			gameObjects[index].SetActive(false);
			 gameObjects[newIndex].SetActive(true);
			 index++; 
		}
}

public function close(panel : GameObject) {
	//panel =  GameObject.FindGameObjectWithTag ("CardsClosePanel");
	for(var i = 0 ; i < gameObjects.length ; i ++) {	
	   	gameObjects[i].SetActive(true);
     }
	panel.SetActive(false);
	MacroCards.interactable = true; 
	
}



