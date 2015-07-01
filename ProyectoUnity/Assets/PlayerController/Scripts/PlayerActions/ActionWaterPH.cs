using UnityEngine;
using System.Collections;

public class ActionWaterPH : MonoBehaviour {

	GameObject player;
	GameObject txt;
	GameObject txt2;
		
	public void performAction(GameObject pnl,GameObject pnl2,  int zona) {
		pnl.SetActive(false);
		pnl2.SetActive(true);
		txt= GameObject.Find ("Analisis");
		txt2= GameObject.Find ("InfoExtra");
		txt.GetComponent<ChangeText> ().Change(zona,txt);
		txt2.GetComponent<ChangeText> ().Change2(zona,txt2);
		pnl2.transform.position = new Vector3(Screen.width/2,((Screen.height-155)/2)+155, 0);
				
	}

	//Cambiar el llamado a este SelectPH
	public string SelectPH(string resultado){

			float choice = Random.Range(0.0F, 14.0F);
			
		if(choice > 0.0 && choice < 2.5){
			resultado="ab";

		}else{
			if(choice > 2.6 && choice < 4.9){
				resultado="cd";
				
			}else{
				if(choice > 5.0 && choice < 5.5){
					resultado="ef";
					
				}else{
					if(choice > 5.6 && choice < 6.0){
						resultado="gh";
						
					}else{
						if(choice > 6.1 && choice < 7.0){
							resultado="ij";
							
						}else{
							if(choice > 7.1 && choice < 10.0){
								resultado="kl";
								
							}else{
								if(choice > 10.1 && choice < 11.5){
									resultado="mn";
									
								}else{
									if(choice > 11.6 && choice < 14.0){
										resultado="op";
										
									}
								}
							}
						}
					}
				}
			}
		}
		return resultado;
	}
}
