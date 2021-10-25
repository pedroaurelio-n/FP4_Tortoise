using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
