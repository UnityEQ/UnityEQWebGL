using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EQBrowser;

public class EquipScript : MonoBehaviour {

	public WorldConnect WorldConnection2;
	public string name;
	public int slotId;
	public int iconId;

	public void setupBtn()
	{
		GetComponent<Button>().onClick.AddListener(delegate { EquipClick(slotId); });
	}
	
	public void EquipClick(int slotId2)
	{
		if(name != "" && slotId2 == slotId)
		{
//			WorldConnection2.DoLootItem(slotId);
			WorldConnection2.DoMoveItem(slotId);
			this.gameObject.SetActive(false);
			this.gameObject.GetComponent<RawImage>().texture = null;
			this.gameObject.GetComponent<RawImage>().color = new Color(0f, 0f, 0f, 0f);
			this.name = "";
			this.slotId = 0;
		}
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