using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;
using System.Linq;
using UnityEngine.UI;
using System.Text.RegularExpressions;


namespace EQBrowser
{
	public partial class WorldConnect : MonoBehaviour
	{
		public void DoEmuKeepAlive()
		{
			byte[] KeepAlive = null;
			GenerateAndSendWorldPacket (0, 550 /* OP_EmuKeepAlive */, 2, curInstanceId, KeepAlive);
		}
		public void DoEmuRequestClose()
		{
			byte[] EmuRequestClose = null;
			GenerateAndSendWorldPacket (0, 551 /* OP_EmuRequestClose */, 2, curInstanceId, EmuRequestClose);
		}
		
		public void DoMoveItem(int slotId)
		{
			byte[] MoveItemRequest = new byte[12];
			Int32 position = 0;
			
			if(cursorIconId == 0)
			{
				WriteInt32(slotId, ref MoveItemRequest, ref position);//from_slot
				WriteInt32(30, ref MoveItemRequest, ref position);//to_slot
				Debug.Log("MoveItem: " + slotId + "to: 30");
			}
			if(cursorIconId > 0)
			{
				WriteInt32(30, ref MoveItemRequest, ref position);//from_slot
				WriteInt32(slotId, ref MoveItemRequest, ref position);//to_slot
				Debug.Log("MoveItem: 30" + "to: " + slotId);
			}
			WriteInt32(0, ref MoveItemRequest, ref position);//number_in_stack
			GenerateAndSendWorldPacket (MoveItemRequest.Length, 332 /* OP_Moveitem */, 2, curInstanceId, MoveItemRequest);
		}
	
		public void DoLootItem(int slotId)
		{
			DoClientUpdate();
			Debug.Log("LOOTINGITEM");
			cursorSlotId = 30;
			isLooting = true;
			byte[] LootRequest = new byte[16];
			Int32 position = 0;

			WriteInt32(OurTargetLootID, ref LootRequest, ref position);
			WriteInt32(OurEntityID, ref LootRequest, ref position);
			WriteInt16((short)slotId, ref LootRequest, ref position);
			WriteInt8(121, ref LootRequest, ref position);
			WriteInt32(0, ref LootRequest, ref position); //autoloot
			
			GenerateAndSendWorldPacket (LootRequest.Length, 299 /* OP_LootItem */, curZoneId, curInstanceId, LootRequest);
		
		}
		
		public void DoChannelMessage(string name, int channel, string message)
		{
			
		
		int messageLength = message.Length;
		int RequestSize = 140 + messageLength;
		byte[] ChannelMessageRequest = new byte[RequestSize];
		Int32 position = 0;
		
		WriteFixedLengthString(OurTargetID.ToString(), ref ChannelMessageRequest, ref position, 64);
		WriteFixedLengthString(name, ref ChannelMessageRequest, ref position, 64);
		WriteInt32(0, ref ChannelMessageRequest, ref position);
		WriteInt32(channel, ref ChannelMessageRequest, ref position);
		WriteInt32(0, ref ChannelMessageRequest, ref position);
		WriteFixedLengthString(message, ref ChannelMessageRequest, ref position, messageLength);
		GenerateAndSendWorldPacket (ChannelMessageRequest.Length, 69 /* OP_ChannelMessage */, curZoneId, curInstanceId, ChannelMessageRequest);				
		}
		
	IEnumerator Wait(float duration)
    {
        //This is a coroutine
         yield return new WaitForSeconds(duration);   //Wait
    }

//work in progress	
		public void DoZoneChange(string name)
		{

			byte[] ZoneChangeRequest = new byte[88];
			int position = 0;
			
			WriteFixedLengthString(name, ref ZoneChangeRequest, ref position,  64); //charname
			WriteInt16(2, ref ZoneChangeRequest, ref position);//zoneID
//			WriteInt16(0, ref ZoneChangeRequest, ref position);//instanceId
			WriteInt32(0, ref ZoneChangeRequest, ref position); //y
			WriteInt32(0, ref ZoneChangeRequest, ref position); //x
			WriteInt32(0, ref ZoneChangeRequest, ref position); //z
			WriteInt32(0, ref ZoneChangeRequest, ref position); //zone reason
			WriteInt32(1, ref ZoneChangeRequest, ref position); //success
			GenerateAndSendWorldPacket (ZoneChangeRequest.Length, 539 /* OP_ZoneChange */, curZoneId, curInstanceId, ZoneChangeRequest);
		}
		
		public void DoTarget(string targetID)
		{
			Debug.Log("targetID: " + targetID);
			int targetInt = int.Parse(targetID);
			OurTargetID = targetInt;
				
//			GameObject temp = ObjectPool.instance.spawnlist.Where(obj => obj.name == targetID).SingleOrDefault();
			GameObject temp = ObjectPool.instance.spawnlist.FirstOrDefault(obj => obj.name == targetID); 
			if(temp != null)
			{
				string targetName = temp.GetComponent<NPCController>().name;
				int curhp = temp.GetComponent<NPCController>().curHp;
				int maxhp = temp.GetComponent<NPCController>().maxHp;
				
				temp.GetComponent<NPCController>().isTarget = true;
				
				int isDead = temp.GetComponent<NPCController>().isDead;
	
				string targetClean = Regex.Replace(targetName, "[0-9]", "");
				string targetName2 = Regex.Replace(targetClean, "[_]", " ");
				string targetName3 = Regex.Replace(targetName2, "[\0]", "");
				UIScript.TargetBox.SetActive(true);
				UIScript.TargetName.text = targetName3;
	
				if(isDead == 0)
				{
					float hpPercent = ((float)curhp/(float)maxhp)*100;
					UIScript.TargetHP.sizeDelta = new Vector2( (int)hpPercent, 10);
					UIScript.TargetHPText.text = ((int)hpPercent + "%");
				}
				else
				{
				}
			}
			else
			{
				UIScript.TargetBox.SetActive(false);
			}
			byte[] TargetMouseRequest = new byte[4];
			Int32 position = 0;
			WriteInt32 (targetInt, ref TargetMouseRequest, ref position);
			GenerateAndSendWorldPacket (TargetMouseRequest.Length, 478 /* OP_TargetMouse */, 2, curInstanceId, TargetMouseRequest);


		}

//work in progress		
	
		public void DoLoot(string targetID)
		{
			OurTargetLootID = int.Parse(targetID);
			byte[] DoLootRequest = new byte[4];
			Int32 position = 0;
			WriteInt32(OurTargetLootID, ref DoLootRequest, ref position);
			GenerateAndSendWorldPacket (DoLootRequest.Length, 300 /* OP_DeleteSpawn */, 2, curInstanceId, DoLootRequest);
			Debug.Log("Looting: " + OurTargetLootID);
			UIScript.InventoryClick("clack");
		}
		
		public void DoEndLoot()
		{
			byte[] DoEndLootRequest = new byte[4];
			Int32 position = 0;
			WriteInt32(OurTargetLootID, ref DoEndLootRequest, ref position);
			GenerateAndSendWorldPacket (DoEndLootRequest.Length, 148 /* OP_DeleteSpawn */, 2, curInstanceId, DoEndLootRequest);
			
			Debug.Log("EndLoot: " + OurTargetLootID);
			OurTargetLootID = 0;
			UIScript.InventoryClick("clack");
			
//			Texture2D itemIcon = (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/Resources/Icons/InventoryEmpty.png", typeof(Texture2D));
			foreach (GameObject lootItem in UIScript.slotList)
			{
//				GameObject temp = UIScript.slotList.Where(obj => obj.name == lootItem.name).SingleOrDefault();
				GameObject temp = ObjectPool.instance.spawnlist.FirstOrDefault(obj => obj.name == lootItem.name); 
				if(temp != null)
				{
					temp.SetActive(false);
					temp.GetComponent<RawImage>().texture = null;
					temp.GetComponent<RawImage>().color = new Color(0f, 0f, 0f, 0f);
				}
			}
		}
		
		public void DoLootComplete()
		{
			byte[] DoLootCompleteRequest = new byte[1];
			Int32 position = 0;
			WriteInt8(0, ref DoLootCompleteRequest, ref position);
			GenerateAndSendWorldPacket (DoLootCompleteRequest.Length, 298 /* OP_DeleteSpawn */, 2, curInstanceId, DoLootCompleteRequest);
			Debug.Log("LootComplete");

		}
		
		public void DoAttack(byte toggle)
		{
			switch(toggle)
			{
				case 0:
					DoClientUpdate();
					isAttacking = 0;
					ChatText2.text += (Environment.NewLine + "Auto attack is off");
					break;
				case 1:
					DoClientUpdate();
					isAttacking = 1;
					ChatText2.text += (Environment.NewLine + "Auto attack is on");
					break;
				default:
					break;

			}
			byte[] DeleteSpawnRequest = new byte[4];
			int position = 0;
			WriteInt8(toggle, ref DeleteSpawnRequest, ref position); 
			GenerateAndSendWorldPacket (DeleteSpawnRequest.Length, 43 /* OP_DeleteSpawn */, 2, curInstanceId, DeleteSpawnRequest);
			DoAttack2(toggle);
		}
		public void DoAttack2(byte toggle)
		{
			byte[] DeleteSpawnRequest = new byte[4];
			int position = 0;
			WriteInt8(toggle, ref DeleteSpawnRequest, ref position); 
			GenerateAndSendWorldPacket (DeleteSpawnRequest.Length, 44 /* OP_DeleteSpawn */, 2, curInstanceId, DeleteSpawnRequest);
		}
		
