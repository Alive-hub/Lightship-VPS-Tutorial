using System;
using System.Collections;
using System.Collections.Generic;
using Niantic.Lightship.AR.VpsCoverage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(CoverageClientManager))]
public class GetCoverageItems : MonoBehaviour
{
    private CoverageClientManager _coverageClientManager;
    [SerializeField] private Button _sendQueryButton;
    [SerializeField] private Slider _queryRadiusSlider;
    [SerializeField] private TMP_Text _queryRadiusText; 
    
    private int queryRadius;

    private List<AreaTarget> _areaTargets;
    public static event Action<LatLng, List<AreaTarget>> _Arearesults;
    private LatLng currentRequestCoords;

    public LatLng CurrentRequestCoords
    {
        get => currentRequestCoords;
        set => currentRequestCoords = value; 
    }
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        _coverageClientManager = GetComponent<CoverageClientManager>();
        _sendQueryButton.onClick.AddListener(() => SendQuery());
    }


    void CreateQuery()
    {
        _coverageClientManager.QueryRadius = queryRadius;
        _coverageClientManager.UseCurrentLocation = true; 
    }


    void SendQuery()
    {
        CreateQuery();
        _coverageClientManager.TryGetCoverage(OnResponse);
    }

    void OnResponse(AreaTargetsResult _areaTargetsResult)
    {
        _areaTargets = _areaTargetsResult.AreaTargets;
        currentRequestCoords = _areaTargetsResult.QueryLocation; 
        
        _Arearesults?.Invoke(currentRequestCoords ,_areaTargets);

        foreach (var target in _areaTargets)
        {
            Debug.Log(target.Target.Name);
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        queryRadius = (int)_queryRadiusSlider.value;
        _queryRadiusText.text = queryRadius.ToString();
    }
}
