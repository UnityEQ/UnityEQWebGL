using UnityEngine;
using System.Collections;
using System;

public class NetworkEntity : MonoBehaviour
{

    private Int32 m_EntityID;
    private static string m_Name;
    public TextMesh namePlate;
    //Var definition 
    public bool swimming = false;                    //Can be triggert to slow down the movements (like when u swim) 
    public string moveStatus = "idle";               //movestatus for animations 

    //Movement speeds 
    private float jumpSpeed = 8.0f;                  //Jumpspeed / Jumpheight 
    private float gravity = 20.0f;                   //Gravity for jump 
    private float runSpeed = 10.0f;                  //Speed when the Character is running 
    private float walkSpeed = 20.0f;                  //Speed when the Character is walking (normal movement) 
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
    private CharacterController controller;          //CharacterController for movement 
    EQBrowser.CharacterAnimator m_avatar;
    private Animator anim;
    private bool Casting = false;
    //		public bool CastButton = true;



    public void SetID(Int32 id)
    {
        m_EntityID = id;
    }

    public void SetName(string name)
    {
        m_Name = name;
        namePlate.text = name;
    }
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
//        if (!SceneManagerObj.Singleton.isStarted)
//        {
//            return;
//        }
        //			Debug.Log (UIScripts.CastButton);
        //			Debug.Log (Casting);

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("CastHeal"))
        {
            Casting = true;
            moveStatus = "CastHeal";
        }
        else
        {
            Casting = false;
        }

        isWalking = false;

        // Only allow movement and jumps while grounded 
        if (grounded)
        {

            //movedirection 
            moveDirection = new Vector3(0, 0, 0);

            //pushbuffer to avoid on/off flipping 
            if (pbuffer > 0)
                pbuffer -= Time.deltaTime;
            if (pbuffer < 0) pbuffer = 0;

            //L+R MouseButton Movement 
            //				if (Input.GetMouseButton(0) && Input.GetMouseButton(1) || mouseSideButton) 
            //					moveDirection.z += 1; 
            //				if (moveDirection.z > 1)
            //					moveDirection.z = 1;

            //Strafing move (like Q/E movement     

            //Speedmodification / is moving forward or side/backward 
            speedMod = 1.0f;

            //Use run or walkspeed 
            moveDirection *= isWalking ? walkSpeed * speedMod : runSpeed * speedMod;

            //reduce movement by 70% when swimming is toggled    
            moveDirection *= swimming ? 0.7f : 1;

            //transform direction 
            moveDirection = transform.TransformDirection(moveDirection);

        }

        //Apply gravity 
        moveDirection.y -= gravity * Time.deltaTime;

        //Get CharacterController 
        controller = GetComponent<CharacterController>();
        //Move Charactercontroller and check if grounded 
        grounded = ((controller.Move(moveDirection * Time.deltaTime)) & CollisionFlags.Below) != 0;

        //Reset jumping after landing 
        jumping = grounded ? false : jumping;

        //movestatus jump/swimup (for animations)       
        if (jumping)
            moveStatus = "jump";
        if (jumping && swimming)
            moveStatus = "swimup";
    }
}