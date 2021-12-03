using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckMaxHeight : MonoBehaviour
{
    public PlayerGroundCheck groundCheck;
    public UI_DevStats devStats;

    private float _height;
    private float _maxHeight;
    private float _airTime;

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
