using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour {

	public static int defaultResourceActivationRange = 25;

	[Header("Entity Lists")]
	public List<AI_Character> villagerList = new List<AI_Character>();
	public List<Resource> treeList = new List<Resource>();
	public List<Resource> rockList = new List<Resource>();
	public List<Building> buildingsList = new List<Building>();
	public List<EnemyCharacter> enemyList = new List<EnemyCharacter>();

	[Header("Villager/Citizen count")]
	public int totalVillagers;
	public int startingVillagers;
	public int maxVillagers;
	[Header("Workers and Job")]
	public int villagers;
	public int miners;
	public int woodcutters;
	public int militia;
	[Header("Unit Prefabs")]
	public GameObject villagerPrefab;
	public GameObject enemyPrefab;
	//public RockResource[] rockResourceArray;
	[Header("Resource Count")]
	public int WoodAmount;
	public int StoneAmount;
	public int GoldAmount;


	public static GameManager instance;

	public float villagerSpawnTimer = 0.0f;

	void Awake(){
		if (instance == null)
			instance = this;
		else
			Destroy (this);
	}

	void Start () {
		Time.timeScale = 1.0f;

		//starting
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		for (int i = 0; i < startingVillagers; i++) {
			GameObject newGuy = Instantiate (villagerPrefab, new Vector3 (Random.Range (-5.0f, 5.0f), 0.0f, Random.Range (-5.0f, 5.0f)), Quaternion.identity) as GameObject;
			newGuy.name = villagerPrefab.name;
		}

		for (int i = 0; i < 2; i++) {
			//let's spawn some baddies far away. they'll come close soon
			Vector3 pos = Vector3.zero;

			pos.x = Random.Range (-5.0f, 5.0f) + 350.0f;
			pos.z = Random.Range (-5.0f, 5.0f);
			pos.y = 0.01f;


			GameObject newGuy = Instantiate (enemyPrefab, pos, Quaternion.identity) as GameObject;
			newGuy.name = enemyPrefab.name;
		}
	}
	void Update () {
		villagerSpawnTimer += Time.deltaTime;

		if (villagerSpawnTimer > 30.0f && totalVillagers < maxVillagers) {
			villagerSpawnTimer = 0.0f;
			for(int i = 0; i < (maxVillagers - totalVillagers)/2; i++){
				GameObject newGuy = Instantiate (villagerPrefab, new Vector3 (Random.Range (-5.0f, 5.0f), 0.0f, Random.Range (-5.0f, 5.0f)), Quaternion.identity) as GameObject;
				newGuy.name = villagerPrefab.name;
			}
		}

	}


	public void Add(AI_Character newVillager){
		if (villagerList.Contains (newVillager))
			return;
		villagerList.Add (newVillager);
		villagers++;
		totalVillagers++;
	}

	public void Remove(AI_Character oldVillager){
		if (villagerList.Contains (oldVillager)) {
			villagerList.Remove (oldVillager);
			totalVillagers--;
			switch(oldVillager.myRole){
			case AI_Character.Role.Villager:
				villagers--;
				break;
			case AI_Character.Role.Woodcutter:
				woodcutters--;
				break;
			case AI_Character.Role.Miner:
				miners--;
				break;
			case AI_Character.Role.Militia:
				militia--;
				break;
			default:
				break;
			}
			
		} else {
			Debug.Log ("Tried to remove non-existing villager");
		}
			
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

		int num = 0;
		foreach (Building building in buildingsList) {
			if (building.structure == Building.Structure.House)
				num++;
		}
		maxVillagers = num * 8;
	}

	public void Remove(Building newBuilding){
		if (buildingsList.Contains (newBuilding)) {
			buildingsList.Remove (newBuilding);
			if (newBuilding.structure == Building.Structure.House)
				maxVillagers -= 8;
			//TODO: Disconnect resources from buildings about to be deleted
		} else
			Debug.Log ("Tried to remove non-existing building");
	}

	public void Add(EnemyCharacter newEnemy){
		if (enemyList.Contains (newEnemy))
			return;
		enemyList.Add (newEnemy);
	}

	public void Remove(EnemyCharacter oldEnemy){
		if (enemyList.Contains (oldEnemy)) {
			enemyList.Remove (oldEnemy);

			//instantiate a new enemy!!
			Vector3 pos = Vector3.zero;

			pos.x = Random.Range (-5.0f, 5.0f) + 350.0f;
			pos.z = Random.Range (-5.0f, 5.0f);
			pos.y = 0.01f;


			GameObject newGuy = Instantiate (enemyPrefab, pos, Quaternion.identity) as GameObject;
			newGuy.name = enemyPrefab.name;
		} else {
			Debug.Log ("Tried to remove non-existing enemy");
		}
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

	public GameObject GetAlliedTarget(EnemyCharacter character, float detectionRange){
		List<GameObject> allAllies = new List<GameObject> ();
		allAllies.Add (GameObject.FindGameObjectWithTag ("Player"));
		foreach (AI_Character villager in villagerList) {
			allAllies.Add (villager.gameObject);
		}

		int num = -1;
		float dist = detectionRange + 0.1f;

		NavMeshPath path = new NavMeshPath();
		for (int i = 0; i < allAllies.Count; i++) {
			if (!Distance.GetPath (character.transform.position, allAllies [i].transform.position, NavMesh.AllAreas, path)) {
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

		return allAllies [num];

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
			//Debug.Log ("no resource found");
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

	/// <summary>
	/// Used by any character to find a path to a building target
	/// </summary>
	/// <returns>The building.</returns>
	/// <param name="character">Character.</param>
	/// <param name="buildingType">Building type.</param>
	public Building FindBuilding(AI_Character character, Building.Structure buildingType){
		int num = -1;
		float dist = 9999;
		NavMeshPath path = new NavMeshPath ();
		for (int i = 0; i < buildingsList.Count; i++) {
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

	/// <summary>
	/// used specifically for dropping off resources at the correcct building
	/// </summary>
	/// <returns>The building resource drop.</returns>
	/// <param name="character">Character requesting path.</param>
	/// <param name="buildingType">Building type.</param>
	/// <param name="resourceType">Resource type.</param>
	public Building FindBuildingResourceDrop(AI_Character character, Building.Structure buildingType, Resource.ResourceType resourceType){
		int num = -1;
		float dist = 9999;
		NavMeshPath path = new NavMeshPath ();
		for (int i = 0; i < buildingsList.Count; i++) {
			//check for right structure
			if (buildingsList [i].structure != buildingType) 
				continue;


			//check for right drop type
			if (buildingsList [i].dropType != Building.ResourceDropType.All && buildingsList [i].dropType.ToString () != resourceType.ToString ())
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
