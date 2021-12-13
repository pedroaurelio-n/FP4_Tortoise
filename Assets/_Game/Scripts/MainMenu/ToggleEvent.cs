using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ToggleEvent : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    public Toggle.ToggleEvent onValueChangedOverride;
 
    void Start()
    {
        toggle.isOn = DataManager.Instance.Data.FpsCounter;
        toggle.onValueChanged = onValueChangedOverride;
    }
}