		public void DoDeleteSpawn(int spawnid)
		{

			byte[] DeleteSpawnRequest = new byte[88];
			int position = 0;
			
			WriteInt32(120, ref DeleteSpawnRequest, ref position);
			GenerateAndSendWorldPacket (DeleteSpawnRequest.Length, 116 /* OP_DeleteSpawn */, 2, curInstanceId, DeleteSpawnRequest);
			
		}
		
		public void DoChatDisplay(string ChannelSender, string ChannelMessage)
		{
			Int32 position = 0;
			ChatText2 = ChatText.GetComponent<Text>();

			string ChatText3 = (ChannelMessage);
			ChatText3 = ChatText3.Substring(0, ChatText3.Length - 1);

			char[] emptySpace = {'\0'};
			string trimmedName = ChannelSender.TrimEnd(emptySpace);
			ChatText2.text += (Environment.NewLine + trimmedName + " says in ooc, '" + ChatText3 + "'");
		}
		
		public void DoClientUpdate()
		{
			GameObject us = EqemuConnectObject;
			float x = -us.transform.position.x;
			float y = us.transform.position.z;
			float z = us.transform.position.y;

			float h = Mathf.Lerp(255,0,us.transform.eulerAngles.y/360f);
			var controller = us.GetComponent<CharacterController>(); 
			//float x = 234;
			//float y = 11;
			//float z = 2;
			//float h = 122;
//			Debug.Log("CUpdate: " + x + "," + y + "," + z);

			byte[] PositionUpdateRequest = new byte[38];
			Int32 position = 0;
				
			WriteInt16((short)OurEntityID, ref PositionUpdateRequest, ref position);
			WriteInt32(BitConverter.ToInt32(BitConverter.GetBytes(x), 0), ref PositionUpdateRequest, ref position);
			WriteInt32(BitConverter.ToInt32(BitConverter.GetBytes(y), 0), ref PositionUpdateRequest, ref position);
			WriteInt32(BitConverter.ToInt32(BitConverter.GetBytes(z), 0), ref PositionUpdateRequest, ref position);
			WriteInt32(BitConverter.ToInt32(BitConverter.GetBytes(-controller.velocity.x), 0), ref PositionUpdateRequest, ref position);
			WriteInt32(BitConverter.ToInt32(BitConverter.GetBytes(controller.velocity.z), 0), ref PositionUpdateRequest, ref position);
			WriteInt32(BitConverter.ToInt32(BitConverter.GetBytes(controller.velocity.y), 0), ref PositionUpdateRequest, ref position);
			WriteInt32(0, ref PositionUpdateRequest, ref position);
			WriteInt32(BitConverter.ToInt32(BitConverter.GetBytes(controller.velocity.magnitude), 0), ref PositionUpdateRequest, ref position);
			WriteInt32(BitConverter.ToInt32(BitConverter.GetBytes(h), 0), ref PositionUpdateRequest, ref position);
			if(playerLock == false)
			{
				GenerateAndSendWorldPacket (PositionUpdateRequest.Length, 87 /* OP_ClientUpdate */, 2, curInstanceId, PositionUpdateRequest);
			}
		}
		
		/* UI Events Below */
		public void DoEnterWorld(string name)
		{
			ourPlayerName = name;
			if (CSel == null)
				return;

			AttemptingZoneConnect = true;
			byte[] EnterWorldRequest = new byte[72];
			int position = 0;

			WriteFixedLengthString(name, ref EnterWorldRequest, ref position,  64); //charname
			WriteInt32(0, ref EnterWorldRequest, ref position); // enter tutorial
			WriteInt32(0, ref EnterWorldRequest, ref position); // return home
			GenerateAndSendWorldPacket (EnterWorldRequest.Length, 151 /* OP_EnterWorld */, -1, -1, EnterWorldRequest);				
		}

		public void DoDeleteChar(string name)
		{
			if (CSel == null)
				return;

			AttemptingZoneConnect = false;
			byte[] DeleteCharRequest = new byte[72];
			int position = 0;

			WriteFixedLengthString(name, ref DeleteCharRequest, ref position,  64); //charname
			GenerateAndSendWorldPacket (DeleteCharRequest.Length, 112 /* OP_DeleteCharacter */, -1, -1, DeleteCharRequest);				
		}
		
		public void DoLogOut()
		{
			byte[] LogOutRequest = new byte[2];
			int position = 0;
			WriteInt8(0, ref LogOutRequest, ref position);
			GenerateAndSendWorldPacket (LogOutRequest.Length, 295 /* OP_LogOut */, curZoneId, -1, LogOutRequest);				
		}

		public void DoNameApproval()
		{

			if (CSel == null)
				return;

			if (CSel.CreationName.text == "") {
				CSel.CreationStatus.text = "Please enter a character name.";
				return;
			}
			string TidyName = CSel.TidyCase(CSel.CreationName.text);
			byte[] NameApprovalRequest = new byte[72];
			int position = 0;
			WriteFixedLengthString(TidyName, ref NameApprovalRequest, ref position,  64);
			WriteInt32(CSel._RaceSelection, ref NameApprovalRequest, ref position);
			WriteInt32(CSel._ClassSelection, ref NameApprovalRequest, ref position);
			GenerateAndSendWorldPacket (NameApprovalRequest.Length, 36 /* OP_ApproveName */, -1, -1, NameApprovalRequest);	


		}
		
		public void DoWorldComplete()
		{

			AttemptingZoneConnect = false;
			byte[] WorldCompleteRequest = new byte[2];
			int position = 0;

			WriteInt8(0, ref WorldCompleteRequest, ref position); 
			GenerateAndSendWorldPacket (WorldCompleteRequest.Length, 531 /* OP_WorldComplete */, -1, -1, WorldCompleteRequest);				
		}
		
		public void DoCamp()
		{
			byte[] CampRequest = new byte[4];
			int position = 0;
			DoDeleteSpawn(OurEntityID);
			WriteInt32(0, ref CampRequest, ref position); 
			GenerateAndSendWorldPacket (CampRequest.Length, 64 /* OP_Camp */, curZoneId, -1, CampRequest);				
		}
		
		public void DoZoneEntry()
		{
			DoEmuKeepAlive();
			//ignore packet contents, browser doesn't use this, instead send request based on selected character
			byte[] ZoneEntryRequest = new byte[68];
			Int32 pos = 0;
		
			WriteInt32 (0, ref ZoneEntryRequest, ref pos);
			WriteFixedLengthString(ourPlayerName, ref ZoneEntryRequest, ref pos, 64);
			AttemptingZoneConnect = false;
			GenerateAndSendWorldPacket (ZoneEntryRequest.Length, 541, curZoneId, curInstanceId, ZoneEntryRequest);
		}
		
		//op_spawnappearance
		public void DoSit()
		{
			byte[] SpawnRequest = new byte[8];
			Int32 position = 0;	
			WriteInt16 ((short)OurEntityID, ref SpawnRequest, ref position);
			WriteInt16 (14, ref SpawnRequest, ref position);
			WriteInt32 (110, ref SpawnRequest, ref position);
			GenerateAndSendWorldPacket (SpawnRequest.Length, 465, curZoneId, curInstanceId, SpawnRequest);
		}
		
		//op_spawnappearance
		public void DoStand()
		{
			byte[] SpawnRequest = new byte[8];
			Int32 position = 0;	
			WriteInt16 ((short)OurEntityID, ref SpawnRequest, ref position);
			WriteInt16 (14, ref SpawnRequest, ref position);
			WriteInt32 (100, ref SpawnRequest, ref position);
			GenerateAndSendWorldPacket (SpawnRequest.Length, 465, curZoneId, curInstanceId, SpawnRequest);
		}

		
		
		
		/* Incoming packet handlers below */
		public void HandleWorldMessage_FormattedMessage(byte[] data, int datasize)
		{
			Int32 position = 0;
			Int32 unk = ReadInt32(data, ref position);
			Int32 stringID = ReadInt32(data, ref position);
			Int32 type = ReadInt32(data, ref position);
			Int32 ChannelVarLength = datasize - position;
			string ChannelMessage = ReadFixedLengthString(data, ref position, ChannelVarLength);
//			Debug.Log("formatmessagestring: " + stringID);
//			Debug.Log("formatmessagetype: " + type);
			Debug.Log("formatmessagetype: " + ChannelMessage);
		}
		
		public void HandleWorldMessage_WorldMOTD(byte[] data, int datasize)
		{
			Int32 position = 0;
			Int32 WorldMOTDLength = datasize - position;
			string ChannelMessage = ReadFixedLengthString(data, ref position, WorldMOTDLength);
			string MessageFinal = Regex.Replace(ChannelMessage, "[\0]", "");
			ChatText2.text += (Environment.NewLine + "<color=#ffa500ff><b>MESSAGE OF THE DAY: </b></color>" + "<color=#ffa500ff><b>" + MessageFinal + "</b></color>");
		}

