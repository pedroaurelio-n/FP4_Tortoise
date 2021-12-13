using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Tayx.Graphy;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static bool canInput;

    [SerializeField] private bool lockMouse;
    [SerializeField] private bool showGraphy;
    
    private GameObject graphy;

    private void Awake()
    {
        Instance = this;

        canInput = true;

        graphy = GraphyManager.Instance.gameObject;

        if (lockMouse)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    private void Start()
    {
        graphy = GraphyManager.Instance.gameObject;
        graphy.SetActive(DataManager.Instance.Data.FpsCounter);
    }

    public void ShowGraphy(bool value)
    {
        graphy.SetActive(value);
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
