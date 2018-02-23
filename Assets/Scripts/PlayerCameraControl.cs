using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraControl : MonoBehaviour {
	public Transform target;
	public Vector3 offset;
	//public GameObject player;
	// Use this for initialization
	void Start () {
	//	player = GameObject.FindGameObjectWithTag ("Player");
		if (!target)
			target = GameObject.Find ("CameraTarget").transform;
		offset = target.transform.position - transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (target) {
			transform.position = target.transform.position - offset;

		}
	}
}
