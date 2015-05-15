#pragma strict

var music : AudioSource;

function Awake() {
	music = GameObject.FindWithTag("Music").GetComponent(AudioSource);

	//MenuMusic.music = GameObject.FindWithTag("Music").GetComponent(AudioSource);
	DontDestroyOnLoad(music);
}

function QuitGame () {
Debug.Log ("Game is exiting...");
	Application.Quit ();
}

function StartGame () {
	// para que el volumen no se vaya en la proxima escena
	//DontDestroyOnLoad(music);
	Application.LoadLevel("default");

}

function showMap () {
	// para que el volumen no se vaya en la proxima escena
	DontDestroyOnLoad(music);
	Application.LoadLevel("map");

}

function showStation (station : int) {
	// para que el volumen no se vaya en la proxima escena
	var gameObject : GameObject;
	gameObject =  GameObject.FindGameObjectWithTag ("station1");
	gameObject.SetActive(true);

}

/*

function StartGame (level: String) {
	Application.LoadLevel(level);
}

*/

function SetGameVolume (vol : float) {
	//MenuMusic.music.volume = vol;
	music.volume = vol;
}

var gameObjects : GameObject[];

function HideMenu () {
	gameObjects =  GameObject.FindGameObjectsWithTag ("menu1");
     for(var i = 0 ; i < gameObjects.length ; i ++)
         gameObjects[i].SetActive(false);
}


function ShowMenu () {
 	Application.LoadLevel("menu");
     for(var i = 0 ; i < gameObjects.length ; i ++)
         gameObjects[i].SetActive(true);
}







