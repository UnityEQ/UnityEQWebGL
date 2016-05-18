using UnityEngine;
using System.Collections;

public class Spinning : MonoBehaviour
{
  public float Speed = -50f;

  // Use this for initialization
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    this.transform.Rotate(0, Time.deltaTime * Speed, 0);
  }
}
