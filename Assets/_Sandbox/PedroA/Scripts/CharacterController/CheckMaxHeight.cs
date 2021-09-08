using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckMaxHeight : MonoBehaviour
{
    public PlayerMovement2 playerMovement;

    float height;
    float maxHeight;

    private void Update()
    {
        if (!playerMovement.isGrounded)
        {

            height = transform.position.y;

            if (height > maxHeight)
                maxHeight = height;
        }

        else
        {
            if (maxHeight != 0)
                Debug.Log(maxHeight - transform.position.y);

            maxHeight = 0;
        }
    }
}
