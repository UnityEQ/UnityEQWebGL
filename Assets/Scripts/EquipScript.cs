using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EQBrowser;
using System.Linq;


public class EquipScript : MonoBehaviour {

	public WorldConnect WorldConnection2;
	public string name;
	public int slotName;
	public int slotId;
	public int iconId;
	
	[System.Flags]
	public enum SlotMask {
		Charm = 1,
		Head = 4,
		Face = 8,
		Ears = 18,
		Neck = 32,
		Shoulder = 64,
		Arms = 128,
		Back = 256,
		Bracers = 1536,
		Range = 2048,
		Hands = 4096,
		Primary = 8192,
		Secondary = 16384,
		Rings = 98304,
		Chest = 131072,
		Legs = 262144,
		Feet = 524288,
		Waist = 1048576,
		Ammo = 2097152
	}

	public void setupBtn()
	{
		GetComponent<Button>().onClick.AddListener(delegate { EquipClick(int.Parse(this.gameObject.name)); });
	}
	
	public static IEnumerable<string> GetFlagStrings<T>(int input) where T : struct, IConvertible
	{
		if (typeof(T).IsEnum)
		{
			foreach (T value in Enum.GetValues(typeof(T).GetType()))
			if ((input & Convert.ToInt32(value)) > 0)
				yield return value.ToString();
		}
		else
		{
			throw new Exception("T must be an Enum.");
		}
	}
	public static string[] GetFlagStringArray<T>(int input) where T : struct, IConvertible
	{
		if (typeof(T).IsEnum)
		{
			var output = new List<string>();
			foreach (T value in Enum.GetValues(typeof(T)))
			{
				if ((input & Convert.ToInt32(value)) > 0)
				{
					output.Add(value.ToString());
				}
			}
			return output.ToArray();
		}
		else
		{
			throw new Exception("T must be an Enum.");
		}
	}
		
	public void EquipClick(int slotId2)
	{
		//click on an item and move it to cursor
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
		//holding an item, clicking on another item and swapping their slots
		else if(this.name != "" && WorldConnection2.cursorIconId > 0)
		{
			bool slotBool = (24576 & this.slotName) > 0;
			if(slotBool == true){
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
			if(slotBool == false){
				WorldConnection2.ChatText2.text += (Environment.NewLine + "<color=#ff0000ff><b>Item doesn't fit</b></color>");
			}
		}
		//holding an item, moving it to an empty slot
		else if(this.name == "" && WorldConnection2.cursorIconId > 0)
		{
			bool slotBool = (24576 & this.slotName) > 0;
			if(slotBool == true){
				int nameParse = int.Parse(this.gameObject.name);
				WorldConnection2.DoMoveItem(nameParse);
				Texture2D itemIcon = (Texture2D) Resources.Load("Icons/item_" + WorldConnection2.cursorIconId, typeof(Texture2D));
				this.gameObject.GetComponent<EquipScript>().iconId = WorldConnection2.cursorIconId;
				this.gameObject.GetComponent<RawImage>().texture = itemIcon;
				this.gameObject.GetComponent<RawImage>().color = new Color(255f, 255f, 255f, 255f);
				this.name = WorldConnection2.cursorItemName;
				this.slotId = int.Parse(this.gameObject.name);
				this.iconId = WorldConnection2.cursorIconId;
				WorldConnection2.cursorIconId = 0;
				WorldConnection2.cursorItemName = "";
				WorldConnection2.cursorSlotId = 0;
			}
			if(slotBool == false){
				WorldConnection2.ChatText2.text += (Environment.NewLine + "<color=#ff0000ff><b>Item doesn't fit</b></color>");
			}
		}

	}
	// Use this for initialization
	void Start () {
		setupBtn ();
		if(int.Parse(this.gameObject.name) == 1){this.slotName = (int)SlotMask.Ears;}
		if(int.Parse(this.gameObject.name) == 2){this.slotName = (int)SlotMask.Head;}
		if(int.Parse(this.gameObject.name) == 3){this.slotName = (int)SlotMask.Face;}
		if(int.Parse(this.gameObject.name) == 4){this.slotName = (int)SlotMask.Ears;}
		if(int.Parse(this.gameObject.name) == 5){this.slotName = (int)SlotMask.Neck;}
		if(int.Parse(this.gameObject.name) == 6){this.slotName = (int)SlotMask.Shoulder;}
		if(int.Parse(this.gameObject.name) == 7){this.slotName = (int)SlotMask.Arms;}
		if(int.Parse(this.gameObject.name) == 8){this.slotName = (int)SlotMask.Back;}
		if(int.Parse(this.gameObject.name) == 9){this.slotName = (int)SlotMask.Bracers;}
		if(int.Parse(this.gameObject.name) == 10){this.slotName = (int)SlotMask.Bracers;}
		if(int.Parse(this.gameObject.name) == 11){this.slotName = (int)SlotMask.Range;}
		if(int.Parse(this.gameObject.name) == 12){this.slotName = (int)SlotMask.Hands;}
		if(int.Parse(this.gameObject.name) == 13){this.slotName = (int)SlotMask.Primary;}
		if(int.Parse(this.gameObject.name) == 14){this.slotName = (int)SlotMask.Secondary;}
		if(int.Parse(this.gameObject.name) == 15){this.slotName = (int)SlotMask.Rings;}
		if(int.Parse(this.gameObject.name) == 16){this.slotName = (int)SlotMask.Rings;}
		if(int.Parse(this.gameObject.name) == 17){this.slotName = (int)SlotMask.Chest;}
		if(int.Parse(this.gameObject.name) == 18){this.slotName = (int)SlotMask.Legs;}
		if(int.Parse(this.gameObject.name) == 19){this.slotName = (int)SlotMask.Feet;}
		if(int.Parse(this.gameObject.name) == 20){this.slotName = (int)SlotMask.Waist;}
		if(int.Parse(this.gameObject.name) == 21){this.slotName = (int)SlotMask.Ammo;}
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
}