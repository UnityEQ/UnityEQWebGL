using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using EQBrowser;
 
public class JoystickScript : MonoBehaviour {
	public GameObject Character;
	public GameObject Camera;
	private CharacterController controller;
	public bool _isMoveup;
	public bool _isMoveback;
	public bool _isStrafeleft;
	public bool _isStraferight;
	public bool _isStrafeleftup;
	public bool _isStraferightup;
	public bool _isTurnleft;
	public bool _isTurnright;
	public bool _isLookup;
	public bool _isLookdown;
	public float speed;
	public float rotatespeed;
	
	public WorldConnect WorldConnection;
	
	public float distanceTravelled = 0;
	public Vector3 lastPosition;

	public float rotationTravelled = 0;
	public Vector3 lastRotation;



	void Start()
	{
			speed = 20.0f;
			rotatespeed = 100.0f;

		controller = Character.GetComponent<CharacterController>();
	}
	void Update () 
	{
		
		distanceTravelled += Vector3.Distance(Character.transform.position, lastPosition);
		lastPosition = Character.transform.position;
				
		rotationTravelled += Vector3.Distance(Character.transform.eulerAngles, lastRotation);
		lastRotation = Character.transform.eulerAngles;
				
		if(distanceTravelled > 10)
		{
			distanceTravelled = 0;
			WorldConnection.DoClientUpdate();
		}
	
		if(rotationTravelled > 20)
		{
			rotationTravelled = 0;
			WorldConnection.DoClientUpdate();
		}
				
				
		if (_isMoveup == true)
		{ 
			controller.SimpleMove(Character.transform.forward*speed);
			//Character.transform.Translate (Vector3.forward);
		}
		if (_isMoveback == true)
		{
			controller.SimpleMove(Character.transform.TransformDirection(Vector3.back)*speed);
			//Character.transform.Translate (Vector3.back);
		}
		if (_isStrafeleft == true)
		{
			controller.SimpleMove(Character.transform.TransformDirection(Vector3.left)*speed);
			//Character.transform.Translate (Vector3.left);
		}
		if (_isStraferight == true)
		{
			controller.SimpleMove(Character.transform.TransformDirection(Vector3.right)*speed);
			//Character.transform.Translate (Vector3.right);
		}
		if (_isStrafeleftup == true)
		{
			controller.SimpleMove(Character.transform.TransformDirection(-1f, 0f, 1f)*speed);
			//Character.transform.Translate (-1f, 0f, 1f);
		}
		if (_isStraferightup == true)
		{
			controller.SimpleMove(Character.transform.TransformDirection(1f, 0f, 1f)*speed);
			//Character.transform.Translate (1f, 0f, 1f);
		}
		
		if (_isTurnleft == true)
		{
			Character.transform.Rotate (0f, -1f *rotatespeed * Time.deltaTime, 0f);
			
		}
		if (_isTurnright == true)
		{
			Character.transform.Rotate (0f, 1f *rotatespeed * Time.deltaTime, 0f);
			
		}
		if (_isLookup == true)
		{
			//Character.transform.Rotate (0f, 1f, 0f);
			float y = Camera.GetComponent<ThirdPersonCamera>().y;
			if(y > -35){Camera.GetComponent<ThirdPersonCamera>().y += -1;}
			
		}
		if (_isLookdown == true)
		{
			//Character.transform.Rotate (0f, 1f, 0f);
			float y = Camera.GetComponent<ThirdPersonCamera>().y;
			if(y < 50){Camera.GetComponent<ThirdPersonCamera>().y += 1;}
			
		}
		
		
		
	}
			
	public void OnPointerEnter_up()
	{
		_isMoveup = true;
		Debug.Log("up");
	}
	public void OnPointerExit_up()
	{
		_isMoveup = false;
		Debug.Log("done");
	}
	public void OnPointerEnter_back()
	{
		_isMoveback = true;
		Debug.Log("back");
	}
	public void OnPointerExit_back()
	{
		_isMoveback = false;
		Debug.Log("done");
	}
	
	public void OnPointerEnter_strafeleft()
	{
		_isStrafeleft = true;
		Debug.Log("left");
	}
	public void OnPointerExit_strafeleft()
	{
		_isStrafeleft = false;
		Debug.Log("done");
	}
	public void OnPointerEnter_strafeleftup()
	{
		_isStrafeleftup = true;
		Debug.Log("leftup");
	}
	public void OnPointerExit_strafeleftup()
	{
		_isStrafeleftup = false;
		Debug.Log("done");
	}
	
	public void OnPointerEnter_straferight()
	{
		_isStraferight = true;
		Debug.Log("right");
	}
	public void OnPointerExit_straferight()
	{
		_isStraferight = false;
		Debug.Log("done");
	}
	
	public void OnPointerEnter_straferightup()
	{
		_isStraferightup = true;
		Debug.Log("rightup");
	}
	public void OnPointerExit_straferightup()
	{
		_isStraferightup = false;
		Debug.Log("done");
	}
	
	public void OnPointerEnter_turnleft()
	{
		_isTurnleft = true;
		Debug.Log("turnleft");
	}
	public void OnPointerExit_turnleft()
	{
		_isTurnleft = false;
		Debug.Log("done");
	}
	
	
	public void OnPointerEnter_turnright()
	{
		_isTurnright = true;
		Debug.Log("turnright");
	}
	public void OnPointerExit_turnright()
	{
		_isTurnright = false;
		Debug.Log("done");
	}
	
	public void OnPointerEnter_lookup()
	{
		_isLookup = true;
		Debug.Log("lookup");
	}
	public void OnPointerExit_lookup()
	{
		_isLookup = false;
		Debug.Log("done");
	}
	public void OnPointerEnter_lookdown()
	{
		_isLookdown = true;
		Debug.Log("lookdown");
	}
	public void OnPointerExit_lookdown()
	{
		_isLookdown = false;
		Debug.Log("done");
	}
}