using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

	public GameObject FirstPersonCamera;
	public GameObject TopDownCamera;


	public static CameraManager instance;

	public enum CameraState{
		FirstPerson,
		TopDown
	}




	void Awake(){
		if (instance == null)
			instance = this;
		else
			Destroy (this);
		
		if (!FirstPersonCamera)
			FirstPersonCamera = GameObject.Find ("FirstPersonCamera");
		if (!TopDownCamera)
			TopDownCamera = GameObject.Find ("TopDownCamera");

		GameObject cameraTarget = GameObject.Find ("CameraTarget");
		FirstPersonCamera.GetComponent<PlayerCameraControl>().initialOffset = cameraTarget.transform.position - FirstPersonCamera.transform.position;
		TopDownCamera.GetComponent<PlayerCameraControl>().initialOffset = cameraTarget.transform.position - TopDownCamera.transform.position;

		FirstPersonCamera.transform.parent = null;
		TopDownCamera.transform.parent = null;
	}

	public static CameraState CurrentCameraState;

	public void SetCameraState(int num){
		SetCameraState ((CameraState)num);
	}

	public void SetCameraState(CameraState newState){
		CurrentCameraState = newState;
		UIManager.instance.SwitchActionBar (newState);

		switch (newState) {
		case CameraState.FirstPerson:
			FirstPersonCamera.SetActive (true);
			TopDownCamera.SetActive (false);
			UIManager.instance.ShowJobPanel (false);
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;


			foreach (Building building in GameManager.instance.buildingsList) {
				building.HighlightArea (false);
			}


                break;
		case CameraState.TopDown:
			FirstPersonCamera.SetActive (false);
			TopDownCamera.SetActive (true);
			UIManager.instance.ShowJobPanel (true);
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			Cursor.lockState = CursorLockMode.Confined;


                break;
		default:
			break;


		}
	}

	public void SwitchCameraState(){
		if (CurrentCameraState == CameraState.FirstPerson) 
			SetCameraState (CameraState.TopDown);
		else
			SetCameraState (CameraState.FirstPerson);

	}

}
