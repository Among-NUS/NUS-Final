using UnityEngine;
using System.Collections.Generic;

public class HeroBehaviour : MonoBehaviour
{
    public float speed = 0.1f;

    private Shooter shooter;
    private bool facingLeft = true;       // ← 记录当前朝向

    void Awake()
    {
        shooter = GetComponentInChildren<Shooter>();
    }

    void FixedUpdate()
    {
        HandleMovement();

        // J 键开火（与上次示例一致）
        if (Input.GetKey(KeyCode.J) && shooter != null && shooter.CanFire())
        {
            shooter.faceLeft = facingLeft;
            shooter.Fire();
        }

        if (GameManager.Instance.currentPhase == GameManager.GamePhase.Recording)
            GameManager.Instance.RecordKeyInput(CollectKeyInputs());
    }

    void HandleMovement()
    {
        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.A)) { move += Vector3.left; facingLeft = true; }
        if (Input.GetKey(KeyCode.D)) { move += Vector3.right; facingLeft = false; }

        /* --- 用 localScale.x 的正负代表左右 --- */
        Vector3 s = transform.localScale;
        float targetSign = facingLeft ? -1f : 1f;          // prefab 默认朝右；如默认朝左就反过来
        if (Mathf.Sign(s.x) != targetSign)                 // 只在朝向改变时改一次
        {
            s.x = Mathf.Abs(s.x) * targetSign;
            transform.localScale = s;
        }

        transform.position += move * speed;
    }


    List<char> CollectKeyInputs()
    {
        List<char> keys = new();
        if (Input.GetKey(KeyCode.A)) keys.Add('a');
        if (Input.GetKey(KeyCode.D)) keys.Add('d');
        if (Input.GetKey(KeyCode.W)) keys.Add('w');
        if (Input.GetKey(KeyCode.S)) keys.Add('s');
        if (Input.GetKey(KeyCode.E)) keys.Add('e');
        if (Input.GetKey(KeyCode.J)) keys.Add('j');
        return keys;
    }
}
