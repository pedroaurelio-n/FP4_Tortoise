using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class UI_DevStats : MonoBehaviour
{
    [SerializeField] private TMP_Text hVelocity;
    [SerializeField] private TMP_Text vVelocity;
    [SerializeField] private TMP_Text availableJumps;
    [SerializeField] private TMP_Text jumpHeight;
    [SerializeField] private TMP_Text airTimeText;
    [SerializeField] private TMP_Text grounded;
    [SerializeField] private TMP_Text sprinting;
    [SerializeField] private TMP_Text gliding;
    [SerializeField] private TMP_Text currentInput;
    [SerializeField] private TMP_Text cameraSensitivity;
    [SerializeField] private TMP_Text cameraXSpeed;
    [SerializeField] private TMP_Text cameraYSpeed;

    private CheckMaxHeight checkMaxHeight;
    private PlayerMovement playerMovement;
    private PlayerGroundCheck groundCheck;
    private Rigidbody playerRb;
    private CinemachineInputChange cinemachineInputChange;

    private Vector2 horizontalVelocity;

    private void Awake()
    {
        checkMaxHeight = GetComponent<CheckMaxHeight>();
        playerMovement = checkMaxHeight.player.GetComponent<PlayerMovement>();
        groundCheck = checkMaxHeight.player.GetComponentInChildren<PlayerGroundCheck>();
        playerRb = checkMaxHeight.player.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        cinemachineInputChange = GameManager.Instance.freeLookCamera.GetComponent<CinemachineInputChange>();
        if (playerMovement == null)
            Debug.LogException(new System.Exception("PlayerMovement couldn't be found."));
        
        if (groundCheck == null)
            Debug.LogException(new System.Exception("PlayerGroundCheck couldn't be found."));
        
        if (playerRb == null)
            Debug.LogException(new System.Exception("PlayerRigidBody couldn't be found."));
    }

    private void Update()
    {
        horizontalVelocity = new Vector2(playerMovement.GetPlayerVelocity().x, playerMovement.GetPlayerVelocity().z);

        hVelocity.text = horizontalVelocity.magnitude.ToString("0.00");
        vVelocity.text = playerMovement.GetPlayerVelocity().y.ToString("0.00");
        availableJumps.text = playerMovement.GetAvalilableJumps().ToString();
        grounded.text = groundCheck.isGrounded.ToString();
        sprinting.text = playerMovement.isSprinting.ToString();
        gliding.text = playerMovement.isGliding.ToString();
        cameraSensitivity.text = cinemachineInputChange.CameraSensitivity.ToString("0.0");
        cameraXSpeed.text = cinemachineInputChange.GetCameraSpeed().x.ToString("0.000");
        cameraYSpeed.text = cinemachineInputChange.GetCameraSpeed().y.ToString("0.000");
    }

    public void ChangeJumpHeight(float height)
    {
        jumpHeight.text = height.ToString("0.00");
    }

    public void ChangeAirTime(float airTime)
    {
        airTimeText.text = airTime.ToString("0.00");
    }

    private void ChangeCurrentControlScheme(PlayerInput input)
    {
        var currentControlScheme = input.currentControlScheme;

        switch (currentControlScheme)
        {
            case "KeyboardMouse":
                currentInput.text = "Keyboard Mouse";
                break;
            
            case "Gamepad":
                currentInput.text = "Gamepad";
                break;

            default:
                throw new System.Exception("Control scheme not defined.");
        }
    }

    private void OnEnable()
    {
        PlayerInputManager.onControlSchemeChange += ChangeCurrentControlScheme;
    }

    private void OnDisable()
    {
        PlayerInputManager.onControlSchemeChange -= ChangeCurrentControlScheme;
    }
}
