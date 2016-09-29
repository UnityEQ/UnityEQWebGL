using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EQBrowser;

public class CharacterSelect : MonoBehaviour {
	//charselect

	public GameObject CCPanel0;
	public GameObject CCPanel1;
	public GameObject CCPanel2;
	public GameObject CCPanel3;
	public GameObject CButton1;
	public GameObject CButton2;
	public GameObject CButton3;
	public GameObject CButton4;
	public GameObject CButton5;
	public GameObject CButton6;
	public GameObject CButton7;
	public GameObject CButton8;
	
	public GameObject SaltyServer;
	public GameObject Server2Server;
	public Text SaltyServerText;
	public Text Server2ServerText;
	
	public Int32 _ClassSelection;
	public Int32 _RaceSelection;
	public Int32 _DeitySelection;

	public Int32 ZoneSelection;
	
	public Int32 GenderSelection;
	
	public Int32 EyeColorSelection;
	public Int32 BeardSelection;
	public Int32 HairSelection;

	public Int32 TutorialSelection;

	public Int32 STR;
	public Int32 STA;
	public Int32 DEX;
	public Int32 AGI;
	public Int32 INT;
	public Int32 WIS;
	public Int32 CHA;

	public Text CButton1Text;
	public Text CButton2Text;
	public Text CButton3Text;
	public Text CButton4Text;
	public Text CButton5Text;
	public Text CButton6Text;
	public Text CButton7Text;
	public Text CButton8Text;

	public GameObject CEnter;
	public GameObject CQuit;
	public GameObject CDelete;
	//raceselect
	public GameObject Back1;
	public GameObject Next1;
	public GameObject Back2;
	public GameObject Next2;
	public GameObject Male;
	public Text MaleText;
	public GameObject Female;
	public Text FemaleText;
	public GameObject Barbarian;
	public Text BarbarianText;
	public GameObject DarkElf;
	public Text DarkElfText;
	public GameObject Dwarf;
	public Text DwarfText;
	public GameObject Erudite;
	public Text EruditeText;
	public GameObject Gnome;
	public Text GnomeText;
	public GameObject HalfElf;
	public Text HalfElfText;
	public GameObject Halfling;
	public Text HalflingText;
	public GameObject HighElf;
	public Text HighElfText;
	public GameObject Human;
	public Text HumanText;
	public GameObject Iksar;
	public Text IksarText;
	public GameObject Ogre;
	public Text OgreText;
	public GameObject Troll;
	public Text TrollText;
	public GameObject WoodElf;
	public Text WoodElfText;
	//classselect
	public GameObject Bard;
	public Text BardText;
	public GameObject Cleric;
	public Text ClericText;
	public GameObject Druid;
	public Text DruidText;
	public GameObject Enchanter;
	public Text EnchanterText;
	public GameObject Magician;
	public Text MagicianText;
	public GameObject Monk;
	public Text MonkText;
	public GameObject Necromancer;
	public Text NecromancerText;
	public GameObject Paladin;
	public Text PaladinText;
	public GameObject Ranger;
	public Text RangerText;
	public GameObject Rogue;
	public Text RogueText;
	public GameObject ShadowKnight;
	public Text ShadowKnightText;
	public GameObject Shaman;
	public Text ShamanText;
	public GameObject Warrior;
	public Text WarriorText;
	public GameObject Wizard;
	public Text WizardText;

	//dietyselect
	public GameObject Agnostic;
	public Text AgnosticText;
	public GameObject Bertoxxulous;
	public Text BertoxxulousText;
	public GameObject Bristlebane;
	public Text BristlebaneText;
	public GameObject CazicThule;
	public Text CazicThuleText;
	public GameObject Innoruuk;
	public Text InnoruukText;
	public GameObject Karana;
	public Text KaranaText;
	public GameObject Erollisi;
	public Text ErollisiText;
	public GameObject Mithaniel;
	public Text MithanielText;
	public GameObject Rodcet;
	public Text RodcetText;
	public GameObject Prexus;
	public Text PrexusText;
	public GameObject Quellious;
	public Text QuelliousText;
	public GameObject Solusek;
	public Text SolusekText;
	public GameObject Brell;
	public Text BrellText;
	public GameObject Tribunal;
	public Text TribunalText;
	public GameObject Tunare;
	public Text TunareText;
	public GameObject Veeshan;
	public Text VeeshanText;
	public GameObject Rallos;
	public Text RallosText;
	//cityselect
	public GameObject Erudin;
	public Text ErudinText;
	public GameObject Qeynos;
	public Text QeynosText;
	public GameObject Halas;
	public Text HalasText;
	public GameObject Rivervale;
	public Text RivervaleText;
	public GameObject Freeport;
	public Text FreeportText;
	public GameObject Neriak;
	public Text NeriakText;
	public GameObject Grobb;
	public Text GrobbText;
	public GameObject Oggok;
	public Text OggokText;
	public GameObject Kaladim;
	public Text KaladimText;
	public GameObject Kelethin;
	public Text KelethinText;
	public GameObject Felwithe;
	public Text FelwitheText;
	public GameObject Akanon;
	public Text AkanonText;
	public GameObject Cabilis;
	public Text CabilisText;

	//For naming a character.
	public InputField CreationName;
	public Text CreationStatus;
	
	//Login Panel
	public InputField UserNameInput;
	public InputField PasswordInput;
	public InputField HostNameInput;
	public Button LoginButton;
	public Text LoginStatus;
	public WorldConnect WorldConnection;
	public PlayerCharacterLookAPI CharLookAPI;
	
	public int CharSelected;
	
