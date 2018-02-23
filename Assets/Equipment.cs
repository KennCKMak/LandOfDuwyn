using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour {


	public GameObject rightHandParent;
	public enum RightHand{
		Axe,
		Battleaxe,
		Hammer,
		Warhammer,
		Pickaxe,
		Spear,
		Sword,
		RoyalSword,
		None
	}


	public GameObject leftHandParent;
	public enum LeftHand {
		Bow,
		SmallShield,
		TowerShield,
		KiteShield,
		KnightShield,
		WoodStaff,
		HolyStaff,
		ArcaneStaff,
		None
	}


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void EquipItem(RightHand rightHand, LeftHand leftHand){

		//right hand
	
		ActivateItem(rightHandParent, (int)rightHand);
		ActivateItem(leftHandParent, (int)leftHand);
	}

	void ActivateItem(GameObject parentObj, int childNum){
		for (int i = 0; i < parentObj.transform.childCount; i++) {
			if (childNum != i)
				parentObj.transform.GetChild (i).gameObject.SetActive (false);
			else
				parentObj.transform.GetChild (i).gameObject.SetActive (true);
		}
	}

}
