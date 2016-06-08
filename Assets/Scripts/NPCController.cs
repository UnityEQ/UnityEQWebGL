using UnityEngine	;
using System.Collections;
using EQBrowser;


	public class NPCController : MonoBehaviour 
	{
		public int RaceID = 0;
		public int spawnId = 0;
		public string name = "";// Player's Name
		public string prefabName = "";// Player's prefab Name
		public float x = 0;// x coord
		public float y = 0;// y coord
		public float z = 0;// z coord
		public float heading = 0;// heading
		public int deity = 0;// Player's Deity
		public float size = 0;// Model size
		public byte NPC = 0;// 0=player,1=npc,2=pc corpse,3=npc corpse,a
		public byte curHp = 0;// Current hp %%% wrong
		public byte maxHp = 0;// Current hp %%% wrong
		public byte level = 0;// Spawn Level
		public byte gender = 0;// Gender (0=male, 1=female)
		public int animationState = 0;//animation

		private CharacterController controller;
		private float gravity = 20.0f;
		private Vector3 moveDirection = Vector3.zero;
		private bool grounded = false;

//-x,z,y 		
		public float movetoX;// x coord
		public float movetoY;// y coord
		public float movetoZ;// z coord
		public float movetoH;// z coord
//-x,z,y 		
		public float deltaX;// x coord
		public float deltaY;// y coord
		public float deltaZ;// z coord
		public float deltaH;// z coord
		
		public float stepFinal;
		public int isWalk;
		public int isIdle;
		public int isDead;
		public int isPunch;
		
		void Update () 
		{ 
			if(NPC == 2)
			{
				deadNow();
			}
			else
			{
				CharacterController controller = GetComponent<CharacterController>();
				//Apply gravity 
				moveDirection.y -= gravity * Time.deltaTime; 
				//Get CharacterController 
				controller = GetComponent<CharacterController>(); 
				//Move Charactercontroller and check if grounded 
				grounded = ((controller.Move(moveDirection * Time.deltaTime)) & CollisionFlags.Below) != 0; 
				
				//wander
				if (movetoX != 0 && movetoY != 0 && movetoZ != 0 && movetoH != 0)
				{
					if(isWalk == 0)
					{
						walkNow();
					}
					
					Vector3 targetPosition = new Vector3 (movetoX,movetoY,movetoZ);
	
					Vector3 deltaF = new Vector3 (deltaX,deltaY,deltaZ);
					if (deltaF.magnitude != 0) {
						//step = delta time x speed. The server is calculating the speed which is represented as the magnitude of vector x y z. Translate the game object by those deltas multiplied by delta time	
						float step = deltaF.magnitude * Time.deltaTime;
						transform.position = Vector3.MoveTowards(this.gameObject.transform.position, targetPosition, 1);

//						Debug.DrawRay (this.gameObject.transform.position, (this.gameObject.transform.position - targetPosition), Color.green);
						Debug.DrawRay (this.gameObject.transform.position, (targetPosition - this.gameObject.transform.position), Color.green);
					}
					//heading
//					float h = Mathf.Lerp(360,0,movetoH/255f);
//					transform.localEulerAngles = new Vector3(0,h,0);
				}
				else
				{
					if(isIdle == 0)
					{
						idleNow();
					}
				}
			}
		}

//trigger animations
		public void walkNow()
		{
			isWalk = 1;
			isIdle = 0;
			isPunch = 0;
			isDead = 0;
			GetComponent<Animator>().Play("Walk");
		}
		public void idleNow()
		{
			isWalk = 0;
			isIdle = 1;
			isPunch = 0;
			isDead = 0;
			GetComponent<Animator>().Play("Idle");
		}
		public void punchNow()
		{
			isWalk = 0;
			isIdle = 0;
			isPunch = 1;
			isDead = 0;
			GetComponent<Animator>().Play("Punch");
		}
		public void deadNow()
		{
			isWalk = 0;
			isIdle = 0;
			isPunch = 0;
			isDead = 1;
			GetComponent<Animator>().Play("Dead");
		}
			
	}
