using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour {


	public static UIManager instance;

	public TextMeshProUGUI centerText;
	public TextMeshProUGUI woodCountText;
	public TextMeshProUGUI stoneCountText;
	public TextMeshProUGUI goldCountText;

	void Awake(){
		if (instance == null)
			instance = this;
		else
			Destroy (this);
		
		}


	void Start(){

		UpdateResourceCount ();
	}
	public void ShowCenterText(string message){
		centerText.gameObject.SetActive (true);
		centerText.text = message;
	}

	public void HideCenterText(){
		centerText.gameObject.SetActive (false);
	}

	public void UpdateResourceCount(){
		woodCountText.text = "Wood: " + GameManager.instance.WoodAmount;
		stoneCountText.text = "Stone: " + GameManager.instance.StoneAmount;
		goldCountText.text = "Gold: " + GameManager.instance.GoldAmount;

	}

	public void IncreaseRole(int num){
		VillagerManager.IncreaseRole((AI_Character.Role)num);
	}

	public void DecreaseRole(int num){
		VillagerManager.DecreaseRole((AI_Character.Role)num);
	}


}
