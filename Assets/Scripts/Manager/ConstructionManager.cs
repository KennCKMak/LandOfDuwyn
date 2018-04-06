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
		
	}
	
	// Update is called once per frame
	void Update () {
		if (CameraManager.CurrentCameraState != CameraManager.CameraState.TopDown) {
			currentPhase = ConstructionPhase.NotBuilding;
			return;
			//this only works IF you are currently building
		} else  {
			if(currentPhase == ConstructionPhase.NotBuilding)
				currentPhase = ConstructionPhase.SelectingBuilding;
		}


		if (Input.GetKeyDown (KeyCode.Escape)) {
			Cancel ();
		}

		if (currentPhase == ConstructionPhase.SelectingBuilding) {
			if (Input.GetKeyDown (KeyCode.Alpha1)) 
				StartBuilding (BuildingPrefabs [0]);
			
		} else if (currentPhase == ConstructionPhase.PlacingBuilding) {

			UpdateHologramMovement ();
			if (Input.GetMouseButtonDown (0)) {
				PlaceBuilding ();
			}
		}



	}

	void StartBuilding(GameObject selectedBuilding){

		GameObject newBuilding = Instantiate (selectedBuilding, Camera.main.ScreenToWorldPoint (Input.mousePosition), Quaternion.identity) as GameObject;

		if (newBuilding.GetComponent<Building> ().structure == Building.Structure.ResourceDrop)
			newBuilding.transform.FindChild ("HighlightRange").gameObject.SetActive (true);


		newBuilding.GetComponent<Building> ().SetColliders (false);
		//disable the building scripts immediately
		newBuilding.GetComponent<Building> ().enabled = false;


		//disable all collision

		hologram = newBuilding;
		currentPhase = ConstructionPhase.PlacingBuilding;

	}

	void UpdateHologramMovement(){
		if (!hologram) {
			Debug.Log ("wtf? Lost hologram target");
			currentPhase = ConstructionPhase.SelectingBuilding;
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
		return false;
	}

	void PlaceBuilding(){
		//placing onto the terrain
		if (!isHologramValid())
			return;



		//update gamemanager list
		GameManager.instance.buildingsList.Add (hologram.GetComponent<Building> ());
		//building is now functioning
		hologram.GetComponent<Building> ().enabled = true;

		hologram.GetComponent<Building> ().SetColliders (false);


		hologram = null;



		currentPhase = ConstructionPhase.NotBuilding;
	}

	void Cancel(){
		switch (currentPhase) {
		case ConstructionPhase.NotBuilding:
			break;

		case ConstructionPhase.SelectingBuilding:
			//here is where you press a button and select your choice


			break;

		case ConstructionPhase.PlacingBuilding:
			//here hologram is placing something


			break;

		default:
			break;
		}
	}


}
