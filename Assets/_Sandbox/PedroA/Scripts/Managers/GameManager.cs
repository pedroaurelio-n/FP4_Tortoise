using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool lockMouse;
    [SerializeField] private bool showGraphy;
    [SerializeField] private GameObject graphy;

    private void Awake()
    {
        graphy.SetActive(showGraphy);

        if (lockMouse)
        {
            Cursor.visible = false;
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
