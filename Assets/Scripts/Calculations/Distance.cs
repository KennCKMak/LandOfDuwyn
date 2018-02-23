using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Distance {

	public static float GetHorizontalDistance(GameObject obj2, GameObject obj1){
		return GetHorizontalDistance (obj2.transform, obj1.transform);
	}

	public static float GetHorizontalDistance(Transform obj2, Transform obj1){
		return GetHorizontalDistance (obj2.position, obj1.position);
	}

	public static float GetHorizontalDistance(Vector3 obj2, Vector3 obj1){
		return GetHorizontalVector (obj2, obj1).magnitude;
	}

	public static Vector3 GetHorizontalVector(Vector3 obj2, Vector3 obj1){
		Vector3 vect = obj2 - obj1;
		vect.y = 0;
		return vect;

	}


	public static bool GetPath(Vector3 fromPos, Vector3 toPos, int passableMask, NavMeshPath path){
		path.ClearCorners ();
		if (NavMesh.CalculatePath (fromPos, toPos, passableMask, path) == false)
			return false;
		return true;
	}

	public static float GetPathLength(NavMeshPath path){
		float length = 0.0f;

		if ((path.status != NavMeshPathStatus.PathInvalid) && (path.corners.Length > 1)) {
			for (int i = 1; i < path.corners.Length; ++i) {
				length += Vector3.Distance (path.corners [i - 1], path.corners [i]);
			}
		}
		return length;
	}


}