		public void HandleWorldMessage_ItemPacket(byte[] data, int datasize)
		{
			Int32 position = 0;
			byte packetType = ReadInt8(data, ref position);
			Int16 slotId = ReadInt16(data, ref position);
			string RestOfItem = ReadFixedLengthString(data, ref position, datasize - 3);
			string[] words = RestOfItem.Split('|');
			string slotid = "0";
			GameObject temp = null;
			//ItemPacketLoot= 0x66
			switch(packetType)
			{

			case 102:
			Int32 i1 = 0;
				foreach (string word in words)
				{
					i1++;
//					Debug.Log("i: " + i1 + "word: " + word);
					if(i1 == 3)
					{
						slotid = word;
						int slotInt = int.Parse(word);
						int slotOffset = slotInt - 22;
						temp = UIScript.slotList[slotOffset];
						temp.GetComponent<LootScript>().slotId = slotInt;
					}
					if(i1 == 13){
						string itemName = word;
						int slotInt = int.Parse(slotid);
						int slotOffset = slotInt - 22;
						temp = UIScript.slotList[slotOffset];
						temp.GetComponent<LootScript>().name = itemName;
					}
					if(i1 == 23){
						string iconId = word;
						Texture2D itemIcon = (Texture2D) Resources.Load("Icons/item_" + iconId, typeof(Texture2D));
						int slotInt = int.Parse(slotid);
						int iconInt = int.Parse(iconId);
						int slotOffset = slotInt - 22;
						temp = UIScript.slotList[slotOffset];
						temp.GetComponent<LootScript>().iconId = iconInt;
						temp.SetActive(true);
						temp.GetComponent<RawImage>().texture = itemIcon;
						temp.GetComponent<RawImage>().color = new Color(255f, 255f, 255f, 255f);

					}
				}

			break;
			//ItemPacketTrade= 0x67
			case 103:
				Int32 i2 = 0;
				if(isLooting == true)
				{
					foreach (string word in words)
					{
						i2++;
//						Debug.Log("i: " + i2 + "word: " + word);
						if(i2 == 3)
						{
							slotid = word;
	
						}
						if(i2 == 23 && int.Parse(slotid) == 30)
						{
							string iconId = word;
							Debug.Log("CURSORICON");
							cursorIconId = int.Parse(iconId);
						}
					}
				}
			break;
			//ItemPacketCharInventory= 0x69
			default:
				Int32 i3 = 0;
				foreach (string word in words)
				{
					i3++;
//					Debug.Log("i: " + i3 + "word: " + word);
					if(i3 == 179){i3 = 0;}
					if(i3 == 2)
					{
						slotid = word;
						int slotInt = int.Parse(word);
						Debug.Log("slotid: " + slotid);
						if(slotInt == 30){cursorSlotId = slotInt;}
						if(slotInt > 0 && slotInt < 22)
						{
							int slotOffset = slotInt - 1;
							temp = UIScript.equipList[slotOffset];
							temp.GetComponent<EquipScript>().slotId = slotInt;
						}
						if(slotInt > 21 && slotInt < 30)
						{
							int slotOffset = slotInt - 22;
							temp = UIScript.bagList[slotOffset];
							temp.GetComponent<BagScript>().slotId = slotInt;
						}
						
					}
					if(i3 == 13)
					{
						string itemName = word;
						int slotInt = int.Parse(slotid);
						Debug.Log("itemName: " + itemName);
						if(slotInt == 30){cursorItemName = itemName;}
						if(slotInt > 0 && slotInt < 22)
						{
							int slotOffset = slotInt - 1;
							temp = UIScript.equipList[slotOffset];
							temp.GetComponent<EquipScript>().name = itemName;
						}	
						if(slotInt > 21 && slotInt < 30)
						{
							int slotOffset = slotInt - 22;
							temp = UIScript.bagList[slotOffset];
							temp.GetComponent<BagScript>().name = itemName;
						}	
					}
					if(i3 == 22)
					{
						string iconId = word;
						Texture2D itemIcon = (Texture2D) Resources.Load("Icons/item_" + iconId, typeof(Texture2D));
						int slotInt = int.Parse(slotid);
						int iconInt = int.Parse(iconId);
						if(slotInt == 30){cursorIconId = iconInt;}
						if(slotInt > 0 && slotInt < 22)
						{
							int slotOffset = slotInt - 1;
							temp = UIScript.equipList[slotOffset];
							temp.GetComponent<EquipScript>().iconId = iconInt;
							temp.SetActive(true);
							temp.GetComponent<RawImage>().texture = itemIcon;
							temp.GetComponent<RawImage>().color = new Color(255f, 255f, 255f, 255f);
						}
						if(slotInt > 21 && slotInt < 30)
						{
							int slotOffset = slotInt - 22;
							temp = UIScript.bagList[slotOffset];
							temp.GetComponent<BagScript>().iconId = iconInt;
							temp.SetActive(true);
							temp.GetComponent<RawImage>().texture = itemIcon;
							temp.GetComponent<RawImage>().color = new Color(255f, 255f, 255f, 255f);
						}	
					}
				}
			break;
			}
		//end				
		}
		
		public void HandleWorldMessage_SpawnAppearance(byte[] data, int datasize)
		{
			Int32 position = 0;
			Int16 spawnId = ReadInt16(data, ref position);
			Int16 type = ReadInt16(data, ref position);
			Int32 parameter = ReadInt32(data, ref position);
			
			Debug.Log("spawnid: " + spawnId + " type: " + type + " parameter: " + parameter);
		}
		
		public void HandleWorldMessage_ExpUpdate(byte[] data, int datasize)
		{
			Int32 position = 0;
			Int32 exp = ReadInt32(data, ref position);
			Int32 aaxp = ReadInt32(data, ref position);
			if(initXP == true)
			{
				if(exp > 0)
				{
					ChatText2.text += (Environment.NewLine + "<color=#ffa500ff><b>You gained experience!</b></color>");
				}
				else
				{
					ChatText2.text += (Environment.NewLine + "<color=#ffa500ff><b>You lost experience!</b></color>");
				}
			}
			initXP = true;
		}

		public void HandleWorldMessage_BecomeCorpse(byte[] data, int datasize)
		{
			UIScript.LosePanel.SetActive(true);
			UIScript.LeftPanel.SetActive(false);
			UIScript.CenterPanel.SetActive(false);
			UIScript.RightPanel.SetActive(false);
			Debug.Log("BECOMECORPSE");
			isDead = true;
			DoEmuRequestClose();
		}
		public void HandleWorldMessage_Death(byte[] data, int datasize)
		{
			Int32 position = 0;
			Int32 spawnId = ReadInt32(data, ref position);
			Int32 killerId = ReadInt32(data, ref position);
			Int32 corpseId = ReadInt32(data, ref position);
			Int32 bindzoneId = ReadInt32(data, ref position);
			Int32 spellId = ReadInt32(data, ref position);
			Int32 attackskillId = ReadInt32(data, ref position);
			Int32 damage = ReadInt32(data, ref position);
			
//			GameObject temp = ObjectPool.instance.spawnlist.Where(obj => obj.name == spawnId.ToString()).SingleOrDefault();
			GameObject temp = ObjectPool.instance.spawnlist.FirstOrDefault(obj => obj.name == spawnId.ToString());

			if(temp != null)
			{
				temp.GetComponent<NPCController>().isDead = 1;
				temp.GetComponent<NPCController>().name = temp.GetComponent<NPCController>().name + "'s corpse";
			}

		
			if(spawnId == OurTargetID)
			{
				DoTarget("0");
				DoAttack(0);
			}
			if(killerId == OurEntityID)
			{
				if(temp != null)
				{
					string targetName = temp.GetComponent<NPCController>().name;// Player's Name
					string targetClean = Regex.Replace(targetName, "[0-9]", "");
					string targetName2 = Regex.Replace(targetClean, "[_]", " ");
					string targetName3 = Regex.Replace(targetName2, "[\0]", "");
					ChatText2.text += (Environment.NewLine + "You hit " + targetName3 + " for " + damage + " points of damage.");
	
					int pv = temp.GetComponent<NPCController>().NPC;
					
//					if(pv == 0){googleAnalytics.LogEvent("PvP-Death", ourPlayerName, "Killer", 1);googleAnalytics.LogEvent("PvP-Death", targetName3, "Victim", 1);}
//					if(pv == 1){googleAnalytics.LogEvent("PvE-Death", ourPlayerName, "Killer", 1);googleAnalytics.LogEvent("PvE-Death", targetName3, "Victim", 1);}
				}
			}
			if(spawnId == OurEntityID)
			{
				GameObject temp2 = ObjectPool.instance.spawnlist.FirstOrDefault(obj => obj.name == killerId.ToString());
				if(temp2 != null)
				{
					string targetName = temp2.GetComponent<NPCController>().name;// Player's Name
					string targetClean = Regex.Replace(targetName, "[0-9]", "");
					string targetName2 = Regex.Replace(targetClean, "[_]", " ");
					string targetName3 = Regex.Replace(targetName2, "[\0]", "");
					ChatText2.text += (Environment.NewLine + "<color=#ff0000ff><b>" + targetName3 + " hits" + " YOU for " + damage + " points of damage.</b></color>");
					int pv = temp2.GetComponent<NPCController>().NPC;
//					if(pv == 0){googleAnalytics.LogEvent("PvP-Death", ourPlayerName, "Victim", 1);googleAnalytics.LogEvent("PvP-Death", targetName3, "Killer", 1);}
//					if(pv == 1){googleAnalytics.LogEvent("PvE-Death", ourPlayerName, "Victim", 1);googleAnalytics.LogEvent("PvE-Death", targetName3, "Killer", 1);}
				}
			}
			
		}
			
