using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraControl : MonoBehaviour {
	public Transform target;
	public Vector3 initialOffset;
	public Vector3 additionalOffset;
	public float scrollPower;
	public float cameraSmoothness;
	public enum CameraType {
		FPS,
		TopDown
	};

	public CameraType cameraType;

	//public GameObject player;
	// Use this for initialization
	void Awake(){
		
	}


	void Start () {
	//	player = GameObject.FindGameObjectWithTag ("Player");
		if (!target)
			target = GameObject.Find ("CameraTarget").transform;
		additionalOffset = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
		if (target) {
			if(cameraType == CameraType.TopDown)
				transform.position = Vector3.Lerp(transform.position, target.transform.position - initialOffset - additionalOffset, cameraSmoothness * Time.deltaTime);
			else 
				transform.position = target.transform.position - initialOffset - additionalOffset;
		}


		//zoom in, zoom out
		float mouseWheel = Input.GetAxis ("Mouse ScrollWheel");
		if (cameraType == CameraType.TopDown) {
			additionalOffset.y = Mathf.Clamp (additionalOffset.y += scrollPower * mouseWheel, -50.0f, 10.0f);
		}

		if (cameraType == CameraType.TopDown) {
			DrawLineTD ();
		}
	}

	void OnEnable(){
		transform.position = target.transform.position - initialOffset - additionalOffset;
	}

	void OnDisable(){

		if (cameraType == CameraType.TopDown) {
			for (int i = 0; i < GameManager.instance.treeList.Count; i++) {

				GameManager.instance.treeList [i].seenByCamera = false;
			}


			for (int i = 0; i < GameManager.instance.stoneList.Count; i++) {

				GameManager.instance.stoneList [i].seenByCamera = false;
			}
		}
	}

	public void DrawLineTD(){
		for (int i = 0; i < GameManager.instance.treeList.Count; i++) {
			if (!GameManager.instance.treeList [i])
				continue;
			Vector3 viewPointPort = GetComponent<Camera>().WorldToViewportPoint (GameManager.instance.treeList [i].transform.position);
			if (viewPointPort.x >= 0 && viewPointPort.x <= 1 && viewPointPort.y >= 0 && viewPointPort.y <= 1) { 
				GameManager.instance.treeList [i].seenByCamera = true;
			} else {
				GameManager.instance.treeList [i].seenByCamera = false;
			}
		}


		for (int i = 0; i < GameManager.instance.stoneList.Count; i++) {
			if (!GameManager.instance.stoneList [i])
				continue;
			Vector3 viewPointPort = GetComponent<Camera>().WorldToViewportPoint (GameManager.instance.stoneList [i].transform.position);
			if (viewPointPort.x >= 0 && viewPointPort.x <= 1 && viewPointPort.y >= 0 && viewPointPort.y <= 1) { 
				GameManager.instance.stoneList [i].seenByCamera = true;
			} else {
				GameManager.instance.stoneList [i].seenByCamera = false;
			}
		}
	}
}
