using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class CinemachineInputChange : MonoBehaviour
{
    [Range(0.1f, 3f)]
    [SerializeField] private float startCameraSensitivity = 1f;

    private CinemachineFreeLook freeLookCamera;

    [HideInInspector]
    public float CameraSensitivity;

    private float _normalYSpeed;
    private float _normalXSpeed;
    private string currentControlScheme;

    private void Awake()
    {
        freeLookCamera = GetComponent<CinemachineFreeLook>();
    }

    private void Start()
    {
        CameraSensitivity = startCameraSensitivity;
    }

    private void Update()
    {
        if (Keyboard.current.numpadPlusKey.wasPressedThisFrame)
        {
            CameraSensitivity += 0.1f;
            CameraSensitivity = Mathf.Clamp(CameraSensitivity, 0.1f, 3f);

            freeLookCamera.m_YAxis.m_MaxSpeed = _normalYSpeed * CameraSensitivity;
            freeLookCamera.m_XAxis.m_MaxSpeed = _normalXSpeed * CameraSensitivity;
        }

        if (Keyboard.current.numpadMinusKey.wasPressedThisFrame)
        {
            CameraSensitivity -= 0.1f;
            CameraSensitivity = Mathf.Clamp(CameraSensitivity, 0.1f, 2f);

            freeLookCamera.m_YAxis.m_MaxSpeed = _normalYSpeed * CameraSensitivity;
            freeLookCamera.m_XAxis.m_MaxSpeed = _normalXSpeed * CameraSensitivity;
        }
    }

    public void ChangeCameraSettingsByControlScheme(PlayerInput input)
    {
        var currentControlScheme = input.currentControlScheme;

        switch (currentControlScheme)
        {
            case "KeyboardMouse":
                Debug.Log("User on keyboard and mouse");
                ApplyMouseCameraSettings();
                break;
            
            case "Gamepad":
                Debug.Log("User on gamepad");
                ApplyGamepadCameraSettings();
                break;

            default:
                throw new System.Exception("Control scheme not defined.");
        }
    }

    private void ApplyMouseCameraSettings()
    {
        _normalYSpeed = 0.002f;
        _normalXSpeed = 0.15f;

        freeLookCamera.m_YAxis.m_SpeedMode = AxisState.SpeedMode.InputValueGain;
        freeLookCamera.m_XAxis.m_SpeedMode = AxisState.SpeedMode.InputValueGain;

        freeLookCamera.m_YAxis.m_MaxSpeed = _normalYSpeed * CameraSensitivity;
        freeLookCamera.m_XAxis.m_MaxSpeed = _normalXSpeed * CameraSensitivity;
    }

    private void ApplyGamepadCameraSettings()
    {
        _normalYSpeed = 1f;
        _normalXSpeed = 110f;

        freeLookCamera.m_YAxis.m_SpeedMode = AxisState.SpeedMode.MaxSpeed;
        freeLookCamera.m_XAxis.m_SpeedMode = AxisState.SpeedMode.MaxSpeed;

        freeLookCamera.m_YAxis.m_MaxSpeed = _normalYSpeed * CameraSensitivity;
        freeLookCamera.m_XAxis.m_MaxSpeed = _normalXSpeed * CameraSensitivity;
    }

    public Vector2 GetCameraSpeed()
    {
        return new Vector2(freeLookCamera.m_XAxis.m_MaxSpeed, freeLookCamera.m_YAxis.m_MaxSpeed);
    }

    private void OnEnable()
    {
        PlayerInputManager.onControlSchemeChange += ChangeCameraSettingsByControlScheme;
    }

    private void OnDisable()
    {
        PlayerInputManager.onControlSchemeChange -= ChangeCameraSettingsByControlScheme;
    }
}
