using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using EQBrowser;
 
public class Draggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
        public Transform target;
        private bool isMouseDown = false;
        private Vector3 startMousePosition;
        private Vector3 startPosition;
		private Vector3 resetPosition;
 
        // Use this for initialization
        void Start () 
		{
            resetPosition = target.position;
        }
 
        public void OnPointerDown(PointerEventData dt) {
                isMouseDown = true;
 
//                Debug.Log ("Draggable Mouse Down");
 
                startMousePosition = Input.mousePosition;
	            startPosition = target.position;
				
        }
 
        public void OnPointerUp(PointerEventData dt) {
//                Debug.Log ("Draggable mouse up");
 
                isMouseDown = false;
 
        }
		
		public void Reset(GameObject panelName)
		{
			panelName.transform.position = resetPosition;
		}
		
 
        // Update is called once per frame
        void Update () {
                if (isMouseDown) {
						Vector3 currentPosition = Input.mousePosition;
 
                        Vector3 diff = currentPosition - startMousePosition;
 
                        Vector3 pos = startPosition + diff;
 
                        target.position = pos;
                }
        }
}