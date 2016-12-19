using UnityEngine;
using System.Collections;

public class PlayMove : MonoBehaviour {

	public float moveSpeed=3f;
	CharacterController controller;
	
	public Vector3 destPoint;
	Vector3 moveDirection;
	
	public GameObject pointEffect;
	

	void Start () {
		controller= GetComponent<CharacterController>();
		destPoint = transform.position;
	
	}
	

	void Update () {
		if(Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera .main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			
			if(Physics.Raycast(ray,out hit, 500))
			{
				destPoint = hit.point;
				GameObject pointer= (GameObject) Instantiate(pointEffect,
					     				destPoint,Quaternion.identity);
				Destroy(pointer,0.5f);
				
				moveDirection = new Vector3(destPoint.x-transform.position.x,
					 						destPoint.y-transform.position.y,
											destPoint.z-transform.position.z);
					moveDirection = moveDirection.normalized;
					moveDirection *=moveSpeed;
					moveDirection.y =-10f;
			}
		}
		if(Vector3.Distance(transform.position,destPoint)>1.2)
		  controller.Move(moveDirection*Time.deltaTime);
		
		transform.rotation = Quaternion.RotateTowards(transform.rotation,
			Quaternion.LookRotation(new Vector3(moveDirection.x,0,moveDirection.z)),
			Time.deltaTime*360);
	}
	
}
