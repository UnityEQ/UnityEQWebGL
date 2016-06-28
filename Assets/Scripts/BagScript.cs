using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EQBrowser;

public class BagScript : MonoBehaviour {

	public WorldConnect WorldConnection2;
	public string name;
	public int slotId;
	public int iconId;

	public void setupBtn()
	{
		GetComponent<Button>().onClick.AddListener(delegate { BagClick(int.Parse(this.gameObject.name)); });
		Debug.Log("thisgameobjectname:" + int.Parse(this.gameObject.name));
	}
	
	public void BagClick(int slotId2)
	{
		if(this.name != "" && WorldConnection2.cursorIconId == 0)
		{
			Debug.Log("if1");
			WorldConnection2.DoMoveItem(slotId);
			this.gameObject.GetComponent<RawImage>().texture = null;
			this.gameObject.GetComponent<RawImage>().color = new Color(0f, 0f, 0f, 0f);
			WorldConnection2.cursorIconId = iconId;
			WorldConnection2.cursorItemName = name;
			WorldConnection2.cursorSlotId = int.Parse(this.gameObject.name);
			this.name = "";
			this.slotId = 0;
			this.iconId = 0;
		}
		else if(this.name != "" && WorldConnection2.cursorIconId > 0)
		{
			Debug.Log("if2");
			WorldConnection2.DoMoveItem(slotId);
	
			string tempName = WorldConnection2.cursorItemName;
			int tempslotId = WorldConnection2.cursorSlotId;
			int tempiconId = WorldConnection2.cursorIconId;
			
			Texture2D itemIcon = (Texture2D) Resources.Load("Icons/item_" + tempiconId, typeof(Texture2D));
			this.gameObject.GetComponent<RawImage>().texture = itemIcon;
			this.gameObject.GetComponent<RawImage>().color = new Color(255f, 255f, 255f, 255f);

			
			WorldConnection2.cursorIconId = this.iconId;
			WorldConnection2.cursorItemName = this.name;
			WorldConnection2.cursorSlotId = int.Parse(this.gameObject.name);
			
			this.name = tempName;
			this.slotId = int.Parse(this.gameObject.name);
			this.iconId = tempiconId;
			
		}
		else if(this.name == "" && WorldConnection2.cursorIconId > 0)
		{
			Debug.Log("if3");
			int nameParse = int.Parse(this.gameObject.name);
			WorldConnection2.DoMoveItem(nameParse);
			Texture2D itemIcon = (Texture2D) Resources.Load("Icons/item_" + WorldConnection2.cursorIconId, typeof(Texture2D));
			this.gameObject.GetComponent<BagScript>().iconId = WorldConnection2.cursorIconId;
			this.gameObject.GetComponent<RawImage>().texture = itemIcon;
			this.gameObject.GetComponent<RawImage>().color = new Color(255f, 255f, 255f, 255f);
			this.name = WorldConnection2.cursorItemName;
			this.slotId = int.Parse(this.gameObject.name);
			this.iconId = WorldConnection2.cursorIconId;
			WorldConnection2.cursorIconId = 0;
			WorldConnection2.cursorItemName = "";
			WorldConnection2.cursorSlotId = 0;
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