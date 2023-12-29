using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacements : MonoBehaviour
{
   // Start is called before the first frame update
    private ARRaycastManager arRaycastManager;
    [SerializeField] private GameObject instantiatedObject; 
    private List<GameObject> instantiatedObjects = new();
    private Camera mainCam;
    
    

    private void Start()
    {
        mainCam = FindObjectOfType<Camera>();
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("UI Hit was recognized");
                return;
            }
            TouchToRay(Input.mousePosition);
        }
#endif
#if UNITY_IOS || UNITY_ANDROID
        
        if (Input.touchCount > 0 && Input.touchCount < 2 &&
            Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Touch touch = Input.GetTouch(0);
            
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = touch.position;

            List<RaycastResult> results = new List<RaycastResult>();

            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count > 0) {
                // We hit a UI element
                Debug.Log("We hit an UI Element");
                return;
            }
            
            Debug.Log("Touch detected, fingerId: " + touch.fingerId);  
            
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                Debug.Log("Is Pointer Over GOJ, No placement ");
                return;
            }
            TouchToRay(touch.position);
        }
#endif
    }
    
    void TouchToRay(Vector3 touch)
    {
        Ray ray = mainCam.ScreenPointToRay(touch);
        List<ARRaycastHit> hits = new();

        arRaycastManager.raycastPrefab = instantiatedObject; 
        arRaycastManager.Raycast(ray, hits, TrackableType.PlaneEstimated);
        
        Debug.Log("ShootingRay");
        //
        if (hits.Count > 0)
        {
            GameObject placeObject = Instantiate(instantiatedObject);
            placeObject.transform.position = hits[0].pose.position;
            placeObject.transform.rotation = hits[0].pose.rotation;
            instantiatedObjects.Add(placeObject);
        }
    }
}
