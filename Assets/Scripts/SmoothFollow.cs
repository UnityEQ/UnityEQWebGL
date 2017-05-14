// Smooth Follow from Standard Assets
// Converted to C# because I fucking hate UnityScript and it's inexistant C# interoperability
// If you have C# code and you want to edit SmoothFollow's vars ingame, use this instead.
using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.EventSystems;
using EQBrowser;

public class SmoothFollow : MonoBehaviour {
    
	// The target we are following
	public Transform target;
	// The distance in the x-z plane to the target
	public float distance = 10.0f;
	// the height we want the camera to be above the target
	public float height = 5.0f;
	// How much we 
	public float heightDamping = 2.0f;
	public float rotationDamping = 3.0f;
	public float y = 0.0f;
	public float distanceMin = 0f;
	public float distanceMax = 10f;
	public WorldConnect WorldConnection;
	public int currentCursor = 0;
	public bool doCursorUpdate = false;
    public Vector2 hotSpot = Vector2.zero;
	public Texture2D cursorTexture;
	bool m_isLocked = true;
	public GameObject m_curCharacterTarget;
	
	public void CursorUpdate()
	{
		if(WorldConnection.cursorIconId == 0){Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);}
		else{
				cursorTexture = (Texture2D) Resources.Load("Icons/item_" + WorldConnection.cursorIconId, typeof(Texture2D));
				Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.ForceSoftware);
			}
			currentCursor = WorldConnection.cursorIconId;		
	}

	void FindHeadTarget()
	{
		Vector3 lookAtPos = target.position;
	}



	// Use this for initialization
	void Start () 
	{
		FindHeadTarget();

	}

	// Place the script in the Camera-Control group in the component menu
	[AddComponentMenu("Camera-Control/Smooth Follow")]

	void Update()
	{
		if(WorldConnection.cursorIconId != currentCursor){CursorUpdate();}
		
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
						
						string target = hit.transform.root.name;
						Debug.Log("targetid: " + target);
						WorldConnection.DoTarget(target);
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

					string target = hit.transform.root.name;
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
	void LateUpdate () {
		
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
		
		
		// Early out if we don't have a target
		if (!target) return;

		// Calculate the current rotation angles
		float wantedRotationAngle = target.eulerAngles.y;
		float wantedHeight = target.position.y + height;

		float currentRotationAngle = transform.eulerAngles.y;
		float currentHeight = transform.position.y;

		// Damp the rotation around the y-axis
		currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
	
		// Damp the height
		currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

		// Convert the angle into a rotation
		var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);
	
		// Set the position of the camera on the x-z plane to:
		// distance meters behind the target
		
		Quaternion rotation = Quaternion.Euler(y, target.transform.rotation.eulerAngles.y, 0f);
		distance = Mathf.Clamp(distance - 2f * 5, distanceMin, distanceMax);
		RaycastHit hit;
		if (Physics.Linecast(target.position, transform.position, out hit, 1 << LayerMask.NameToLayer("Terrain")))
		{
			distance -= hit.distance;
		}
		Vector3 negDistance = new Vector3(0.0f, 3.0f, -distance);
		Vector3 position = rotation * negDistance + target.position;
		transform.rotation = rotation;
		transform.position = position;



		
//		transform.position = target.position;
//		transform.position -= currentRotation * Vector3.forward * distance;

		// Set the height of the camera
		transform.position = new Vector3(transform.position.x,currentHeight,transform.position.z);
	
		// Always look at the target
//		transform.LookAt(target);
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
	
}