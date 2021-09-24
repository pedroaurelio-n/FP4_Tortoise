using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMain : MonoBehaviour
{
    [Header("Script Components")]
    public PlayerMovement PlayerMovement;
    public InputManager PlayerInputManager;
    public PlayerAnimationManager PlayerAnimationManager;

    private void Update()
    {
        PlayerInputManager.HandleAllInputs();
        PlayerMovement.HandleUpdateMovements();
    }

    private void FixedUpdate()
    {
        PlayerMovement.HandleFixedUpdateMovements();
    }

    private void LateUpdate()
    {
        PlayerMovement.HandleLateUpdateMovements();
    }
}
