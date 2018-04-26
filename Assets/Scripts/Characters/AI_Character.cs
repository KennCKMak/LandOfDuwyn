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
		Wait,
		Gather,
		Return,
		Patrol,
		Chasing
	};

	//Job
	public enum Role{
		Villager,
		Woodcutter,
		Miner,
		Militia,
	};

	State myState;
	public Role myRole;
	public Task myTask;

	Equipment equipment;

	//times we hit something
	[SerializeField]
	int hitCount = 0;
	int maxHitsForResource = 3;

	//maximum timer before hitcount resets
	[SerializeField]
	float hitTimerReset;
	[SerializeField]
	float hitTimer;

	public Resource.ResourceType myItem = Resource.ResourceType.None;

	public GameObject target;
	public float stoppingDist = 0.8f;
	public float actionDistThreshold = 1.5f;

	float elapsedTime = 0.0f;

	bool deathFalling = false;

	// Use this for initialization

	void Awake(){
		InitializeComponents ();
		InitializeVillager (); 


	}
	void Start () {
		GameManager.instance.Add (this);
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
		SetRole (Role.Villager);
		SetTask (Task.Idle);
		SetState (State.Stationary);
		SetMyItem (Resource.ResourceType.None);

		equipment = GetComponent<Equipment> ();


		health = maxHealth;
		hitCount = 0;
		maxRunSpeed = maxWalkSpeed * runMultiplier;
		CharacterSpeed = maxWalkSpeed;

		hitTimerReset = weaponSpeed * 4;
		hitTimer = 0;
	}

	// Update is called once per frame
	void Update () {
		if (myState == State.Dead) {
			if(deathFalling)
				transform.position = transform.position + (Vector3.down * (2.0f * Time.deltaTime));
			return;
		}



		anim.SetBool("Moving", (new Vector2(agent.velocity.x, agent.velocity.z).magnitude > 0.5f));

		if (hitTimer >= 0)
			hitTimer -= Time.deltaTime;
		else
			hitCount = 0;

		if(weaponTimer >= 0)
			weaponTimer -= Time.deltaTime;
		UpdateState ();
		UpdateRole ();

		if (health <= 0)
			SetState (State.Dead);
	}

	public void StartMoveTo(Vector3 dest){
		Vector3 finalDest = dest;
		Vector2 circ = Random.insideUnitCircle * stoppingDist;
		finalDest.x += circ.x;
		finalDest.z += circ.y;

		if (Distance.GetPath (transform.position, finalDest, NavMesh.AllAreas, myPath)) {
			//agent.SetPath (myPath);
			SetState (State.Moving);
			agent.SetDestination (finalDest);
			agent.Resume ();
		} else {
			Debug.Log ("Failed path");
		}
	}

	public void UpdateMove(){
		//reached destination
		//Vector3 dist = transform.position- targetPos;
		//dist.y = 0;
		if (agent.hasPath && agent.remainingDistance < stoppingDist) {
			StopMove ();
		}
	}

	public void StopMove(){
		if(agent.hasPath){
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

			
			Death ();
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
		StopRunning();
		target = null;
		switch (newTask) {
		case Task.Idle:
			//no jobs atm
			break;
		case Task.Wait:
			//this is when gatherer's are waiting around for something to do
			elapsedTime = 99.0f; //high number so they immediately start walking around
			break;
		case Task.Gather:
			break;
		case Task.Return:
			break;

		case Task.Patrol:
			break;
		case Task.Chasing:
			
			StartRunning();
			break;


		default:
			break;

		}
	}

	public void UpdateRole(){
		switch (myRole) {
		case Role.Villager:
			break;


		case Role.Miner:
		case Role.Woodcutter:
			UpdateGatherer ();


			break;
		case Role.Militia:
			UpdateMilitia ();
			break;
		default:
			break;
		}
	}

	public void SetRole(Role newRole){
		if (myRole == newRole)
			return;


		gameObject.name = myRole.ToString ();

		SetMyItem (Resource.ResourceType.None);
		SetState (State.Stationary);


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
			SetTask (Task.Patrol);
			break;


		case Role.Villager:
		default:
			equipment.EquipItem (Equipment.RightHand.None, Equipment.LeftHand.None);
			break;
		}


		target = null;
		myRole = newRole;
		hitCount = 0;

	}

	public void UpdateGatherer(){
		if (myTask == Task.Gather) {
			//not holding something
			//no tree found
			if (!target) {
				if (myRole == Role.Woodcutter) {
					if (FindResource (Resource.ResourceType.Wood)) {
						target = FindResource (Resource.ResourceType.Wood).gameObject;
					} else { 
						//no resources found
						Debug.Log("No lumber found!");
						SetTask(Task.Wait);
						return;
					}
				} else if (myRole == Role.Miner) { 
					if (FindResource (Resource.ResourceType.Stone)) {
						target = FindResource (Resource.ResourceType.Stone).gameObject;
					} else { 
						Debug.Log ("No stone found!");
						SetTask(Task.Wait);
						return;
					}

				}
				if (target)
					StartMoveTo (target.transform.position);
				else
					return;
			} else {
				//moving, forces a move if something bad happens (i..e pushed out)
				if (myState == State.Stationary && Distance.GetHorizontalDistance (this.gameObject, target) > actionDistThreshold)
					StartMoveTo (target.transform.position);

				//reached target
				if (myState == State.Stationary && Distance.GetHorizontalDistance (this.gameObject, target) < actionDistThreshold && hitCount < 3) {
					Gather (target);
				}
				//we got the wood
				if (hitCount >= maxHitsForResource) {

					if(target.GetComponent<Resource>())
						target.GetComponent<Resource> ().DepleteAmount (1);
					
					if (myRole == Role.Woodcutter) {
						SetMyItem (Resource.ResourceType.Wood);
						anim.SetBool ("Wood", true);
					} else {
						SetMyItem (Resource.ResourceType.Stone);
						anim.SetBool ("Bag", true);
					}

					SetTask(Task.Return);
				}
			}
		//have something, bring back to base
		}

		if (myTask == Task.Return) {
			//Find base
			if (!target) {
				target = FindBuildingResourceDrop (Building.Structure.ResourceDrop, myItem).buildingTarget;
				if (!target)
					return;
				StartMoveTo (target.transform.position);
			} else {
				//moving, forces a move if something bad happens (i..e pushed out)
				if (myState == State.Stationary && Distance.GetHorizontalDistance (this.gameObject, target) > stoppingDist)
					StartMoveTo (target.transform.position);

				//reached target
				if (myState == State.Stationary && Distance.GetHorizontalDistance(this.gameObject, target) < stoppingDist)  {

					GameManager.instance.AddResource (myItem, 1);
					SetMyItem(Resource.ResourceType.None);
					hitCount = 0;

					SetTask (Task.Gather);

					anim.SetBool ("Wood", false);
					anim.SetBool ("Bag", false);
				}
			}
		}

		if (myTask == Task.Wait) {
			//idle in the center of the base
			if (!target) {
				if (myRole == Role.Woodcutter) {
					if (FindResource (Resource.ResourceType.Wood)) {
						target = FindResource (Resource.ResourceType.Wood).gameObject;
						if (target)
							SetTask (Task.Gather);
					} else { 
						//no enemies atm, let's wander around...
						if (myState == State.Stationary) {
							elapsedTime += Time.deltaTime;
							if (elapsedTime < 10.0f)
								return;
							Vector2 ranCircle = Random.insideUnitCircle * 20; //* patrol size
							Vector3 randomLocation = new Vector3 (ranCircle.x, 0.1f, ranCircle.y);
							StartMoveTo (randomLocation);
						} else if (myState == State.Moving) {
							elapsedTime = 0.0f;
						}
						return;
					}
				} else { 
					if (FindResource (Resource.ResourceType.Stone)) {
						target = FindResource (Resource.ResourceType.Stone).gameObject;
						if (target)
							SetTask (Task.Gather);
					} else { 
						//no work atm, let's wander around...

						if (myState == State.Stationary) {
							elapsedTime += Time.deltaTime;
							if (elapsedTime < 10.0f)
								return;
							Vector2 ranCircle = Random.insideUnitCircle * 15; //* patrol size
							Vector3 randomLocation = new Vector3 (ranCircle.x, 0.1f, ranCircle.y);
							StartMoveTo (randomLocation);
						} else if (myState == State.Moving) {
							elapsedTime = 0.0f;
						}
						return;
					}

				}
				if (target)
					StartMoveTo (target.transform.position);
				else
					return;
			}
		}
	}

	public void UpdateMilitia(){
		if (myTask == Task.Patrol) {
			if (!target) {
				if (!GetCloseEnemy ()) {
					//no enemies sighted, let's wander around...
					if (myState == State.Stationary) {

						elapsedTime += Time.deltaTime; 
						//waits 1.5f seconds only befor emoving again
						if (elapsedTime < 1.5f)
							return;

						Vector2 ranCircle = Random.insideUnitCircle * 20; //* patrol size
						Vector3 randomLocation = new Vector3 (ranCircle.x, 0.1f, ranCircle.y);
						StartMoveTo (randomLocation);
					} else if (myState == State.Moving) {

						elapsedTime = 0.0f;

					}
				} else {
					SetTask (Task.Chasing);
				}
			}
		} else if (myTask == Task.Chasing) {
			if (target) {
				//enemy has moved

				//out of range
				if (Distance.GetHorizontalDistance (target.transform.position, transform.position) > 10.0f * 1.5f) {
					SetTask (Task.Patrol);
					return;
				}

				//target moved, refind path
				if (myState == State.Moving && Distance.GetHorizontalDistance (target.transform.position, agent.destination) > 3.0f + actionDistThreshold) {
					StartMoveTo (target.transform.position);
				} else  if (myState == State.Stationary && Distance.GetHorizontalDistance (target.transform.position, transform.position) > actionDistThreshold) {
					//Continuously update to chase 
					StartMoveTo (target.transform.position);
				} else if (myState == State.Stationary && Distance.GetHorizontalDistance (target.transform.position, transform.position) < actionDistThreshold) {
					//reached target
					AttackEnemy(target);
				}
			} else {
				//no target/lost target
				if(!GetCloseEnemy()){
					SetTask (Task.Patrol);
				} else {
					StopMove ();
					target = GetCloseEnemy().gameObject;
				}
			}

		}
	}

	public EnemyCharacter GetCloseEnemy(){
		//10.0f = detection range
		return GameManager.instance.FindClosestEnemy(this, 10.0f);
	}

	public Resource FindResource(Resource.ResourceType resourceType){
		return GameManager.instance.FindClosestResource (this, resourceType);
	}

	public Building FindBuilding(Building.Structure buildingType){
		return GameManager.instance.FindBuilding (this, buildingType);
	}

	public Building FindBuildingResourceDrop(Building.Structure buildingType, Resource.ResourceType resourceType){
		return GameManager.instance.FindBuildingResourceDrop (this, buildingType, resourceType);
	}
		

	public void SetMyItem(Resource.ResourceType itemType){
		
		if (myItem == itemType)
			return;
		myItem = itemType;

		switch (itemType) {

		case Resource.ResourceType.Wood:
			anim.SetBool ("Wood", true);
			anim.SetBool("Bag", false);
			break;

		case Resource.ResourceType.Stone:
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

	public void AttackEnemy(GameObject obj){
		if (weaponTimer > 0)
			return;
		transform.LookAt (new Vector3(obj.transform.position.x, transform.position.y, obj.transform.position.z));
		Attack ();
		obj.GetComponent<EnemyCharacter> ().TakeDamage (20);

	}

	public void Attack(){
		
		if (weaponTimer > 0)
			return;

		if (target)
			GetComponent<UnitSFX> ().UpdateHitSFX (target);

		anim.SetTrigger ("Attack");
		weaponTimer = weaponSpeed;
		agent.speed = 0;
		Invoke ("RestoreMovement", 0.833f);
	}



	public void Gather(){
		if (weaponTimer > 0)
			return;

		if (target)
			GetComponent<UnitSFX> ().UpdateHitSFX (target);

		anim.SetTrigger ("Attack");
		weaponTimer = weaponSpeed;
		agent.speed = 0;



		//makes it so itemAmount won't degrade
		hitTimer = hitTimerReset;

		hitCount++;

		Invoke ("RestoreMovement", 0.833f);
	}

	public void Gather(GameObject obj){
		if (weaponTimer > 0)
			return;
		transform.LookAt (new Vector3(obj.transform.position.x, transform.position.y, obj.transform.position.z));
		Gather ();
	}


	public void RestoreMovement(){
		agent.speed = CharacterSpeed;
	}

	public void TakeDamage(int num){
		health -= num;
	}

	public void Death(){


		if(Random.value > 0.5f)
			anim.SetTrigger ("DeathA");
		else 
			anim.SetTrigger ("DeathB");
		GameManager.instance.Remove (this);
		StartCoroutine (DeathSequence());
	}
	IEnumerator DeathSequence(){
		Destroy (gameObject, 4.5f);
		yield return new WaitForSeconds (4.0f);
		deathFalling = true;
	}


}
