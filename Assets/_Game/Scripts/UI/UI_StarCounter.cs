using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_StarCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text starCounter;

    private void UpdateStarCountUI(int newCount)
    {
        starCounter.text = newCount.ToString();
    }

    private void OnEnable()
    {        
        StarManager.onStarCountChange += UpdateStarCountUI;
    }

    private void OnDisable()
    {
        StarManager.onStarCountChange -= UpdateStarCountUI;
    }
}
