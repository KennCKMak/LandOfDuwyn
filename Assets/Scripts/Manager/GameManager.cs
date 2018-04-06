using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour {

	public static int defaultResourceActivationRange = 50;

	[Header("Entity Lists")]
	public List<AI_Character> villagerList = new List<AI_Character>();
	public List<Resource> treeList = new List<Resource>();
	public List<Resource> rockList = new List<Resource>();
	public List<Building> buildingsList = new List<Building>();
	public List<EnemyCharacter> enemyList = new List<EnemyCharacter>();

	[Header("Villager/Citizen count")]
	public int villagers;
	public int startingVillagers;
	public int maxVillagers;
	[Header("Workers and Job")]
	public int miners;
	public int woodcutters;
	public int militia;
	[Header("Unit Prefabs")]
	public GameObject villagerPrefab;
	//public RockResource[] rockResourceArray;
	[Header("Resource Count")]
	public int WoodAmount;
	public int StoneAmount;
	public int GoldAmount;


	public static GameManager instance;

	void Awake(){
		if (instance == null)
			instance = this;
		else
			Destroy (this);
	}

	void Start () {
		Time.timeScale = 1.0f;

		//Cursor.lockState = CursorLockMode.Locked;

	}

	void TestTrees(){

	}



	public void Add(AI_Character newVillager){
		if (villagerList.Contains (newVillager))
			return;
		villagerList.Add (newVillager);
	}

	public void Add(Resource newResource){

		if (treeList.Contains (newResource) || rockList.Contains(newResource))
			return;

		switch (newResource.resourceType) {
		case Resource.ResourceType.Wood:
			treeList.Add (newResource);
			break;
		case Resource.ResourceType.Stone:
			rockList.Add (newResource);
			break;
		default:
			break;
		}
	}


	public void Add(Building newBuilding){
		if (buildingsList.Contains (newBuilding))
			return;
		buildingsList.Add (newBuilding);
	}


	public void Add(EnemyCharacter newEnemy){
		if (enemyList.Contains (newEnemy))
			return;
		enemyList.Add (newEnemy);
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


		if (resourceList.Count <= 0) {
			Debug.Log ("no resource found");
			return null;
		}


		NavMeshPath path = new NavMeshPath();
		for (int i = 0; i < resourceList.Count; i++) {

			if (resourceList [i].Activated == false) {
				continue;
			}

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
		Debug.Log ("In function");
		for (int i = 0; i < buildingsList.Count; i++) {
			Debug.Log ("In searching of " + i);
			if (buildingsList [i].structure != buildingType) 
				continue;
			
			if (!Distance.GetPath (character.transform.position, buildingsList [i].buildingTarget.transform.position, NavMesh.AllAreas, path)) {
				continue;
			} else {
				if(Distance.GetPathLength(path)<dist) {
					dist = Distance.GetPathLength (path);
					num = i;
				}
			}

		}

		if (num == -1) {
			return null;
		}
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
			Debug.Log ("Added strange type: " + resourceType.ToString());
			break;
		}
		UIManager.instance.UpdateResourceCount ();

	}
}
