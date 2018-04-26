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


	[HideInInspector]
	public List<Collider> colliders = new List<Collider>();


	public enum Structure {
		Base,
		House,
		ResourceDrop
	}
	public Structure structure;

	public enum ResourceDropType{
		None,
		Wood,
		Stone,
		All
	}
	public ResourceDropType dropType = ResourceDropType.None;


	[Header("Range of Activation")]
	public float resourceActivationRange;

	[Range(3, 256)]
	public int numSegments = 126;
	public GameObject LineRendererPrefab;
	LineRenderer lineRenderer;
	public GameObject highlightRange;

	void Awake(){
		foreach (Collider c in GetComponentsInChildren<Collider>()) {
			colliders.Add (c);
		}
		if (resourceActivationRange == 0 && structure == Structure.ResourceDrop)
			resourceActivationRange = GameManager.defaultResourceActivationRange;
		PrepareCircle ();
	}

	// Use this for initialization
	void Start () {
		GameManager.instance.Add (this);

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


	public void ActivateResourcesAroundDrop(){
		if (dropType == ResourceDropType.All || dropType == ResourceDropType.Wood) {
			for (int i = 0; i < GameManager.instance.treeList.Count; i++) {
				if(!(GameManager.instance.treeList [i]))
					continue;
				if (Distance.GetHorizontalDistance (GameManager.instance.treeList [i].gameObject, this.gameObject) <= resourceActivationRange) {
					//Debug.Log (gameObject.name + " activated " + GameManager.instance.treeList [i].gameObject.name + "w/ range " + Distance.GetHorizontalDistance (GameManager.instance.treeList [i].gameObject, this.gameObject));
					GameManager.instance.treeList [i].Activated = true;
					GameManager.instance.treeList [i].AddActivatingStructure (this);
				}
			}
		}

		if (dropType == ResourceDropType.All || dropType == ResourceDropType.Stone) {
			for (int i = 0; i < GameManager.instance.rockList.Count; i++) {
				if(!(GameManager.instance.rockList [i]))
					continue;
				if (Distance.GetHorizontalDistance (GameManager.instance.rockList [i].gameObject, this.gameObject) <= resourceActivationRange) {
					GameManager.instance.rockList [i].Activated = true;
					GameManager.instance.rockList [i].AddActivatingStructure (this);
				}
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
		highlightRange.transform.localScale = new Vector3 (resourceActivationRange*2, highlightRange.transform.localScale.y, resourceActivationRange*2);
		highlightRange.SetActive (b);
		ShowCircle (b);
	}

	public void SetColliders(bool b){
		foreach (Collider c in colliders) {
			c.enabled = b;
		}
	}

	public void PrepareCircle(){
		if (resourceActivationRange == 0)
			return;
		if (!lineRenderer) {
			GameObject newLineRenderer = Instantiate (LineRendererPrefab, transform) as GameObject;
			newLineRenderer.transform.localPosition = Vector3.zero;
			lineRenderer = newLineRenderer.GetComponent<LineRenderer> ();
		}
		lineRenderer.numPositions = numSegments + 1;
		lineRenderer.useWorldSpace = false;

		float deltaTheta = (float)(2.0f * Mathf.PI) / numSegments;
		float theta = 0.0f;

		for (int i = 0; i < numSegments + 1; i++) {
			float x = resourceActivationRange * Mathf.Cos (theta);
			float z = resourceActivationRange * Mathf.Sin (theta);
			Vector3 pos = new Vector3 (x, 0.05f, z);
			lineRenderer.SetPosition (i, pos);
			theta += deltaTheta;
		}
		ShowCircle (false);
	}

	public void ShowCircle(bool b){
		if (!lineRenderer) {
			return;
		}
		lineRenderer.enabled = b;
	}
}
