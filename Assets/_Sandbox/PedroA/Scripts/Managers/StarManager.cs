using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarManager : MonoBehaviour
{
    public delegate void StarCountChange(int newCount);
    public static event StarCountChange onStarCountChange;

    private int _starCount;

    private void Awake()
    {
        
        _starCount = 0;
    }

    private void Start()
    {
        if (onStarCountChange != null)
            onStarCountChange(_starCount);
    }

    private void UpdateStarCount(int value)
    {
        _starCount += value;

        if (onStarCountChange != null)
            onStarCountChange(_starCount);
    }

    private void OnEnable()
    {
        Star.onStarCollected += UpdateStarCount;
    }   

    private void OnDisable()
    {
        Star.onStarCollected -= UpdateStarCount;        
    } 
}
