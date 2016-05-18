using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ZoneToNorthQeynos : MonoBehaviour {

	void OnTriggerEnter (Collider other) 
	{
		SceneManager.LoadScene("2 North Qeynos");
	}
}
