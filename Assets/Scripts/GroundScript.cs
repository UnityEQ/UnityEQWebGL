using UnityEngine;
using System.Collections;
 
//this is combined version of both previous scripts, one that was using raycasts
//for detecting colliders and terrains, and other that was using character controllers controller.isGronded value.
//adding raycast becase it takes too long for controller.isGrounded to
//finnish with calculations while the object was on terrain parts that had very low angled surface
 
[RequireComponent (typeof (CharacterController))]
public class GroundScript : MonoBehaviour {
   
    //simply getting controller.isGrounded was not working properly because the state from true to false was r
    //apidly swithing, solved it with a timer
 
    private float startTime;
    public float duration = 0.08f;
    private bool counterStarted = false;
    public bool isGrounded = false;
   
    public RaycastHit hit;                                  //use this if you want to acces objects that are hit with the raycast
    public float   distance   = 1.2f;                       //set this to go beyond your collider
    public Vector3 direction= new Vector3(0f, -1f, 0f);
   
    void Update(){
        isGrounded = IsGrounded();
    }
   
    void OnGUI(){
        float scrW = Screen.width, scrH = Screen.height;
        GUI.Label(new Rect(scrW/10f, scrH/3f   , 250f, 100f), "Grounded State:\t\t\t " + isGrounded.ToString()
                                                            + "\nIn Air Time:\t\t\t " + (CountTime() <= duration ? "0" : CountTime().ToString() ));
        GUI.Label(new Rect(scrW/10f, scrH*2f/3f, 250f, 100f), "We use one raycast in addition to .isGrounded, check scene window in playmode for debug raycast line");
    }
   
    public float CountTime(){
        return Time.time - startTime;
    }
 
    public bool IsGrounded(){
        return IsGroundedByCController() || IsGroundedByRaycast();      //this also doesn't call raycast if we know we are grounded
 
    }
 
    public bool IsGroundedByCController()
    {
        CharacterController controller = GetComponent<CharacterController>();
        if (controller.isGrounded == false){
            if(counterStarted == false){
                startTime = Time.time;
                counterStarted = true;
            }
        }
        else counterStarted = false;
 
        if(CountTime() > duration){
            return false;
        }
        return true;
    }
   
    public bool IsGroundedByRaycast(){
        Debug.DrawRay(transform.position, direction * distance, Color.green);       //draw the line to be seen in scene window
       
        if(Physics.Raycast(transform.position, direction, out hit, distance)){      //if we hit something
            return true;
        }
        return false;
    }
}