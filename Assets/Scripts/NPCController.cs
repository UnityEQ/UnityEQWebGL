using UnityEngine	;
using System.Collections;
using EQBrowser;


	public class NPCController : MonoBehaviour 
	{
		public int RaceID = 0;
		public int spawnId = 0;
		public string name = "";// Player's Name
		public float x = 0;// x coord
		public float y = 0;// y coord
		public float z = 0;// z coord
		public float heading = 0;// heading
		public int deity = 0;// Player's Deity
		public float size = 0;// Model size
		public byte NPC = 0;// 0=player,1=npc,2=pc corpse,3=npc corpse,a
		public byte curHp = 0;// Current hp %%% wrong
		public byte level = 0;// Spawn Level
		public byte gender = 0;// Gender (0=male, 1=female)

		private CharacterController controller;
		private float gravity = 20.0f;
		private Vector3 moveDirection = Vector3.zero;
		private bool grounded = false;
		
		public float movetoX = 0;// x coord
		public float movetoY = 0;// y coord
		public float movetoZ = 0;// z coord
		public float movetoH = 0;// z coord

		void Update () 
		{ 
			CharacterController controller = GetComponent<CharacterController>();
			Vector3 speedv = controller.velocity;
//			Debug.Log(speedv);
			
//			GetComponent<Animator>().Play("Walk");

			//Apply gravity 
			moveDirection.y -= gravity * Time.deltaTime; 
			//Get CharacterController 
			controller = GetComponent<CharacterController>(); 
			//Move Charactercontroller and check if grounded 
			grounded = ((controller.Move(moveDirection * Time.deltaTime)) & CollisionFlags.Below) != 0; 
			
			//wander
			if ((movetoX != 0) && (movetoY != 0) && (movetoZ != 0) && (movetoH != 0))
			{
				
				Vector3 targetPosition = new Vector3 (-movetoX,movetoZ,movetoY);
				
				float step = Time.deltaTime * 5.0f;
				transform.position = Vector3.MoveTowards(this.transform.position, targetPosition, step);
				//heading
				transform.rotation = Quaternion.Euler(0, movetoH, 0);
			}
		}
			
	}
