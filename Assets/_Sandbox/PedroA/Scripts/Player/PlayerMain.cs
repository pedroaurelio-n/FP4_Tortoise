using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMain : MonoBehaviour
{
    [Header("Script Components")]
    public PlayerMovement PlayerMovement;
    public PlayerInputManager PlayerInputManager;
    public PlayerAnimationManager PlayerAnimationManager;
    public PlayerCombatController PlayerCombatController;

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
