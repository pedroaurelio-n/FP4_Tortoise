using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool lockMouse;

    private void Awake()
    {
        if (lockMouse)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}
