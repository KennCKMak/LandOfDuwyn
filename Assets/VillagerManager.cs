using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VillagerManager : MonoBehaviour {


	void Awake(){
		
	}
	// Use this for initialization
	void Start () {
		IncreaseRole (AI_Character.Role.Woodcutter);
		IncreaseRole (AI_Character.Role.Woodcutter);
		IncreaseRole (AI_Character.Role.Woodcutter);
		IncreaseRole (AI_Character.Role.Miner);
		IncreaseRole (AI_Character.Role.Miner);
		IncreaseRole (AI_Character.Role.Miner);
		IncreaseRole (AI_Character.Role.Militia);
		IncreaseRole (AI_Character.Role.Militia);
		
	}
	
	// Update is called once per frame
	void Update () {
		
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

	public static void IncreaseRole(AI_Character.Role newRole){
		if (CheckAvailableVillager ()) {
			GetAvailableVillager ().SetRole (newRole);
		} else {
			Debug.Log ("NO VILLAGER AVAILABLE");
		}
	}

}
