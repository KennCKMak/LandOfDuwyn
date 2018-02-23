using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VillagerManager : MonoBehaviour {


	void Awake(){
		
	}
	// Use this for initialization
	void Start () {
		/*IncreaseRole (AI_Character.Role.Woodcutter);
		IncreaseRole (AI_Character.Role.Woodcutter);
		IncreaseRole (AI_Character.Role.Woodcutter);
		IncreaseRole (AI_Character.Role.Miner);
		IncreaseRole (AI_Character.Role.Miner);
		IncreaseRole (AI_Character.Role.Miner);
		IncreaseRole (AI_Character.Role.Militia);
		IncreaseRole (AI_Character.Role.Militia);*/
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.R)) {
			IncreaseRole (AI_Character.Role.Militia);
		}
		if (Input.GetKeyDown (KeyCode.T)) {
			DecreaseRole (AI_Character.Role.Militia);
		}
		if (Input.GetKeyDown (KeyCode.Y)) {
			IncreaseRole (AI_Character.Role.Miner);
		}
		if (Input.GetKeyDown (KeyCode.U)) {
			DecreaseRole (AI_Character.Role.Miner);
		}
	}

	/// <summary>
	/// Checks for an available villager.
	/// </summary>
	public static bool CheckAvailableVillager(){
		for (int i = 0; i < GameManager.instance.villagerList.Count; i++) {
			if (GameManager.instance.villagerList [i].myRole == AI_Character.Role.Nothing) {
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Returns true/false, and returns one via parameter
	/// </summary>
	/// <returns>The available villager.</returns>
	/// <param name="outVillager">Output an available villager.</param>
	public static AI_Character GetAvailableVillager(){
		for (int i = 0; i < GameManager.instance.villagerList.Count; i++) {
			if (GameManager.instance.villagerList [i].myRole == AI_Character.Role.Nothing) {
				return GameManager.instance.villagerList [i];
			}
		}
		return null;
	}

	public static bool CheckVillagerInRole(AI_Character.Role role){
		for (int i = 0; i < GameManager.instance.villagerList.Count; i++) {
			if (GameManager.instance.villagerList [i].myRole == role) {
				return true;
			}
		}
		return false;
	}

	public static AI_Character GetVillagerInRole(AI_Character.Role role){
		for (int i = 0; i < GameManager.instance.villagerList.Count; i++) {
			if (GameManager.instance.villagerList [i].myRole == role) {
				return GameManager.instance.villagerList [i];
			}
		}
		return null;
	}


	public static void IncreaseRole(AI_Character.Role newRole){
		if (CheckAvailableVillager ()) {
			AI_Character chosenVillager = GetAvailableVillager ();
			UpdateRoleCount (chosenVillager.myRole, newRole);
			chosenVillager.SetRole (newRole);

		} else {
			Debug.Log ("NO VILLAGER AVAILABLE");
		}
	}

	public static void DecreaseRole(AI_Character.Role removingRole){
		if (CheckVillagerInRole (removingRole)) {
			AI_Character chosenVillager = GetVillagerInRole (removingRole);
			UpdateRoleCount (chosenVillager.myRole, AI_Character.Role.Nothing);
			chosenVillager.SetRole (AI_Character.Role.Nothing);
		} else {
			Debug.Log ("No villagers in that role");
		}
	}

	public static void UpdateRoleCount(AI_Character.Role oldRole, AI_Character.Role newRole){
		switch (oldRole) {
		case AI_Character.Role.Nothing:
			GameManager.instance.villagers--;
			break;
		case AI_Character.Role.Miner:
			GameManager.instance.miners--;
			break;
		case AI_Character.Role.Woodcutter:
			GameManager.instance.woodcutters--;
			break;
		case AI_Character.Role.Militia:
			GameManager.instance.militia--;
			break;

		default:
			break;
		}


		switch (oldRole) {
		case AI_Character.Role.Nothing:
			GameManager.instance.villagers++;
			break;
		case AI_Character.Role.Miner:
			GameManager.instance.miners++;
			break;
		case AI_Character.Role.Woodcutter:
			GameManager.instance.woodcutters++;
			break;
		case AI_Character.Role.Militia:
			GameManager.instance.militia++;
			break;

		default:
			break;
		}
	}

}
