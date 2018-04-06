using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour {

	/// <summary>
	/// This is where our guy should go to 
	/// I.e. where should we drop it off, relative to this building...
	/// At the door? At a certain prop?
	/// 
	/// </summary>
	public GameObject buildingTarget;


	public float resourceActivationRange;
	public GameObject highlightRange;

	public List<Collider> colliders = new List<Collider>();



	public enum Structure {
		Base,
		House,
		ResourceDrop
	}

	public Structure structure;

	void Awake(){
		foreach (Collider c in GetComponentsInChildren<Collider>()) {
			colliders.Add (c);
		}
	}

	// Use this for initialization
	void Start () {
		GameManager.instance.Add (this);
		if (resourceActivationRange == 0 && structure == Structure.ResourceDrop)
			resourceActivationRange = GameManager.defaultResourceActivationRange;

		if (!buildingTarget)
			buildingTarget = this.gameObject;

		if (structure == Structure.ResourceDrop) {
			//if this is a resource drop, then we are going to activate the reosurces around us
			ActivateResourcesAroundDrop();

		}
	}
	
	// Update is called once per frame
	void Update () {

	}


	void ActivateResourcesAroundDrop(){
		for (int i = 0; i < GameManager.instance.treeList.Count; i++) {
			if (Distance.GetHorizontalDistance (GameManager.instance.treeList [i].gameObject, this.gameObject) <= resourceActivationRange) {
				GameManager.instance.treeList [i].Activated = true;
				GameManager.instance.treeList [i].AddActivatingStructure (this);
			}
		}

		for (int i = 0; i < GameManager.instance.rockList.Count; i++) {
			if (Distance.GetHorizontalDistance (GameManager.instance.rockList [i].gameObject, this.gameObject) <= resourceActivationRange) {
				GameManager.instance.rockList [i].Activated = true;
				GameManager.instance.rockList [i].AddActivatingStructure (this);
			}
		}
	}

	void OnMouseOver(){
		if (CameraManager.CurrentCameraState != CameraManager.CameraState.TopDown)
			return;
		HighlightArea (true);
	}
	void OnMouseExit(){
		if (CameraManager.CurrentCameraState != CameraManager.CameraState.TopDown)
			return;
		HighlightArea (false);

	}

	public void HighlightArea(bool b){
		if (structure != Structure.ResourceDrop)
			return;

		if (!highlightRange)
			highlightRange = transform.FindChild ("HighlightRange").gameObject;
		highlightRange.transform.localScale = new Vector3 (resourceActivationRange, highlightRange.transform.localScale.y, resourceActivationRange);
		highlightRange.SetActive (b);
	}

	public void SetColliders(bool b){
		foreach (Collider c in colliders) {
			c.enabled = b;
		}
	}
}
