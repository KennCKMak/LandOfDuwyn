using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// The villager class: All roles should inherit from this
/// Contains AI movement
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(NavMeshAgent), typeof(Animator))]
public class AI_Character : Character {


	NavMeshAgent agent;
	Rigidbody rb;
	Animator anim;


	public enum Task {
		Idle,
		Move,
		Gather,
		Return,
		Patrol,
		Attack
	};

	public enum Role{
		Villager,
		Woodcutter,
		Miner,
		Militia,
	};

	Task myTask;
	Role myRole;

	public GameObject target;
	public Vector3 targetPos;



	// Use this for initialization
	void Start () {
		InitializeComponents ();
	}

	void InitializeComponents(){
		agent = GetComponent<NavMeshAgent> ();
		anim = GetComponent<Animator> ();
		rb = GetComponent<Rigidbody> ();
		InitializeAgent ();
	}

	void InitializeAgent(){
		agent.speed = this.maxSpeed;
		agent.acceleration = this.acceleration;
		agent.angularSpeed = this.rotSpeed;
		running = false;
	}

	void InitializeVillager(){

	}

	// Update is called once per frame
	void Update () {
		anim.SetFloat ("Speed", agent.velocity.magnitude);
		if (Input.GetMouseButtonDown (1)) {
			RaycastHit hit;
			if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 100)) {
				StartMoveTo(hit.point);
			}
		}
		if (Input.GetKeyDown (KeyCode.LeftShift)) {
			StartRunning ();
		}
		if(Input.GetKeyUp(KeyCode.LeftShift)){
			StopRunning();
		}



		CheckTask ();

	}

	public void StartMoveTo(Vector3 dest){
		myTask = Task.Move;
		targetPos = dest;


		agent.destination = targetPos;
		agent.Resume ();
	}



	public void UpdateMove(){
		//reached destination
		if ((transform.position - targetPos).magnitude < 0.5f) {
			myTask = Task.Idle;
			agent.Stop();
		}
	}


	public void CheckTask(){
		switch (myTask) {
		case Task.Move:
			UpdateMove ();
			break;
		case Task.Gather:
			break;
		case Task.Return:
			break;
		case Task.Idle:
		default:
			break;
		}
	}

	public void SwitchTask(Task newTask){
		switch (newTask) {
		case Task.Idle:
			break;
		case Task.Move:
			break;
		case Task.Gather:
			break;
		case Task.Return:
			break;
		case Task.Patrol:
			break;
		case Task.Attack:
			break;
		default:
			break;

		}
		myTask = newTask;
	}





	public void StartRunning(){
		anim.SetBool ("Running", true);
		agent.speed *= runMultiplier;
	}

	public void StopRunning(){
		anim.SetBool ("Running", false);
		agent.speed /= runMultiplier;
	}

	public void Attack(){

	}

	public void TakeDamage(){

	}

	public void Death(){

	}
}
