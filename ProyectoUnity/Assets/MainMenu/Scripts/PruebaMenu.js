#pragma strict

function Start () {
	Debug.Log ("escena cargada");
	if (Application.loadedLevel == "menu") {
		Debug.Log ("llamando a escena menu");
	}
	
	var gameObjects : GameObject[];
	gameObjects =  GameObject.FindGameObjectsWithTag ("menuPrueba");
	for(var i = 0 ; i < gameObjects.length ; i ++) {
         if (gameObjects[i].active == true) {
         Debug.Log (gameObjects[i].name+": cargado");
         
         }
    }
    
    var gameObjects2 : GameObject[];
	gameObjects2 =  GameObject.FindGameObjectsWithTag ("panelPrueba");
	for(var j = 0 ; j < gameObjects2.length ; j ++) {
         if (gameObjects2[j].active == true) {
         Debug.Log (gameObjects2[j].name+": cargado");
         
         }
    }
    
     var gameObjects3 : GameObject[];
	gameObjects3 =  GameObject.FindGameObjectsWithTag ("botonesPrueba");
	for(var k = 0 ; k < gameObjects3.length ; k ++) {
         if (gameObjects3[k].active == true) {
         Debug.Log (gameObjects3[k].name+": cargado");
         
         }
    }
    
     var gameObjects4 : GameObject[];
	gameObjects4 =  GameObject.FindGameObjectsWithTag ("tituloPrueba");
	for(var h = 0 ; h < gameObjects4.length ; h ++) {
         if (gameObjects4[h].active == true) {
         Debug.Log (gameObjects4[h].name+": cargado");
         
         }
    }
	
}
