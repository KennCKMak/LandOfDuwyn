﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour {

	public enum ResourceType{
		None,
		Wood,
		Rock,
		Gold
	}

	public ResourceType resourceType;
	public int amount;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}