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


	}

	void ReadInput(){
		float turn = Input.GetAxis ("Horizontal") * Time.deltaTime * 150.0f;
		float move = Input.GetAxis ("Vertical") * Time.deltaTime * CharacterSpeed;
		transform.Rotate (0, turn, 0);
		transform.Translate (Vector3.forward * move);

		anim.SetBool ("Moving", move != 0 || turn != 0);
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
	}
	public void RestoreMovement(){
		CharacterSpeed = maxWalkSpeed;
	}


}