	public void setupBtn()
	{
		string param = "clack";
		CCPanel0.SetActive(true);
		CCPanel1.SetActive(false);
		CCPanel2.SetActive(false);
		CCPanel3.SetActive(false);
		
		


		LoginStatus.text = "Waiting to connect...";

		SaltyServer.GetComponent<Button>().onClick.AddListener(delegate { SaltyClicked(param); });
		Server2Server.GetComponent<Button>().onClick.AddListener(delegate { Server2Clicked(param); });

		
		CButton1.GetComponent<Button>().onClick.AddListener(delegate { Cbtn1Clicked(param); });
		CButton2.GetComponent<Button>().onClick.AddListener(delegate { Cbtn2Clicked(param); });
		CButton3.GetComponent<Button>().onClick.AddListener(delegate { Cbtn3Clicked(param); });
		CButton4.GetComponent<Button>().onClick.AddListener(delegate { Cbtn4Clicked(param); });
		CButton5.GetComponent<Button>().onClick.AddListener(delegate { Cbtn5Clicked(param); });
		CButton6.GetComponent<Button>().onClick.AddListener(delegate { Cbtn6Clicked(param); });
		CButton7.GetComponent<Button>().onClick.AddListener(delegate { Cbtn7Clicked(param); });
		CButton8.GetComponent<Button>().onClick.AddListener(delegate { Cbtn8Clicked(param); });
		CEnter.GetComponent<Button>().onClick.AddListener(delegate { CEnterClicked(param); });
		CQuit.GetComponent<Button>().onClick.AddListener(delegate { QbtnClicked(param); });
		CDelete.GetComponent<Button>().onClick.AddListener(delegate { DbtnClicked(param); });

		Back1.GetComponent<Button>().onClick.AddListener(delegate { Back1Click(param); });
		Next1.GetComponent<Button>().onClick.AddListener(delegate { Next1Click(param); });

		Back2.GetComponent<Button>().onClick.AddListener(delegate { Back2Click(param); });
		Next2.GetComponent<Button>().onClick.AddListener(delegate { Next2Click(param); });

		Male.GetComponent<Button>().onClick.AddListener(delegate { MaleClick(param); });
		Female.GetComponent<Button>().onClick.AddListener(delegate { FemaleClick(param); });
		//race
		Barbarian.GetComponent<Button>().onClick.AddListener(delegate { BarbarianClick(param); });
		DarkElf.GetComponent<Button>().onClick.AddListener(delegate { DarkElfClick(param); });
		Dwarf.GetComponent<Button>().onClick.AddListener(delegate { DwarfClick(param); });
		Erudite.GetComponent<Button>().onClick.AddListener(delegate { EruditeClick(param); });
		Gnome.GetComponent<Button>().onClick.AddListener(delegate { GnomeClick(param); });
		HalfElf.GetComponent<Button>().onClick.AddListener(delegate { HalfElfClick(param); });
		Halfling.GetComponent<Button>().onClick.AddListener(delegate { HalflingClick(param); });
		HighElf.GetComponent<Button>().onClick.AddListener(delegate { HighElfClick(param); });
		Human.GetComponent<Button>().onClick.AddListener(delegate { HumanClick(param); });
		Iksar.GetComponent<Button>().onClick.AddListener(delegate { IksarClick(param); });
		Ogre.GetComponent<Button>().onClick.AddListener(delegate { OgreClick(param); });
		Troll.GetComponent<Button>().onClick.AddListener(delegate { TrollClick(param); });
		WoodElf.GetComponent<Button>().onClick.AddListener(delegate { WoodElfClick(param); });
		//class
		Bard.GetComponent<Button>().onClick.AddListener(delegate { BardClick(param); });
		Cleric.GetComponent<Button>().onClick.AddListener(delegate { ClericClick(param); });
		Druid.GetComponent<Button>().onClick.AddListener(delegate { DruidClick(param); });
		Enchanter.GetComponent<Button>().onClick.AddListener(delegate { EnchanterClick(param); });
		Magician.GetComponent<Button>().onClick.AddListener(delegate { MagicianClick(param); });
		Monk.GetComponent<Button>().onClick.AddListener(delegate { MonkClick(param); });
		Necromancer.GetComponent<Button>().onClick.AddListener(delegate { NecromancerClick(param); });
		Paladin.GetComponent<Button>().onClick.AddListener(delegate { PaladinClick(param); });
		Ranger.GetComponent<Button>().onClick.AddListener(delegate { RangerClick(param); });
		Rogue.GetComponent<Button>().onClick.AddListener(delegate { RogueClick(param); });
		ShadowKnight.GetComponent<Button>().onClick.AddListener(delegate { ShadowKnightClick(param); });
		Shaman.GetComponent<Button>().onClick.AddListener(delegate { ShamanClick(param); });
		Warrior.GetComponent<Button>().onClick.AddListener(delegate { WarriorClick(param); });	
		Wizard.GetComponent<Button>().onClick.AddListener(delegate { WizardClick(param); });
		//deity
		Agnostic.GetComponent<Button>().onClick.AddListener(delegate { AgnosticClick(param); });
		Bertoxxulous.GetComponent<Button>().onClick.AddListener(delegate { BertoxxulousClick(param); });
		Bristlebane.GetComponent<Button>().onClick.AddListener(delegate { BristlebaneClick(param); });
		CazicThule.GetComponent<Button>().onClick.AddListener(delegate { CazicThuleClick(param); });
		Innoruuk.GetComponent<Button>().onClick.AddListener(delegate { InnoruukClick(param); });
		Karana.GetComponent<Button>().onClick.AddListener(delegate { KaranaClick(param); });
		Erollisi.GetComponent<Button>().onClick.AddListener(delegate { ErollisiClick(param); });
		Mithaniel.GetComponent<Button>().onClick.AddListener(delegate { MithanielClick(param); });
		Rodcet.GetComponent<Button>().onClick.AddListener(delegate { RodcetClick(param); });
		Prexus.GetComponent<Button>().onClick.AddListener(delegate { PrexusClick(param); });
		Quellious.GetComponent<Button>().onClick.AddListener(delegate { QuelliousClick(param); });
		Solusek.GetComponent<Button>().onClick.AddListener(delegate { SolusekClick(param); });
		Brell.GetComponent<Button>().onClick.AddListener(delegate { BrellClick(param); });
		Tribunal.GetComponent<Button>().onClick.AddListener(delegate { TribunalClick(param); });
		Tunare.GetComponent<Button>().onClick.AddListener(delegate { TunareClick(param); });
		Veeshan.GetComponent<Button>().onClick.AddListener(delegate { VeeshanClick(param); });
		Rallos.GetComponent<Button>().onClick.AddListener(delegate { RallosClick(param); });
		//city
		Erudin.GetComponent<Button>().onClick.AddListener(delegate { ErudinClick(param); });
		Qeynos.GetComponent<Button>().onClick.AddListener(delegate { QeynosClick(param); });
		Halas.GetComponent<Button>().onClick.AddListener(delegate { HalasClick(param); });
		Rivervale.GetComponent<Button>().onClick.AddListener(delegate { RivervaleClick(param); });
		Freeport.GetComponent<Button>().onClick.AddListener(delegate { FreeportClick(param); });
		Neriak.GetComponent<Button>().onClick.AddListener(delegate { NeriakClick(param); });
		Grobb.GetComponent<Button>().onClick.AddListener(delegate { GrobbClick(param); });
		Oggok.GetComponent<Button>().onClick.AddListener(delegate { OggokClick(param); });
		Kaladim.GetComponent<Button>().onClick.AddListener(delegate { KaladimClick(param); });
		Kelethin.GetComponent<Button>().onClick.AddListener(delegate { KelethinClick(param); });
		Felwithe.GetComponent<Button>().onClick.AddListener(delegate { FelwitheClick(param); });
		Akanon.GetComponent<Button>().onClick.AddListener(delegate { AkanonClick(param); });
		Cabilis.GetComponent<Button>().onClick.AddListener(delegate { CabilisClick(param); });
		LoginButton.onClick.AddListener(delegate { LoginButtonClicked(param); });
	}
	
	public void ResetCharCreate()
	{
		CreationStatus.color = Color.white;
		CreationStatus.text = "Please enter a character name.";
		CreationName.text = "";
	}

	public void UpdateCharButtonText(int index, string name) 
	{
		if (index == 0)
			CButton1Text.text = name;
		else if (index == 1)
			CButton2Text.text = name;
		else if (index == 2)
			CButton3Text.text = name;
		else if (index == 3)
			CButton4Text.text = name;
		else if (index == 4)
			CButton5Text.text = name;
		else if (index == 5)
			CButton6Text.text = name;
		else if (index == 6)
			CButton7Text.text = name;
		else if (index == 7)
			CButton8Text.text = name;
	}

	public void ClearCharButtonText() 
	{
		string ClearTo = "Create a New Character";
		CButton1Text.text = ClearTo;
		CButton2Text.text = ClearTo;
		CButton3Text.text = ClearTo;
		CButton4Text.text = ClearTo;
		CButton5Text.text = ClearTo;
		CButton6Text.text = ClearTo;
		CButton7Text.text = ClearTo;
		CButton8Text.text = ClearTo;
	}

	public void LoginButtonClicked(string param)
	{
		LoginStatus.text = "Connecting to " + HostNameInput.text + "...";
		if (UserNameInput.text.Length > 0 && PasswordInput.text.Length > 0 && HostNameInput.text.Length > 0)
		{
//			string token = "aksdjlka23ij3l1j23lk1j23j123jkjql";
			string token = UserNameInput.text;
			StartCoroutine(WorldConnection.ConnectToWebSocketServer (HostNameInput.text, token, UserNameInput.text, PasswordInput.text));
		}
		else
		{
			LoginStatus.text = "Please enter credentials.";
		}
	}

