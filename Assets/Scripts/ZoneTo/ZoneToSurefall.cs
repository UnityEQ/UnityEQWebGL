using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ZoneToSurefall : MonoBehaviour {

	void OnTriggerEnter (Collider other) 
	{
		SceneManager.LoadScene("6 Surefall Glade");
	}
}
