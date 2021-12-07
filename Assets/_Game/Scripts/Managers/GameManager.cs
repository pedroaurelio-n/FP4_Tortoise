using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static bool canInput;

    [SerializeField] private bool lockMouse;
    [SerializeField] private bool showGraphy;
    [SerializeField] private GameObject graphy;

    private void Awake()
    {
        Instance = this;

        canInput = true;
        
        graphy.SetActive(showGraphy);

        if (lockMouse)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    public void LockMouse(bool value)
    {
        if (value)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
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
