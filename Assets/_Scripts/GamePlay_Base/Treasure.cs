using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour
{
    public bool isNotBlocked;

    void Update()
    {
        CheckRaycast();
    }

    void CheckRaycast()
    {
        // Set the origin of the ray to the position of the game object this script is attached to
        Vector3 origin = transform.position;

        // Set the direction of the ray to be -z relative to the game object's local space
        Vector3 direction = -transform.forward;

        // Shoot the raycast
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit))
        {
            // Check the tag of the first object hit by the raycast
            if (hit.collider.GetComponent<Block>())
            {
                isNotBlocked = false;
            }
            else
            {
                isNotBlocked = true;
            }
        }
        else
        {
            // If the raycast doesn't hit anything, you may want to handle that case as well
            isNotBlocked = true;
        }

        // For debugging purposes, you can visualize the raycast in the scene view
        Debug.DrawRay(origin, direction * 100, Color.red);
    }
}
