using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Niantic.Lightship.AR.LocationAR;
using Niantic.Lightship.AR.PersistentAnchors;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class SaveLoadAllObjects : MonoBehaviour
{
    [SerializeField] private Button saveButton;

    public static event Action<string> saveEvent;

    private ARLocationManager _arLocationManager;
    private ARLocation _lastARLocation;

    private List<GameObject> spawnedObjects;
    [SerializeField] private ScriptableObjectID _scriptableObject;
    private Transform currentParentTransform; 
    
    // Start is called before the first frame update
    void Start()
    {
        _arLocationManager = FindObjectOfType<ARLocationManager>();
        saveButton.onClick.AddListener(() => saveEvent?.Invoke(_arLocationManager.ARLocations[0].Payload.ToBase64()));
        PersistantObject.myPersistantData += SaveData;
        _arLocationManager.locationTrackingStateChanged += CheckIfShouldLoadData;
    }

    private void OnDisable()
    {
        saveButton.onClick.RemoveAllListeners();
        PersistantObject.myPersistantData -= SaveData;
        _arLocationManager.locationTrackingStateChanged -= CheckIfShouldLoadData;
    }

    void CheckIfShouldLoadData(ARLocationTrackedEventArgs eventArgs)
    {
        Debug.Log("Check if should load data");
        
        if(!eventArgs.Tracking)
            return;
        
        if (_lastARLocation == null || eventArgs.ARLocation != _lastARLocation)
        {
            Debug.Log("CheckIfLoadData too");
            if(eventArgs.ARLocation != null)
                LoadData(eventArgs.ARLocation);
        }
        else
        {
            Debug.Log("Location was already loaded");
        }
    }


    void LoadData(ARLocation location)
    {
        Debug.Log("Loading Data....");
        
        string pathToLoad = Application.persistentDataPath;
        string folderToLoad = location.Payload.ToBase64();
        string combinedPath = Path.Combine(pathToLoad + "/" + folderToLoad);

        
        currentParentTransform = FindObjectOfType<ARLocation>().transform;

        List<PersistantObjectData> objectsToSpawn = new();

        if (Directory.Exists(combinedPath))
        {
            foreach (var file in Directory.GetFiles(combinedPath))
            {
                string readFile = File.ReadAllText(file);
                objectsToSpawn.Add(JsonUtility.FromJson<PersistantObjectData>(readFile));
            }

            foreach (var spawnObject in objectsToSpawn)
            {
                SpawnObjectsFromPersistantObjectData(spawnObject);
            }
        }
        else
        {
            Debug.Log("Nothing has been placed here yet");
        }

    }

    
    //TODO: After scriptable object, spawn objects 
    void SpawnObjectsFromPersistantObjectData(PersistantObjectData data)
    {
        spawnedObjects = new();

        GameObject toSpawn = Instantiate(_scriptableObject.ReturnObjectByID(data._prefabID));
        toSpawn.GetComponent<PersistantObject>().ObjectUUID = data._uuid;

        if (currentParentTransform != null)
        {
            toSpawn.transform.parent = currentParentTransform;
        }
        
        toSpawn.transform.position = data._position;
        toSpawn.transform.rotation = data._rotation;
        toSpawn.transform.localScale = data._localScale;
    }


    void SaveData(PersistantObjectData objectData)
    {
        string pathToSave = Application.persistentDataPath;
        string folderToSave = objectData._locationID;
        string combinedPath = Path.Combine(pathToSave + "/" + folderToSave);
        
        
        if(Directory.Exists(combinedPath))
        {
            File.WriteAllText(combinedPath + "/" + $"{objectData._uuid}.json", JsonUtility.ToJson(objectData));
        }
        else
        {
            Directory.CreateDirectory(combinedPath);
            File.WriteAllText(combinedPath + "/" +  $"{objectData._uuid}.json", JsonUtility.ToJson(objectData));
        }
        
        Debug.Log($"Saving {objectData._prefabID} to persistantData");
        
                
#if UNITY_EDITOR
        OpenFolderInFinder(pathToSave);
#endif

    }
    
    public void OpenFolderInFinder(string folderPath)
    {
        // Ensure the folder path is correct and exists
        if (System.IO.Directory.Exists(folderPath))
        {
            // Open the folder
            Process.Start(new ProcessStartInfo()
            {
                FileName = folderPath,
                UseShellExecute = true,
                Verb = "open"
            });
        }
        else
        {
            Debug.LogError("Folder not found: " + folderPath);
        }
    }
}
