using UnityEngine;
using System.Collections.Generic;

public class HeroBehaviour : MonoBehaviour
{
    public float speed = 0.1f;

    void FixedUpdate()
    {
        HandleMovement();

        if (GameManager.Instance.IsRecording)
            GameManager.Instance.RecordKeyInput(CollectKeyInputs());
    }

    void HandleMovement()
    {
        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.A)) move += Vector3.left;
        if (Input.GetKey(KeyCode.D)) move += Vector3.right;

        transform.position += move * speed;
    }

    List<char> CollectKeyInputs()
    {
        List<char> keys = new();
        if (Input.GetKey(KeyCode.A)) keys.Add('a');
        if (Input.GetKey(KeyCode.D)) keys.Add('d');
        return keys;
    }
}
