using UnityEngine;
using System.Collections;

public class SafeSpotScript : MonoBehaviour {
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(this.gameObject.transform.position.y < -100)
		transform.position = new Vector3(-113,4.1f,354);
	}
}