		public void HandleWorldMessage_HPUpdate(byte[] data, int datasize)
		{
			Int32 position = 0;
			Int32 curhp = ReadInt32(data, ref position);
			Int32 maxhp = ReadInt32(data, ref position);
			Int16 spawnId = ReadInt16(data, ref position);

		
			if(spawnId == OurEntityID)
			{
				
				float hpPercent = ((float)curhp/(float)maxhp)*100;
				UIScript.OurHP.sizeDelta = new Vector2( (int)hpPercent, 10);
				UIScript.HPText.text = ((int)hpPercent + "%");
				UIScript.inventoryCurHp.text = curhp.ToString();
				UIScript.inventoryMaxHp.text = maxhp.ToString();
			}
		}
		
		public void HandleWorldMessage_MobHealth(byte[] data, int datasize)
		{
			Int32 position = 0;
			Int16 spawnId = ReadInt16(data, ref position); //hp percent
			byte hp = ReadInt8(data, ref position);

//			GameObject temp = ObjectPool.instance.spawnlist.Where(obj => obj.name == spawnId.ToString()).SingleOrDefault();
			GameObject temp = null;
			if(isDead == false){temp = ObjectPool.instance.spawnlist.FirstOrDefault(obj => obj.name == spawnId.ToString());}
			if(temp != null)
			{
				temp.GetComponent<NPCController>().curHp = hp;// Player's Name
				UIScript.TargetHP.sizeDelta = new Vector2( (int)hp, 10);
				UIScript.TargetHPText.text = ((int)hp + "%");
			}
		}
		
		public void HandleWorldMessage_Damage(byte[] data, int datasize)
		{
			Int32 position = 0;
			
			Int16 target = ReadInt16(data, ref position);
			Int16 source = ReadInt16(data, ref position);
			byte type = ReadInt8(data, ref position); //slashing, etc. 231 (0xE7) for spells
			Int16 spellId = ReadInt16(data, ref position);
			Int32 damage = ReadInt32(data, ref position);
			float force = ReadInt32(data, ref position);
			float meleepush_xy = ReadInt32(data, ref position);
			float meleepush_z = ReadInt32(data, ref position);
			Int32 special = ReadInt32(data, ref position); // 2 = Rampage, 1 = Wild Rampage
		
			if(target == OurEntityID)
			{
//				GameObject temp = ObjectPool.instance.spawnlist.Where(obj => obj.name == source.ToString()).SingleOrDefault();
				DoClientUpdate();
				GameObject temp = ObjectPool.instance.spawnlist.FirstOrDefault(obj => obj.name == source.ToString());
				string sourceName = temp.GetComponent<NPCController>().name;// Player's Name
				string sourceClean = Regex.Replace(sourceName, "[0-9]", "");
				string sourceName2 = Regex.Replace(sourceClean, "[_]", " ");
				string sourceName3 = Regex.Replace(sourceName2, "[\0]", "");

				switch(type)
				{
					case 4:
						ChatText2.text += (Environment.NewLine + "<color=#ff0000ff><b>" + sourceName3 + " hits" + " YOU for " + damage + " points of damage.</b></color>");
						break;
					case 30:
						ChatText2.text += (Environment.NewLine + "<color=#ff0000ff><b>" + sourceName3 + " kicks" + " YOU for " + damage + " points of damage.</b></color>");
						break;

					default:
						ChatText2.text += (Environment.NewLine + "<color=#ff0000ff><b>" + sourceName3 + " " + type + " YOU for " + damage + " points of damage.</b></color>");
						break;
				}
			}
			
			if(source == OurEntityID)
			{
//				GameObject temp2 = ObjectPool.instance.spawnlist.Where(obj => obj.name == target.ToString()).SingleOrDefault();
				GameObject temp2 = ObjectPool.instance.spawnlist.FirstOrDefault(obj => obj.name == target.ToString());
				string targetName = temp2.GetComponent<NPCController>().name;// Player's Name
				string targetClean = Regex.Replace(targetName, "[0-9]", "");
				string targetName2 = Regex.Replace(targetClean, "[_]", " ");
				string targetName3 = Regex.Replace(targetName2, "[\0]", "");
				
				switch(type)
				{
					case 4:
						ChatText2.text += (Environment.NewLine + "You hit " + targetName3 + " for " + damage + " points of damage.");
						break;
					default:
//						ChatText2.text += (Environment.NewLine + "You " + type + " " + targetName3 + " for " + damage + " points of damage.");
						ChatText2.text += (Environment.NewLine + "You hit " + targetName3 + " for " + damage + " points of damage.");
						break;
				}
			}
		}
		
		public void HandleWorldMessage_SimpleMessage(byte[] data, int datasize)
		{
			Int32 position = 0;
			Int32 stringId = ReadInt32(data, ref position);
			if(stringId == 124)
			{
				ChatText2.text += (Environment.NewLine + "Your target is too far away, get closer!");
			}
		}
		//545
		public void HandleWorldMessage_ZoneServerInfo(byte[] data, int datasize)
		{
			//ignore packet contents, browser doesn't use this, instead send request based on selected character
			byte[] ZoneEntryRequest = new byte[68];
			Int32 pos = 0;
		
//			curZoneId = 2;
			WriteInt32 (0, ref ZoneEntryRequest, ref pos);
			WriteFixedLengthString(ourPlayerName, ref ZoneEntryRequest, ref pos, 64);
			AttemptingZoneConnect = false;
			GenerateAndSendWorldPacket (ZoneEntryRequest.Length, 541, curZoneId, curInstanceId, ZoneEntryRequest);
			DoWorldComplete();
			SceneManager.LoadScene("2 North Qeynos");
			CharSelectCamera.SetActive(false);
			CharSelect.SetActive(false);
			UIScriptsObject.SetActive(true);
			GameCamera.SetActive(true);
			isTyping = false;
			AttemptingZoneConnect = true;
			//WorldConnectObject.AddComponent<EQBrowser>("UIScripts");
//			WorldConnectObject.AddComponent<UIScripts>();
		}

//work in progress
		public void HandleWorldMessage_ZonePlayerToBind(byte[] data, int datasize)
		{
//			AttemptingZoneConnect = true;
			Int32 position = 0;
			Int16 bindZoneid = ReadInt16(data, ref position);
			Int16 bindInstanceid = ReadInt16(data, ref position);
			float x = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);
			float y = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);
			float z = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);
			float h = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);
//			string zoneName = ReadFixedLengthString(data, ref position, 1);

			
			byte[] ZoneChangeRequest = new byte[88];
			Int32 pos = 0;
			WriteFixedLengthString(ourPlayerName, ref ZoneChangeRequest, ref pos, 64);
			WriteInt16 (2, ref ZoneChangeRequest, ref pos);
			WriteInt16 (0, ref ZoneChangeRequest, ref pos);
			WriteInt32 (0, ref ZoneChangeRequest, ref pos);
			WriteInt32 (0, ref ZoneChangeRequest, ref pos);
			WriteInt32 (0, ref ZoneChangeRequest, ref pos);
			WriteInt32 (10, ref ZoneChangeRequest, ref pos);
			WriteInt32 (0, ref ZoneChangeRequest, ref pos);
			GenerateAndSendWorldPacket (ZoneChangeRequest.Length, 539, 2, bindInstanceid, ZoneChangeRequest);
		}
		public void HandleWorldMessage_DeleteSpawn(byte[] data, int datasize)
		{

			Int32 position = 0;
			Int32 spawn_id = ReadInt32(data, ref position);
//			byte decay = ReadInt8(data, ref position); // 0 = vanish immediately, 1 = 'Decay' sparklies for corpses.
//			GameObject temp = ObjectPool.instance.spawnlist.Where(obj => obj.name == spawn_id.ToString()).SingleOrDefault();
			GameObject temp = null;
			if(isDead == false){temp = ObjectPool.instance.spawnlist.FirstOrDefault(obj => obj.name == spawn_id.ToString());}
			if(temp != null)
			{
					string PrefabName = temp.GetComponent<NPCController>().prefabName;			
					temp.name = PrefabName;
					Debug.Log("DELETESPAWN: " + spawn_id);
					ObjectPool.instance.PoolObject(temp);
					ObjectPool.instance.spawnlist.Remove(temp); 
			}
    
		}

//work in progress		
		public void HandleWorldMessage_ZoneChange(byte[] data, int datasize)
		{
			
			DoDeleteSpawn(OurEntityID);
			Debug.Log("ZONECHANGE");
		}
		
		
		
		public void HandleWorldMessage_ChannelMessage(byte[] data, int datasize)
		{
			Int32 position = 0;
			string ChannelTargetName = ReadFixedLengthString(data, ref position, 64);
			string ChannelSender = ReadFixedLengthString(data, ref position, 64);
			Int32 ChannelLanguage = ReadInt32(data, ref position);
			Int32 ChannelNumber = ReadInt32(data, ref position);
			Int32 ChannelSkill = ReadInt32(data, ref position);
			Int32 ChannelVarLength = datasize - position;
			string ChannelMessage = ReadFixedLengthString(data, ref position, ChannelVarLength);
			DoChatDisplay(ChannelSender,ChannelMessage);
		}

	
		public void HandleWorldMessage_ClientUpdate(byte[] data, int datasize)
		{
			Int32 position = 0;
			
			Int16 spawn_id = ReadInt16(data, ref position);
			float x = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);
			float y = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);
			float z = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);
			float deltaX = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);
			float deltaY = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);
			float deltaZ = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);
			float deltaH = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);
			Int32 animationspeed = ReadInt32(data, ref position);
			float rotation = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);
			
			
			GameObject temp = null;
			if(isDead == false){temp = ObjectPool.instance.spawnlist.FirstOrDefault(obj => obj.name == spawn_id.ToString());}
