using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterLookAPI : MonoBehaviour
{
  protected const string CharacterTexturesAreHere = "Characters/Textures/";

  public GameObject[] Heads;
  public GameObject Body;
  public int ActiveHead;
  public int TextureSet;

  MeshRenderer[] _Heads;
  int _ActiveHead = -1;
  protected Material[] _BodyMaterials;
  protected int _TextureSet = 2;
  

   public virtual void Start()
  {
    #region cache heads
    _Heads = new MeshRenderer[Heads.Length];
    for (int i = 0; i < Heads.Length; i++)
    {
      _Heads[i] = Heads[i].GetComponent<MeshRenderer>();
    }
    #endregion

    #region cache body materials
    if (Body != null)
    {
      _BodyMaterials = Body.GetComponent<SkinnedMeshRenderer>().materials;
    }
    #endregion
  }

  public virtual void Update()
  {
    if (Heads != null)
    {
      #region update active head
      if (_ActiveHead != ActiveHead)
      {
        _ActiveHead = ActiveHead;
        for (int i = 0; i < _Heads.Length; i++)
        {
          _Heads[i].enabled = (i == ActiveHead);
        }
      }
      #endregion
    }

    if (Body != null)
    {
      #region update active texture set (whole body)
      ChangeTextureSet();
      #endregion
    }
  }

  /// <summary>
  /// this will change texture set for the whole body
  /// </summary>
  public virtual void ChangeTextureSet()
  {
    if (_TextureSet != TextureSet)
    {
      _TextureSet = TextureSet;
      if (TextureSet >= 0) //TextureSet == -1 => custom
      {
        foreach (Material material in _BodyMaterials)
        {
          string regiTexturanev = material.GetTexture("_MainTex").name;
          string ujTexturanev = string.Format("{3}{0}{1:00}{2}"
            , regiTexturanev.Substring(0, regiTexturanev.Length - 4)
            , TextureSet
            , regiTexturanev.Substring(regiTexturanev.Length - 2, 2)
            , CharacterTexturesAreHere
            );
          //Debug.Log(ujTexturanev);
          material.SetTexture("_MainTex", Resources.Load<Texture>(ujTexturanev));
        }
      }
    }
  }
}
