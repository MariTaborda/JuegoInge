using UnityEngine;
using System.Collections;

public class ActionWaterO2 : MonoBehaviour {
	
	GameObject player;
	GameObject txt;
	
	public void performAction(GameObject pnl,GameObject pnl5) {

		int choice= Random.Range(0,3);
		Debug.Log (choice);
		pnl.SetActive(false);
		pnl5.SetActive(true);
		txt= GameObject.Find ("AnalisisO2");
		txt.GetComponent<ChangeText>().Change3(choice);
		pnl5.transform.position = new Vector3(Screen.width/2,((Screen.height-155)/2)+155, 0);
		
	}
	
	//Cambiar el llamado a este SelectO2
	public string SelectO2(string resultado){
		
		int choice = Random.Range(0, 2);
		
		if(choice ==0){
			resultado="El agua presenta un nivel de oxigeno abundando, esto indica que el ambiente acuatico es sano, esta libre de contaminacion ";
			
		}else{
			if(choice ==1){
				resultado="El agua presenta un nivel de oxigeno medio, esto indica que el ambiente acuatico puede ocasionar problemas para algunas especies.";
				
			}else{
				if(choice ==2){
					resultado="El agua presenta un nivel de oxigeno bajo, esto indica que el ambiente acuatico esta contaminado, muchas especies no pueden vivir en estas aguas.";
					
				}
			}
		}
		return resultado;
	}
}
