using UnityEngine;
using System.Collections.Generic;

public class HeroBehaviour : MonoBehaviour
{
    public float speed = 0.1f;

    void FixedUpdate()
    {
        HandleMovement();

        if (GameManager.Instance.currentPhase == GameManager.GamePhase.Recording)
            GameManager.Instance.RecordKeyInput(CollectKeyInputs());
    }


    void HandleMovement()
    {
        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.A)) move += Vector3.left;
        if (Input.GetKey(KeyCode.D)) move += Vector3.right;

       
        transform.position += move * speed ;
    }

    List<char> CollectKeyInputs()
    {
        List<char> keys = new();
        if (Input.GetKey(KeyCode.A)) keys.Add('a');
        if (Input.GetKey(KeyCode.D)) keys.Add('d');
        if (Input.GetKey(KeyCode.W)) keys.Add('w');   // ⬅ 新增
        if (Input.GetKey(KeyCode.S)) keys.Add('s');   // ⬅ 新增
        if (Input.GetKey(KeyCode.E)) keys.Add('e');   // ⬅ 新增
        return keys;
    }
    
}
