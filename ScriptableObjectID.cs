using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;


[CreateAssetMenu(fileName = "ScriptablePrefabID", menuName = "ScriptableObjects/ScriptablePrefabID")]
public class ScriptableObjectID : ScriptableObject
{
    [SerializeField] private List<GameObject> persistantObjects;

    private Dictionary<string, GameObject> keyToObject = new(); 

    public void OnEnable()
    {
        if(persistantObjects.Count == 0){ return; }
        
        foreach (var objects in persistantObjects)
        { 
            keyToObject.Add(objects.GetComponent<PersistantObject>().PrefabID, objects);
            Debug.Log($"Adding Object to dict {objects.GetComponent<PersistantObject>().PrefabID} + {objects.name}");
        }
    }

    public GameObject ReturnObjectByID(string id)
    {
        return keyToObject[id];
    }
}
