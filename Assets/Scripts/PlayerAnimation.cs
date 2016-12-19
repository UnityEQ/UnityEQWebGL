using UnityEngine;
using System.Collections;

public class PlayerAnimation : MonoBehaviour {
	
	PlayMove playerMove;
	Animation animation;


	void Start () {
		playerMove = GetComponent<PlayMove>();
		animation = GetComponentInChildren<Animation>();
		
//		animation["KK_Idle"].wrapMode = WrapMode.Loop;
//		animation["KK_Run_No"].wrapMode = WrapMode.Loop;
		//animation["KK_Attack"].wrapMode = WrapMode.Once;
		
//		animation["KK_Run_No"].speed = 1.1f;
		
		//animation["KK_Attack"].layer=1;
		//Transform mt = transform.Find("Koko@KK_Idle/Koko_Bip001/Koko_Bip001 Pelvis");
		//animation["KK_Attack"].AddMixingTransform(mt);
	
	}
	

	void Update () {
		if(Vector3.Distance(transform.position,playerMove.destPoint)>2)
		{
			//KK_Run_No
//			animation.CrossFade("KK_Run_No",0.1f);
		}
		else
		{
			//KK_Idle
			//if(animation.IsPlaying("KK_Attack")==false)
//			animation.CrossFade("KK_Idle");
	
	}
		
		//if(Input.GetMouseButtonDown(1))
		{
			//animation.CrossFadeQueued("KK_Attack",0f,QueueMode.PlayNow);
		}
}
}