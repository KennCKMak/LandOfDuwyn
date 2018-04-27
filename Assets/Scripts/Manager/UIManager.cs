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

	protected Color startColor;
	protected Color destColor;
	protected bool[] flashing = new bool[3];


    public GameObject panelRoles;
    public TextMeshProUGUI villagerCountText;
    public TextMeshProUGUI minerCountText;
    public TextMeshProUGUI woodcutterCountText;
    public TextMeshProUGUI militiaCountText;

	public TextMeshProUGUI targetText;
	public HealthBar healthBar;

	public GameObject TPSBar;
	public GameObject TDBar;
	public Color highlightedColor;
	public Color defaultColor;


	public GameObject ConstructionHelpText;
	public GameObject ConstructionInfoText;

    void Awake(){
		if (instance == null)
			instance = this;
		else
			Destroy (this);
		
		}


	void Start(){

		UpdateResourceCount ();
        UpdateVillagerCount ();


		startColor = woodCountText.color;
		destColor = startColor;
		for (int i = 0; i < 3; i++) {
			flashing [i] = false;
		}


	}

    void Update() {
        UpdateVillagerCount();

		for (int i = 0; i < 3; i++) {
			UpdateColorChange (i);
		}
    }
		
	public void SetCenterText(string message){
		centerText.text = message;
	}
	public void ShowCenterText(string message){
		centerText.gameObject.SetActive (true);
		centerText.text = message;
	}

	public void ShowCenterText(){

		centerText.gameObject.SetActive (true);
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


	public void SetTargetText(string name){
		targetText.text = name;
	}
	public void ShowTargetText(string name){
		SetTargetText (name);
		targetText.gameObject.SetActive (true);
	}

	public void ShowTargetText(){
		targetText.gameObject.SetActive (true);
	}
	public void HideTargetText(){
		targetText.gameObject.SetActive (false);
	}

	public void UpdateHealthBar(float currHP, float maxHP){
		healthBar.UpdateBarNoLerp (currHP, maxHP);
		ShowHealthBar ();
	}

	public void UpdateHealthBar(float currHP, float maxHP, string name){
		healthBar.UpdateBarNoLerp (currHP, maxHP);
		ShowTargetText(name);

		ShowHealthBar ();
	}

	public void ShowHealthBar(){
		ShowTargetText ();
		healthBar.gameObject.SetActive (true);
	}

	public void HideHealthBar(){
		HideTargetText ();
		healthBar.gameObject.SetActive (false);
	}

	public void ShowFlavourText(Building building){
		SetTargetText (building.name);
		string flavourText;


		switch (building.name) {
		case "House":
			flavourText =  ("Used to increase maximum workers\nCurrent Population: " + GameManager.instance.totalVillagers + "/" + GameManager.instance.maxVillagers);
			break;
		case "Smith":
			flavourText = ("Drop site for rocks and gold minerals\nRange: " + building.resourceActivationRange);
			break;
		case "Sawmill":
			flavourText = ("Drop site for wood/lumber\nRange: " + building.resourceActivationRange);
			break;
		case "Windmill":
			flavourText = ("Drop site for all types of materials\nRange: " + building.resourceActivationRange);
			break;
		default:
			flavourText = ("No info available for " + building.name);
			break;
		}

		ShowTargetText ();
		ShowCenterText (flavourText);

	}

	public void SelectActionBar(int num){
		DeselectActionBar ();
		if (CameraManager.CurrentCameraState == CameraManager.CameraState.FirstPerson) {
			TPSBar.transform.GetChild (num).GetComponent<Image> ().color = highlightedColor;
			TPSBar.transform.GetChild (num).GetComponent<RectTransform>().localScale = new Vector3 (1.08f, 1.08f, 1.0f);
		} else {
			TDBar.transform.GetChild (num).GetComponent<Image> ().color = highlightedColor;
			TDBar.transform.GetChild (num).GetComponent<RectTransform>().localScale = new Vector3 (1.08f, 1.08f, 1.0f);
		}
			
	}

	public void DeselectActionBar(){

		//loop through action bar tools
		for (int i = 0; i < 3; i++) {
			TPSBar.transform.GetChild (i).GetComponent<RectTransform>().localScale = new Vector3 (1.0f, 1.0f, 1.0f);
			TPSBar.transform.GetChild (i).GetComponent<Image> ().color = defaultColor;
		}

		//loop through td action bar tools
		for (int i = 0; i < 4; i++) {
			TDBar.transform.GetChild (i).GetComponent<RectTransform>().localScale = new Vector3 (1.0f, 1.0f, 1.0f);
			TDBar.transform.GetChild (i).GetComponent<Image> ().color = defaultColor;
		}
	}

	public void SwitchActionBar(CameraManager.CameraState newState){
		switch (newState) {
		case CameraManager.CameraState.FirstPerson:
			TPSBar.SetActive (true);
			TDBar.SetActive (false);
			break;
		case CameraManager.CameraState.TopDown:
			TPSBar.SetActive (false);
			TDBar.SetActive (true);
			break;
		default:
			Debug.Log ("Wtf?");
			break;
		}
	}

	public void ShowConstructionHelpText(bool b){
		ConstructionHelpText.SetActive (b);
	}

	public void ShowConstructionInfoText(bool b){
		ConstructionInfoText.SetActive (b);
		ConstructionInfoText.GetComponent<TextMeshProUGUI> ().text = "";
	}

	public void UpdateConstructionInfoText(ConstructionManager.BuildingCost cost){
		string words = "<b>"+cost.Name + "</b>\n";
		if (cost.Wood > 0)
			words += "Wood: " + cost.Wood + "\n";
		if (cost.Stone > 0)
			words += "Stone: " + cost.Stone + "\n";
		if (cost.Gold > 0)
			words += "Gold: " + cost.Gold + "\n";

		string flavourText = "";
		switch (cost.Name) {
		case "Sawmill":
			flavourText = "Activates nearby tree resources\nin a small area";
			break;
		case "Smith":
			flavourText = "Activates nearby stone and gold resources\nin an area";
			break;
		case "House":
			flavourText = "Increases maximum villager population by 8";
			break;
		case "Windmill":
			flavourText = "Activates all resources\nin a massive area";
			break;

		default:
			Debug.Log ("Weird building name here");
			break;
		}

		ConstructionInfoText.GetComponent<TextMeshProUGUI> ().text = words + flavourText;
	}




	public void ResetColor(){
		for(int i = 0; i < 3; i++){
			ResetColor (i);
		}
	}

	public void ResetColor(int num){
		destColor = startColor;
	}


	public void FlashResourceText(Color color, Resource.ResourceType resourceType){
		//Debug.Log ("FLASH");
		//woodCountText.color = Color.red;
		switch (resourceType) {
		case Resource.ResourceType.Wood: 
			StartCoroutine (FlashCoroutine (color, 0));
			break;
		case Resource.ResourceType.Stone: 
			StartCoroutine(FlashCoroutine (color, 1));
			break;
		case Resource.ResourceType.Gold: 
			StartCoroutine(FlashCoroutine (color, 2));
			break;
		default:
			Debug.Log ("Defaulting");
			break;
		}
	}

	IEnumerator FlashCoroutine(Color color, int num){
		flashing [num] = true;

		if (num == 0)
			woodCountText.color = color;
		else if (num == 1)
			stoneCountText.color = color;//Color.Lerp (woodCountText.color, color, 30.0f * Time.deltaTime);
		else if (num == 2)
			goldCountText.color = color;//Color.Lerp (woodCountText.color, color, 30.0f * Time.deltaTime);
		
		yield return new WaitForSeconds (0.1f);

		flashing[num] = false;
		
	}

	private void UpdateColorChange(int num){
		if (num == 0)
			woodCountText.color = Color.Lerp (woodCountText.color, destColor, 8.0f * Time.deltaTime);
		if(num == 1)
			stoneCountText.color = Color.Lerp (stoneCountText.color, destColor, 8.0f * Time.deltaTime);
		if(num == 2)
			goldCountText.color = Color.Lerp (goldCountText.color, destColor, 8.0f * Time.deltaTime);

	}
}
