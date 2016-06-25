/* ThirdPersonCamera - Controls the camera
 * Derived from http://wiki.unity3d.com/index.php?title=MouseOrbitImproved
 * Created - March 24 2013
 * PegLegPete (goatdude@gmail.com)
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EQBrowser;
using UnityEngine.EventSystems;

//[AddComponentMenu("Camera-Control/Mouse Look")]

public class ThirdPersonCamera : MonoBehaviour
{
	public GameObject m_curCharacterTarget;
	public WorldConnect WorldConnection;

	public Transform target;
	
	public float distance = 0f;
	public float xSpeed = 120.0f;
	public float ySpeed = 120.0f;

	public float yMinLimit = -20f;
	public float yMaxLimit = 80f;

	public float distanceMin = 0f;
	public float distanceMax = 10f;



	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;
	
	public float minimumX = -360F;
	public float maximumX = 360F;
	
	public float minimumY = -60F;
	public float maximumY = 60F;
	float x = 0.0f;
	float y = 0.0f;
	int vDir;

	float targetLastRot;

	bool m_isLocked = true;

	void FindHeadTarget()
	{
		Vector3 lookAtPos = target.position;
	}



	// Use this for initialization
	void Start () 
	{
		FindHeadTarget();
	}




	void Update()
	{
		if (Input.GetKey(KeyCode.PageUp))
		{
			y += -1;
		}
		if (Input.GetKey(KeyCode.PageDown))
		{
			y += 1;
		}
		if (Input.GetKey(KeyCode.Home))
		{
			y = 0;
		}
	
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				if ((!EventSystem.current.IsPointerOverGameObject()) && (hit.collider.tag=="Targetable"))
				{
						WorldConnection.DoTarget(hit.collider.name);
				}
			}
		}
		
		if (Input.GetMouseButtonDown(1))
		{
			Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				if ((!EventSystem.current.IsPointerOverGameObject()) && (hit.collider.tag=="Targetable"))
				{
					string target = hit.collider.name;
					GameObject temp = ObjectPool.instance.spawnlist.Where(obj => obj.name == target).SingleOrDefault();
					if(temp != null)
					{
						if(temp.GetComponent<NPCController>().isDead == 1 && WorldConnection.OurTargetLootID == 0)
						{
							Debug.Log("ISDEADLOOT");
							WorldConnection.DoLoot(target);
						}
						else
						{
							Debug.Log("ISNOTDEADCONSIDER");
						}
					}
				}
			}
		}
	}

	// zer0sum: mouserightdown
	protected static bool m_r_d = false;

	void LateUpdate()
	{

		float xDelta = 0f;
		float yDelta = 0f;
		float zoomDelta = 0f;
		
		zoomDelta = Input.GetAxis("Mouse ScrollWheel");

		if (m_isLocked)
		{
		   UpdateCameraLocked(zoomDelta, xDelta, yDelta);
		}
		else
		{
			UpdateCameraLocked(zoomDelta, xDelta, yDelta);
		}
		
	}
	
	public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }

	void UpdateCameraLocked(float distDelta, float xDelta, float yDelta)
	{
		//First rotate the player's character
		if (!Mathf.Approximately(xDelta, 0f))
		{
			float sign = Mathf.Sign(xDelta);
		}

		//Now update the camera position
//		y -= yDelta;

//		y = ClampAngle(y, yMinLimit, yMaxLimit);

		Quaternion rotation = Quaternion.Euler(y, m_curCharacterTarget.transform.rotation.eulerAngles.y, 0f);
		
		distance = Mathf.Clamp(distance - distDelta * 5, distanceMin, distanceMax);
		RaycastHit hit;
		if (Physics.Linecast(target.position, transform.position, out hit, 1 << LayerMask.NameToLayer("Terrain")))
		{
			distance -= hit.distance;
		}
		Vector3 negDistance = new Vector3(0.0f, 3.0f, -distance);
		Vector3 position = rotation * negDistance + target.position;

		if (!Input.GetMouseButton (1)) {
			transform.rotation = rotation;
//			x = m_curCharacterTarget.transform.rotation.eulerAngles.y;
//			y = m_curCharacterTarget.transform.rotation.eulerAngles.x;
		}
			transform.position = position;
	}
	
	public static Transform GetChildTransformByTag(Transform rootTransform, string tag)
    {
        Transform[] childTrans = rootTransform.GetComponentsInChildren<Transform>();

        for (int i = 0; i < childTrans.Length; i++)
        {
            if (childTrans[i].tag == tag)
            {
                return childTrans[i];
            }
        }

        return null;
    }
}