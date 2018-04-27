using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TreeUsed : MonoBehaviour {
	public float fallForce = 2.0f;
	public GameObject topPoint;
	float topY;

	Vector3 directionFall;
	bool slidingDown;
	float startY;

	bool playedLandingSFX = false;
	// Use this for initialization
	void Start () {
		//apply random force so this thing starts falling
		if (!topPoint)
			topPoint = transform.FindChild ("TopPoint").gameObject;
		topY = topPoint.transform.position.y;


		float x = Random.Range (-1.0f, 1.0f);
		float z = Random.Range (-1.0f, 1.0f);
		directionFall = new Vector3 (x, 0, z);

		PlayTreeSound ("TreeCrack");
		StartCoroutine (DestroyTree ());
		Destroy (gameObject, 14.0f);
	}
	
	// Update is called once per frame
	void Update () {
		if (Mathf.Abs(topPoint.transform.position.y - topY) <= 0.2f)
			Fall ();

		if (!playedLandingSFX && topPoint.transform.position.y - 0.1f <= (0.95f * topY)) {
			playedLandingSFX = true;
			PlayTreeSound ("TreeCrash");
		}

		if (slidingDown) {
			transform.position = transform.position + (Vector3.down * (2.0f * Time.deltaTime));
			if (Mathf.Abs(topPoint.transform.position.y - startY) >= 5.0f)
				slidingDown = false;
		}
	}

	void Fall(){
		//Debug.Log ("falling");
		Vector3 topPosition = topPoint.transform.position;
		//topPosition.y = gameObject.GetComponent<CapsuleCollider> ().height;

		gameObject.GetComponent<Rigidbody> ().AddForceAtPosition (directionFall * fallForce, topPoint.transform.position, ForceMode.Force);
	}


	IEnumerator DestroyTree(){
		yield return new WaitForSeconds (12.0f);
		gameObject.GetComponent<Rigidbody> ().isKinematic = true;
		gameObject.GetComponent<NavMeshObstacle> ().carving = false;

		startY = topPoint.transform.position.y;
		slidingDown = true;
	}

	void PlayTreeSound(string s){
		SFX sfx = AudioManager.instance.RequestSFX (s);
		if (sfx == null) 
			return;
		if (!GetComponent<AudioSource> ())
			gameObject.AddComponent<AudioSource> ();
		AudioSource source = GetComponent<AudioSource> ();
		source.volume = sfx.volume;
		source.pitch = sfx.pitch;
		source.spatialBlend = 1.0f;
		source.PlayOneShot (sfx.clip);
	}

}
