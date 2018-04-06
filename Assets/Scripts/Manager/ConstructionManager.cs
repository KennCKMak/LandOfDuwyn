using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionManager : MonoBehaviour {



	public GameObject[] BuildingPrefabs;



	public static ConstructionManager instance;


	public enum ConstructionPhase{
		NotBuilding,
		SelectingBuilding,
		PlacingBuilding,
	}

	public ConstructionPhase currentPhase;

	public GameObject hologram;
	public float rotationElapsedTime = 0.0f;

	void Awake(){
		if (instance == null)
			instance = this;
		else
			Destroy (gameObject);
		currentPhase = ConstructionPhase.NotBuilding;
	}


	// Use this for initialization
	void Start () {

		setCurrentPhase(ConstructionPhase.NotBuilding);
	}
	
	// Update is called once per frame
	void Update () {
		if (CameraManager.CurrentCameraState != CameraManager.CameraState.TopDown) {
			if (currentPhase != ConstructionPhase.NotBuilding) {
				setCurrentPhase (ConstructionPhase.NotBuilding);
				return;
			}
			//this only works IF you are currently building
		} else if (CameraManager.CurrentCameraState == CameraManager.CameraState.TopDown) {
			if (currentPhase == ConstructionPhase.NotBuilding)
				setCurrentPhase (ConstructionPhase.SelectingBuilding);
		}



		if (Input.GetKeyDown (KeyCode.Escape) || Input.GetMouseButtonDown (1)) {
			Cancel ();
		}
		if (Input.GetKeyDown (KeyCode.Q)) {

		}

		if (currentPhase == ConstructionPhase.SelectingBuilding) {
			if (Input.GetKeyDown (KeyCode.Alpha1))
				StartBuilding (BuildingPrefabs [0]);
			if (Input.GetKeyDown (KeyCode.Alpha2))
				StartBuilding (BuildingPrefabs [1]);
			if (Input.GetKeyDown (KeyCode.Alpha3))
				StartBuilding (BuildingPrefabs [2]);
			if (Input.GetKeyDown (KeyCode.Alpha4))
				StartBuilding (BuildingPrefabs [3]);
		
		} else if (currentPhase == ConstructionPhase.PlacingBuilding) {

			UpdateHologramMovement ();
			if (Input.GetMouseButtonDown (0)) {
				PlaceBuilding ();
			}
		}
	}

	void StartBuilding(GameObject selectedBuilding){
		GameObject newBuilding = Instantiate (selectedBuilding, Camera.main.ScreenToWorldPoint (Input.mousePosition), Quaternion.identity) as GameObject;

		if (newBuilding.GetComponent<Building> ().structure == Building.Structure.ResourceDrop) {
			GameObject highlightRange = newBuilding.transform.FindChild ("HighlightRange").gameObject;
			highlightRange.SetActive (true);
			highlightRange.transform.localScale = 
				new Vector3 (newBuilding.GetComponent<Building> ().resourceActivationRange*2, 
					highlightRange.transform.localScale.y, 
					newBuilding.GetComponent<Building> ().resourceActivationRange*2);

		}
		newBuilding.name = selectedBuilding.name;

		newBuilding.GetComponent<Building> ().SetColliders (false);
		//disable the building scripts immediately
		newBuilding.GetComponent<Building> ().enabled = false;


		//disable all collision

		hologram = newBuilding;
		setCurrentPhase(ConstructionPhase.PlacingBuilding);

	}

	void UpdateHologramMovement(){
		if (!hologram) {
			Debug.Log ("wtf? Lost hologram target");
			setCurrentPhase(ConstructionPhase.SelectingBuilding);
			return;
		}

		//moving the object
		int terrainLayerMask = 1 << 9;
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast (ray, out hit, 9999.0f, terrainLayerMask)) {
			hologram.transform.position = hit.point;
		}


		float multiplier = 1.0f;
		if (Input.GetKey (KeyCode.R) || Input.GetKey (KeyCode.T)) {
			rotationElapsedTime += Time.deltaTime;
			if (rotationElapsedTime > 2.0f)
				multiplier = 3.0f;
		}

		if (Input.GetKey(KeyCode.R)) 
			hologram.transform.Rotate (Vector3.up, 40.0f * multiplier * Time.deltaTime);
		if (Input.GetKey(KeyCode.T)) 
			hologram.transform.Rotate (Vector3.up, -40.0f * multiplier * Time.deltaTime);
		
		if (Input.GetKeyUp (KeyCode.R) || Input.GetKeyUp (KeyCode.T)) {
			rotationElapsedTime = 0.0f;
			Debug.Log ("resetting");
		}
		


		//if (GameObject.Find("Terrain").gameObject.GetComponent<Collider>().Raycast (ray, out hit, Mathf.Infinity)) {
		//	hologram.transform.position = hit.point;
		//}
		//hologram.transform.position = Camera.main.ScreenToWorldPoint (Input.mousePosition);



	}

	bool isHologramValid(){
		//TODO: CHECK FOR VALID PLACEMENT
		return true;
	}

	void PlaceBuilding(){
		//placing onto the terrain
		if (!isHologramValid())
			return;



		//update gamemanager list
		GameManager.instance.buildingsList.Add (hologram.GetComponent<Building> ());
		//building is now functioning
		hologram.GetComponent<Building> ().enabled = true;

		hologram.GetComponent<Building> ().SetColliders (true);


		hologram = null;


		setCurrentPhase (ConstructionPhase.NotBuilding);
	}

	public void setCurrentPhase(ConstructionPhase phase){
		currentPhase = phase;
		switch (phase) {
		case ConstructionPhase.NotBuilding:
			UIManager.instance.ShowConstructionHelpText (false);

			break;
		case ConstructionPhase.SelectingBuilding:
			UIManager.instance.ShowConstructionHelpText (true);

			break;
		case ConstructionPhase.PlacingBuilding:
			UIManager.instance.ShowConstructionHelpText (true);
			break;
		default:
			break;
			

		}
	}

	void Cancel(){
		switch (currentPhase) {
		case ConstructionPhase.NotBuilding:
			//nothing happens here, you haven't even selected anything!
			break;

		case ConstructionPhase.SelectingBuilding:
			//here is where you press a number and select your choice


			break;

		case ConstructionPhase.PlacingBuilding:
			//here hologram is placing something
			if (hologram) {
				Destroy (hologram.gameObject);
				//hologram is now null
				setCurrentPhase(ConstructionPhase.SelectingBuilding);


			}

			break;

		default:
			break;
		}
	}


}