//			GameObject temp = ObjectPool.instance.spawndict[spawn_id];
			if(temp != null)
			{
				temp.GetComponent<NPCController>().movetoX = -x;// Player's Name
				temp.GetComponent<NPCController>().movetoY = z;// Player's Name
				temp.GetComponent<NPCController>().movetoZ = y;// Player's Name
				temp.GetComponent<NPCController>().movetoH = rotation;// Player's Name
				temp.GetComponent<NPCController>().animationspeed = animationspeed;// animationspeed
				temp.GetComponent<NPCController>().deltaX = -deltaX;
				temp.GetComponent<NPCController>().deltaY = deltaZ;
				temp.GetComponent<NPCController>().deltaZ = deltaY;
				temp.GetComponent<NPCController>().deltaH = deltaH;// Player's Name
			}
			
			
		}

		public void HandleWorldMessage_PlayerStateAdd(byte[] data, int datasize)
		{
			Int32 position = 0;
			
			Int32 spawnId = ReadInt32(data, ref position);
			Int32 state = ReadInt32(data, ref position);
			Debug.Log("StateAdd: Spawnid: " + spawnId + " state: " + state);

//			GameObject temp = ObjectPool.instance.spawnlist.Where(obj => obj.name == spawnId.ToString()).SingleOrDefault();
			GameObject temp = ObjectPool.instance.spawnlist.FirstOrDefault(obj => obj.name == spawnId.ToString()); 
			if(temp != null)
			{
				temp.GetComponent<NPCController>().animationState = state;// Player's Name
			}
			
		}
		public void HandleWorldMessage_PlayerStateRemove(byte[] data, int datasize)
		{
			Int32 position = 0;
			
			Int32 spawnId = ReadInt32(data, ref position);
			Int32 state = ReadInt32(data, ref position);
			Debug.Log("StateRemove: Spawnid: " + spawnId + " state: " + state);
		}
		public void HandleWorldMessage_ZoneEntryInfo(byte[] data, int datasize)
		{

			Int32 position = 0;
			Int32 id = ReadInt32(data, ref position);

//			SceneManagerObj.elfid = (UInt32)id;
			string CharName = ReadFixedLengthString(data, ref position, 64);
			float x = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);
			float y = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);
			float z = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);
			EqemuConnectObject.transform.position = new Vector3(-x, z, y);
			playerLock = false;
			AttemptingZoneConnect = false;
			
		}

		//548
		public void HandleWorldMessage_ZoneUnavailable(byte[] data, int datasize)
		{
		AttemptingZoneConnect = false;
		ws_.Close ();
		CSel.BackToLogin ();
		CSel.LoginStatus.text = "You have been disconnected.";
		}

		//365
		public void HandleWorldMessage_PlayerProfile(byte[] data, int datasize)
		{
			byte[] NewZoneRequest = null;
//			curZoneId = 2;
			Int32 position = 0;
			Int32 checksum = ReadInt32(data, ref position);
			string CharName = ReadFixedLengthString(data, ref position, 64);
			string CharLastName = ReadFixedLengthString(data, ref position, 32);
			Int16 zone_id = ReadInt16(data, ref position);
			Int32 gender = ReadInt32(data, ref position);
			Int32 race = ReadInt32(data, ref position);
			Int32 eqclass = ReadInt32(data, ref position);
			Int32 strength = ReadInt32(data, ref position);
			Int32 stamina = ReadInt32(data, ref position);
			Int32 charisma = ReadInt32(data, ref position);
			Int32 dexterity = ReadInt32(data, ref position);
			Int32 intellect = ReadInt32(data, ref position);
			Int32 agility = ReadInt32(data, ref position);
			Int32 wisdom = ReadInt32(data, ref position);
			Int32 level = ReadInt32(data, ref position);
			float y	= BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);
			float z = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);
			float x = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);
			Int32 diety = ReadInt32(data, ref position);
			Int32 guildid = ReadInt32(data, ref position);
			int j = 0;
			for(j = 0; j < 5; j++)
			{
				Int32 zoneid = ReadInt32(data, ref position);
				float x2 = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);
				float y2 = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);
				float z2 = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);
				float heading2 = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);
				Int32 isntanceid = ReadInt32(data, ref position);
			}
			Int32 birthday = ReadInt32(data, ref position);
			Int32 lastlogin = ReadInt32(data, ref position);
			Int32 timeplayedmin = ReadInt32(data, ref position);
			byte pvp = ReadInt8 (data, ref position); 
			byte level2 = ReadInt8 (data, ref position); 
			byte anon = ReadInt8 (data, ref position); 
			byte gm = ReadInt8 (data, ref position); 
			byte guildrank = ReadInt8 (data, ref position); 
			byte guildbanker = ReadInt8 (data, ref position); 
			Int32 intoxication = ReadInt32(data, ref position);
			int a = 0;
			for(a = 0; a < 9; a++)
			{
				Int32 spellslotrefresh = ReadInt32(data, ref position);
			}
			Int32 abilityslotrefresh = ReadInt32(data, ref position);
			byte haircolor = ReadInt8 (data, ref position); 
			byte beardcolor = ReadInt8 (data, ref position); 
			byte eyecolor1 = ReadInt8 (data, ref position); 
			byte eyecolor2 = ReadInt8 (data, ref position); 
			byte hairstyle = ReadInt8 (data, ref position); 
			byte beard = ReadInt8 (data, ref position); 
			byte abilitytimeseconds = ReadInt8 (data, ref position); 
			byte abilitynumber = ReadInt8 (data, ref position); 
			byte abilitytimeminutes = ReadInt8 (data, ref position); 
			byte abilitytimehours = ReadInt8 (data, ref position); 
			int b = 0;
			for(b = 0; b < 9; b++)
			{
				Int32 itemmaterial = ReadInt32(data, ref position);
			}
			int k = 0;
			for(k = 0; k < 9; k++)
			{
				Int32 itemtint = ReadInt32(data, ref position);
			}
			int p = 0;
			for(p = 0; p < 240; p++)
			{
				Int32 aa = ReadInt32(data, ref position);
				Int32 value = ReadInt32(data, ref position);
				Int32 charges = ReadInt32(data, ref position);
			}
			string servername = ReadFixedLengthString(data, ref position, 32);
			string title = ReadFixedLengthString(data, ref position, 32);
			string suffix = ReadFixedLengthString(data, ref position, 32);
			Int32 guildid2 = ReadInt32(data, ref position);
			Int32 exp = ReadInt32(data, ref position);
			Int32 points = ReadInt32(data, ref position);
			Int32 mana = ReadInt32(data, ref position);
			Int32 curhp = ReadInt32(data, ref position);
			byte face = ReadInt8 (data, ref position);
			int w = 0;
			for(w = 0; w < 28; w++)
			{
				byte languages = ReadInt8 (data, ref position);
			}
			int v = 0;
			for(v = 0; v < 480; v++)
			{
				Int32 spellbook = ReadInt32(data, ref position);
			}
			int n = 0;
			for(n = 0; n < 9; n++)
			{
				Int32 memspells = ReadInt32(data, ref position);
			}
			float heading = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);
			Int32 platinum = ReadInt32(data, ref position);
			Int32 gold = ReadInt32(data, ref position);
			Int32 silver = ReadInt32(data, ref position);
			Int32 copper = ReadInt32(data, ref position);
			Int32 platinumbank = ReadInt32(data, ref position);
			Int32 goldbank = ReadInt32(data, ref position);
			Int32 silverbank = ReadInt32(data, ref position);
			Int32 copperbank = ReadInt32(data, ref position);
			Int32 platinumcursor = ReadInt32(data, ref position);
			Int32 goldcursor = ReadInt32(data, ref position);
			Int32 silvercursor = ReadInt32(data, ref position);
			Int32 coppercursor = ReadInt32(data, ref position);
			Int32 platinumshared = ReadInt32(data, ref position);
			int t = 0;
			for(t = 0; t < 100; t++)
			{
					Int32 skills = ReadInt32(data, ref position);
			}
			Int32 pvp2 = ReadInt32(data, ref position);
			Int32 pvptype = ReadInt32(data, ref position);
			Int32 abilitydown = ReadInt32(data, ref position);
			Int32 autosplit = ReadInt32(data, ref position);
			Int32 zonechangecount = ReadInt32(data, ref position);
			Int32 drakkin_heritage = ReadInt32(data, ref position);
			Int32 drakkin_tattoo = ReadInt32(data, ref position);
			Int32 drakkin_details = ReadInt32(data, ref position);
			Int32 expansions = ReadInt32(data, ref position);
			Int32 toxicity = ReadInt32(data, ref position);
			Int32 hungerlevel = ReadInt32(data, ref position);
			Int32 thirstlevel = ReadInt32(data, ref position);
			Int32 abilityup = ReadInt32(data, ref position);
			Int16 zoneinstance = ReadInt16(data, ref position);
			int q = 0;
			for(q = 0; q < 25; q++)
			{
				byte slotid = ReadInt8 (data, ref position);
				byte level3 = ReadInt8 (data, ref position);
				byte bardmodifier = ReadInt8 (data, ref position);
				byte effect = ReadInt8 (data, ref position);
				Int32 spellid = ReadInt32(data, ref position);
				Int32 duration = ReadInt32(data, ref position);
				Int32 counters = ReadInt32(data, ref position);
				Int32 playerid = ReadInt32(data, ref position);
				
			}
			int r = 0;
			for(r = 0; r < 6; r++)
			{
				string groupmember = ReadFixedLengthString(data, ref position, 64);
			}
			Int32 entityid = ReadInt32(data, ref position);
			OurEntityID = entityid;
			
			UIScript.inventoryName.text = CharName;
			UIScript.inventoryLevel.text = level.ToString();
