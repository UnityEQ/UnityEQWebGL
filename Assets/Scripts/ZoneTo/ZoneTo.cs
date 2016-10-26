using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using EQBrowser;

public class ZoneTo : MonoBehaviour
{
	
	public WorldConnect WorldConnection;
	void OnTriggerEnter (Collider other) 
	{
		WorldConnection.DoZoneChange(WorldConnection.ourPlayerName, 4, 0);
	}
}

