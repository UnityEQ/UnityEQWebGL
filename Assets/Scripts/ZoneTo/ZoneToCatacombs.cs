using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ZoneToCatacombs : MonoBehaviour {

	void OnTriggerEnter (Collider other) 
	{
		SceneManager.LoadScene("4 Qeynos Catacombs");
	}
}
