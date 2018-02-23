using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour {


	public List<AI_Character> villagerList = new List<AI_Character>();

	public List<Resource> treeList = new List<Resource>();
	public List<Resource> rockList = new List<Resource>();
	public List<Building> buildingsList = new List<Building>();
	//public RockResource[] rockResourceArray;


	public static GameManager instance;

	void Awake(){
		if (instance == null)
			instance = this;
		else
			Destroy (this);
		
		InitializeVillagers();
		InitializeResource();
		InitializeBuildings ();
	}

	void Start () {
	}

	void InitializeVillagers(){
		AI_Character[] villagersArray = FindObjectsOfType(typeof(AI_Character)) as AI_Character[];
		for (int i = 0; i < villagersArray.Length; i++) {
			villagerList.Add (villagersArray [i]);
		}
	}

	void InitializeResource(){
		Resource[] resourceArray = FindObjectsOfType(typeof(Resource)) as Resource[];
		for (int i = 0; i < resourceArray.Length; i++) {
			if (resourceArray [i].resourceType == Resource.ResourceType.Wood)
				treeList.Add (resourceArray [i]);
			else if (resourceArray [i].resourceType == Resource.ResourceType.Rock)
				rockList.Add (resourceArray [i]);
		}

	}


	void InitializeBuildings(){
		Building[] buildingArray = FindObjectsOfType (typeof(Building)) as Building[];
		for (int i = 0; i < buildingArray.Length; i++) {
			if (buildingArray [i].structure == Building.Structure.Base)
				buildingsList.Insert (0, buildingArray [i]);
			else
				buildingsList.Add (buildingArray[i]);
			
		}

	}




	// Update is called once per frame
	void Update () {
		
	}




	public Resource FindClosestResource(AI_Character character, Resource.ResourceType resource){
		int num = -1;
		float dist = 9999;

		List<Resource> resourceList;
		if (resource == Resource.ResourceType.Wood)
			resourceList = treeList;
		else if (resource == Resource.ResourceType.Rock)
			resourceList = rockList;
		else
			return null;

		NavMeshPath path = new NavMeshPath();
		for (int i = 0; i < resourceList.Count; i++) {
			if (!Distance.GetPath (character.transform.position, resourceList [i].transform.position, NavMesh.AllAreas, path)) {
				break;
			} else {
				if (Distance.GetPathLength (path) < dist) {
					dist = Distance.GetPathLength (path);
					num = i;
				}
			}
		}

		if (num == -1)
			return null;

		return resourceList [num];
	}

	public Building FindBuilding(AI_Character character, Building.Structure buildingType){
		int num = -1;
		float dist = 9999;
		NavMeshPath path = new NavMeshPath ();

		for (int i = 0; i < buildingsList.Count; i++) {
			if (buildingsList [i].structure != buildingType)
				break;
			if (!Distance.GetPath (character.transform.position, buildingsList [i].transform.position, NavMesh.AllAreas, path)) {
				break;
			} else {
				if(Distance.GetPathLength(path)<dist) {
					dist = Distance.GetPathLength (path);
					num = i;
				}
			}

		}
		if (num == -1)
			return null;
		return buildingsList [num];

	}
}