	public void SaltyClicked(string param)
	{
		//		Debug.Log("foo " + param);
		SaltyServerText.color = Color.red;
		Server2ServerText.color = Color.black;
		HostNameInput.text = "158.69.221.200:80";
//		HostNameInput.text = "68.83.231.103:52685";
		
		#if UNITY_EDITOR
			//for dev quick login
			UserNameInput.text = "saltyx";
			PasswordInput.text = "saltyx";
		#endif
	}
	
	public void Server2Clicked(string param)
	{
		//		Debug.Log("foo " + param);
		SaltyServerText.color = Color.black;
		Server2ServerText.color = Color.red;
		HostNameInput.text = "128.0.0.1:80";
		UserNameInput.text = "xxx";
		PasswordInput.text = "xxx";
	}
	
	public void BackToLogin()
	{
		//		Debug.Log("foo " + param);
		CCPanel1.SetActive(false);	
		CCPanel2.SetActive(false);	
		CCPanel3.SetActive(false);
		CCPanel0.SetActive(true);
		ClearCharButtonText();
		if (WorldConnection.ws_ != null && WorldConnection.ws_.Error != null)
			WorldConnection.ws_.Close ();
		LoginStatus.text = "You have been disconnected.";
	}

	public void ToCharList()
	{
		//		Debug.Log("foo " + param);
		CCPanel1.SetActive(true);	
		CCPanel2.SetActive(false);	
		CCPanel3.SetActive(false);
		CCPanel0.SetActive(false);
	}

	public void QbtnClicked(string param)
	{
		//		Debug.Log("foo " + param);
		WorldConnection.ws_.Close();
		BackToLogin ();


	}
	
	public void DbtnClicked(string param)
	{
		//		Debug.Log("foo " + param);
		switch (CharSelected)
		{
			case 1:
				WorldConnection.DoDeleteChar(CButton1Text.text);
				break;
			case 2:
				WorldConnection.DoDeleteChar(CButton2Text.text);
				break;
			case 3:
				WorldConnection.DoDeleteChar(CButton3Text.text);
				break;
			case 4:
				WorldConnection.DoDeleteChar(CButton4Text.text);
				break;
			case 5:
				WorldConnection.DoDeleteChar(CButton5Text.text);
				break;
			case 6:
				WorldConnection.DoDeleteChar(CButton6Text.text);
				break;
			case 7:
				WorldConnection.DoDeleteChar(CButton7Text.text);
				break;
			case 8:
				WorldConnection.DoDeleteChar(CButton8Text.text);
				break;
			default:
				break;
		}		
	}
	
	public void CEnterClicked(string param)
	{
		switch (CharSelected)
		{
			case 1:
				WorldConnection.curZoneId = 2;
				WorldConnection.DoEnterWorld(CButton1Text.text);
				break;
			case 2:
				WorldConnection.curZoneId = 2;
				WorldConnection.DoEnterWorld(CButton2Text.text);
				break;
			case 3:
				WorldConnection.curZoneId = 2;
				WorldConnection.DoEnterWorld(CButton3Text.text);
				break;
			case 4:
				WorldConnection.curZoneId = 2;
				WorldConnection.DoEnterWorld(CButton4Text.text);
				break;
			case 5:
				WorldConnection.curZoneId = 2;
				WorldConnection.DoEnterWorld(CButton5Text.text);
				break;
			case 6:
				WorldConnection.curZoneId = 2;
				WorldConnection.DoEnterWorld(CButton6Text.text);
				break;
			case 7:
				WorldConnection.curZoneId = 2;
				WorldConnection.DoEnterWorld(CButton7Text.text);
				break;
			case 8:
				WorldConnection.curZoneId = 2;
				WorldConnection.DoEnterWorld(CButton8Text.text);
				break;
			default:
				break;
		}
	}


	public void Cbtn1Clicked(string param)
	{
//		Debug.Log("foo " + param);

		if (CButton1Text.text == "Create a New Character") {
			CCPanel1.SetActive (false);	
			CCPanel2.SetActive (true);	
			CCPanel3.SetActive (false);
			CCPanel0.SetActive (false);
			ResetCharCreate();
		} else {
			CharSelected = 1;
		}
	}

	public void Cbtn2Clicked(string param)
	{
		//		Debug.Log("foo " + param);
		
		if (CButton2Text.text == "Create a New Character") {
			CCPanel1.SetActive (false);	
			CCPanel2.SetActive (true);	
			CCPanel3.SetActive (false);
			CCPanel0.SetActive (false);
			ResetCharCreate();
		} else {
			CharSelected = 2;
//			WorldConnection.DoEnterWorld(CButton2Text.text);
		}
	}

	public void Cbtn3Clicked(string param)
	{
		//		Debug.Log("foo " + param);
		
		if (CButton3Text.text == "Create a New Character") {
			CCPanel1.SetActive (false);	
			CCPanel2.SetActive (true);	
			CCPanel3.SetActive (false);
			CCPanel0.SetActive (false);
			ResetCharCreate();
		} else {
			CharSelected = 3;
//			WorldConnection.DoEnterWorld(CButton3Text.text);
		}
	}

	public void Cbtn4Clicked(string param)
	{
		//		Debug.Log("foo " + param);
		
		if (CButton4Text.text == "Create a New Character") {
			CCPanel1.SetActive (false);	
			CCPanel2.SetActive (true);	
			CCPanel3.SetActive (false);
			CCPanel0.SetActive (false);
			ResetCharCreate();
		} else {
			CharSelected = 4;
//			WorldConnection.DoEnterWorld(CButton4Text.text);
		}
	}

	public void Cbtn5Clicked(string param)
	{
		//		Debug.Log("foo " + param);
		
		if (CButton5Text.text == "Create a New Character") {
			CCPanel1.SetActive (false);	
			CCPanel2.SetActive (true);	
			CCPanel3.SetActive (false);
			CCPanel0.SetActive (false);
			ResetCharCreate();
		} else {
			CharSelected = 5;
//			WorldConnection.DoEnterWorld(CButton5Text.text);
		}
	}

	public void Cbtn6Clicked(string param)
	{
		//		Debug.Log("foo " + param);
		
		if (CButton6Text.text == "Create a New Character") {
			CCPanel1.SetActive (false);	
			CCPanel2.SetActive (true);	
			CCPanel3.SetActive (false);
			CCPanel0.SetActive (false);
			ResetCharCreate();
		} else {
			CharSelected = 6;
//			WorldConnection.DoEnterWorld(CButton6Text.text);
		}
	}

	public void Cbtn7Clicked(string param)
	{
		//		Debug.Log("foo " + param);
		
		if (CButton7Text.text == "Create a New Character") {
			CCPanel1.SetActive (false);	
			CCPanel2.SetActive (true);	
			CCPanel3.SetActive (false);
			CCPanel0.SetActive (false);
			ResetCharCreate();
		} else {
			CharSelected = 7;
//			WorldConnection.DoEnterWorld(CButton7Text.text);
		}
	}

	public void Cbtn8Clicked(string param)
	{
		//		Debug.Log("foo " + param);
		
		if (CButton8Text.text == "Create a New Character") {
			CCPanel1.SetActive (false);	
			CCPanel2.SetActive (true);	
			CCPanel3.SetActive (false);
			CCPanel0.SetActive (false);
			ResetCharCreate();
		} else {
			CharSelected = 8;
//			WorldConnection.DoEnterWorld(CButton8Text.text);
		}
	}

