using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_DevStats : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private CharacterController playerCharacter;
    [SerializeField] private PlayerGroundCheck groundCheck;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private TMP_Text hVelocity;
    [SerializeField] private TMP_Text vVelocity;
    [SerializeField] private TMP_Text jumpHeight;
    [SerializeField] private TMP_Text airTimeText;
    [SerializeField] private TMP_Text grounded;
    [SerializeField] private TMP_Text sprinting;
    [SerializeField] private TMP_Text gliding;

    private Vector2 horizontalVelocity;

    private void Update()
    {
        horizontalVelocity = new Vector2(playerCharacter.velocity.x, playerCharacter.velocity.z);

        hVelocity.text = horizontalVelocity.magnitude.ToString("0.00");
        vVelocity.text = playerCharacter.velocity.y.ToString("0.00");
        grounded.text = groundCheck.isGrounded.ToString();
        sprinting.text = playerMovement.isSprinting.ToString();
        gliding.text = playerMovement.isGliding.ToString();
    }

    public void ChangeJumpHeight(float height)
    {
        jumpHeight.text = height.ToString("0.00");
    }

    public void ChangeAirTime(float airTime)
    {
        airTimeText.text = airTime.ToString("0.00");
    }
}
