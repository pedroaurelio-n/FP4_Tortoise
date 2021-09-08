using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerAnimatorManager animatorManager;

    public bool isInteracting;

    private void Update()
    {
        inputManager.HandleAllInputs();
    }

    private void FixedUpdate()
    {
        playerMovement.HandleAllMovement();
    }

    private void LateUpdate()
    {
        isInteracting = animatorManager.GetInteractionBool();
        playerMovement.isJumping = animatorManager.GetJumpingBool();
        animatorManager.SetGroundedBool(playerMovement.isGrounded);
    }
}
