/// <summary> 
/// CharacterAnimator.cs 
/// Character Controller in CSharp v2.3 
/// </summary> 
using UnityEngine; 
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
 
namespace EQBrowser
{
	public class CharacterAnimatorSP : MonoBehaviour { 
		
		//Var definition 
		public bool swimming = false;                    //Can be triggert to slow down the movements (like when u swim) 
		public string moveStatus = "idle";               //movestatus for animations 
		
		//Movement speeds 
		private float jumpSpeed = 8.0f;                  //Jumpspeed / Jumpheight 
		private float gravity = 20.0f;                   //Gravity for jump 
		private float runSpeed = 10.0f;                  //Speed when the Character is running 
		private float walkSpeed = 30.0f;                  //Speed when the Character is walking (normal movement) 
		private float rotateSpeed = 250.0f;              //Rotationspeed of the Character 
		private float walkBackMod = 0.75f;               //Speed in Percent for walk backwards and sidewalk 
		
		//Internal vars to work with 
		private float speedMod = 0.0f;                   //temp Var for Speedcalculation 
		private bool grounded = false;                   //temp var if the character is grounded 
		private Vector3 moveDirection = Vector3.zero;    //move direction of the Character 
		private bool isWalking = false;                  //toggle var between move and run 
		private bool jumping = false;                    //temp var for jumping 
		private bool CastHealbool = false;                    //temp var for casting 
		private bool mouseSideButton = false;            //temp var for mouse side buttons 
		private float pbuffer = 0.0f;                    //Cooldownpuffer for SideButtons 
		private float coolDown = 0.5f;                   //Cooldowntime for SideButtons 
		public CharacterController controller;          //CharacterController for movement 
		EQBrowser.CharacterAnimator m_avatar;
		private Animator anim;
		private bool Casting = false;
		//		public bool CastButton = true;
		private bool chatting = false;
		public GameObject ChatInputField;
		
		public float distanceTravelled = 0;
		public Vector3 lastPosition;

		public float rotationTravelled = 0;
		public Vector3 lastRotation;
		
		
		void Start () 
		{ 
			anim = gameObject.GetComponent<Animator>();
			lastPosition = transform.position;
			lastRotation = transform.eulerAngles;

		}

		public void ToggleChatOn()
		{
			Debug.Log("chaton");
			InputField inputField = ChatInputField.GetComponent<InputField>();
			inputField.interactable = true;
			inputField.ActivateInputField();
			chatting = true;


		}
		public void ToggleChatOff()
		{
			Debug.Log("chatoff");
			InputField inputField = ChatInputField.GetComponent<InputField>();

			chatting = false;
			inputField.interactable = false;
			inputField.DeactivateInputField();
			inputField.text = string.Empty;
		}
		