//			UIScript.inventoryClass.text = eqclass.ToString();
			UIScript.inventoryClass.text = "Warrior";
			UIScript.inventoryExp.text = exp.ToString();
//			UIScript.inventoryRace.text = race.ToString();
//			UIScript.inventoryAc.text =
//			UIScript.inventoryAtk.text =
			UIScript.inventoryStrength.text = strength.ToString();
			UIScript.inventoryStamina.text = stamina.ToString();
			UIScript.inventoryCharisma.text = charisma.ToString();
			UIScript.inventoryDexterity.text = dexterity.ToString();
			UIScript.inventoryIntellect.text = intellect.ToString();
			UIScript.inventoryAgility.text = agility.ToString();
			UIScript.inventoryWisdom.text = wisdom.ToString();
			UIScript.inventoryPlatinum.text = platinum.ToString();
			UIScript.inventoryGold.text = gold.ToString();
			UIScript.inventorySilver.text = silver.ToString();
			UIScript.inventoryCopper.text = copper.ToString();

			GameObject us = EqemuConnectObject;
//			us.transform.position = new Vector3(-x,(y+4),-z);
//			us.transform.position = new Vector3(-z,x,y);

			float h = Mathf.Lerp(360,0,heading/255f);
			us.transform.localEulerAngles = new Vector3(0,h,0);
			
			GenerateAndSendWorldPacket (0, 403 /* OP_ReqNewZone */, curZoneId, curInstanceId, NewZoneRequest);
			
//			if(platinum > 0){googleAnalytics.LogEvent("Currency_PP-Inventory", ourPlayerName, "Platinum", platinum);}
//			if(gold > 0){googleAnalytics.LogEvent("Currency_PP-Inventory", ourPlayerName, "Gold", gold);}
//			if(silver > 0){googleAnalytics.LogEvent("Currency_PP-Inventory", ourPlayerName, "Silver", silver);}
//			if(copper > 0){googleAnalytics.LogEvent("Currency_PP-Inventory", ourPlayerName, "Copper", copper);}

		}
		//338
		public void HandleWorldMessage_NewZone(byte[] data, int datasize)
		{
			byte[] ReqClientSpawn = null;
			Int32 position = 0;

			string CharName = ReadFixedLengthString(data, ref position, 64);
			string ZoneName = ReadFixedLengthString(data, ref position, 32);
//			SceneManagerObj.zoneName = ZoneName;


 			GenerateAndSendWorldPacket (0, 402 /* OP_ReqClientSpawn */, curZoneId, curInstanceId, ReqClientSpawn);
		}

		public void HandleWorldMessage_NewSpawn(byte[] data, int datasize)
		{
			byte[] ReqClientSpawn = null;
			Int32 position = 0;
			UInt32 spawnid = (UInt32)ReadInt32(data, ref position);
			string CharName = ReadFixedLengthString(data, ref position, 64);
			float x = -BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);
			float y = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);
			float z = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);

//			NetworkEntity nObj = SceneManagerObj.Singleton.AddEntity((int)spawnid, x, z, y);

//			nObj.SetName(CharName);
//			nObj.SetID((int)spawnid);

		}

		public void SendPositionUpdate()
		{
			byte[] PositionUpdateRequest = new byte[38];
			Int32 pos = 0;
//			WriteInt16((short)SceneManagerObj.elfid, ref PositionUpdateRequest, ref pos);
//			Int32 xinv = (BitConverter.ToInt32(BitConverter.GetBytes(SceneManagerObj.elfpal.transform.position.x), 0));
//			WriteInt32(xinv, ref PositionUpdateRequest, ref pos);
//			WriteInt32(BitConverter.ToInt32(BitConverter.GetBytes(SceneManagerObj.elfpal.transform.position.x), 0), ref PositionUpdateRequest, ref pos);
//			WriteInt32(BitConverter.ToInt32(BitConverter.GetBytes(SceneManagerObj.elfpal.transform.position.y), 0), ref PositionUpdateRequest, ref pos);
			WriteInt32(0, ref PositionUpdateRequest, ref pos);
			WriteInt32(0, ref PositionUpdateRequest, ref pos);
			WriteInt32(0, ref PositionUpdateRequest, ref pos);
			WriteInt32(0, ref PositionUpdateRequest, ref pos);
			WriteInt32(0, ref PositionUpdateRequest, ref pos);
			WriteInt32(0, ref PositionUpdateRequest, ref pos);
			GenerateAndSendWorldPacket(PositionUpdateRequest.Length, 468 /* OP_EmuKeepAlive */, curZoneId, curInstanceId, PositionUpdateRequest);
			

		}

		public void HandleWorldMessage_PositionUpdate(byte[] data, int datasize)
		{
			Int32 position = 0;
			Int32 entid = ReadInt16(data, ref position);
			float x = -BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);
			float y = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);
			float z = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);
//			NetworkEntity ent = SceneManagerObj.Singleton.GetEntity(entid);
//			if (ent)
//			{
//				ent.transform.position = new Vector3(x, z, y);
//			}

		}



		//
		public void HandleWorldMessage_ZoneServerReady(byte[] data, int datasize)
		{
			byte[] NotifyClientReady = null;
//			Application.LoadLevel(SceneManagerObj.zoneName);
//			SceneManagerObj.Singleton.TrueStart();
//			SceneManagerObj.Singleton.ForceCamera(SceneManagerObj.CameraMode.ThirdPersonLocked);
			GenerateAndSendWorldPacket (0, 85 /* OP_ClientReady */, curZoneId, curInstanceId, NotifyClientReady);
		}

