using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EQBrowser;

public class LootScript : MonoBehaviour {

	public WorldConnect WorldConnection2;
	public string name;
	public int slotId;

	public void setupBtn()
	{
		GetComponent<Button>().onClick.AddListener(delegate { LootClick(); });
	}
	
	public void LootClick()
	{
		WorldConnection2.DoLootItem(slotId);
		this.gameObject.SetActive(false);
		this.gameObject.GetComponent<RawImage>().texture = null;
		this.gameObject.GetComponent<RawImage>().color = new Color(0f, 0f, 0f, 0f);
	}
	// Use this for initialization
	void Start () {
		setupBtn ();
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
}