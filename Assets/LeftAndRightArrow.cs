using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftAndRightArrow : MonoBehaviour
{
    public float moveSpeed = 2f;  // Speed of movement
    public float moveRange = 3f;  // How far it moves up and down
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;  // Save the initial position
    }

    void Update()
    {
        // Calculate the new position using Mathf.Sin for smooth up/down motion
        float newX = startPosition.x + Mathf.Sin(Time.time * moveSpeed) * moveRange;
        transform.position = new Vector3(newX,transform.position.y , transform.position.z);
    }
}