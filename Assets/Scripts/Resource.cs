using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour {

	public enum ResourceType{
		None,
		Wood,
		Stone,
		Gold
	}
	public bool Activated = false;
	public ResourceType resourceType;
	public int amount;
	public int maxAmount;

	private bool depleted = false;
	public GameObject usedResourcePrefab;


	public bool seenByCamera = false;
	public GameObject lineRenderPrefab;

	public List<Building> listOfNearbyBuildings = new List<Building>();
	private int lineRenderers = 0;
	private List<LineRenderer> lines = new List<LineRenderer>();

	void Awake(){
		amount = maxAmount;
	}

	// Use this for initialization
	void Start () {
		GameManager.instance.Add (this);
		FindSurroundingBuildings ();
	}


	
	// Update is called once per frame
	void Update () {

		if (seenByCamera)
			drawActivatingLines ();
		else
			hideActivatingLines ();
	}

	void UsedAnimation(){
		if (usedResourcePrefab) 
			Instantiate (usedResourcePrefab, transform.position, transform.rotation);
		DestroyResource ();
	}

	public void DepleteAmount(int num){
		amount -= num;
		if (amount <= 0 && !depleted) {
			depleted = true;
			UsedAnimation ();
		}
	}

	public void DestroyResource(){
		GameManager.instance.treeList.Remove (this);
		Destroy (this.gameObject);
	}


	public void AddActivatingStructure(Building activatingBuilding){
		Activated = true;

		listOfNearbyBuildings.Add (activatingBuilding);
		GameObject newLineObject = Instantiate (lineRenderPrefab, transform);
		lines.Add (newLineObject.GetComponent<LineRenderer>());
		lineRenderers++;
	}

	public void FindSurroundingBuildings(){
		foreach (Building building in GameManager.instance.buildingsList) {
			if (building.structure == Building.Structure.ResourceDrop &&
			   Distance.GetHorizontalDistance (gameObject, building.gameObject) <= building.resourceActivationRange) {
				if (building.dropType.ToString () == resourceType.ToString () || building.dropType == Building.ResourceDropType.All) {
					if (listOfNearbyBuildings.Contains (building))
						continue;
					AddActivatingStructure (building);
					Activated = true;
				}
			}
		}
	}


	/// <summary>
	/// Draws lines from which this resource was activated by
	/// </summary>
	public void drawActivatingLines(){
		for (int i = 0; i < lineRenderers; i++) {

			Vector3 startPos = transform.position;
			startPos.y += 1.0f;
			Vector3 endPos = listOfNearbyBuildings [i].transform.position;
			endPos.y += 1.0f;
			lines [i].numPositions = 2;
			lines [i].SetPosition (0, startPos);
			lines [i].SetPosition (1, endPos);
			lines [i].enabled = true;
		}
	}

	public void hideActivatingLines(){
		for (int i = 0; i < lineRenderers; i++) {
			lines [i].enabled = false;
		}
	}
}
