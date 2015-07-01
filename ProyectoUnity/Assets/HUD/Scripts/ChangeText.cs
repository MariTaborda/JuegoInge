using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChangeText : MonoBehaviour {
	public Text txt;
	
	//private ActionWaterPH AW;
	private string hola;
	
	// Update is called once per frame
	public void Change(int zona, GameObject y) {
		
		hola= SelectPH(hola,zona,y); 
		txt.text = hola;
		
	}
	
	public void Change3(int choice) {
		
		hola= SelectO2(hola, choice); 
		txt.text = hola;
		
	}
	
	public void Change2(int zona, GameObject x) {
		
		hola= SelectPH(hola,zona,x); 
		txt.text = hola;
		
	}
	
	public string SelectO2(string resultado, int choice){
		// 0 = zona alta,1= zona baja y 2 = zona media
		if(choice ==0){
			resultado="El agua presenta un nivel de oxigeno abundante, esto indica que el ambiente acuatico es sano, esta libre de contaminacion ";
			
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
	
	public string SelectPH(string resultado,int zona,GameObject go){


			if(zona==0){// alta
				int choice= Random.Range(0,2);


						if(choice== 0){ // zona alta
							//cerca del 5.6 en la escala de PH
							if(go.name=="Analisis"){
								resultado="El papel indicador cambio a un color amarillento, este valor indica que el agua presenta un estado neutral con restos acidos, la lluvia limpia se encuentra a estos niveles, por lo que este nivel no es perfudicial para la mayoria de las criaturas.";
							}else{
								resultado ="Las truchas arco iris no resisten estos niveles y perecen.";
							}
						}else{
							if(choice == 1){ // zona alta
								//cerca del 7.0 en la escala de PH
								if(go.name=="Analisis"){
									resultado="El papel indicador cambio a un color entre amarillo y verde claro, este valor indica que el agua presenta un estado neutro.";
								}else{
									resultado ="El agua no presenta contaminates y es adecuada para la vida de animales de rio.";
								}
							}
						}
					
			}else{
				if(zona==1){ // baja
					int choice= Random.Range(0,3);


						if(choice == 0){ // zona donde estan las plantaciones (zona baja por mientras)
							//cerca del 5.0 en la escala de PH
							if(go.name=="Analisis"){
								resultado="El papel indicador cambio a un color entre naranja y amarillo, este valor indica que el agua presenta un estado acido leve, restos de cultivos de banano o cafe podrian estar mezclados con el agua.";
							}else{
								resultado ="La reproduccion de los peces resulta afectada. Huevos de otras criaturas como la rana, los rencuajos y los cangrejos de rio no sobreviven";
							}
						}else{
							if(choice == 1){// zona baja
								//cerca del 8.0 en la escala de PH
								if(go.name=="Analisis"){ 
									resultado="El papel indicador cambio a un color entre verde claro y azul marino, este valor indica que el agua presenta un estado base leve.";
								}else{
									resultado ="Acido urico, restos de jabon o bicarbonato de sodio podrian estar mezclados con el agua.";
								}
							}else{
								if(choice == 2){ // zona donde estan las plantaciones (zona baja por mientras)
									//cerca del 11,5 en la escala de PH
									if(go.name=="Analisis"){
										resultado="El papel indicador cambio a un color Azul oscuro, este valor indica que el agua presenta un estado base medio.";
									}else{
										resultado ="Pesticidas o resina que utilizan Amoniaco podrian estar mezclados con el agua.";
									}
								}
							}
						}

				}else{// zona = 2 -> media
					int choice= Random.Range(0,3);

				if(choice == 0){ // zona media
					//cerca del 2.5 en la escala de PH
					if(go.name=="Analisis"){
						resultado ="El papel indicador cambio a un color entre rojo y vino,este valor indica que el agua presenta un estado bastante acido.";
					}else{
						resultado = "Refrescos gaseosos, restos de acido de bateria o vinagre podrian estar mezclados con el agua ";
					}
				} else{
					if(choice == 1){ // zona media
						//cerca del 4.2 en la escala de PH
						if(go.name=="Analisis"){
							resultado="El papel indicador cambio a un color naranja oscuro, este valor indica que el agua esta en un estado acido medio, la cerveza y la lluvia acida podrian estar mezcladas con el agua.";
						}else{
							resultado = "No se ven peces en la zona, es posible que se extinguienran debido al nivel de PH.";
						}
					}else{
						if(choice == 2){ // zona media
							//cerca del 12 o 13 en la escala de PH
							if(go.name=="Analisis"){
								resultado="El papel indicador cambio a un color entre azul oscuro y morado, este valor indica que el agua presenta un estado base alto.";
							}else{
								resultado ="Blanqueadores o limpiadores para desagues podrian estar mezclados con el agua.";
							}
						}
					}
				}
			}
		}
					
		return resultado;
	}
}