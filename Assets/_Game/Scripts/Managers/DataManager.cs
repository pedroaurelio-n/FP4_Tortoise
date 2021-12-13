using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public DataHolder Data;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void ChangeSFXVolume(float value)
    {
        Data.ChangeSFXVolume(value);
        AudioManager.Instance.ChangeVolume(value);
    }

    public void ChangeMusicVolume(float value)
    {
        Data.ChangeMusicVolume(value);
        MusicManager.Instance.ChangeVolume(value);
    }

    public void ChangeFpsCounter(bool value)
    {
        Data.ChangeFpsCounter(value);
        GameManager.Instance.ShowGraphy(value);
    }

    public void Test()
    {
        Debug.Log("test");
    }
}