	public void Back1Click(string param)
	{
		//		Debug.Log("foo " + param);
		CCPanel1.SetActive(true);	
		CCPanel2.SetActive(false);	
		CCPanel3.SetActive(false);
		CCPanel0.SetActive(false);
	}
	public void Next1Click(string param)
	{
		//		Debug.Log("foo " + param);
		CCPanel1.SetActive(true);	
		CCPanel2.SetActive(false);	
		CCPanel3.SetActive(false);
		CCPanel0.SetActive(false);
		WorldConnection.DoNameApproval();
	}
	public void Back2Click(string param)
	{
		//		Debug.Log("foo " + param);
		CCPanel1.SetActive(false);	
		CCPanel2.SetActive(true);	
		CCPanel3.SetActive(false);
		CCPanel0.SetActive(false);
		
	}
	public void Next2Click(string param)
	{
		//		Debug.Log("foo " + param);
		CCPanel1.SetActive(true);	
		CCPanel2.SetActive(false);	
		CCPanel3.SetActive(false);
		CCPanel0.SetActive(false);
		WorldConnection.DoNameApproval();
	}
	public void MaleClick(string param)
	{
		//GenderSelection = 0;
		MaleText.color = Color.green;
		FemaleText.color = Color.white;
	}
	public void FemaleClick(string param)
	{
		//GenderSelection = 1;
		FemaleText.color = Color.green;
		MaleText.color = Color.white;
	}
	public void BarbarianClick(string param)
	{
		//_RaceSelection = 2;
		BarbarianText.color = Color.green;
		DarkElfText.color = Color.white;
		DwarfText.color = Color.white;
		EruditeText.color = Color.white;
		GnomeText.color = Color.white;
		HalfElfText.color = Color.white;
		HalflingText.color = Color.white;
		HighElfText.color = Color.white;
		HumanText.color = Color.white;
		IksarText.color = Color.white;
		OgreText.color = Color.white;
		TrollText.color = Color.white;
		WoodElfText.color = Color.white;

	}
	public void DarkElfClick(string param)
	{
		//_RaceSelection = 6;
		BarbarianText.color = Color.white;
		DarkElfText.color = Color.green;
		DwarfText.color = Color.white;
		EruditeText.color = Color.white;
		GnomeText.color = Color.white;
		HalfElfText.color = Color.white;
		HalflingText.color = Color.white;
		HighElfText.color = Color.white;
		HumanText.color = Color.white;
		IksarText.color = Color.white;
		OgreText.color = Color.white;
		TrollText.color = Color.white;
		WoodElfText.color = Color.white;
	}
	public void DwarfClick(string param)
	{
		//_RaceSelection = 8;
		BarbarianText.color = Color.white;
		DarkElfText.color = Color.white;
		DwarfText.color = Color.green;
		EruditeText.color = Color.white;
		GnomeText.color = Color.white;
		HalfElfText.color = Color.white;
		HalflingText.color = Color.white;
		HighElfText.color = Color.white;
		HumanText.color = Color.white;
		IksarText.color = Color.white;
		OgreText.color = Color.white;
		TrollText.color = Color.white;
		WoodElfText.color = Color.white;
	}
	public void EruditeClick(string param)
	{
		//_RaceSelection = 3;
		BarbarianText.color = Color.white;
		DarkElfText.color = Color.white;
		DwarfText.color = Color.white;
		EruditeText.color = Color.green;
		GnomeText.color = Color.white;
		HalfElfText.color = Color.white;
		HalflingText.color = Color.white;
		HighElfText.color = Color.white;
		HumanText.color = Color.white;
		IksarText.color = Color.white;
		OgreText.color = Color.white;
		TrollText.color = Color.white;
		WoodElfText.color = Color.white;
	}
	public void GnomeClick(string param)
	{
		//_RaceSelection = 12;
		BarbarianText.color = Color.white;
		DarkElfText.color = Color.white;
		DwarfText.color = Color.white;
		EruditeText.color = Color.white;
		GnomeText.color = Color.green;
		HalfElfText.color = Color.white;
		HalflingText.color = Color.white;
		HighElfText.color = Color.white;
		HumanText.color = Color.white;
		IksarText.color = Color.white;
		OgreText.color = Color.white;
		TrollText.color = Color.white;
		WoodElfText.color = Color.white;
	}
	public void HalfElfClick(string param)
	{
		//_RaceSelection = 7;
		BarbarianText.color = Color.white;
		DarkElfText.color = Color.white;
		DwarfText.color = Color.white;
		EruditeText.color = Color.white;
		GnomeText.color = Color.white;
		HalfElfText.color = Color.green;
		HalflingText.color = Color.white;
		HighElfText.color = Color.white;
		HumanText.color = Color.white;
		IksarText.color = Color.white;
		OgreText.color = Color.white;
		TrollText.color = Color.white;
		WoodElfText.color = Color.white;
	}
	public void HalflingClick(string param)
	{
		//_RaceSelection = 11;
		BarbarianText.color = Color.white;
		DarkElfText.color = Color.white;
		DwarfText.color = Color.white;
		EruditeText.color = Color.white;
		GnomeText.color = Color.white;
		HalfElfText.color = Color.white;
		HalflingText.color = Color.green;
		HighElfText.color = Color.white;
		HumanText.color = Color.white;
		IksarText.color = Color.white;
		OgreText.color = Color.white;
		TrollText.color = Color.white;
		WoodElfText.color = Color.white;
	}
	public void HighElfClick(string param)
	{
		//_RaceSelection = 5;
		BarbarianText.color = Color.white;
		DarkElfText.color = Color.white;
		DwarfText.color = Color.white;
		EruditeText.color = Color.white;
		GnomeText.color = Color.white;
		HalfElfText.color = Color.white;
		HalflingText.color = Color.white;
		HighElfText.color = Color.green;
		HumanText.color = Color.white;
		IksarText.color = Color.white;
		OgreText.color = Color.white;
		TrollText.color = Color.white;
		WoodElfText.color = Color.white;
	}
	public void HumanClick(string param)
	{
		//_RaceSelection = 1;
		BarbarianText.color = Color.white;
		DarkElfText.color = Color.white;
		DwarfText.color = Color.white;
		EruditeText.color = Color.white;
		GnomeText.color = Color.white;
		HalfElfText.color = Color.white;
		HalflingText.color = Color.white;
		HighElfText.color = Color.white;
		HumanText.color = Color.green;
		IksarText.color = Color.white;
		OgreText.color = Color.white;
		TrollText.color = Color.white;
		WoodElfText.color = Color.white;
	}
	public void IksarClick(string param)
	{
		//_RaceSelection = 128;
		BarbarianText.color = Color.white;
		DarkElfText.color = Color.white;
		DwarfText.color = Color.white;
		EruditeText.color = Color.white;
		GnomeText.color = Color.white;
		HalfElfText.color = Color.white;
		HalflingText.color = Color.white;
		HighElfText.color = Color.white;
		HumanText.color = Color.white;
		IksarText.color = Color.green;
		OgreText.color = Color.white;
		TrollText.color = Color.white;
		WoodElfText.color = Color.white;
	}
	public void OgreClick(string param)
	{
		//_RaceSelection = 10;
		BarbarianText.color = Color.white;
		DarkElfText.color = Color.white;
		DwarfText.color = Color.white;
		EruditeText.color = Color.white;
		GnomeText.color = Color.white;
		HalfElfText.color = Color.white;
		HalflingText.color = Color.white;
		HighElfText.color = Color.white;
		HumanText.color = Color.white;
		IksarText.color = Color.white;
		OgreText.color = Color.green;
		TrollText.color = Color.white;
		WoodElfText.color = Color.white;
	}
	public void TrollClick(string param)
	{
		//_RaceSelection = 9;
		BarbarianText.color = Color.white;
		DarkElfText.color = Color.white;
		DwarfText.color = Color.white;
		EruditeText.color = Color.white;
		GnomeText.color = Color.white;
		HalfElfText.color = Color.white;
		HalflingText.color = Color.white;
		HighElfText.color = Color.white;
		HumanText.color = Color.white;
		IksarText.color = Color.white;
		OgreText.color = Color.white;
		TrollText.color = Color.green;
		WoodElfText.color = Color.white;
	}
	public void WoodElfClick(string param)
	{
		//_RaceSelection = 4;
		BarbarianText.color = Color.white;
		DarkElfText.color = Color.white;
		DwarfText.color = Color.white;
		EruditeText.color = Color.white;
		GnomeText.color = Color.white;
		HalfElfText.color = Color.white;
		HalflingText.color = Color.white;
		HighElfText.color = Color.white;
		HumanText.color = Color.white;
		IksarText.color = Color.white;
		OgreText.color = Color.white;
		TrollText.color = Color.white;
		WoodElfText.color = Color.green;
	}


