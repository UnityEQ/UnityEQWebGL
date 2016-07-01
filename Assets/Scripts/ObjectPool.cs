using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EQBrowser; 
public class ObjectPool : MonoBehaviour
{
	public List<GameObject> spawnlist;
	public Dictionary<int,GameObject> poop;

    public static ObjectPool instance;
   
    /// <summary>
    /// The object prefabs which the pool can handle.
    /// </summary>
    public GameObject[] objectPrefabs;
   
    /// <summary>
    /// The pooled objects currently available.
    /// </summary>
    public List<GameObject>[] pooledObjects;
   
    /// <summary>
    /// The amount of objects of each type to buffer.
    /// </summary>
    public int[] amountToBuffer;
   
    public int defaultBufferAmount = 3;
   
    /// <summary>
    /// The container object that we will keep unused pooled objects so we dont clog up the editor with objects.
    /// </summary>
    public GameObject containerObject;
   
    void Awake ()
    {
        instance = this;
	}
   
    // Use this for initialization
	
    void Start ()
    {
		
		
//        containerObject = new GameObject("ObjectPool");

       
        //Loop through the object prefabs and make a new list for each one.
        //We do this because the pool can only support prefabs set to it in the editor,
        //so we can assume the lists of pooled objects are in the same order as object prefabs in the array
        pooledObjects = new List<GameObject>[objectPrefabs.Length];
       
        int i = 0;
        foreach ( GameObject objectPrefab in objectPrefabs )
        {
            pooledObjects[i] = new List<GameObject>(); 
           
            int bufferAmount;
           
            if(i < amountToBuffer.Length) bufferAmount = amountToBuffer[i];
            else
                bufferAmount = defaultBufferAmount;
           
            for ( int n=0; n<bufferAmount; n++)
            {
                GameObject newObj = Instantiate(objectPrefab) as GameObject;
                newObj.name = objectPrefab.name;
//				UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(newObj, "Assets/Scripts/ObjectPool.cs (63,5)", "NPCController");
                PoolObject(newObj);
            }
           
            i++;
        }
    }
   
    /// <summary>
    /// Gets a new object for the name type provided.  If no object type exists or if onlypooled is true and there is no objects of that type in the pool
    /// then null will be returned.
    /// </summary>
    /// <returns>
    /// The object for type.
    /// </returns>
    /// <param name='objectType'>
    /// Object type.
    /// </param>
    /// <param name='onlyPooled'>
    /// If true, it will only return an object if there is one currently pooled.
    /// </param>

    public GameObject GetObjectForType ( string objectType , bool onlyPooled, float x, float y, float z, int spawnId, int race, string name, float heading, int deity, float size, byte NPC, byte curHp, byte maxHp, byte level, byte gender )
    {
        for(int i=0; i<objectPrefabs.Length; i++)
        {
            GameObject prefab = objectPrefabs[i];
            if(prefab.name == objectType)
            {
               
                if(pooledObjects[i].Count > 0)
                {
                    GameObject pooledObject = pooledObjects[i][0];
                    pooledObjects[i].RemoveAt(0);
                    pooledObject.transform.parent = null;
                    pooledObject.SetActiveRecursively(true);
					Vector3 pos = new Vector3(x,y,z);
					pooledObject.transform.position = pos;
					//heading
					float h = Mathf.Lerp(360,0,heading/255f);
//					pooledObject.transform.eulerAngles.y = h;
					pooledObject.transform.localEulerAngles = new Vector3(0,h,0);
					pooledObject.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
					pooledObject.name = spawnId.ToString();
					pooledObject.GetComponent<NPCController>().RaceID = race;
					pooledObject.GetComponent<NPCController>().spawnId = spawnId;
					pooledObject.GetComponent<NPCController>().name = name;// Player's Name
					pooledObject.GetComponent<NPCController>().prefabName = prefab.name;
					pooledObject.GetComponent<NPCController>().x = x;// x coord
					pooledObject.GetComponent<NPCController>().y = y;// y coord
					pooledObject.GetComponent<NPCController>().z = z;// z coord
					pooledObject.GetComponent<NPCController>().heading = heading;// heading
					pooledObject.GetComponent<NPCController>().deity = deity;// Player's Deity
					pooledObject.GetComponent<NPCController>().size = size;// Model size
					pooledObject.GetComponent<NPCController>().NPC = NPC;// 0=player,1=npc,2=pc corpse,3=npc corpse,a
					pooledObject.GetComponent<NPCController>().curHp = curHp;// Current hp %%% wrong
					pooledObject.GetComponent<NPCController>().maxHp = maxHp;// Current hp %%% wrong
					pooledObject.GetComponent<NPCController>().level = level;// Spawn Level
					pooledObject.GetComponent<NPCController>().gender = gender;// Gender (0=male, 1=female)
					
                   spawnlist.Add(pooledObject);
//					poop.Add(spawnId, pooledObject);
                    return pooledObject;
                   
                } else if(!onlyPooled) {
                    return Instantiate(objectPrefabs[i]) as GameObject;
                }
               
                break;
               
            }
        }
           
        //If we have gotten here either there was no object of the specified type or non were left in the pool with onlyPooled set to true
        return null;
    }
   
    /// <summary>
    /// Pools the object specified.  Will not be pooled if there is no prefab of that type.
    /// </summary>
    /// <param name='obj'>
    /// Object to be pooled.
    /// </param>
    public void PoolObject ( GameObject obj )
    {
        for ( int i=0; i<objectPrefabs.Length; i++)
        {
            if(objectPrefabs[i].name == obj.name)
            {
//				obj.name = spawnid.ToString();
                obj.SetActiveRecursively(false);
                obj.transform.parent = containerObject.transform;
                pooledObjects[i].Add(obj);
                return;
            }
        }
    }
   
}