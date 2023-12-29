using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Niantic.Lightship.AR.LocationAR;
using Niantic.Lightship.AR.PersistentAnchors;
using Niantic.Lightship.AR.VpsCoverage;
using TMPro;
using UnityEngine;


public class ManageCoverageAndLocalizationList : MonoBehaviour
{
    [SerializeField] private GameObject VpsUiObject;
    [SerializeField] private Transform UIParent; 
    
    private List<GameObject> InstantiatedVPSUiObjects = new();
    private Dictionary<GameObject, AreaTarget> UiItemToTargetRef = new();
    private LatLng currentQueryPlayerLocation;
    private ARLocationManager _arLocationManager;

    private GameObject ARLocationObject;
    private bool isTracking = false; 
    
    void Start()
    {
        _arLocationManager = FindObjectOfType<ARLocationManager>();
        GetCoverageItems._Arearesults += HandleAreaResults; 
        UIItem_LocalizeNavigate.localizeMyVps += UIItem_LocalizeNavigateOnlocalizeMyVps;
        UIItem_LocalizeNavigate.navigateMyVps += UIItem_LocalizeNavigateOnnavigateMyVps;
        _arLocationManager.locationTrackingStateChanged += DebugOnTracking;
    }

    private void UIItem_LocalizeNavigateOnnavigateMyVps(GameObject obj)
    {
        AreaTarget areaTargetInfo = UiItemToTargetRef[obj];
        LatLng goalDestination = areaTargetInfo.Area.Centroid;
        
        Debug.Log("Navigating to " +  areaTargetInfo.Target.Name);

        OpenRouteInMapApp(currentQueryPlayerLocation, to: goalDestination);
    }
    
    private void OpenRouteInMapApp(LatLng from, LatLng to)
    {
        var sb = new StringBuilder();
        
        sb.Append("https://www.google.com/maps/dir/?api=1&origin=");
        sb.Append(from.Latitude);
        sb.Append("+");
        sb.Append(from.Longitude);
        sb.Append("&destination=");
        sb.Append(to.Latitude);
        sb.Append("+");
        sb.Append(to.Longitude);
        sb.Append("&travelmode=walking");

        Application.OpenURL(sb.ToString());
    }

    void StopLocalize()
    {
        _arLocationManager.StopTracking();
        isTracking = false;
        
        //When we don't localize any longer, destroy the object with a delay
        Destroy(ARLocationObject,5);
    }

    private void UIItem_LocalizeNavigateOnlocalizeMyVps(GameObject obj)
    {
        if (isTracking)
        {
            StopLocalize();
            return;
        }
        
        AreaTarget areaTargetInfo = UiItemToTargetRef[obj];

        ARLocationObject = new GameObject(areaTargetInfo.Target.Name);
        ARLocationObject.transform.parent = _arLocationManager.transform;

        var locationComponentHolder = ARLocationObject.AddComponent<ARLocation>();
        ARPersistentAnchorPayload locationPayload = new ARPersistentAnchorPayload(areaTargetInfo.Target.DefaultAnchor);
        

        locationComponentHolder.Payload = locationPayload; 
        
        _arLocationManager.SetARLocations(locationComponentHolder);
        
        Debug.Log("Setting AR location to " + locationComponentHolder.name);
        
        _arLocationManager.StartTracking();
        isTracking = true;
    }

    //For debugging
    void DebugOnTracking(ARLocationTrackedEventArgs args)
    {
        Debug.Log("Tracking state is changing to " + args.Tracking);
    }

    private void OnDestroy()
    {
        GetCoverageItems._Arearesults -= HandleAreaResults; 
    }

    void HandleAreaResults(LatLng playerPosition ,List<AreaTarget> areaTargets)
    {
        currentQueryPlayerLocation = playerPosition; 
        
        foreach (var target in areaTargets)
        {
            GameObject keyObject = CreateVisualRepresentation(target.Target.Name);
            UiItemToTargetRef.Add(keyObject, target);
        }
    }


    GameObject CreateVisualRepresentation(string name)
    {
        GameObject newVPSUIObjects = Instantiate(VpsUiObject, parent: UIParent);
        newVPSUIObjects.GetComponentInChildren<TMP_Text>().text = name.Substring(0, 10);
        
        InstantiatedVPSUiObjects.Add(newVPSUIObjects);
        
        return newVPSUIObjects;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
