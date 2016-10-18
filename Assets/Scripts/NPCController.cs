using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;
using EQBrowser;

public class NPCController : MonoBehaviour 
{
	public int RaceID = 0;
	public int spawnId = 0;
	public int corpseId = 0;
	public string name = "";// Player's Name
	public string prefabName = "GnollPrefab";// Player's prefab Name
	public float x = 0;// x coord
	public float y = 0;// y coord
	public float z = 0;// z coord
	public float heading = 0;// heading

	public float step;
	public Vector3 targetPosition;
	public Vector3 deltaF;

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

	private float gravity = 1.0f;
	public bool isGrounded = false;

	public bool isTarget = false;
	public bool colorActivate = false;
	public int isWalk;
	public int isIdle;
	public int isDead;
	public int isPunch;
	public int isHurt;
	public bool clientUpdate;

	public int animationspeed;
	public int deity = 0;// Player's Deity
	public float size = 0;// Model size
	public byte NPC = 0;// 0=player,1=npc,2=pc corpse,3=npc corpse,a
	public byte curHp = 0;// Current hp %%% wrong
	public int maxHp = 0;// Current hp %%% wrong
	public byte level = 0;// Spawn Level
	public byte gender = 0;// Gender (0=male, 1=female)
	public int animationState;//animation
	public CharacterController controller;

	public float fixnum;
	public float ycalc;
	public float offset;
	
	public GameObject NameObject;
	

	void Start()
	{
			//clean name
			string targetName = name;
			string targetClean = Regex.Replace(targetName, "[0-9]", "");
			string targetName2 = Regex.Replace(targetClean, "[_]", " ");
			string targetName3 = Regex.Replace(targetName2, "[\0]", "");
			//generate name above head				
			NameObject.GetComponent<TextMesh>().text = targetName3;
			controller = this.GetComponent<CharacterController>();

			this.transform.position = new Vector3(x, y, z);
	}
	void Update () 
	{
			//make overhead names face camera
			NameObject.transform.LookAt(2 * NameObject.transform.position - Camera.main.transform.position);
			//change color if targetted
			Renderer rend = NameObject.GetComponent<Renderer>();
			rend.material.color = Color.red;
			
			if(isTarget == true && colorActivate == false)
			{
				rend.material.color = Color.green;
			}
			if(isTarget == false && colorActivate == true)
			{
				rend.material.color = Color.red;
			}
		
		if(NPC == 2 || isDead == 1){deadNow();}
		else
		{
			//Touching ground
			if ((controller.collisionFlags & CollisionFlags.Below)!=0)
			{
				offset = this.transform.position.y;
			}
			if (controller.collisionFlags == CollisionFlags.Above)
			{
//				y += 1;
			}
			if (controller.collisionFlags == CollisionFlags.None)
			{
//				y -= 1;
			}
			//deep underneath world
			if (!controller.isGrounded)
			{
				if(this.transform.position.y < -500){fixit();}
			}
			if (controller.isGrounded)
			{
				
			}

			//heading
			float h = Mathf.Lerp(360,0,movetoH/255f);
			transform.localEulerAngles = new Vector3(0,h,0);
			//set delta vector
			deltaF = new Vector3 (deltaX,deltaY,deltaZ);


			//wandering
			if (deltaF.magnitude != 0)
			{
				if(isWalk == 0){walkNow();}
				//step = delta time x speed. The server is calculating the speed which is represented as the magnitude of vector x y z. Translate the game object by those deltas multiplied by delta time
				if(NPC == 1)
				{
					step = (deltaF.magnitude * 10f) * Time.deltaTime;
					//sets clientupdate flag to false when an npc is autorunning, waiting for another clientupdate packet
					if(this.transform.position.x == movetoX && this.transform.position.z == movetoZ){clientUpdate = false;}
					//if new update from server, move there
					if(clientUpdate == true)
					{
						//initial movement
						ycalc = movetoY - deltaY;
//						offset = movetoY - deltaY;
						targetPosition = new Vector3 (movetoX,ycalc,movetoZ);
					}
					//if waiting on update from server, move along the delta positions
					if(clientUpdate == false)
					{
						//continuing to move in between updates
						targetPosition += new Vector3 (deltaX,0f,deltaZ);
					}
					//move now
					this.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, targetPosition, step);
				//if this a player not an npc
				}
				else
				{
					targetPosition = new Vector3 (movetoX,movetoY,movetoZ);
					this.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, targetPosition, 1);
				}
			
			//draw pretty lines of pathing for scene editor
			//Debug.DrawRay (this.gameObject.transform.position, (this.gameObject.transform.position - targetPosition), Color.green);
			Debug.DrawRay (this.gameObject.transform.position, (targetPosition - this.gameObject.transform.position), Color.green);
			}
			//idle npc after reaching a target destination.
			else
			{
				if (deltaX == 0 && deltaY == 0 && deltaZ == 0 && movetoX != 0 && movetoY != 0 && movetoZ != 0)
				{
					idleNow();
					ycalc = movetoY;
					this.transform.position = new Vector3(movetoX, ycalc, movetoZ);
				}
				else
				{
					//FOR  Y ADJUSTMENTS IF UNDER OR BENEATH WORLD WHEN NOT MOVING AND NO POSITION UPDATES FROM SERVER
				}
			}
		}
	}
	public void fixit()
	{
		Debug.Log("hisda");
		fixnum += 5;
		this.transform.position = new Vector3(movetoX, y + fixnum, movetoZ);
	}

	public void walkNow()
	{
		isWalk = 1;
		isIdle = 0;
		isPunch = 0;
		isDead = 0;
		isHurt = 0;
		GetComponent<Animator>().Play("Walk");
	}
	public void idleNow()
	{
		isWalk = 0;
		isIdle = 1;
		isPunch = 0;
		isDead = 0;
		isHurt = 0;
		GetComponent<Animator>().Play("Idle");
	}
	public void punchNow()
	{
		isWalk = 0;
		isIdle = 0;
		isPunch = 1;
		isDead = 0;
		isHurt = 0;
		GetComponent<Animator>().Play("Punch");
	}
	public void hurtNow()
	{
		isWalk = 0;
		isIdle = 0;
		isPunch = 0;
		isDead = 0;
		isHurt = 1;
		GetComponent<Animator>().Play("Hurt");
	}
	public void deadNow()
	{
		isWalk = 0;
		isIdle = 0;
		isPunch = 0;
		isDead = 1;
		isHurt = 0;
		GetComponent<Animator>().Play("Dead");
		name = NameObject.GetComponent<TextMesh>().text + "'s corpse";
	}
}