using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// The villager class: All roles should inherit from this
/// Contains AI movement
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(NavMeshAgent), typeof(Animator))]

public class EnemyCharacter : Character {


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
		Return,
		Patrol,
		Chasing
	};


	State myState;
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
	float checkNewTargetTimer = 0.0f;
	bool deathFalling = false;

	// Use this for initialization

	void Awake(){
		InitializeComponents ();
		InitializeEnemy (); 


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

	void InitializeEnemy(){
		SetTask (Task.Idle);
		SetState (State.Stationary);


		equipment = GetComponent<Equipment> ();
		equipment.EquipItem (Equipment.RightHand.Battleaxe, Equipment.LeftHand.SmallShield);


		health = maxHealth;
		hitCount = 0;
		maxRunSpeed = maxWalkSpeed * runMultiplier;
		CharacterSpeed = maxWalkSpeed;

		hitTimerReset = weaponSpeed * 4;
		hitTimer = 0;
		SetTask (Task.Patrol);
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

		UpdateEnemy ();

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


	public void UpdateEnemy(){

		//this sections makes it so that it will auto go to whatever
		checkNewTargetTimer += Time.deltaTime;
		if (checkNewTargetTimer > 4.0f) {
			Debug.Log ("getting new target");
			target = GetAlliedTarget ();
			if (target)
				Debug.Log ("Found " + target.name);
			checkNewTargetTimer = 0.0f;
		}

		if (myTask == Task.Patrol) {
			if (!target) {
				if (!GetAlliedTarget ()) {
					//no enemies sighted, let's wander around...
					if (myState == State.Stationary) {

						elapsedTime += Time.deltaTime; 
						//waits 1.5f seconds only befor emoving again
						if (elapsedTime < 1.5f)
							return;

						Vector2 ranCircle = Random.insideUnitCircle * 40; //* patrol size
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
					AttackTarget(target);
				}
			} else {
				//no target/lost target
				if(!GetAlliedTarget()){
					SetTask (Task.Patrol);
				} else {
					StopMove ();
					target = GetAlliedTarget();
				}
			}

		}
	}

	public GameObject GetAlliedTarget(){
		//10.0f = detection range
		return GameManager.instance.GetAlliedTarget(this, 10.0f);
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

	public void AttackTarget(GameObject obj){
		if (weaponTimer > 0)
			return;
		transform.LookAt (new Vector3(obj.transform.position.x, transform.position.y, obj.transform.position.z));
		Attack ();
		if(obj.GetComponent<AI_Character>())
			obj.GetComponent<AI_Character> ().TakeDamage (10);

		if (obj.GetComponent<PlayerCharacter> ())
			return;
			//obj.GetComponent<PlayerCharacter> ().TakeDamage (0);

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

	public void Respawn(){
		elapsedTime = 0;
		Vector2 circle = Random.insideUnitCircle * 22;
		Vector3 randLocation = new Vector3 (circle.x, 0, circle.y);
		transform.position = randLocation;
		health = maxHealth;
	}
}
