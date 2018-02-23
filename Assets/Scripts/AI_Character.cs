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


	[HideInInspector] public NavMeshAgent agent;
	[HideInInspector] public NavMeshObstacle obstacle;
	[HideInInspector] public Rigidbody rb;
	[HideInInspector] public Animator anim;

	NavMeshPath myPath;

	//State = controls movement
	public enum State {
		Stationary,
		Moving,
		Dead
	}

	//What part should I be doing now
	public enum Task {
		Idle,
		Gather,
		Return,
		Patrol,
		Chasing
	};

	//Job
	public enum Role{
		Nothing,
		Woodcutter,
		Miner,
		Militia,
	};

	State myState;
	public Role myRole;
	public Task myTask;

	Equipment equipment;

	//times we hit something
	int hit;

	public Resource.ResourceType myItem = Resource.ResourceType.None;

	public GameObject target;
	public float stoppingDist = 0.8f;
	public float resourceGatheringDistThreshold = 1.5f;




	// Use this for initialization

	void Awake(){
		InitializeComponents ();
		InitializeVillager (); 

	}
	void Start () {
	}

	void InitializeComponents(){
		agent = GetComponent<NavMeshAgent> ();
		anim = GetComponent<Animator> ();
		rb = GetComponent<Rigidbody> ();

		obstacle = GetComponent<NavMeshObstacle> ();

		InitializeAgent ();
	}

	void InitializeAgent(){
		agent.speed = this.maxWalkSpeed;
		agent.acceleration = this.acceleration;
		agent.angularSpeed = this.rotSpeed;
		agent.avoidancePriority = Random.Range (50, 100);
		running = false;
		myPath = new NavMeshPath ();
	}

	void InitializeVillager(){
		SetRole (Role.Nothing);
		SetTask (Task.Idle);
		SetState (State.Stationary);
		myItem = Resource.ResourceType.None;

		equipment = GetComponent<Equipment> ();

		health = maxHealth;
		hit = 0;
		maxRunSpeed = maxWalkSpeed * runMultiplier;
		CharacterSpeed = maxWalkSpeed;
	}

	// Update is called once per frame
	void Update () {
		if (myState == State.Dead)
			return;

		/*if (Input.GetMouseButtonDown (1)) {
			SetRole (Role.Nothing);
			RaycastHit hit;
			if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 100)) {
				StartMoveTo(hit.point);
			}
		}
		if (Input.GetKeyDown (KeyCode.LeftShift))
			StartRunning ();
		if(Input.GetKeyUp(KeyCode.LeftShift))
			StopRunning ();
		if (Input.GetKeyDown (KeyCode.Space)) {
			SetRole (Role.Woodcutter);
		}
		if (Input.GetKeyDown (KeyCode.C)) {
			SetRole (Role.Miner);
		}
		if (Input.GetKeyDown (KeyCode.D)) {
			SetState (State.Dead);//
		}*/

		anim.SetBool("Moving", (new Vector2(agent.velocity.x, agent.velocity.z).magnitude > 0.5f));

		if(weaponTimer >- 0)
			weaponTimer -= Time.deltaTime;
		UpdateState ();
		UpdateRole ();
	}

	public void StartMoveTo(Vector3 dest){
		Vector3 finalDest = dest;
		Vector2 circ = Random.insideUnitCircle * stoppingDist;
		finalDest.x += circ.x;
		finalDest.z += circ.y;

		if (Distance.GetPath (transform.position, finalDest, NavMesh.AllAreas, myPath)){
			//agent.SetPath (myPath);
			SetState (State.Moving);
			agent.SetDestination (finalDest);
			agent.Resume ();
		}
	}

	public void UpdateMove(){
		//reached destination
		//Vector3 dist = transform.position- targetPos;
		//dist.y = 0;
		if (agent.hasPath && agent.remainingDistance < stoppingDist) {
			agent.Stop();
			agent.ResetPath ();

			SetState (State.Stationary);
		}
	}

	public void UpdateState(){
		switch (myState) {

		case State.Moving:
			UpdateMove ();
			break;

		case State.Dead:
		case State.Stationary:
		default:
			break;
		}
	
	}

	public void SetState(State newState){

		myState = newState;
		agent.enabled = false;
		obstacle.enabled = false;
		switch (newState) {
		case State.Stationary:
			rb.constraints = RigidbodyConstraints.FreezeAll;
			obstacle.enabled = true;
			break;
		case State.Moving:
			rb.constraints = RigidbodyConstraints.None;
			agent.enabled = true;
			break;
		case State.Dead:
			rb.constraints = RigidbodyConstraints.FreezeAll;
			anim.SetTrigger ("DeathA");
			break;
		default:
			break;

		}

	}

	public void CheckTask(){
		switch (myTask) {
		case Task.Idle:
			break;
		case Task.Gather:
			
			break;
		case Task.Return:
			break;
		default:
			break;
		}
	}

	public void SetTask(Task newTask){
		if (myTask == newTask)
			return;


		myTask = newTask;
		target = null;
		switch (newTask) {
		case Task.Idle:
			break;
		case Task.Gather:
			break;
		case Task.Return:
			break;



		default:
			break;

		}
	}

	public void UpdateRole(){
		switch (myRole) {
		case Role.Nothing:
			break;
		case Role.Miner:
			UpdateGatherer ();
			break;
		case Role.Woodcutter:
			UpdateGatherer ();
			break;
		case Role.Militia:
			break;
		default:
			break;
		}
	}

	public void SetRole(Role newRole){
		if (myRole == newRole)
			return;


		switch (newRole) {
		case Role.Miner:
			equipment.EquipItem (Equipment.RightHand.Pickaxe, Equipment.LeftHand.None);
			SetTask (Task.Gather);
			break;
		case Role.Woodcutter:
			equipment.EquipItem (Equipment.RightHand.Axe, Equipment.LeftHand.None);
			SetTask (Task.Gather);
			break;
		case Role.Militia:
			equipment.EquipItem (Equipment.RightHand.Battleaxe, Equipment.LeftHand.SmallShield);
			break;


		case Role.Nothing:
		default:
			equipment.EquipItem (Equipment.RightHand.None, Equipment.LeftHand.None);
			break;
		}


		target = null;
		myRole = newRole;
		hit = 0;

	}

	public void UpdateGatherer(){
		if (myTask == Task.Gather) {
			//not holding something
			//no tree found
			if (!target) {
				if(myRole == Role.Woodcutter)
					target = FindResource (Resource.ResourceType.Wood).gameObject;
				else 
					target = FindResource (Resource.ResourceType.Rock).gameObject;

				if (target) 
					StartMoveTo (target.transform.position);
			} else {
				//moving, forces a move if something bad happens (i..e pushed out)
				if (myState == State.Stationary && Distance.GetHorizontalDistance (this.gameObject, target) > resourceGatheringDistThreshold)
					StartMoveTo (target.transform.position);

				//reached target
				if (myState == State.Stationary && Distance.GetHorizontalDistance (this.gameObject, target) < resourceGatheringDistThreshold && hit < 3) {
					Attack (target);
				}
				//we got the wood
				if (hit >= 3) {
					if (myRole == Role.Woodcutter) {
						SetMyItem (Resource.ResourceType.Wood);
						anim.SetBool ("Wood", true);
					} else {
						SetMyItem (Resource.ResourceType.Rock);
						anim.SetBool ("Bag", true);
					}

					SetTask(Task.Return);

					hit = 0;
				}
			}
		//have something, bring back to base
		} else if (myTask == Task.Return) {
			//Find base
			if (!target) {
				target = FindBuilding (Building.Structure.ResourceDrop).gameObject;
				if (!target)
					return;
				StartMoveTo (target.transform.position);
			} else {
				//moving, forces a move if something bad happens (i..e pushed out)
				if (myState == State.Stationary && Distance.GetHorizontalDistance (this.gameObject, target) > stoppingDist)
					StartMoveTo (target.transform.position);

				//reached target
				if (myState == State.Stationary && Distance.GetHorizontalDistance(this.gameObject, target) < stoppingDist)  {
					
					SetMyItem(Resource.ResourceType.None);
					SetTask (Task.Gather);

					anim.SetBool ("Wood", false);
					anim.SetBool ("Bag", false);
				}
			}
		}
	}



	public Resource FindResource(Resource.ResourceType resourceType){
		return GameManager.instance.FindClosestResource (this, resourceType);
	}

	public Building FindBuilding(Building.Structure buildingType){
		return GameManager.instance.FindBuilding (this, buildingType);
	}






	public void SetMyItem(Resource.ResourceType itemType){
		if (myItem == itemType)
			return;
		

		switch (itemType) {

		case Resource.ResourceType.Wood:
			anim.SetBool ("Wood", true);
			anim.SetBool("Bag", false);
			break;

		case Resource.ResourceType.Rock:
		case Resource.ResourceType.Gold:
			anim.SetBool ("Wood", false);
			anim.SetBool ("Bag", true);
			break;

		case Resource.ResourceType.None:
		default:
			anim.SetBool ("Wood", false);
			anim.SetBool("Bag", false);
			break;

		}
	}



	public void StartRunning(){
		anim.SetBool ("Running", true);
		CharacterSpeed = maxRunSpeed;
		agent.speed = CharacterSpeed;
	}

	public void StopRunning(){
		anim.SetBool ("Running", false);
		CharacterSpeed = maxWalkSpeed;
		agent.speed = CharacterSpeed;
	}

	public void Attack(GameObject obj){
		if (weaponTimer > 0)
			return;
		transform.LookAt (new Vector3(obj.transform.position.x, transform.position.y, obj.transform.position.z));
		Attack ();
	}

	public void Attack(){
		
		if (weaponTimer > 0)
			return;
		
		anim.SetTrigger ("Attack");
		weaponTimer = weaponSpeed;
		hit++;
		agent.speed = 0;
		Invoke ("RestoreMovement", 0.833f);
	}

	public void RestoreMovement(){
		agent.speed = CharacterSpeed;
	}

	public void TakeDamage(){

	}

	public void Death(){

	}


}