	public void BardClick(string param)
	{
		//_ClassSelection = 8;
		BardText.color = Color.green;
		ClericText.color = Color.white;
		DruidText.color = Color.white;
		EnchanterText.color = Color.white;
		MagicianText.color = Color.white;
		MonkText.color = Color.white;
		NecromancerText.color = Color.white;
		PaladinText.color = Color.white;
		RangerText.color = Color.white;
		RogueText.color = Color.white;
		ShadowKnightText.color = Color.white;
		ShamanText.color = Color.white;
		WarriorText.color = Color.white;
		WizardText.color = Color.white;
	}
	public void ClericClick(string param)
	{
		//_ClassSelection = 2;
		BardText.color = Color.white;
		ClericText.color = Color.green;
		DruidText.color = Color.white;
		EnchanterText.color = Color.white;
		MagicianText.color = Color.white;
		MonkText.color = Color.white;
		NecromancerText.color = Color.white;
		PaladinText.color = Color.white;
		RangerText.color = Color.white;
		RogueText.color = Color.white;
		ShadowKnightText.color = Color.white;
		ShamanText.color = Color.white;
		WarriorText.color = Color.white;
		WizardText.color = Color.white;
	}
	public void DruidClick(string param)
	{
		//_ClassSelection = 6;
		BardText.color = Color.white;
		ClericText.color = Color.white;
		DruidText.color = Color.green;
		EnchanterText.color = Color.white;
		MagicianText.color = Color.white;
		MonkText.color = Color.white;
		NecromancerText.color = Color.white;
		PaladinText.color = Color.white;
		RangerText.color = Color.white;
		RogueText.color = Color.white;
		ShadowKnightText.color = Color.white;
		ShamanText.color = Color.white;
		WarriorText.color = Color.white;
		WizardText.color = Color.white;
	}
	public void EnchanterClick(string param)
	{
		//_ClassSelection = 14;
		BardText.color = Color.white;
		ClericText.color = Color.white;
		DruidText.color = Color.white;
		EnchanterText.color = Color.green;
		MagicianText.color = Color.white;
		MonkText.color = Color.white;
		NecromancerText.color = Color.white;
		PaladinText.color = Color.white;
		RangerText.color = Color.white;
		RogueText.color = Color.white;
		ShadowKnightText.color = Color.white;
		ShamanText.color = Color.white;
		WarriorText.color = Color.white;
		WizardText.color = Color.white;
	}
	public void MagicianClick(string param)
	{
		//_ClassSelection = 13;
		BardText.color = Color.white;
		ClericText.color = Color.white;
		DruidText.color = Color.white;
		EnchanterText.color = Color.white;
		MagicianText.color = Color.green;
		MonkText.color = Color.white;
		NecromancerText.color = Color.white;
		PaladinText.color = Color.white;
		RangerText.color = Color.white;
		RogueText.color = Color.white;
		ShadowKnightText.color = Color.white;
		ShamanText.color = Color.white;
		WarriorText.color = Color.white;
		WizardText.color = Color.white;
	}
	public void MonkClick(string param)
	{
		//_ClassSelection = 7;
		BardText.color = Color.white;
		ClericText.color = Color.white;
		DruidText.color = Color.white;
		EnchanterText.color = Color.white;
		MagicianText.color = Color.white;
		MonkText.color = Color.green;
		NecromancerText.color = Color.white;
		PaladinText.color = Color.white;
		RangerText.color = Color.white;
		RogueText.color = Color.white;
		ShadowKnightText.color = Color.white;
		ShamanText.color = Color.white;
		WarriorText.color = Color.white;
		WizardText.color = Color.white;
	}
	public void NecromancerClick(string param)
	{
		//_ClassSelection = 11;
		BardText.color = Color.white;
		ClericText.color = Color.white;
		DruidText.color = Color.white;
		EnchanterText.color = Color.white;
		MagicianText.color = Color.white;
		MonkText.color = Color.white;
		NecromancerText.color = Color.green;
		PaladinText.color = Color.white;
		RangerText.color = Color.white;
		RogueText.color = Color.white;
		ShadowKnightText.color = Color.white;
		ShamanText.color = Color.white;
		WarriorText.color = Color.white;
		WizardText.color = Color.white;
	}
	public void PaladinClick(string param)
	{
		//_ClassSelection = 3;
		BardText.color = Color.white;
		ClericText.color = Color.white;
		DruidText.color = Color.white;
		EnchanterText.color = Color.white;
		MagicianText.color = Color.white;
		MonkText.color = Color.white;
		NecromancerText.color = Color.white;
		PaladinText.color = Color.green;
		RangerText.color = Color.white;
		RogueText.color = Color.white;
		ShadowKnightText.color = Color.white;
		ShamanText.color = Color.white;
		WarriorText.color = Color.white;
		WizardText.color = Color.white;
	}
	public void RangerClick(string param)
	{
		//_ClassSelection = 4;
		BardText.color = Color.white;
		ClericText.color = Color.white;
		DruidText.color = Color.white;
		EnchanterText.color = Color.white;
		MagicianText.color = Color.white;
		MonkText.color = Color.white;
		NecromancerText.color = Color.white;
		PaladinText.color = Color.white;
		RangerText.color = Color.green;
		RogueText.color = Color.white;
		ShadowKnightText.color = Color.white;
		ShamanText.color = Color.white;
		WarriorText.color = Color.white;
		WizardText.color = Color.white;
	}
	public void RogueClick(string param)
	{
		//_ClassSelection = 9;
		BardText.color = Color.white;
		ClericText.color = Color.white;
		DruidText.color = Color.white;
		EnchanterText.color = Color.white;
		MagicianText.color = Color.white;
		MonkText.color = Color.white;
		NecromancerText.color = Color.white;
		PaladinText.color = Color.white;
		RangerText.color = Color.white;
		RogueText.color = Color.green;
		ShadowKnightText.color = Color.white;
		ShamanText.color = Color.white;
		WarriorText.color = Color.white;
		WizardText.color = Color.white;
	}
	public void ShadowKnightClick(string param)
	{
		//_ClassSelection = 5;
		BardText.color = Color.white;
		ClericText.color = Color.white;
		DruidText.color = Color.white;
		EnchanterText.color = Color.white;
		MagicianText.color = Color.white;
		MonkText.color = Color.white;
		NecromancerText.color = Color.white;
		PaladinText.color = Color.white;
		RangerText.color = Color.white;
		RogueText.color = Color.white;
		ShadowKnightText.color = Color.green;
		ShamanText.color = Color.white;
		WarriorText.color = Color.white;
		WizardText.color = Color.white;
	}
	public void ShamanClick(string param)
	{
		//_ClassSelection = 10;
		BardText.color = Color.white;
		ClericText.color = Color.white;
		DruidText.color = Color.white;
		EnchanterText.color = Color.white;
		MagicianText.color = Color.white;
		MonkText.color = Color.white;
		NecromancerText.color = Color.white;
		PaladinText.color = Color.white;
		RangerText.color = Color.white;
		RogueText.color = Color.white;
		ShadowKnightText.color = Color.white;
		ShamanText.color = Color.green;
		WarriorText.color = Color.white;
		WizardText.color = Color.white;
	}
	public void WarriorClick(string param)
	{
		//_ClassSelection = 1;
		BardText.color = Color.white;
		ClericText.color = Color.white;
		DruidText.color = Color.white;
		EnchanterText.color = Color.white;
		MagicianText.color = Color.white;
		MonkText.color = Color.white;
		NecromancerText.color = Color.white;
		PaladinText.color = Color.white;
		RangerText.color = Color.white;
		RogueText.color = Color.white;
		ShadowKnightText.color = Color.white;
		ShamanText.color = Color.white;
		WarriorText.color = Color.green;
		WizardText.color = Color.white;
	}
	public void WizardClick(string param)
	{
		//_ClassSelection = 12;
		BardText.color = Color.white;
		ClericText.color = Color.white;
		DruidText.color = Color.white;
		EnchanterText.color = Color.white;
		MagicianText.color = Color.white;
		MonkText.color = Color.white;
		NecromancerText.color = Color.white;
		PaladinText.color = Color.white;
		RangerText.color = Color.white;
		RogueText.color = Color.white;
		ShadowKnightText.color = Color.white;
		ShamanText.color = Color.white;
		WarriorText.color = Color.white;
		WizardText.color = Color.green;
	}
	public void AgnosticClick (string param)
	{
		//_DeitySelection = 140;
		AgnosticText.color = Color.green;
		BertoxxulousText.color = Color.white;
		BristlebaneText.color = Color.white;
		CazicThuleText.color = Color.white;
		InnoruukText.color = Color.white;
		KaranaText.color = Color.white;
		ErollisiText.color = Color.white;
		MithanielText.color = Color.white;
		RodcetText.color = Color.white;
		PrexusText.color = Color.white;
		QuelliousText.color = Color.white;
		SolusekText.color = Color.white;
		BrellText.color = Color.white;
		TribunalText.color = Color.white;
		TunareText.color = Color.white;
		VeeshanText.color = Color.white;
		RallosText.color = Color.white;
	}
	public void BertoxxulousClick (string param)
	{
		//_DeitySelection = 201;
		AgnosticText.color = Color.white;
		BertoxxulousText.color = Color.green;
		BristlebaneText.color = Color.white;
		CazicThuleText.color = Color.white;
		InnoruukText.color = Color.white;
		KaranaText.color = Color.white;
		ErollisiText.color = Color.white;
		MithanielText.color = Color.white;
		RodcetText.color = Color.white;
		PrexusText.color = Color.white;
		QuelliousText.color = Color.white;
		SolusekText.color = Color.white;
		BrellText.color = Color.white;
		TribunalText.color = Color.white;
		TunareText.color = Color.white;
		VeeshanText.color = Color.white;
		RallosText.color = Color.white;
	}
	public void BristlebaneClick (string param)
	{
		//_DeitySelection = 205;
		AgnosticText.color = Color.white;
		BertoxxulousText.color = Color.white;
		BristlebaneText.color = Color.green;
		CazicThuleText.color = Color.white;
		InnoruukText.color = Color.white;
		KaranaText.color = Color.white;
		ErollisiText.color = Color.white;
		MithanielText.color = Color.white;
		RodcetText.color = Color.white;
		PrexusText.color = Color.white;
		QuelliousText.color = Color.white;
		SolusekText.color = Color.white;
		BrellText.color = Color.white;
		TribunalText.color = Color.white;
		TunareText.color = Color.white;
		VeeshanText.color = Color.white;
		RallosText.color = Color.white;
	}
	public void CazicThuleClick (string param)
	{
		//_DeitySelection = 203;
		AgnosticText.color = Color.white;
		BertoxxulousText.color = Color.white;
		BristlebaneText.color = Color.white;
		CazicThuleText.color = Color.green;
		InnoruukText.color = Color.white;
		KaranaText.color = Color.white;
		ErollisiText.color = Color.white;
		MithanielText.color = Color.white;
		RodcetText.color = Color.white;
		PrexusText.color = Color.white;
		QuelliousText.color = Color.white;
		SolusekText.color = Color.white;
		BrellText.color = Color.white;
		TribunalText.color = Color.white;
		TunareText.color = Color.white;
		VeeshanText.color = Color.white;
		RallosText.color = Color.white;
	}
	public void InnoruukClick (string param)
	{
		//_DeitySelection = 206;
		AgnosticText.color = Color.white;
		BertoxxulousText.color = Color.white;
		BristlebaneText.color = Color.white;
		CazicThuleText.color = Color.white;
		InnoruukText.color = Color.green;
		KaranaText.color = Color.white;
		ErollisiText.color = Color.white;
		MithanielText.color = Color.white;
		RodcetText.color = Color.white;
		PrexusText.color = Color.white;
		QuelliousText.color = Color.white;
		SolusekText.color = Color.white;
		BrellText.color = Color.white;
		TribunalText.color = Color.white;
		TunareText.color = Color.white;
		VeeshanText.color = Color.white;
		RallosText.color = Color.white;
	}
	public void KaranaClick (string param)
	{
		//_DeitySelection = 207;
		AgnosticText.color = Color.white;
		BertoxxulousText.color = Color.white;
		BristlebaneText.color = Color.white;
		CazicThuleText.color = Color.white;
		InnoruukText.color = Color.white;
		KaranaText.color = Color.green;
		ErollisiText.color = Color.white;
		MithanielText.color = Color.white;
		RodcetText.color = Color.white;
		PrexusText.color = Color.white;
		QuelliousText.color = Color.white;
		SolusekText.color = Color.white;
		BrellText.color = Color.white;
		TribunalText.color = Color.white;
		TunareText.color = Color.white;
		VeeshanText.color = Color.white;
		RallosText.color = Color.white;
	}
	public void ErollisiClick (string param)
	{
		//_DeitySelection = 204;
		AgnosticText.color = Color.white;
		BertoxxulousText.color = Color.white;
		BristlebaneText.color = Color.white;
		CazicThuleText.color = Color.white;
		InnoruukText.color = Color.white;
		KaranaText.color = Color.white;
		ErollisiText.color = Color.green;
		MithanielText.color = Color.white;
		RodcetText.color = Color.white;
		PrexusText.color = Color.white;
		QuelliousText.color = Color.white;
		SolusekText.color = Color.white;
		BrellText.color = Color.white;
		TribunalText.color = Color.white;
		TunareText.color = Color.white;
		VeeshanText.color = Color.white;
		RallosText.color = Color.white;
	}
	public void MithanielClick (string param)
	{

		//_DeitySelection = 208;
		AgnosticText.color = Color.white;
		BertoxxulousText.color = Color.white;
		BristlebaneText.color = Color.white;
		CazicThuleText.color = Color.white;
		InnoruukText.color = Color.white;
		KaranaText.color = Color.white;
		ErollisiText.color = Color.white;
		MithanielText.color = Color.green;
		RodcetText.color = Color.white;
		PrexusText.color = Color.white;
		QuelliousText.color = Color.white;
		SolusekText.color = Color.white;
		BrellText.color = Color.white;
		TribunalText.color = Color.white;
		TunareText.color = Color.white;
		VeeshanText.color = Color.white;
		RallosText.color = Color.white;
	}
	public void RodcetClick (string param)
	{
		//_DeitySelection = 212;
		AgnosticText.color = Color.white;
		BertoxxulousText.color = Color.white;
		BristlebaneText.color = Color.white;
		CazicThuleText.color = Color.white;
		InnoruukText.color = Color.white;
		KaranaText.color = Color.white;
		ErollisiText.color = Color.white;
		MithanielText.color = Color.white;
		RodcetText.color = Color.green;
		PrexusText.color = Color.white;
		QuelliousText.color = Color.white;
		SolusekText.color = Color.white;
		BrellText.color = Color.white;
		TribunalText.color = Color.white;
		TunareText.color = Color.white;
		VeeshanText.color = Color.white;
		RallosText.color = Color.white;
	}
	public void PrexusClick (string param)
	{
		//_DeitySelection = 209;
		AgnosticText.color = Color.white;
		BertoxxulousText.color = Color.white;
		BristlebaneText.color = Color.white;
		CazicThuleText.color = Color.white;
		InnoruukText.color = Color.white;
		KaranaText.color = Color.white;
		ErollisiText.color = Color.white;
		MithanielText.color = Color.white;
		RodcetText.color = Color.white;
		PrexusText.color = Color.green;
		QuelliousText.color = Color.white;
		SolusekText.color = Color.white;
		BrellText.color = Color.white;
		TribunalText.color = Color.white;
		TunareText.color = Color.white;
		VeeshanText.color = Color.white;
		RallosText.color = Color.white;
	}
	public void QuelliousClick (string param)
	{
		//_DeitySelection = 210;
		AgnosticText.color = Color.white;
		BertoxxulousText.color = Color.white;
		BristlebaneText.color = Color.white;
		CazicThuleText.color = Color.white;
		InnoruukText.color = Color.white;
		KaranaText.color = Color.white;
		ErollisiText.color = Color.white;
		MithanielText.color = Color.white;
		RodcetText.color = Color.white;
		PrexusText.color = Color.white;
		QuelliousText.color = Color.green;
		SolusekText.color = Color.white;
		BrellText.color = Color.white;
		TribunalText.color = Color.white;
		TunareText.color = Color.white;
		VeeshanText.color = Color.white;
		RallosText.color = Color.white;
	}
	public void SolusekClick (string param)
	{
		//_DeitySelection = 213;
		AgnosticText.color = Color.white;
		BertoxxulousText.color = Color.white;
		BristlebaneText.color = Color.white;
		CazicThuleText.color = Color.white;
		InnoruukText.color = Color.white;
		KaranaText.color = Color.white;
		ErollisiText.color = Color.white;
		MithanielText.color = Color.white;
		RodcetText.color = Color.white;
		PrexusText.color = Color.white;
		QuelliousText.color = Color.white;
		SolusekText.color = Color.green;
		BrellText.color = Color.white;
		TribunalText.color = Color.white;
		TunareText.color = Color.white;
		VeeshanText.color = Color.white;
		RallosText.color = Color.white;
	}
	public void BrellClick (string param)
	{
		//_DeitySelection = 202;
		AgnosticText.color = Color.white;
		BertoxxulousText.color = Color.white;
		BristlebaneText.color = Color.white;
		CazicThuleText.color = Color.white;
		InnoruukText.color = Color.white;
		KaranaText.color = Color.white;
		ErollisiText.color = Color.white;
		MithanielText.color = Color.white;
		RodcetText.color = Color.white;
		PrexusText.color = Color.white;
		QuelliousText.color = Color.white;
		SolusekText.color = Color.white;
		BrellText.color = Color.green;
		TribunalText.color = Color.white;
		TunareText.color = Color.white;
		VeeshanText.color = Color.white;
		RallosText.color = Color.white;
	}
	public void TribunalClick (string param)
	{
		//_DeitySelection = 214;
		AgnosticText.color = Color.white;
		BertoxxulousText.color = Color.white;
		BristlebaneText.color = Color.white;
		CazicThuleText.color = Color.white;
		InnoruukText.color = Color.white;
		KaranaText.color = Color.white;
		ErollisiText.color = Color.white;
		MithanielText.color = Color.white;
		RodcetText.color = Color.white;
		PrexusText.color = Color.white;
		QuelliousText.color = Color.white;
		SolusekText.color = Color.white;
		BrellText.color = Color.white;
		TribunalText.color = Color.green;
		TunareText.color = Color.white;
		VeeshanText.color = Color.white;
		RallosText.color = Color.white;
	}
	public void TunareClick (string param)
	{
		//_DeitySelection = 215;
		AgnosticText.color = Color.white;
		BertoxxulousText.color = Color.white;
		BristlebaneText.color = Color.white;
		CazicThuleText.color = Color.white;
		InnoruukText.color = Color.white;
		KaranaText.color = Color.white;
		ErollisiText.color = Color.white;
		MithanielText.color = Color.white;
		RodcetText.color = Color.white;
		PrexusText.color = Color.white;
		QuelliousText.color = Color.white;
		SolusekText.color = Color.white;
		BrellText.color = Color.white;
		TribunalText.color = Color.white;
		TunareText.color = Color.green;
		VeeshanText.color = Color.white;
		RallosText.color = Color.white;
	}
	public void VeeshanClick (string param)
	{
		//_DeitySelection = 216;
		AgnosticText.color = Color.white;
		BertoxxulousText.color = Color.white;
		BristlebaneText.color = Color.white;
		CazicThuleText.color = Color.white;
		InnoruukText.color = Color.white;
		KaranaText.color = Color.white;
		ErollisiText.color = Color.white;
		MithanielText.color = Color.white;
		RodcetText.color = Color.white;
		PrexusText.color = Color.white;
		QuelliousText.color = Color.white;
		SolusekText.color = Color.white;
		BrellText.color = Color.white;
		TribunalText.color = Color.white;
		TunareText.color = Color.white;
		VeeshanText.color = Color.green;
		RallosText.color = Color.white;
	}
	public void RallosClick (string param)
	{
		//_DeitySelection = 211;
		AgnosticText.color = Color.white;
		BertoxxulousText.color = Color.white;
		BristlebaneText.color = Color.white;
		CazicThuleText.color = Color.white;
		InnoruukText.color = Color.white;
		KaranaText.color = Color.white;
		ErollisiText.color = Color.white;
		MithanielText.color = Color.white;
		RodcetText.color = Color.white;
		PrexusText.color = Color.white;
		QuelliousText.color = Color.white;
		SolusekText.color = Color.white;
		BrellText.color = Color.white;
		TribunalText.color = Color.white;
		TunareText.color = Color.white;
		VeeshanText.color = Color.white;
		RallosText.color = Color.green;
	}
	public void ErudinClick(string param)
	{
		//ZoneSelection = 24;
		ErudinText.color = Color.green;
		QeynosText.color = Color.white;
		HalasText.color = Color.white;
		RivervaleText.color = Color.white;
		FreeportText.color = Color.white;
		NeriakText.color = Color.white;
		GrobbText.color = Color.white;
		OggokText.color = Color.white;
		KaladimText.color = Color.white;
		KelethinText.color = Color.white;
		FelwitheText.color = Color.white;
		AkanonText.color = Color.white;
		CabilisText.color = Color.white;
	}
	public void QeynosClick(string param)
	{
		//ZoneSelection = 2;
		ErudinText.color = Color.white;
		QeynosText.color = Color.green;
		HalasText.color = Color.white;
		RivervaleText.color = Color.white;
		FreeportText.color = Color.white;
		NeriakText.color = Color.white;
		GrobbText.color = Color.white;
		OggokText.color = Color.white;
		KaladimText.color = Color.white;
		KelethinText.color = Color.white;
		FelwitheText.color = Color.white;
		AkanonText.color = Color.white;
		CabilisText.color = Color.white;
	}
	public void HalasClick(string param)
	{
		//ZoneSelection = 29;
		ErudinText.color = Color.white;
		QeynosText.color = Color.white;
		HalasText.color = Color.green;
		RivervaleText.color = Color.white;
		FreeportText.color = Color.white;
		NeriakText.color = Color.white;
		GrobbText.color = Color.white;
		OggokText.color = Color.white;
		KaladimText.color = Color.white;
		KelethinText.color = Color.white;
		FelwitheText.color = Color.white;
		AkanonText.color = Color.white;
		CabilisText.color = Color.white;
	}
	public void RivervaleClick(string param)
	{
		//ZoneSelection = 19;
		ErudinText.color = Color.white;
		QeynosText.color = Color.white;
		HalasText.color = Color.white;
		RivervaleText.color = Color.green;
		FreeportText.color = Color.white;
		NeriakText.color = Color.white;
		GrobbText.color = Color.white;
		OggokText.color = Color.white;
		KaladimText.color = Color.white;
		KelethinText.color = Color.white;
		FelwitheText.color = Color.white;
		AkanonText.color = Color.white;
		CabilisText.color = Color.white;
	}
	public void FreeportClick(string param)
	{
		//ZoneSelection = 9;
		ErudinText.color = Color.white;
		QeynosText.color = Color.white;
		HalasText.color = Color.white;
		RivervaleText.color = Color.white;
		FreeportText.color = Color.green;
		NeriakText.color = Color.white;
		GrobbText.color = Color.white;
		OggokText.color = Color.white;
		KaladimText.color = Color.white;
		KelethinText.color = Color.white;
		FelwitheText.color = Color.white;
		AkanonText.color = Color.white;
		CabilisText.color = Color.white;
	}
	public void NeriakClick(string param)
	{
		//ZoneSelection = 40;
		ErudinText.color = Color.white;
		QeynosText.color = Color.white;
		HalasText.color = Color.white;
		RivervaleText.color = Color.white;
		FreeportText.color = Color.white;
		NeriakText.color = Color.green;
		GrobbText.color = Color.white;
		OggokText.color = Color.white;
		KaladimText.color = Color.white;
		KelethinText.color = Color.white;
		FelwitheText.color = Color.white;
		AkanonText.color = Color.white;
		CabilisText.color = Color.white;
	}
	public void GrobbClick(string param)
	{
		//ZoneSelection = 52;
		ErudinText.color = Color.white;
		QeynosText.color = Color.white;
		HalasText.color = Color.white;
		RivervaleText.color = Color.white;
		FreeportText.color = Color.white;
		NeriakText.color = Color.white;
		GrobbText.color = Color.green;
		OggokText.color = Color.white;
		KaladimText.color = Color.white;
		KelethinText.color = Color.white;
		FelwitheText.color = Color.white;
		AkanonText.color = Color.white;
		CabilisText.color = Color.white;
	}
	public void OggokClick(string param)
	{
		//ZoneSelection = 49;
		ErudinText.color = Color.white;
		QeynosText.color = Color.white;
		HalasText.color = Color.white;
		RivervaleText.color = Color.white;
		FreeportText.color = Color.white;
		NeriakText.color = Color.white;
		GrobbText.color = Color.white;
		OggokText.color = Color.green;
		KaladimText.color = Color.white;
		KelethinText.color = Color.white;
		FelwitheText.color = Color.white;
		AkanonText.color = Color.white;
		CabilisText.color = Color.white;
	}
	public void KaladimClick(string param)
	{
		//ZoneSelection = 60;
		ErudinText.color = Color.white;
		QeynosText.color = Color.white;
		HalasText.color = Color.white;
		RivervaleText.color = Color.white;
		FreeportText.color = Color.white;
		NeriakText.color = Color.white;
		GrobbText.color = Color.white;
		OggokText.color = Color.white;
		KaladimText.color = Color.green;
		KelethinText.color = Color.white;
		FelwitheText.color = Color.white;
		AkanonText.color = Color.white;
		CabilisText.color = Color.white;
	}
	public void KelethinClick(string param)
	{
		//ZoneSelection = 54;
		ErudinText.color = Color.white;
		QeynosText.color = Color.white;
		HalasText.color = Color.white;
		RivervaleText.color = Color.white;
		FreeportText.color = Color.white;
		NeriakText.color = Color.white;
		GrobbText.color = Color.white;
		OggokText.color = Color.white;
		KaladimText.color = Color.white;
		KelethinText.color = Color.green;
		FelwitheText.color = Color.white;
		AkanonText.color = Color.white;
		CabilisText.color = Color.white;
	}
	public void FelwitheClick(string param)
	{
		//ZoneSelection = 61;
		ErudinText.color = Color.white;
		QeynosText.color = Color.white;
		HalasText.color = Color.white;
		RivervaleText.color = Color.white;
		FreeportText.color = Color.white;
		NeriakText.color = Color.white;
		GrobbText.color = Color.white;
		OggokText.color = Color.white;
		KaladimText.color = Color.white;
		KelethinText.color = Color.white;
		FelwitheText.color = Color.green;
		AkanonText.color = Color.white;
		CabilisText.color = Color.white;
	}
	public void AkanonClick(string param)
	{
		//ZoneSelection = 55;
		ErudinText.color = Color.white;
		QeynosText.color = Color.white;
		HalasText.color = Color.white;
		RivervaleText.color = Color.white;
		FreeportText.color = Color.white;
		NeriakText.color = Color.white;
		GrobbText.color = Color.white;
		OggokText.color = Color.white;
		KaladimText.color = Color.white;
		KelethinText.color = Color.white;
		FelwitheText.color = Color.white;
		AkanonText.color = Color.green;
		CabilisText.color = Color.white;
	}
	public void CabilisClick(string param)
	{
		//ZoneSelection = 82;
		ErudinText.color = Color.white;
		QeynosText.color = Color.white;
		HalasText.color = Color.white;
		RivervaleText.color = Color.white;
		FreeportText.color = Color.white;
		NeriakText.color = Color.white;
		GrobbText.color = Color.white;
		OggokText.color = Color.white;
		KaladimText.color = Color.white;
		KelethinText.color = Color.white;
		FelwitheText.color = Color.white;
		AkanonText.color = Color.white;
		CabilisText.color = Color.green;
	}

