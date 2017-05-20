using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections.Generic;
using System.Text;
using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

namespace EQBrowser
{
public class UIScripts : MonoBehaviour {

	public Text MainText;
	public Text AbilitiesText;
	public Text CombatText;
	public Text SocialText;
	public Text InventoryText;

	public Text TargetName;
	public RectTransform TargetHP;
	public Text TargetHPText;
	
	public GameObject DesktopPanel;
	public GameObject MobilePanel;
	
	public GameObject LosePanel;
	public GameObject LeftPanel;
	public GameObject CenterPanel;
	public GameObject RightPanel;

	public GameObject TargetBox;
	public GameObject Main;
	public GameObject Abilities;
	public GameObject Combat;
	public GameObject @Social;
	public GameObject MainPanel;
	public GameObject AbilitiesPanel;
	public GameObject CombatPanel;
	public GameObject SocialPanel;

	public GameObject Inventory;
	public GameObject mInventory;
	public GameObject LootBox;
	public GameObject LootDone;
	public GameObject Sit;
	public GameObject Stand;
	public GameObject Help;
	public GameObject Camp;
	public GameObject mCamp;
	public GameObject InventoryWindow;
	public GameObject MoveInventoryWindow;
	public GameObject SpellGem1;
	public GameObject SpellGem2;
	public GameObject SpellGem3;
	public GameObject SpellGem4;
	public GameObject SpellGem5;
	public GameObject SpellGem6;
	public GameObject SpellGem7;
	public GameObject SpellGem8;
	
	public GameObject Attack;
	
	public GameObject ChatText;
	public GameObject ChatTextInput;
	
	public GameObject mChatBox;
	public GameObject ExpandChatText;
	public GameObject MinimizeChatText;
	public GameObject ExpandChatButton;
	
	public GameObject JoyStickPanel;
	
	public Text profileName;
	public RectTransform OurHP;
	public Text HPText;
	
	public Text inventoryName;
	public Text inventoryLevel;
	public Text inventoryClass;
	public Text inventoryExp;
	public Text inventoryAc;
	public Text inventoryAtk;
	public Text inventoryStrength;
	public Text inventoryStamina;
	public Text inventoryCharisma;
	public Text inventoryDexterity;
	public Text inventoryIntellect;
	public Text inventoryAgility;
	public Text inventoryWisdom;
	public Text inventoryCurHp;
	public Text inventoryMaxHp;
	public Text inventoryPlatinum;
	public Text inventoryGold;
	public Text inventorySilver;
	public Text inventoryCopper;
	
	public List<GameObject> slotList;
	public List<GameObject> bagList;
	public List<GameObject> equipList;
	
	
	public WorldConnect WorldConnection2;
	public Draggable draggable;
	
	
	public static bool CastButton = false;
	

	
	public void setupBtn2()
	{
		string param2 = "clack";
		MainPanel.SetActive(true);	
		AbilitiesPanel.SetActive(false);
		CombatPanel.SetActive(false);
		SocialPanel.SetActive(false);

		Attack.GetComponent<Button>().onClick.AddListener(delegate { AttackClick(param2); });
		
		Main.GetComponent<Button>().onClick.AddListener(delegate { MainClick(param2); });
		Abilities.GetComponent<Button>().onClick.AddListener(delegate { AbilitiesClick(param2); });
		Combat.GetComponent<Button>().onClick.AddListener(delegate { CombatClick(param2); });
		@Social.GetComponent<Button>().onClick.AddListener(delegate { SocialClick (param2); });

		Inventory.GetComponent<Button>().onClick.AddListener(delegate { InventoryClick ("0"); });
		mInventory.GetComponent<Button>().onClick.AddListener(delegate { InventoryClick ("1"); });
		Sit.GetComponent<Button>().onClick.AddListener(delegate { SitClick (param2); });
		Stand.GetComponent<Button>().onClick.AddListener(delegate { StandClick (param2); });
		Help.GetComponent<Button>().onClick.AddListener(delegate { HelpClick (param2); });
		LootDone.GetComponent<Button>().onClick.AddListener(delegate { LootDoneClick (param2); });
		Camp.GetComponent<Button>().onClick.AddListener(delegate { CampClick (param2); });
		mCamp.GetComponent<Button>().onClick.AddListener(delegate { CampClick (param2); });
		ExpandChatButton.GetComponent<Button>().onClick.AddListener(delegate { ExpandChatButtonClick (param2); });
		SpellGem1.GetComponent<Button>().onClick.AddListener(delegate { SpellGem1Click (param2); });
		SpellGem2.GetComponent<Button>().onClick.AddListener(delegate { SpellGem2Click (param2); });
		SpellGem3.GetComponent<Button>().onClick.AddListener(delegate { SpellGem3Click (param2); });
		SpellGem4.GetComponent<Button>().onClick.AddListener(delegate { SpellGem4Click (param2); });
		SpellGem5.GetComponent<Button>().onClick.AddListener(delegate { SpellGem5Click (param2); });
		SpellGem6.GetComponent<Button>().onClick.AddListener(delegate { SpellGem6Click (param2); });
		SpellGem7.GetComponent<Button>().onClick.AddListener(delegate { SpellGem7Click (param2); });
		SpellGem8.GetComponent<Button>().onClick.AddListener(delegate { SpellGem8Click (param2); });

	}
	public void SpellGem1Click(string param2)
	{
		UIScripts.CastButton = true;
	}
	public void SpellGem2Click(string param2)
	{
		UIScripts.CastButton = true;
	}
	public void SpellGem3Click(string param2)
	{
		UIScripts.CastButton = true;
	}
	public void SpellGem4Click(string param2)
	{
		UIScripts.CastButton = true;
	}
	public void SpellGem5Click(string param2)
	{
		UIScripts.CastButton = true;
	}
	public void SpellGem6Click(string param2)
	{
		UIScripts.CastButton = true;
	}
	public void SpellGem7Click(string param2)
	{
		UIScripts.CastButton = true;
	}
	public void SpellGem8Click(string param2)
	{
		UIScripts.CastButton = true;
	}

