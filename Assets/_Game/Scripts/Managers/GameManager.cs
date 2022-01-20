using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Cinemachine;
using Tayx.Graphy;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static bool canInput;

    public CinemachineFreeLook freeLookCamera;

    [SerializeField] private bool lockMouse;
    [SerializeField] private bool showGraphy;

    private void Awake()
    {
        Instance = this;

        canInput = true;

        if (lockMouse)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    private void Start()
    {
        GetGraphyInstance().SetActive(DataManager.Instance.Data.FpsCounter);
    }

    public void ShowGraphy(bool value)
    {
        GetGraphyInstance().SetActive(value);
    }

    private GameObject GetGraphyInstance()
    {
        return GraphyManager.Instance.gameObject;
    }

    public void LockMouse(bool value)
    {
        if (value)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    private void Update()
    {
        if (Keyboard.current[Key.R].wasPressedThisFrame)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
