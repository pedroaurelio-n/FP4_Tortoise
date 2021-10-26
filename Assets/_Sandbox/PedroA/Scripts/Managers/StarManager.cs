using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarManager : MonoBehaviour
{
    public delegate void StarCountChange(int newCount);
    public static event StarCountChange onStarCountChange;

    private static int _STARCOUNT;
    private int _starCount;

    private void Awake()
    {
        _starCount = 0;
        _STARCOUNT = _starCount;
    }

    public static int GetStarCount()
    {
        return _STARCOUNT;
    }

    private void Start()
    {
        if (onStarCountChange != null)
            onStarCountChange(_starCount);
    }

    private void UpdateStarCount(int value)
    {
        _starCount += value;
        _STARCOUNT = _starCount;

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
