using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMain2 : MonoBehaviour
{
    [Header("Script Components")]
    public PlayerMovement2 PlayerMovement;
    public InputManager2 PlayerInputManager;
    public PlayerAnimationManager2 PlayerAnimationManager;

    private void Update()
    {
        PlayerInputManager.HandleAllInputs();
        PlayerMovement.HandleUpdateMovements();
    }

    private void FixedUpdate()
    {
        PlayerMovement.HandleFixedUpdateMovements();
    }
}
