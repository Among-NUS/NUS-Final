using UnityEngine;
using System.Collections.Generic;

public class HeroBehaviour : MonoBehaviour
{
    public float speed = 3f;

    void FixedUpdate()
    {
        if (GameManager.Instance.currentPhase == GameManager.GamePhase.TimeStop)
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
        if (GameManager.Instance.currentPhase != GameManager.GamePhase.TimeStop)
            return;

        HandleMovement();
        /* ★ 手动检测楼梯 ★ */
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
        {
            bool wantUp = Input.GetKeyDown(KeyCode.W);
            TryUseStairs(wantUp);
        }
    }
    void HandleMovement()
    {
        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.A)) move += Vector3.left;
        if (Input.GetKey(KeyCode.D)) move += Vector3.right;

        float dt = (GameManager.Instance.currentPhase == GameManager.GamePhase.TimeStop) ? Time.unscaledDeltaTime : Time.deltaTime; // ★ 停时用真实帧长
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
    public void TryUseStairs(bool wantUp)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.05f);
        foreach (var h in hits)
        {
            StairsBehaviour stairs = h.GetComponent<StairsBehaviour>();
            if (stairs != null)
            {
                stairs.TryTeleport(transform, wantUp);   // 与 Ghost 同一接口
                break;      // 够了
            }
        }
    }
}
