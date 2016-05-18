using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ZoneToSouthQeynos : MonoBehaviour {

	void OnTriggerEnter (Collider other) 
	{
		SceneManager.LoadScene("3 South Qeynos");
	}
}
