using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Animator))]
public class PlayerCharacter : Character {


	[HideInInspector] public Rigidbody rb;
	[HideInInspector] public Animator anim;

	//State = controls movement
	public enum State {
		Stationary,
		Moving,
		Dead
	}

	State myState;
	Equipment equipment;
	Equipment.RightHand myWeapon = Equipment.RightHand.None;
	Resource.ResourceType myItem = Resource.ResourceType.None;
	int hitCount = 0;
	int maxHitCount = 3;


	void Awake(){
		SetState (State.Stationary);
		rb = GetComponent<Rigidbody> ();
		anim = GetComponent<Animator> ();
		equipment = GetComponent<Equipment> ();

		Initialize ();

	}


	void Initialize(){
		health = maxHealth;
		CharacterSpeed = maxWalkSpeed;
		maxRunSpeed = maxWalkSpeed * runMultiplier;

	}


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		ReadInput ();
		CheckLook ();

	}

	void ReadInput(){

        if (Input.GetKeyDown(KeyCode.Q)) {
			if (ConstructionManager.instance.currentPhase == ConstructionManager.ConstructionPhase.NotBuilding ||
				ConstructionManager.instance.currentPhase == ConstructionManager.ConstructionPhase.SelectingBuilding)  {
				CameraManager.instance.SwitchCameraState ();
			}
			anim.SetBool ("Moving", false);
        }
        if (CameraManager.CurrentCameraState != CameraManager.CameraState.FirstPerson)
			return;

		//MOVEMENT
		float move = Input.GetAxis ("Vertical") * Time.deltaTime * CharacterSpeed;
		float strafe = Input.GetAxis ("Horizontal") * Time.deltaTime * CharacterSpeed;
		transform.Translate (Vector3.forward * move);
		transform.Translate (Vector3.right * strafe);

		anim.SetBool ("Moving", move != 0 || strafe != 0);
		if (move < 0) {
			anim.SetFloat ("Direction", -1.0f);
		} else {
			anim.SetFloat ("Direction", 1.0f);
		}

		if (Input.GetKeyDown (KeyCode.LeftShift)) 
			StartRunning ();
		if(Input.GetKeyUp(KeyCode.LeftShift))
			StopRunning ();
		

		if (Input.GetMouseButton (0)) {
			Attack ();
		}

		if (weaponTimer > 0)
			weaponTimer -= Time.deltaTime;

		//CAMERA VIEWING
			float xRot = Camera.main.transform.eulerAngles.x + Input.GetAxis ("Mouse Y") * 8.0f * -1.0f;
			if (xRot < 310 && xRot > 150.0f)
				xRot = 310.0f;
			if (xRot > 50.0f && xRot < 150.0f)
				xRot = 50.0f;
			Vector3 newLook = new Vector3 (xRot, Camera.main.transform.eulerAngles.y + Input.GetAxis ("Mouse X") * 12.0f, 0);
			Camera.main.transform.rotation = Quaternion.Euler (newLook);
			transform.eulerAngles = new Vector3 (0, Camera.main.transform.eulerAngles.y, 0);



		//HOTKEYS
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			Equip (Equipment.RightHand.RoyalSword);
		}
		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			Equip (Equipment.RightHand.Axe);
		}
		if (Input.GetKeyDown (KeyCode.Alpha3)) {
			Equip (Equipment.RightHand.Pickaxe);
		}

		//Drop item
		if (Input.GetKeyDown (KeyCode.G)) {
			DropItem ();
		}
	}

	public void CheckHealth(){
		if (health < 0)
			SetState (State.Dead);


	}

	public void UpdateState(){
		switch (myState) {

		case State.Moving:
			break;
		case State.Dead:
		case State.Stationary:
		default:
			break;
		}

	}

	public void SetState(State newState){

		myState = newState;
		switch (newState) {
		case State.Stationary:
			break;
		case State.Moving:
			break;
		case State.Dead:
			anim.SetTrigger ("DeathA");
			break;
		default:
			break;

		}

	}

	public void StartRunning(){
		anim.SetBool ("Running", true);
		CharacterSpeed = maxRunSpeed;
	}

	public void StopRunning(){
		anim.SetBool ("Running", false);
		CharacterSpeed = maxWalkSpeed;
	}

	public void Attack(){
		if (weaponTimer > 0)
			return;

		weaponTimer = weaponSpeed;
		anim.SetTrigger ("Attack");
		CharacterSpeed = 0;
		Invoke ("RestoreMovement", 0.833f);

		CheckHit ();
	}

	public void CheckLook(){
		UIManager.instance.HideTargetText();
		UIManager.instance.HideCenterText ();
		UIManager.instance.HideHealthBar();

		Ray ray = new Ray (Camera.main.transform.position, Camera.main.transform.forward);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, 4.0f)) {

			if (hit.transform.tag == "Resource") {
				Resource resource = hit.transform.GetComponent<Resource> ();
				UIManager.instance.UpdateHealthBar (resource.amount, resource.maxAmount, hit.transform.name);

				if (hitCount < maxHitCount) {
					if (resource.resourceType == Resource.ResourceType.Wood)
						UIManager.instance.ShowCenterText ("Press LMB with the AXE to collect");
					else
						UIManager.instance.ShowCenterText ("Press LMB with the PICKAXE to collect");
				} else
					UIManager.instance.ShowCenterText ("Can't carry anymore!");

			}
			if (hit.transform.tag == "ResourceDrop" && 
				hit.transform.root.gameObject.GetComponent<Building> ().structure == Building.Structure.ResourceDrop &&
				myItem != Resource.ResourceType.None) {
				UIManager.instance.ShowCenterText ("Press 'G' to drop off resources!");
			}
			if (hit.transform.tag == "Building") {
				Building currentBuilding = hit.transform.root.gameObject.GetComponent<Building> ();
				UIManager.instance.ShowFlavourText (currentBuilding);
			}
			if (hit.transform.root.tag == "Ally") {
				AI_Character ally = hit.transform.root.gameObject.GetComponent<AI_Character> ();
				UIManager.instance.UpdateHealthBar (ally.health, ally.maxHealth, ally.transform.name);
			}
			if (hit.transform.root.tag == "Enemy") {
				EnemyCharacter enemy = hit.transform.root.gameObject.GetComponent<EnemyCharacter> ();
				UIManager.instance.UpdateHealthBar (enemy.health, enemy.maxHealth, enemy.transform.name);
			}

		} else {
			//not seeing anything
		}
	}

	public void CheckHit(){
		Ray ray = new Ray (Camera.main.transform.position, Camera.main.transform.forward);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, 6.0f)) {
			if (hit.transform.tag == "Resource") {
				if (hitCount >= maxHitCount && myItem != Resource.ResourceType.None) {
					UIManager.instance.ShowCenterText ("Can't carry anymore!");
					return;
				}
				Resource resource = hit.transform.gameObject.GetComponent<Resource> ();
				switch (resource.resourceType) {
				case Resource.ResourceType.Wood:
					if (myWeapon != Equipment.RightHand.Axe)
						break;
					hitCount++;


					if (hitCount < 3)
						return;
					myItem = resource.resourceType;
					anim.SetBool ("Wood", true);
					resource.DepleteAmount (1);
					break;

				case Resource.ResourceType.Gold:
				case Resource.ResourceType.Stone:
					if (myWeapon != Equipment.RightHand.Pickaxe)
						break;
					hitCount++;


					if (hitCount < 3)
						return;
					myItem = resource.resourceType;
					anim.SetBool ("Bag", true);
					resource.DepleteAmount (1);
					break;
				default:
					break;

				}
			} else if (hit.transform.tag == "Enemy") {
				hit.transform.GetComponent<EnemyCharacter> ().TakeDamage (20);
				Debug.Log ("Hit enemy");
			}
		}

	}

	public void DropItem(){
		Ray ray = new Ray (Camera.main.transform.position, Camera.main.transform.forward);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, 3.0f)) {

			if (hit.transform.tag == "ResourceDrop" && hit.transform.root.GetComponent<Building>()) {
				if (hit.transform.root.GetComponent<Building> ().structure == Building.Structure.ResourceDrop) {
					GameManager.instance.AddResource (myItem, 1);
					myItem = Resource.ResourceType.None;
					anim.SetBool ("Wood", false);
					anim.SetBool ("Bag", false);
					hitCount = 0;
				}
			}
		}
	}


	public void RestoreMovement(){
		CharacterSpeed = maxWalkSpeed;
	}

	public void Equip(Equipment.RightHand newWeapon){
		if (myWeapon == newWeapon) {
			myWeapon = Equipment.RightHand.None;
			equipment.EquipItem (Equipment.RightHand.None, Equipment.LeftHand.None);
			return;
		} 
		myWeapon = newWeapon;
		switch (newWeapon) {
		case Equipment.RightHand.Axe:
		case Equipment.RightHand.Pickaxe:
			equipment.EquipItem (newWeapon, Equipment.LeftHand.None);
			break;

		case Equipment.RightHand.Sword:
		case Equipment.RightHand.RoyalSword:
			equipment.EquipItem (newWeapon, Equipment.LeftHand.KiteShield);
			break;
		default:
			equipment.EquipItem (Equipment.RightHand.None, Equipment.LeftHand.None);
			break;
		}
	}

}
