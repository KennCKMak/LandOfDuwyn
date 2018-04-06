using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindmillFan : MonoBehaviour {
	public float fanSpeed = 1.0f;
	public GameObject fan;

	// Use this for initialization
	void Start () {
		if (!fan)
			fan = transform.GetChild (0).gameObject;
		
	}
	
	// Update is called once per frame
	void Update () {
		fan.transform.RotateAround (fan.transform.position, Vector3.forward, fanSpeed * Time.deltaTime);
		
	}
}