//424		
		public void HandleWorldMessage_SendExpZonein(byte[] data, int datasize)
		{
			GameObject us = EqemuConnectObject;
			float x = -us.transform.position.x;
			float y = us.transform.position.z;
			float z = us.transform.position.y;
			float h = us.transform.rotation.y;
			var controller = us.GetComponent<CharacterController>(); 
			//float x = 234;
			//float y = 11;
			//float z = 2;
			//float h = 122;
						
			byte[] PositionUpdateRequest = new byte[38];
			Int32 position = 0;
				
			WriteInt16((short)OurEntityID, ref PositionUpdateRequest, ref position);
			WriteInt32(BitConverter.ToInt32(BitConverter.GetBytes(x), 0), ref PositionUpdateRequest, ref position);
			WriteInt32(BitConverter.ToInt32(BitConverter.GetBytes(y), 0), ref PositionUpdateRequest, ref position);
			WriteInt32(BitConverter.ToInt32(BitConverter.GetBytes(z), 0), ref PositionUpdateRequest, ref position);
			WriteInt32(BitConverter.ToInt32(BitConverter.GetBytes(-controller.velocity.x), 0), ref PositionUpdateRequest, ref position);
			WriteInt32(BitConverter.ToInt32(BitConverter.GetBytes(controller.velocity.z), 0), ref PositionUpdateRequest, ref position);
			WriteInt32(BitConverter.ToInt32(BitConverter.GetBytes(controller.velocity.y), 0), ref PositionUpdateRequest, ref position);
			WriteInt32(0, ref PositionUpdateRequest, ref position);
			WriteInt32(BitConverter.ToInt32(BitConverter.GetBytes(controller.velocity.magnitude), 0), ref PositionUpdateRequest, ref position);
			WriteInt32(BitConverter.ToInt32(BitConverter.GetBytes(h), 0), ref PositionUpdateRequest, ref position);

			GenerateAndSendWorldPacket (PositionUpdateRequest.Length, 87 /* OP_ClientUpdate */, 2, curInstanceId, PositionUpdateRequest);
		}

		public void HandleWorldMessage_EmuKeepAlive(byte[] data, int datasize)
		{
			byte[] KeepAlive = null;
			GenerateAndSendWorldPacket (0, 550 /* OP_EmuKeepAlive */, curZoneId, curInstanceId, KeepAlive);
		}
	
		public void HandleWorldMessage_MoneyOnCorpse(byte[] data, int datasize)
		{
				Int32 position = 0;
				byte response = ReadInt8(data, ref position);
				byte unk1 = ReadInt8(data, ref position);
				byte unk2 = ReadInt8(data, ref position);
				byte unk3 = ReadInt8(data, ref position);
				Int32 platinum = ReadInt32(data, ref position);
				Int32 gold = ReadInt32(data, ref position);
				Int32 silver = ReadInt32(data, ref position);
				Int32 copper = ReadInt32(data, ref position);
				
				int monies = platinum + gold + silver + copper;
				
				if(response == 0){ChatText2.text += (Environment.NewLine + "Someone else is looting this corpse");};
				if(response == 1){
					UIScript.LootBox.SetActive (true);
	
					if(monies > 0){ChatText2.text += (Environment.NewLine + "You looted: " + platinum + "pp, " + gold + "gp, " + silver + "sp, " + copper + "sp, ");}
					if(platinum > 0)
					{
//						googleAnalytics.LogEvent("Currency_Corpse", ourPlayerName, "Platinum", platinum);
						int platInt = int.Parse(UIScript.inventoryPlatinum.text);
						int platSum = platInt + platinum; 
						UIScript.inventoryPlatinum.text = platSum.ToString();
					}
					if(gold > 0)
					{
//						googleAnalytics.LogEvent("Currency_Corpse", ourPlayerName, "Gold", gold);
						int goldInt = int.Parse(UIScript.inventoryGold.text);
						int goldSum = goldInt + gold; 
						UIScript.inventoryGold.text = goldSum.ToString();
					}
					if(silver > 0)
					{
//						googleAnalytics.LogEvent("Currency_Corpse", ourPlayerName, "Silver", silver);
						int silverInt = int.Parse(UIScript.inventorySilver.text);
						int silverSum = silverInt + silver; 
						UIScript.inventorySilver.text = silverSum.ToString();
					}
					if(copper > 0)
					{
//						googleAnalytics.LogEvent("Currency_Corpse", ourPlayerName, "Copper", copper);
						int copperInt = int.Parse(UIScript.inventoryCopper.text);
						int copperSum = copperInt + copper; 
						UIScript.inventoryCopper.text = copperSum.ToString();
					}
				};
				if(response == 2){ChatText2.text += (Environment.NewLine + "You may not loot this corpse at this time.");};

		}
		
		//296
		public void HandleWorldMessage_LogOutReply(byte[] data, int datasize)
		{
			if(AttemptingZoneConnect == false)
			{
				Destroy (WorldConnectObject);
				SceneManager.LoadScene("1 Character creation");
				ws_.Close ();
				CSel.BackToLogin ();
//				Destroy (WorldConnectObject);
				CSel.LoginStatus.text = "You have been disconnected.";
			}
		}
		
		//551
		public void HandleWorldMessage_EmuRequestClose(byte[] data, int datasize)
		{
			
			if(AttemptingZoneConnect == false)
			{
				string ActiveScene = SceneManager.GetActiveScene().name;
				if (ActiveScene == "2 North Qeynos")
				{
					SceneManager.LoadScene("1 Character creation");
					Destroy (WorldConnectObject);
				}
				else
				{
					ws_.Close ();
					CSel.BackToLogin ();
					CSel.LoginStatus.text = "You have been disconnected.";
				}
			}
		}
		
		//36
		public void HandleWorldMessage_ApproveName(byte[] data, int datasize)
		{
			if (datasize != 1 || CSel == null)
			{
				return;
			}
			int position = 0;
			byte result = ReadInt8(data, ref position);
			
			if (result == 1) {
				//Create character. Our params are as follows:
				byte[] CharCreateRequest = new byte[92];
				Int32 pos = 0;

				WriteInt32(CSel._ClassSelection, ref CharCreateRequest, ref pos);
				WriteInt32(0, ref CharCreateRequest, ref pos); //haircolor
				WriteInt32(0, ref CharCreateRequest, ref pos);
				WriteInt32(0, ref CharCreateRequest, ref pos); //beard color
				WriteInt32(CSel.GenderSelection, ref CharCreateRequest, ref pos);
				WriteInt32(CSel._RaceSelection, ref CharCreateRequest, ref pos);
				WriteInt32(CSel.ZoneSelection, ref CharCreateRequest, ref pos);
				WriteInt32(0, ref CharCreateRequest, ref pos);
				WriteInt32(CSel._DeitySelection, ref CharCreateRequest, ref pos);
				WriteInt32(110, ref CharCreateRequest, ref pos); //STR
				WriteInt32(85, ref CharCreateRequest, ref pos); //STA
				WriteInt32(75, ref CharCreateRequest, ref pos); //DEX
				WriteInt32(75, ref CharCreateRequest, ref pos); //AGI
				WriteInt32(75, ref CharCreateRequest, ref pos); //INT
				WriteInt32(80, ref CharCreateRequest, ref pos); //WIS
				WriteInt32(75, ref CharCreateRequest, ref pos); //CHA
				WriteInt32(7, ref CharCreateRequest, ref pos); //Face
				WriteInt32(0, ref CharCreateRequest, ref pos); //Eye color
				WriteInt32(0, ref CharCreateRequest, ref pos); //Eye color2
				WriteInt32(117440512, ref CharCreateRequest, ref pos); //Drakkin Heritage
				WriteInt32(0, ref CharCreateRequest, ref pos); //Drakkin Tattoo
				WriteInt32(0, ref CharCreateRequest, ref pos); //Drakkin Details
				WriteInt32(1, ref CharCreateRequest, ref pos); //Tutorial is selected?
				GenerateAndSendWorldPacket (CharCreateRequest.Length, 70 /* OP_CharacterCreate */, -1, -1, CharCreateRequest);

				string TidyName = CSel.TidyCase(CSel.CreationName.text);
//				googleAnalytics.LogEvent("CharCreate", "Name", TidyName, 1);
				
				string classSelect = "";
				switch(CSel._ClassSelection)
				{
					case 1:
						classSelect = "Warrior";
						break;
					
					default:
						break;
				}
//				googleAnalytics.LogEvent("CharCreate", "Class", classSelect, 1);
				
				string raceSelect = "";
				switch(CSel._RaceSelection)
				{
					case 1:
						raceSelect = "Human";
						break;
					
					default:
						break;
				}
//				googleAnalytics.LogEvent("CharCreate", "Race", raceSelect, 1);

			}
			else
			{
			CSel.CCPanel1.SetActive (false);	
			CSel.CCPanel2.SetActive (true);	
			CSel.CCPanel3.SetActive (false);
			CSel.CCPanel0.SetActive (false);
			CSel.CreationStatus.color = Color.yellow;
			CSel.CreationStatus.text = "Name not valid, try again";
			string TidyName = CSel.TidyCase(CSel.CreationName.text);
//			googleAnalytics.LogEvent("CharCreate", "Name_Fail", TidyName, 1);

			}


		}
		
		//423
		public void HandleWorldMessage_SendCharInfo(byte[] data, int datasize)
		{
			
			if (datasize <= 0 || CSel == null)
				return;
			
			CSel.ToCharList();
			CSel.ClearCharButtonText ();
			Int32 position = 0;
			Int32 numChar = ReadInt32(data, ref position);
			Int32 numAllowedChar = ReadInt32(data, ref position);
			int i = 0;
			int curSelIndex = 0;
			for (i = 0; i < numChar; i++) {
				string name = ReadFixedLengthString(data, ref position, 64);
				byte _class = ReadInt8(data, ref position);
				Int32 _race = ReadInt32(data, ref position);
				byte level = ReadInt8(data, ref position);
				byte _shroudclass = ReadInt8(data, ref position);
				Int32 _shroudrace = ReadInt32(data, ref position);
				Int16 zoneid = ReadInt16(data, ref position);
				Int16 instanceid = ReadInt16(data, ref position);
				byte gender = ReadInt8(data, ref position);
				byte face = ReadInt8(data, ref position);
				int j = 0;
				for(j = 0; j < 9; j++)
				{
					Int32 Material1 = ReadInt32 (data, ref position);
					Int32 Material2 = ReadInt32 (data, ref position); 
					byte Red = ReadInt8 (data, ref position); 
					byte Green = ReadInt8 (data, ref position); 
					byte Blue = ReadInt8 (data, ref position);
					byte tint = ReadInt8 (data, ref position);
				}
				Int32 deity = ReadInt32(data, ref position);
				Int32 idfile1 = ReadInt32(data, ref position);
				Int32 idfile2 = ReadInt32(data, ref position);
				byte haircolor = ReadInt8(data, ref position);
				byte beardcolor = ReadInt8(data, ref position);
				byte eyecolor1 = ReadInt8(data, ref position);
				byte eyecolor2 = ReadInt8(data, ref position);
				byte hairstyle = ReadInt8(data, ref position);
				byte beard = ReadInt8(data, ref position);
				byte gohome = ReadInt8(data, ref position);
				byte tutorial = ReadInt8(data, ref position);
				Int32 DrakkinHeritage = ReadInt32(data, ref position);
				byte enabled = ReadInt8(data, ref position);
				Int32 lastlogin = ReadInt32(data, ref position);
				if(_class > 0)
				{
					CSel.UpdateCharButtonText(curSelIndex, name);
					curSelIndex++;
				}

			}
			CSel.ToCharList();
			
		}
		
		//547
		public void HandleWorldMessage_ZoneSpawns(byte[] data, int datasize)
		{
			if (datasize <= 0)
				return;
			
			Int32 position = 0;

			while (position < datasize)
			{
				Int32 spawnId = ReadInt32(data, ref position);// Spawn Id
				string name = ReadFixedLengthString(data, ref position, 64);// Player's Name
				float x = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);// x coord
				float y = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);// y coord
				float z = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);// z coord
				float heading = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);// heading
				Int16 deity = ReadInt16(data, ref position);// Player's Deity
				float size = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);// Model size
				byte NPC = ReadInt8(data, ref position);// 0=player,1=npc,2=pc corpse,3=npc corpse,a
				byte haircolor = ReadInt8(data, ref position);// Hair color
				byte curHp = ReadInt8(data, ref position);// Current hp %%% wrong
				byte max_hp = ReadInt8(data, ref position);// (name prolly wrong)takes on the value 100 for players, 100 or 110 for NPCs and 120 for PC corpses...
				byte findable = ReadInt8(data, ref position);// 0=can't be found, 1=can be found
				float deltaY = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);// change in y
				float deltaX = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);// change in x
				float deltaZ = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);// change in z
				float deltaHeading = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);// change in heading
				byte animation = ReadInt8(data, ref position);// animation
				byte eyecolor1 = ReadInt8(data, ref position);// Player's left eye color
				byte StandState = ReadInt8(data, ref position);// stand state for SoF+ 0x64 for normal animation
				Int32 drakkin_heritage = ReadInt32(data, ref position);// Added for SoF
				Int32 drakkin_tattoo = ReadInt32(data, ref position);// Added for SoF
				Int32 drakkin_details = ReadInt32(data, ref position);// Added for SoF
				byte showhelm = ReadInt8(data, ref position);// 0=no, 1=yes
				byte is_npc = ReadInt8(data, ref position);// 0=no, 1=yes
				byte hairstyle = ReadInt8(data, ref position);// Hair style
				byte beard = ReadInt8(data, ref position);// Beard style (not totally, sure but maybe!)
				byte level = ReadInt8(data, ref position);// Spawn Level
				Int32 PlayerState = ReadInt32(data, ref position);// Controls animation stuff
				byte beardcolor = ReadInt8(data, ref position);// Beard color
				string suffix = ReadFixedLengthString(data, ref position, 32);// Player's suffix (of Veeshan, etc.)
				Int32 petOwnerId = ReadInt32(data, ref position);// If this is a pet, the spawn id of owner
				byte guildrank = ReadInt8(data, ref position);// 0=normal, 1=officer, 2=leader
				int j = 0;
				for(j = 0; j < 5; j++)
				{
					Int32 material = ReadInt32(data, ref position);// Equipment visuals
					Int32 unknown1 = ReadInt32(data, ref position);// Equipment visuals
					Int32 elitematerial = ReadInt32(data, ref position);// Equipment visuals
					Int32 heroforgemodel = ReadInt32(data, ref position);// Equipment visuals
					Int32 material2 = ReadInt32(data, ref position);// Equipment visuals
					
					Int32 material12 = ReadInt32(data, ref position);// Equipment visuals
					Int32 material22 = ReadInt32(data, ref position);// Equipment visuals
					Int32 material32 = ReadInt32(data, ref position);// Equipment visuals
					Int32 material42 = ReadInt32(data, ref position);// Equipment visuals
				}
				float runspeed = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);// Speed when running
				byte invis = ReadInt8(data, ref position);// Invis (0=not, 1=invis)
				byte afk = ReadInt8(data, ref position);// 0=no, 1=afk
				Int32 guildID = ReadInt32(data, ref position);// Current guild
				string title = ReadFixedLengthString(data, ref position, 32);// Title
				string suffix3 = ReadFixedLengthString(data, ref position, 8);// Player's suffix (of Veeshan, etc.)
				byte helm = ReadInt8(data, ref position);// Helm texture
				Int32 race = ReadInt32(data, ref position);// Spawn race
				string lastName = ReadFixedLengthString(data, ref position, 32);// Player's Lastname
				float walkspeed = BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32(data, ref position)), 0);// Speed when walking
				byte is_pet = ReadInt8(data, ref position);// 0=no, 1=yes
				byte light = ReadInt8(data, ref position);// Spawn's lightsource %%% wrong
				byte class_ = ReadInt8(data, ref position);// Player's class
				byte eyecolor2 = ReadInt8(data, ref position);// Left eye color
				byte flymode = ReadInt8(data, ref position);// Fly
				byte gender = ReadInt8(data, ref position);// Gender (0=male, 1=female)
				byte bodytype = ReadInt8(data, ref position);// Bodytype
				byte Equipchest2 = ReadInt8(data, ref position);// Second place in packet for chest texture (usually 0xFF in live packets)// Not sure why there are 2 of them, but it effects chest texture!// drogmor: 0=white, 1=black, 2=green, 3=red// horse: 0=brown, 1=white, 2=black, 3=tan

				string suffix4 = ReadFixedLengthString(data, ref position, 3);// Player's suffix (of Veeshan, etc.)
				byte IsMercenary = ReadInt8(data, ref position);// 0=no, 1=yes

				int n = 0;
				for(n = 0; n < 4; n++)
				{
					byte bodytype1 = ReadInt8(data, ref position);// Bodytype
					byte bodytype2 = ReadInt8(data, ref position);// Bodytype
					byte bodytype3 = ReadInt8(data, ref position);// Bodytype
					byte bodytype4 = ReadInt8(data, ref position);// Bodytype
					byte bodytype5 = ReadInt8(data, ref position);// Bodytype
					byte bodytype6 = ReadInt8(data, ref position);// Bodytype
					byte bodytype7 = ReadInt8(data, ref position);// Bodytype
					byte bodytype8 = ReadInt8(data, ref position);// Bodytype
					byte bodytype9 = ReadInt8(data, ref position);// Bodytype
					
					
				}
				byte lfg = ReadInt8(data, ref position);// 0=off, 1=lfg on
				byte gm = ReadInt8(data, ref position);// 0=no, 1=gm
				byte aatitle = ReadInt8(data, ref position);// 0=none, 1=general, 2=archtype, 3=class
				byte anon = ReadInt8(data, ref position);// 0=normal, 1=anon, 2=roleplay
				byte face = ReadInt8(data, ref position);// Face id for players
				byte DestructibleObject = ReadInt8(data, ref position);// Only used to flag as a destrible object
				string DestructibleModel = ReadFixedLengthString(data, ref position, 64);// Model of the Destructible Object - Required - Seen "DEST_TNT_G"
				string DestructibleName2 = ReadFixedLengthString(data, ref position, 64);// Secondary name - Not Required - Seen "a_tent"
				string DestructibleString = ReadFixedLengthString(data, ref position, 64);// Unknown - Not Required - Seen "ZoneActor_01186"
				Int32 DestructibleAppearance = ReadInt32(data, ref position);// Damage Appearance
				Int32 DestructibleUnk1 = ReadInt32(data, ref position);
				Int32 DestructibleID1 = ReadInt32(data, ref position);
				Int32 DestructibleID2 = ReadInt32(data, ref position);
				Int32 DestructibleID3 = ReadInt32(data, ref position);
				Int32 DestructibleID4 = ReadInt32(data, ref position);
				Int32 DestructibleUnk2 = ReadInt32(data, ref position);
				Int32 DestructibleUnk3 = ReadInt32(data, ref position);
				Int32 DestructibleUnk4 = ReadInt32(data, ref position);
				Int32 DestructibleUnk5 = ReadInt32(data, ref position);
				Int32 DestructibleUnk6 = ReadInt32(data, ref position);
				Int32 DestructibleUnk7 = ReadInt32(data, ref position);
				byte DestructibleUnk8 = ReadInt8(data, ref position);
				Int32 DestructibleUnk9 = ReadInt32(data, ref position);
				byte targetable_with_hotkey = ReadInt8(data, ref position);

				switch (race)
				{
					case 1:
						if(NPC == 0){ObjectPool.instance.GetObjectForType("elf",true,-x,z,y,spawnId,race,name,heading,deity,size,NPC,curHp,max_hp,level,gender);}
						else{ObjectPool.instance.GetObjectForType("GnollPrefab",true,-x,z,y,spawnId,race,name,heading,deity,size,NPC,curHp,max_hp,level,gender);}
						break;
					case 22:
						ObjectPool.instance.GetObjectForType("SpiderPrefab",true,-x,z,y,spawnId,race,name,heading,deity,size,NPC,curHp,max_hp,level,gender);
						break;
					case 34:
						ObjectPool.instance.GetObjectForType("BatPrefab",true,-x,z,y,spawnId,race,name,heading,deity,size,NPC,curHp,max_hp,level,gender);
						break;						
					case 36:
//						ObjectPool.instance.GetObjectForType("RatPrefab",true,-x,z,y,spawnId,race,name,heading,deity,size,NPC,curHp,max_hp,level,gender);
						ObjectPool.instance.GetObjectForType("SkeletonPrefab",true,-x,z,y,spawnId,race,name,heading,deity,size,NPC,curHp,max_hp,level,gender);						
						break;
					case 37:
//						ObjectPool.instance.GetObjectForType("SnakePrefab",true,-x,z,y,spawnId,race,name,heading,deity,size,NPC,curHp,max_hp,level,gender);
						ObjectPool.instance.GetObjectForType("SkeletonPrefab",true,-x,z,y,spawnId,race,name,heading,deity,size,NPC,curHp,max_hp,level,gender);						
						break;							
					case 39:
						ObjectPool.instance.GetObjectForType("GnollPrefab",true,-x,z,y,spawnId,race,name,heading,deity,size,NPC,curHp,max_hp,level,gender);
						break;
					case 60:
						ObjectPool.instance.GetObjectForType("SkeletonPrefab",true,-x,z,y,spawnId,race,name,heading,deity,size,NPC,curHp,max_hp,level,gender);
						break;
					default:
						ObjectPool.instance.GetObjectForType("GnollPrefab",true,-x,z,y,spawnId,race,name,heading,deity,size,NPC,curHp,max_hp,level,gender);
						break;
				}
			}
		}
//end		
	 
	}
}