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
	public int iconId;
	public bool lootedMe = false;

	public void setupBtn()
	{
		GetComponent<Button>().onClick.AddListener(delegate { LootClick(slotId); });
	}
	
	public void LootClick(int slotId2)
	{
		if(name != "" && slotId2 == slotId)
		{
			WorldConnection2.DoLootItem(slotId);
			lootedMe = true;
			
			WorldConnection2.cursorIconId = iconId;
			WorldConnection2.cursorItemName = name;
			WorldConnection2.cursorSlotId = int.Parse(this.gameObject.name);
			
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