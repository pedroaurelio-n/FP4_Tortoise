using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckMaxHeight : MonoBehaviour
{
    public PlayerMain player;

    private UI_DevStats devStats;
    private PlayerGroundCheck groundCheck;

    private float _height;
    private float _maxHeight;
    private float _airTime;

    private void Awake()
    {
        devStats = GetComponent<UI_DevStats>();
        groundCheck = player.gameObject.GetComponentInChildren<PlayerGroundCheck>();
    }

    private void Start()
    {
        if (groundCheck == null)
            Debug.LogException(new System.Exception("PlayerGroundCheck couldn't be found."));
    }

    private void Update()
    {
        if (!groundCheck.isGrounded)
        {
            _airTime += Time.deltaTime;
            devStats.ChangeAirTime(_airTime);

            _height = transform.position.y;

            if (_height > _maxHeight)
                _maxHeight = _height;
        }

        else
        {
            if (_maxHeight != 0)
                devStats.ChangeJumpHeight(_maxHeight - transform.position.y);

            _maxHeight = 0;

            _airTime = 0;
        }
    }
}
