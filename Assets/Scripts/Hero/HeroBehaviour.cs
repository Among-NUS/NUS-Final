using UnityEngine;
using System.Collections.Generic;

public class HeroBehaviour : MonoBehaviour
{
    public float speed = 3f;

    void FixedUpdate()
    {
        if(GameManager.Instance.currentPhase == GameManager.GamePhase.TimeStop)
        {
            // 停止时不处理输入
            return;
        }
        HandleMovement();

        if (GameManager.Instance.currentPhase == GameManager.GamePhase.Recording)
            GameManager.Instance.RecordKeyInput(CollectKeyInputs());
    }

    void Update()
    {
        if (GameManager.Instance.currentPhase == GameManager.GamePhase.TimeStop)
        {
            HandleMovement();  // 停止时仍然允许移动
        }
    }
    void HandleMovement()
    {
        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.A)) move += Vector3.left;
        if (Input.GetKey(KeyCode.D)) move += Vector3.right;

        float dt = (GameManager.Instance.currentPhase == GameManager.GamePhase.TimeStop)? Time.unscaledDeltaTime : Time.deltaTime; // ★ 停时用真实帧长
        transform.position += move * speed * dt;
    }

    List<char> CollectKeyInputs()
    {
        List<char> keys = new();
        if (Input.GetKey(KeyCode.A)) keys.Add('a');
        if (Input.GetKey(KeyCode.D)) keys.Add('d');
        if (Input.GetKey(KeyCode.W)) keys.Add('w');   // ⬅ 新增
        if (Input.GetKey(KeyCode.S)) keys.Add('s');   // ⬅ 新增
        return keys;
    }
}
