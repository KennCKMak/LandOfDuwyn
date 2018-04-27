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
	public GameObject dropZone;

	bool Initialized = false;

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
		Initialize ();
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void Initialize(){
		if (Initialized)
			return;
		GameManager.instance.Add (this);
		if (structure == Structure.House)
			GameManager.instance.UpdateMaxVillagers ();
		if (!buildingTarget)
			buildingTarget = this.gameObject;

		if (structure == Structure.ResourceDrop) {
			//if this is a resource drop, then we are going to activate the reosurces around us
			ActivateResourcesAroundDrop();

		}
		Initialized = true;
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
			for (int i = 0; i < GameManager.instance.stoneList.Count; i++) {
				if(!(GameManager.instance.stoneList [i]))
					continue;
				if (Distance.GetHorizontalDistance (GameManager.instance.stoneList [i].gameObject, this.gameObject) <= resourceActivationRange) {
					GameManager.instance.stoneList [i].Activated = true;
					GameManager.instance.stoneList [i].AddActivatingStructure (this);
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

	public void ShowDropZone(bool b){
		if (structure != Structure.ResourceDrop)
			return;
		if (!dropZone) 
			dropZone = transform.FindChild ("DropZoneIndicator").gameObject;
		
		dropZone.SetActive (b);
	}
}