		//Every Frame 
		void Update () 
		{ 
	
		if(Input.GetKeyDown(KeyCode.F10))
		{
			Screen.fullScreen = !Screen.fullScreen;
		}
		
		if (Input.GetKeyDown("return"))
		{
			if (chatting == true)
			{
				ToggleChatOff();
			}
			else
			{
				ToggleChatOn();
			}
		}
		
		if (this.anim.GetCurrentAnimatorStateInfo (0).IsName ("CastHeal")) 
		{
			Casting = true;
			moveStatus = "CastHeal";
			UIScripts.CastButton = false;
		} 
		else 
		{
			Casting = false;
		}
			//Set idel animation 
			isWalking = true;
			
			// Hold "Run" to run 
			if(Input.GetAxis("Run") != 0) 
				isWalking = false; 
			
			// Only allow movement and jumps while grounded 
			if(grounded)
			{ 

				//movedirection 

					moveDirection = new Vector3((Input.GetMouseButton(1) ? Input.GetAxis("Horizontal") : 0),0,Input.GetAxis("Vertical")); 

				//pushbuffer to avoid on/off flipping 
				if(pbuffer>0) 
					pbuffer -=Time.deltaTime; 
				if(pbuffer<0)pbuffer=0; 
				
				//Automove Sidebuttonmovement 
				if((Input.GetAxis("Toggle Move") !=0) && pbuffer == 0){ 
					pbuffer=coolDown; 
					mouseSideButton = !mouseSideButton; 
				} 
				
				//L+R MouseButton Movement 
//				if (Input.GetMouseButton(0) && Input.GetMouseButton(1) || mouseSideButton) 
//					moveDirection.z += 1; 
//				if (moveDirection.z > 1)
//					moveDirection.z = 1;
				
				//Strafing move (like Q/E movement     
				moveDirection.x -= Input.GetAxis("Strafing"); 
				
				// if moving forward and to the side at the same time, compensate for distance 
				if(Input.GetMouseButton(1) && (Input.GetAxis("Horizontal") != 0) && (Input.GetAxis("Vertical") != 0)) 
				{ 
					moveDirection *= 0.7f; 
				} 
				
				//Speedmodification / is moving forward or side/backward 
				speedMod = ((Input.GetAxis("Vertical") < 0) || (Input.GetMouseButton(1) && (Input.GetAxis("Horizontal")) != 0) || Input.GetAxis("Strafing") != 0) ? walkBackMod : 1.0f;
				
				//Use run or walkspeed 
				moveDirection *= isWalking ? walkSpeed * speedMod : runSpeed * speedMod; 
				
				//reduce movement by 70% when swimming is toggled    
				moveDirection*= swimming ? 0.7f : 1; 
				
				// Jump! 
				if(Input.GetButton("Jump") && (Casting == false))
				{ 
					jumping = true; 
					moveDirection.y = jumpSpeed;
					GetComponent<Animator>().Play("Jump");
				}
				
				
				var turningright = (Input.GetAxis("Horizontal") > 0);
				var turningleft = (Input.GetAxis("Horizontal") < 0);
				
				if ((turningright == true) && (moveDirection.z == 0) && (Casting == false) && (moveDirection.x == 0) && (moveDirection.y == 0))
				{
					moveStatus = "Turn";
					GetComponent<Animator>().Play("Turn");
				}
				if ((turningleft == true) && (moveDirection.z == 0) && (Casting == false) && (moveDirection.x == 0) && (moveDirection.y == 0))
				{
					moveStatus = "Turn";
					GetComponent<Animator>().Play("Turn");
				}
				//movestatus normal movement (for animations)               
				if ((moveDirection.x == 0) && (moveDirection.z == 0) && (turningright == false) && (turningleft == false))
				{
					if((UIScripts.CastButton == true) && (moveStatus == "idle"))
					{ 
						GetComponent<Animator>().Play("CastHeal");
						UIScripts.CastButton = false;
					}
					
					if(Casting == false && UIScripts.CastButton == false)
					{
						moveStatus = "idle";
						GetComponent<Animator>().Play("Idle");
					}
				}
				if((Casting == false))
				{
					if (moveDirection.z > 0)
					{
						UIScripts.CastButton = false;
						moveStatus = isWalking ? "walking" : "running";
						GetComponent<Animator>().Play("Walk");
					}
					if (moveDirection.z < 0)
					{
						UIScripts.CastButton = false;
						moveStatus = isWalking ? "backwalking" : "backrunning";
						GetComponent<Animator>().Play("Walk");
					}
					if (moveDirection.x > 0)
					{
						UIScripts.CastButton = false;
						moveStatus = isWalking ? "sidewalking_r" : "siderunning_r";
						GetComponent<Animator>().Play("Walk");

					}
					if (moveDirection.x < 0)
					{
						UIScripts.CastButton = false;
						moveStatus = isWalking ? "sidewalking_l" : "siderunning_l";
						GetComponent<Animator>().Play("Walk");
					}
				}

				//movestatus swim movement (for animations)               
				if(swimming)
				{
					if ((moveDirection.x == 0) && (moveDirection.z == 0))
					{
						moveStatus = "swimidle";
					}
					if (moveDirection.z > 0)
					{
						moveStatus = isWalking ? "swim" : "swimfast";
					}
					if (moveDirection.z < 0)
					{
						moveStatus = isWalking ? "backswim" : "backswimfast";
					}
					if (moveDirection.x > 0)
					{
						moveStatus = isWalking ? "sideswim_r" : "sideswimfast_r";
					}
					if (moveDirection.x < 0)
					{
						moveStatus = isWalking ? "sidewswim_l" : "sideswimfast_l";
					}
					if (jumping)
					{
						moveStatus = "swimup";
					}
				}   
				
				//transform direction 
				moveDirection = transform.TransformDirection(moveDirection);
				
				//clientupdate
				
				distanceTravelled += Vector3.Distance(transform.position, lastPosition);
				lastPosition = transform.position;
				
				rotationTravelled += Vector3.Distance(transform.eulerAngles, lastRotation);
				lastRotation = transform.eulerAngles;
				

				
			} 
			// Allow turning at anytime. Keep the character facing in the same direction as the Camera if the right mouse button is down. 
			if(Input.GetMouseButton(1)) { 
//				transform.rotation = Quaternion.Euler(0,Camera.main.transform.eulerAngles.y,0); 
			}
			else 
			{ 

					transform.Rotate(0,Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime, 0);
			} 
			
			//Apply gravity 
			moveDirection.y -= gravity * Time.deltaTime; 
			
			//Get CharacterController 
			controller = this.GetComponent<CharacterController>(); 
			
			//Move Charactercontroller and check if grounded 

				grounded = ((controller.Move(moveDirection * Time.deltaTime)) & CollisionFlags.Below) != 0; 

			
			//Reset jumping after landing 
			jumping = grounded ? false : jumping; 
			
			//movestatus jump/swimup (for animations)       
			if(jumping) 
				moveStatus = "jump"; 
			if(jumping && swimming) 
				moveStatus = "swimup";     
		} 
		
	} 
}
