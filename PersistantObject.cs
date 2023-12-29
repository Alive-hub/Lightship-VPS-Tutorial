using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistantObject : MonoBehaviour
{
    [SerializeField] private string prefabId;

    public string PrefabID
    {
        get => prefabId;
        set => prefabId = value;
    }
    
    
    private string objectUUID;

    public string ObjectUUID
    {
        set => objectUUID = value;
        get => objectUUID;
    }


    public static event Action<PersistantObjectData> myPersistantData; 
    
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        if(string.IsNullOrEmpty(objectUUID))
            objectUUID = CreateUUID();


        SaveLoadAllObjects.saveEvent += SaveData;
    }

    private void OnDestroy()
    {
        SaveLoadAllObjects.saveEvent -= SaveData;

    }

    string CreateUUID()
    {
       return Guid.NewGuid().ToString();
    }

    void SaveData(string ARLocation)
    {
        PersistantObjectData objectData = new PersistantObjectData(
            ARLocation,
            prefabId,
            objectUUID,
            transform.position,
            transform.localScale,
            transform.rotation
        );
        
        myPersistantData?.Invoke(objectData);
    }
}


public struct PersistantObjectData
{
    public string _locationID;
    public string _prefabID;
    public string _uuid;

    public Vector3 _position;
    public Vector3 _localScale;
    public Quaternion _rotation;

    public PersistantObjectData(string locationID, string prefabID, string uuid, Vector3 position, Vector3 localScale,
        Quaternion rotation)
    {
        _locationID = locationID;
        _prefabID = prefabID;
        _uuid = uuid;
        _position = position;
        _localScale = localScale;
        _rotation = rotation;
    }

}
