using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;

public class PlayerInputSetReferences : MonoBehaviour
{
    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        if (_playerInput == null)
            Debug.LogException(new System.Exception("PlayerInput couldn't be found."));
        
        if (_playerInput.uiInputModule == null)
            _playerInput.uiInputModule = EventSystem.current.GetComponent<InputSystemUIInputModule>();
        
        if (_playerInput.camera == null)
            _playerInput.camera = Camera.main;
    }
}
