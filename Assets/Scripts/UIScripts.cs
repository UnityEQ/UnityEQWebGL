using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;


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
	public GameObject Help;
	public GameObject Camp;
	public GameObject InventoryWindow;
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
	
	public Text profileName;
	public RectTransform OurHP;
	public Text HPText;
	
	public WorldConnect WorldConnection2;
	
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

		Inventory.GetComponent<Button>().onClick.AddListener(delegate { InventoryClick (param2); });
		Help.GetComponent<Button>().onClick.AddListener(delegate { HelpClick (param2); });
		Camp.GetComponent<Button>().onClick.AddListener(delegate { CampClick (param2); });
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
		if (InventoryWindow.activeSelf == false) 
		{
			InventoryWindow.SetActive (true);
			InventoryText.color = Color.green;
		} 
		else 
		{
			InventoryWindow.SetActive (false);
			InventoryText.color = Color.white;
		}
	}

	public void HelpClick(string param2)
	{
//		Debug.Log("UPDATEPOSITION");
//		WorldConnection2.DoClientUpdate();
//		WorldConnection2.curZoneId = 4;
//		WorldConnection2.DoZoneChange(WorldConnection2.ourPlayerName);
	}
	public void CampClick(string param2)
	{
//		Debug.Log("CLACK");
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
		InventoryWindow.SetActive (false);
		profileName.text = WorldConnection2.ourPlayerName;
		TargetBox.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	}
}
}