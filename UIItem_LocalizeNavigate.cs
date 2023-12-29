using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIItem_LocalizeNavigate : MonoBehaviour
{
    [SerializeField] private Button localizeButton;

    [SerializeField] private Button navigateButton;

    public static event Action<GameObject> localizeMyVps;
    public static event Action<GameObject> navigateMyVps;

    private void Start()
    {
        localizeButton.onClick.AddListener(() => localizeMyVps?.Invoke(this.gameObject));
        navigateButton.onClick.AddListener(() => navigateMyVps?.Invoke(this.gameObject));
    }
}
