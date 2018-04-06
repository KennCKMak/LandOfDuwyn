using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character {
	float elapsedTime = 0.0f;
	// Use this for initialization

	void Awake(){

	}
	void Start () {

		GameManager.instance.Add (this);
	}
	
	// Update is called once per frame
	void Update () {
		elapsedTime += Time.deltaTime;

		if (elapsedTime > 15.0f)
			Respawn ();
	}


	 public void TakeDamage(int num){
		health -= num;
		elapsedTime = 0;
		if (health < 0)
			Respawn ();

	}
	public void Respawn(){
		elapsedTime = 0;
		Vector2 circle = Random.insideUnitCircle * 22;
		Vector3 randLocation = new Vector3 (circle.x, 0, circle.y);
		transform.position = randLocation;
		health = maxHealth;
	}
}