	public void InventoryClick(string param2)
	{
		
//		switch(int.Parse(param2))
//		{
//			case 0:
				if (InventoryWindow.activeSelf == false) 
				{
					InventoryWindow.SetActive (true);
					InventoryText.color = Color.green;
				} 
				else 
				{
					InventoryWindow.SetActive (false);
					MoveInventoryWindow.GetComponent<Draggable>().Reset(InventoryWindow);
					InventoryText.color = Color.white;
				}
//			break;
			
//			case 1:
//				if (mInventoryWindow.activeSelf == false) 
//				{
//					mInventoryWindow.SetActive (true);
//					mInventoryText.color = Color.green;
//				} 
//				else 
//				{
//					mInventoryWindow.SetActive (false);
//					MoveInventoryWindow.GetComponent<Draggable>().Reset(InventoryWindow);
//					mInventoryText.color = Color.white;
//				}
//			break;
			
//			default:
//				break;
//		}
	}
	
	public void ExpandChatButtonClick(string param2)
	{
		if (mChatBox.activeSelf == false) 
		{
			mChatBox.SetActive (true);
			ExpandChatText.SetActive (false);
			MinimizeChatText.SetActive (true);
		} 
		else 
		{
			mChatBox.SetActive (false);
			ExpandChatText.SetActive (true);
			MinimizeChatText.SetActive (false);			
		}
	}

	public void SitClick(string param2)
	{
		WorldConnection2.DoSit();
		Stand.SetActive(true);
		Sit.SetActive(false);
	}

	public void StandClick(string param2)
	{
		WorldConnection2.DoStand();
		Stand.SetActive(false);
		Sit.SetActive(true);
	}

	public void LootDoneClick(string param2)
	{
		WorldConnection2.DoEndLoot();
		LootBox.SetActive(false);
		WorldConnection2.isLooting = false;
	}
	
	public void HelpClick(string param2)
	{
		WorldConnection2.DoEndLoot();
	}

	public void CampClick(string param2)
	{
		WorldConnection2.DoLogOut();
	}
	
	public void AttackClick(string param2)
	{
		Debug.Log("poop");
		switch(WorldConnection2.isAttacking)
		{
			case 0:
				WorldConnection2.DoAttack(1);
				break;
			case 1:
				WorldConnection2.DoAttack(0);
				break;
			default:
				break;
		}
//		if(WorldConnection2.isAttacking == 0){WorldConnection2.DoAttack(1);}
//		if(WorldConnection2.isAttacking == 1){WorldConnection2.DoAttack(0);}
	}

	public void InventoryClick2(string param2)
	{
		InventoryWindow.SetActive (false);	
	}
	public void MainClick(string param2)
	{
		MainPanel.SetActive (true);	
		AbilitiesPanel.SetActive (false);
		CombatPanel.SetActive (false);
		SocialPanel.SetActive (false);

		MainText.color = Color.green;
		AbilitiesText.color = Color.black;
		CombatText.color = Color.black;
		SocialText.color = Color.black;
		
	}
	public void AbilitiesClick(string param2)
	{
		MainPanel.SetActive (false);	
		AbilitiesPanel.SetActive (true);
		CombatPanel.SetActive (false);
		SocialPanel.SetActive (false);

		MainText.color = Color.black;
		AbilitiesText.color = Color.green;
		CombatText.color = Color.black;
		SocialText.color = Color.black;
		
	}
	public void CombatClick(string param2)
	{
		MainPanel.SetActive (false);	
		AbilitiesPanel.SetActive (false);
		CombatPanel.SetActive (true);
		SocialPanel.SetActive (false);

		MainText.color = Color.black;
		AbilitiesText.color = Color.black;
		CombatText.color = Color.green;
		SocialText.color = Color.black;
		
	}
	public void SocialClick(string param2)
	{
		MainPanel.SetActive (false);	
		AbilitiesPanel.SetActive (false);
		CombatPanel.SetActive (false);
		SocialPanel.SetActive (true);

		MainText.color = Color.black;
		AbilitiesText.color = Color.black;
		CombatText.color = Color.black;
		SocialText.color = Color.green;
	}
	// Use this for initialization
	void Start () {
		setupBtn2 ();
		MainText.color = Color.green;
		profileName.text = WorldConnection2.ourPlayerName;
		TargetBox.SetActive(false);
		
		#if UNITY_EDITOR || UNITY_WEBGL || UNITY_STANDALONE
			DesktopPanel.SetActive(true);
		#endif
		
		#if UNITY_IOS || UNITY_ANDROID || UNITY_WP_8_1
			MobilePanel.SetActive(true);
		#endif
		
	}
	
		
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown("1") && WorldConnection2.isTyping == false)
		AttackClick("hotkey1");
	
		if(Input.GetKeyDown(KeyCode.I) && WorldConnection2.isTyping == false)
		InventoryClick("clack");
	
	
	}
}
}