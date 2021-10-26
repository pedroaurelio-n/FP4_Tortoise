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
    public PlayerInteractController PlayerInteractController;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ICollectable collectable))
        {
            collectable.Collect();
        }
    }
}
