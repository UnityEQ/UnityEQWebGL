using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ZoneToQeynosHills : MonoBehaviour {

	void OnTriggerEnter (Collider other) 
	{
		SceneManager.LoadScene("5 Qeynos Hills");
	}
}