	public string TidyCase(string sourceStr)
	{
		sourceStr.Trim();
		if (!string.IsNullOrEmpty(sourceStr))
		{
		char[] allCharacters = sourceStr.ToCharArray();
 
			for (int i = 0; i < allCharacters.Length; i++)
			{
				char character = allCharacters[i];
				if (i == 0)
				{
					if (char.IsLower(character))
					{
						allCharacters[i] = char.ToUpper(character);
					}
				}
				else
				{
					if (char.IsUpper(character))
					{
						allCharacters[i] = char.ToLower(character);
					}
				}
			}
			return new string(allCharacters);
		}
		return sourceStr;
	}



	// Use this for initialization
	void Start ()
	{
		CharLookAPI.ActiveHead = 0;
		CharLookAPI.TextureSet = 3;
		Debug.Log("hi");
		setupBtn ();
		_DeitySelection = 396;
		ZoneSelection = 2;
		_ClassSelection = 1;
		_RaceSelection = 1;
		GenderSelection = 1;
		EyeColorSelection = 255;
		BeardSelection = 255;
		HairSelection = 255;
		
		TutorialSelection = 0;
		WoodElfText.color = Color.green;
		FemaleText.color = Color.green;
		DruidText.color = Color.green;
		WorldConnection.isTyping = true;

	}	

	// Update is called once per frame
	void Update ()
	{
	}
}
