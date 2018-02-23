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

	public int weaponDamage;
	public float weaponRange;
	public float weaponSpeed;
	protected float weaponTimer;

	public Animator animator;

	public bool running;
	[HideInInspector] public float CharacterSpeed;
	public float maxWalkSpeed;
	[HideInInspector] public float maxRunSpeed;
	public float runMultiplier;
	public float acceleration;
	public float rotSpeed;

	// Use this for initialization
	void Start () {

		maxRunSpeed = maxWalkSpeed * runMultiplier;
	}
	
	// Update is called once per frame
	void Update () {
		
	}



}
