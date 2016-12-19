using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {

	public Transform focusingTarget;
	public float moveSpeed=0.8f;
	

	
	// Update is called once per frame
	void Update () {
	//	transform.position = focusingTarget.position;
	 	transform.position = Vector3.Lerp(transform.position,focusingTarget.position,
											Time.deltaTime*moveSpeed);
		if(Input.GetMouseButton(1))
		{
	
			float h=Input.GetAxis("Mouse X");
			Camera.main.transform.RotateAround(transform.position,
				new Vector3(0,1,0),
				h*1.5f); 
		}
	}
		
}
