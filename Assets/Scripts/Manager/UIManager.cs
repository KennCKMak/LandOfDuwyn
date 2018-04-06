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


    public GameObject panelRoles;
    public TextMeshProUGUI villagerCountText;
    public TextMeshProUGUI minerCountText;
    public TextMeshProUGUI woodcutterCountText;
    public TextMeshProUGUI militiaCountText;

	public TextMeshProUGUI targetText;
	public HealthBar healthBar;

    void Awake(){
		if (instance == null)
			instance = this;
		else
			Destroy (this);
		
		}


	void Start(){

		UpdateResourceCount ();
        UpdateVillagerCount ();
	}

    void Update() {
        UpdateVillagerCount();
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
		VillagerManager.IncreaseRole((AI_Character.Role)(num));
        UpdateVillagerCount();
	}

	public void DecreaseRole(int num){
		VillagerManager.DecreaseRole((AI_Character.Role)num);
        UpdateVillagerCount();  
    }

    public void ShowJobPanel(bool b) {
        panelRoles.SetActive(b);
    }

    public void UpdateVillagerCount() {
        villagerCountText.text = "Villager: " + GameManager.instance.villagers;
        minerCountText.text = "Miner: " + GameManager.instance.miners;
        woodcutterCountText.text = "Lumberer: " + GameManager.instance.woodcutters;
        militiaCountText.text = "Militia: " + GameManager.instance.militia;

    }



	public void UpdateHealthBar(float currHP, float maxHP, string name){
		healthBar.UpdateBar (currHP, maxHP);
		targetText.text = name;

		ShowHealthBar ();
	}

	public void ShowHealthBar(){

		targetText.gameObject.SetActive (true);
		healthBar.gameObject.SetActive (true);
	}

	public void HideHealthBar(){
		targetText.gameObject.SetActive (false);
		healthBar.gameObject.SetActive (false);
	}

}
