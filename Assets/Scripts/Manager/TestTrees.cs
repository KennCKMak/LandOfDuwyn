using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTrees : MonoBehaviour {
	public bool active;
	public Terrain terrain; // add current terrain
	public GameObject objectToPlace; // this object will be placed on terrain
	public int numberOfObjects; // number of how many objects will be created
	public int posYMin; // minimum y position
	public int posYMax; // maximum x position
	public float xDistance;
	public float zDistance;
	public bool posMaxIsTerrainHeight; // the maximum height is the terrain height

	public Vector3 centralPoint = Vector3.zero;

	private int numberOfPlacedObjects; // number of the plaed objects

	// Use this for initialization
	void Start ()
	{
		if(posMaxIsTerrainHeight == true)
		{
			posYMax = (int)terrain.terrainData.size.y;
		}
	}
	// Update is called once per frame
	void Update ()
	{
		
		if(numberOfPlacedObjects < numberOfObjects)
		{
			PlaceObject(); // call function placeObject
		}
	}
		// Create objects on the terrain with random positions
	void PlaceObject()
	{
		if (!active)
			return;
		float posx = Random.Range(centralPoint.x - xDistance, centralPoint.x + xDistance); // generate random x position
		float posz = Random.Range(centralPoint.z - zDistance, centralPoint.z + zDistance); // generate random z position
		float posy = Terrain.activeTerrain.SampleHeight(new Vector3(posx, 0, posz)); // get the terrain height at the random position

		if(posy < posYMax && posy > posYMin)
		{
			Vector3 position = new Vector3 (posx - xDistance / 2, posy - 0.4211683f, posz + zDistance / 2);
			GameObject newObject = Instantiate(objectToPlace, position, Quaternion.identity) as GameObject; // create object
			newObject.transform.parent = transform.GetChild(0).transform;
			newObject.transform.name = objectToPlace.transform.name;
			numberOfPlacedObjects++;
		}
		else
		{
			PlaceObject();
		}
	}

}
