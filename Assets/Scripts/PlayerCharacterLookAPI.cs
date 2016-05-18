using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EQBrowser
{

public class PlayerCharacterLookAPI : CharacterLookAPI
{
  public int ActiveBoots;
  public int ActiveLeggings;
  //public int ActiveBelt;
  public int ActiveChest;
  public int ActiveArm;
  public int ActiveBracers;
  public int ActiveGloves;
  public int ActiveNeck;

  
  int _ActiveBoots = 0;
  Material ft0001;
  Material ft0002;
  int _ActiveLeggings = 0;
  Material lg0001;
  Material lg0002;
  Material lg0003;
  //int _ActiveBelt = 0;
  int _ActiveChest = 0;
  Material ch0001;
  int _ActiveArm = 0;
  Material ua0001;
  int _ActiveBracers = 0;
  Material fa0001;
  int _ActiveGloves = 0;
  Material hn0001;
  Material hn0002;
  int _ActiveNeck = 0;
  Material ch0002_neck;
  string materialPrefix = "FIX";

   public override void Start()
  {
    base.Start();

    #region cache body parts
    foreach (Material item in _BodyMaterials)
    {
      string materialName = item.name;
      if (materialName.Length < 9)
        continue;

      string part = materialName.Substring(3, 2);
      string sub = materialName.Substring(5, 4);

      if (part == "FT")
      {
        if (sub == "0001")
          ft0001 = item;
        else if (sub == "0002")
          ft0002 = item;
      }
      else if (part == "HN")
      {
        if (sub == "0001")
          hn0001 = item;
        else if (sub == "0002")
          hn0002 = item;
      }
      else if (part == "LG")
      {
        if (sub == "0001")
          lg0001 = item;
        else if (sub == "0002")
          lg0002 = item;
        else if (sub == "0003")
          lg0003 = item;
      }
      else if (part == "CH")
      {
        if (sub == "0001")
        {
          ch0001 = item; //chest armor
          materialPrefix = materialName.Substring(0, 3);
        }
        else if (sub == "0002")
          ch0002_neck = item; //neck
      }
      else if (part == "FA")
        fa0001 = item;
      else if (part == "UA")
        ua0001 = item;
    }
    #endregion
  }

  public override void Update()
  {
    bool voltBodyUpdate = false;

    if (Body != null)
    {
      #region body parts update
      if (_ActiveBoots != ActiveBoots)
      {
        _ActiveBoots = ActiveBoots;
        ft0001.SetTexture("_MainTex", Resources.Load<Texture>(string.Format("{2}{0}FT{1:00}01", materialPrefix, ActiveBoots, CharacterTexturesAreHere)));
        ft0002.SetTexture("_MainTex", Resources.Load<Texture>(string.Format("{2}{0}FT{1:00}02", materialPrefix, ActiveBoots, CharacterTexturesAreHere)));
        voltBodyUpdate = true;
      }
      if (_ActiveLeggings != ActiveLeggings)
      {
        _ActiveLeggings = ActiveLeggings;
        lg0001.SetTexture("_MainTex", Resources.Load<Texture>(string.Format("{2}{0}LG{1:00}01", materialPrefix, ActiveLeggings, CharacterTexturesAreHere)));
        lg0002.SetTexture("_MainTex", Resources.Load<Texture>(string.Format("{2}{0}LG{1:00}02", materialPrefix, ActiveLeggings, CharacterTexturesAreHere)));
        lg0003.SetTexture("_MainTex", Resources.Load<Texture>(string.Format("{2}{0}LG{1:00}03", materialPrefix, ActiveLeggings, CharacterTexturesAreHere)));
        voltBodyUpdate = true;
      }
      if (_ActiveChest != ActiveChest)
      {
        _ActiveChest = ActiveChest;
        ch0001.SetTexture("_MainTex", Resources.Load<Texture>(string.Format("{2}{0}CH{1:00}01", materialPrefix, ActiveChest, CharacterTexturesAreHere)));
        voltBodyUpdate = true;
      }
      if (_ActiveArm != ActiveArm)
      {
        _ActiveArm = ActiveArm;
        ua0001.SetTexture("_MainTex", Resources.Load<Texture>(string.Format("{2}{0}UA{1:00}01", materialPrefix, ActiveArm, CharacterTexturesAreHere)));
        voltBodyUpdate = true;
      }
      if (_ActiveBracers != ActiveBracers)
      {
        _ActiveBracers = ActiveBracers;
        fa0001.SetTexture("_MainTex", Resources.Load<Texture>(string.Format("{2}{0}FA{1:00}01", materialPrefix, ActiveBracers, CharacterTexturesAreHere)));
        voltBodyUpdate = true;
      }
      if (_ActiveGloves != ActiveGloves)
      {
        _ActiveGloves = ActiveGloves;
        hn0001.SetTexture("_MainTex", Resources.Load<Texture>(string.Format("{2}{0}HN{1:00}01", materialPrefix, ActiveGloves, CharacterTexturesAreHere)));
        hn0002.SetTexture("_MainTex", Resources.Load<Texture>(string.Format("{2}{0}HN{1:00}02", materialPrefix, ActiveGloves, CharacterTexturesAreHere)));
        voltBodyUpdate = true;
      }
      if (_ActiveNeck != ActiveNeck)
      {
        _ActiveNeck = ActiveNeck;
        ch0002_neck.SetTexture("_MainTex", Resources.Load<Texture>(string.Format("{2}{0}CH{1:00}02", materialPrefix, ActiveNeck, CharacterTexturesAreHere)));
        voltBodyUpdate = true;
      }
      #endregion
    }

    if (voltBodyUpdate)
    {
      #region reset "active texture set" after bodyparts
      float osszeg = _ActiveBoots + _ActiveArm + _ActiveBracers + _ActiveChest + _ActiveGloves + _ActiveLeggings + _ActiveNeck;
      float atlag = osszeg / 7f;
      if (atlag == Mathf.Floor(atlag))
      {
      TextureSet = (int)atlag;
        _TextureSet = TextureSet; //már volt update, ne legyen ismét
      }
      else
      {
        TextureSet = -1; //nem szet
      }
      #endregion
    }

    base.Update();
  }

  public override void ChangeTextureSet()
  {
    base.ChangeTextureSet();

    if (TextureSet > -1)
    {
      ActiveArm = TextureSet;
      ActiveBoots = TextureSet;
      ActiveBracers = TextureSet;
      ActiveChest = TextureSet;
      ActiveGloves = TextureSet;
      ActiveLeggings = TextureSet;
      ActiveNeck = TextureSet;

      _ActiveArm = TextureSet;
      _ActiveBoots = TextureSet;
      _ActiveBracers = TextureSet;
      _ActiveChest = TextureSet;
      _ActiveGloves = TextureSet;
      _ActiveLeggings = TextureSet;
      _ActiveNeck = TextureSet;
    }
  }
}
}