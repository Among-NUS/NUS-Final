using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostCameraBehaviour : MonoBehaviour
{
    public string ghostName = "GhostPrefab(Clone)";
    public Camera ghostCamera;
    public GameObject ghost = null;

    // Start is called before the first frame update
    void Awake()
    {
        ghostCamera = GetComponent<Camera>();
        if (ghostCamera == null)
        {
            Debug.LogError("GhostCameraBehaviour: No Camera component found on this GameObject.");
        }
        ghostCamera.enabled = false; // Disable the camera initially
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(ghost == null)
        {
            ghost = GameObject.Find(ghostName);
        }
        if (ghost == null)
        {
            ghostCamera.enabled = false; // Disable the camera if ghost is not found
            return;
        }
        else
        {
            ghostCamera.enabled = true; // Enable the camera if ghost is found
            //if the ghost is found, set the camera to follow it
            Vector3 ghostPosition = ghost.transform.position;
            transform.position = new Vector3(ghostPosition.x, ghostPosition.y, -10f);
        }
    }
}