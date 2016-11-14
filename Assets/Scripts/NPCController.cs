using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using System.Xml;
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
	public string prefabName;// Player's prefab Name
	public float x;// x coord
	public float y;// y coord
	public float z;// z coord
	public float heading;// heading

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
	public Renderer rend;
	
	public GameObject NameObject;
	public bool updateHeading;
	public bool updateDeltas;
	public bool playerRespawn = false;
	public string targetName = "";
	
	void Start()
	{
		//clean name for overhead name
		targetName = name;
		string targetClean = Regex.Replace(targetName, "[0-9]", "");
		string targetName2 = Regex.Replace(targetClean, "[_]", " ");
		string targetName3 = Regex.Replace(targetName2, "[\0]", "");
		//generate name above head				
		this.NameObject.GetComponent<TextMesh>().text = targetName3;
		//define character controller
		controller = this.GetComponent<CharacterController>();
		//place NPCs via ZoneSpawns packet var
//		this.transform.position = new Vector3(x, y + 5f, z);
		//define overhead name object
		rend = this.NameObject.GetComponent<Renderer>();
		rend.material.color = Color.red;
	}
	void Update () 
	{
		//gravity
		
		if(targetName == "")
		{
			//clean name for overhead name
			targetName = name;
			string targetClean = Regex.Replace(targetName, "[0-9]", "");
			string targetName2 = Regex.Replace(targetClean, "[_]", " ");
			string targetName3 = Regex.Replace(targetName2, "[\0]", "");
			//generate name above head				
			this.NameObject.GetComponent<TextMesh>().text = targetName3;
		}
		//if player respawned, reset NPC name Vars in Start
		if(playerRespawn == true){Start();playerRespawn = false;}
		//overhead names face player
		if(Camera.main.velocity != new Vector3(0,0,0) || this.controller.velocity != new Vector3(0,0,0) || updateHeading == true)
		{
			NameObject.transform.LookAt(2 * NameObject.transform.position - Camera.main.transform.position);
		}
		//green name when targetted
		if(isTarget == true && colorActivate == false)
		{
			rend.material.color = Color.green;
			colorActivate = true;
		}
		//red name when targetted
		if(isTarget == false && colorActivate == true)
		{
			rend.material.color = Color.red;
			colorActivate = false;
		}
		//update heading from clientupdate packet
		if(updateHeading == true)
		{
			float h = Mathf.Lerp(360,0,movetoH/255f);
			transform.localEulerAngles = new Vector3(0,h,0);
			updateHeading = false;
		}
		//update deltas from clientupdate packet
		if(updateDeltas == true)
		{
			deltaF = new Vector3 (deltaX,deltaY,deltaZ);
			updateDeltas = false;
		}
		
		// TODO-performance: Only do this when the NPC hp changes (specifically it decreases)
		if(NPC == 2 || isDead == 1){deadNow();}
		else
		{

			//wandering
			// TODO-performance: When a update arrives store whether or not the NPC is moving. Calculating magnitude involves sqrt 
			if (deltaF.magnitude != 0)
			{
				if(isWalk == 0){walkNow();}
				//step = delta time x speed. The server is calculating the speed which is represented as the magnitude of vector x y z. Translate the game object by those deltas multiplied by delta time
				if(NPC == 1)
				{
					step = deltaF.magnitude * 10f;
					//sets clientupdate flag to false when an npc is autorunning, waiting for another clientupdate packet
					if(this.transform.position.x == movetoX && this.transform.position.z == movetoZ){clientUpdate = false;}
					//if new update from server, move there
					if(clientUpdate == true)
					{
						//initial movement
						ycalc = this.gameObject.transform.position.y;
						targetPosition.x = movetoX;
						targetPosition.z = movetoZ;
					}
					//if waiting on update from server, move along the delta positions
					if(clientUpdate == false)
					{
						//continuing to move in between updates
						// TODO-performance: Store this when the pos packet arrives
						targetPosition.x += deltaX;
						targetPosition.z += deltaZ;
					}
					//move now
					raycastY();
					targetPosition.y = this.gameObject.transform.position.y;
					this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, targetPosition, step * Time.deltaTime);
				}
				//if this a player not an npc
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
					if(isIdle == 0){idleNow();}
					this.transform.position = new Vector3(movetoX, this.gameObject.transform.position.y, movetoZ);
					raycastY();
				}
				else
				{
					raycastY();
					//FOR  Y ADJUSTMENTS IF UNDER OR BENEATH WORLD WHEN NOT MOVING AND NO POSITION UPDATES FROM SERVER
				}
			}
		}
	}
	
	public void raycastY()
	{
		RaycastHit[] hitsDown;
		hitsDown = Physics.RaycastAll(this.gameObject.transform.position, Vector3.down, 200.0F);
		for (int i = 0; i < hitsDown.Length; i++) 
		{
			RaycastHit hitDown = hitsDown[i];
//			Debug.Log("HITS" + hitsDown.Length);
			if(hitsDown.Length > 1)
			{
				if(hitDown.distance > 3.1f && hitDown.collider.tag == "Terrain"){this.gameObject.transform.position -= new Vector3(0f,20f * Time.deltaTime,0f);}
				if(hitDown.distance < 3f && hitDown.collider.tag == "Terrain"){this.gameObject.transform.position += new Vector3(0f,20f * Time.deltaTime,0f);}
			}
			else
			{
				this.gameObject.transform.position += new Vector3(0f,20f * Time.deltaTime,0f);
			}
		}
		if(hitsDown.Length == 0){this.gameObject.transform.position += new Vector3(0f,20f * Time.deltaTime,0f);}
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