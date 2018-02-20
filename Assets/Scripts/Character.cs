using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// Character - All individuals in this game should inherit this
/// Contains stats, Get/Set functions, etc.
/// </summary>
public class Character : MonoBehaviour {

	public int health;
	public int maxHealth;

	public int weaponRange;
	public int weaponDamage;

	public Animator animator;

	public bool running;
	public float maxSpeed;
	public float runMultiplier;
	public float acceleration;
	public float rotSpeed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


}
