using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour {


	public List<AI_Character> villagerList = new List<AI_Character>();

	public List<Resource> treeList = new List<Resource>();
	public List<Resource> rockList = new List<Resource>();
	public List<Building> buildingsList = new List<Building>();
	public List<EnemyCharacter> enemyList = new List<EnemyCharacter>();

	public int villagers;
	public int miners;
	public int woodcutters;
	public int militia;
	//public RockResource[] rockResourceArray;
	public int WoodAmount;
	public int StoneAmount;
	public int GoldAmount;


	public static GameManager instance;

	void Awake(){
		if (instance == null)
			instance = this;
		else
			Destroy (this);
		
		InitializeVillagers();
		InitializeResource();
		InitializeBuildings ();
		InitializeEnemies ();
	}

	void Start () {
		Time.timeScale = 1.0f;

		//Cursor.lockState = CursorLockMode.Locked;

	}

	void InitializeVillagers(){
		AI_Character[] villagersArray = FindObjectsOfType(typeof(AI_Character)) as AI_Character[];
		for (int i = 0; i < villagersArray.Length; i++) {
			villagerList.Add (villagersArray [i]);
		}
		villagers = villagerList.Count;
	}

	void InitializeResource(){
		Resource[] resourceArray = FindObjectsOfType(typeof(Resource)) as Resource[];
		for (int i = 0; i < resourceArray.Length; i++) {
			if (resourceArray [i].resourceType == Resource.ResourceType.Wood)
				treeList.Add (resourceArray [i]);
			else if (resourceArray [i].resourceType == Resource.ResourceType.Stone)
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

	void InitializeEnemies(){
		EnemyCharacter[] enemyArray = FindObjectsOfType (typeof(EnemyCharacter)) as EnemyCharacter[];
		for (int i = 0; i < enemyArray.Length; i++) {
			enemyList.Add (enemyArray [i]);
		}
	}




	// Update is called once per frame
	void Update () {
		
	}


	public EnemyCharacter FindClosestEnemy(AI_Character character, float detectionRange){
		int num = -1;
		float dist = detectionRange + 0.1f;

		NavMeshPath path = new NavMeshPath();
		for (int i = 0; i < enemyList.Count; i++) {
			if (!Distance.GetPath (character.transform.position, enemyList [i].transform.position, NavMesh.AllAreas, path)) {
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

		return enemyList [num];
	}

	public Resource FindClosestResource(AI_Character character, Resource.ResourceType resource){
		int num = -1;
		float dist = 9999;

		List<Resource> resourceList;
		if (resource == Resource.ResourceType.Wood)
			resourceList = treeList;
		else if (resource == Resource.ResourceType.Stone)
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

	public void AddResource(Resource.ResourceType resourceType, int num){
		switch (resourceType) {
		case Resource.ResourceType.Wood:
			WoodAmount += num;
			break;
		case Resource.ResourceType.Stone:
			StoneAmount += num;
			break;
		case Resource.ResourceType.Gold:
			GoldAmount += num;
			break;

		default:
			Debug.Log ("Added nothing");
			break;
		}
		UIManager.instance.UpdateResourceCount ();

	}
}
