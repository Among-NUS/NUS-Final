using UnityEngine;
using System.Collections.Generic;

public class HeroBehaviour : MonoBehaviour
{
    public Animator heroAnimator;
    public float layerChangeCooldown = 2f; //for elevator animation
    private float lastCooldown;
    public int nearLayer = 100;
    public int farLayer = -1;
    [SerializeField] private SpriteRenderer heroSR;
    public float speed = 0.1f;
    private bool heroIsWalking = false;  // ← 用于记录是否在行走

    private Shooter shooter;
    private bool facingLeft = true;       // ← 记录当前朝向

    void Awake()
    {
        shooter = GetComponentInChildren<Shooter>();
        heroSR = GetComponent<SpriteRenderer>();
        Debug.Assert(heroAnimator != null);
    }

    void FixedUpdate()
    {
        if (Time.time - lastCooldown > layerChangeCooldown)
        {
            setLayerNear();
        }
        HandleMovement();

        // J 键开火（与上次示例一致）
        if (Input.GetKey(KeyCode.J) && shooter != null && shooter.CanFire())
        {
            shooter.faceLeft = facingLeft;
            shooter.Fire();
            heroAnimator.SetTrigger("shootAnim");
        }

        if (GameManager.Instance.currentPhase == GameManager.GamePhase.Recording)
            GameManager.Instance.RecordKeyInput(CollectKeyInputs());
    }

    void HandleMovement()
    {
        heroIsWalking = false;  // 每次更新前重置
        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.A)) { move += Vector3.left; facingLeft = true; heroIsWalking = true; }
        if (Input.GetKey(KeyCode.D)) { move += Vector3.right; facingLeft = false; heroIsWalking = true; }
        heroAnimator.SetBool("isWalking", heroIsWalking);

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

    void OnTriggerEnter2D(Collider2D collision)
    {
        BulletBehaviour bullet = collision.GetComponent<BulletBehaviour>();
        if (bullet != null && bullet.isEnemy)
        {
            Debug.Log("Hero hit by enemy bullet");
            FindObjectOfType<GameOverUI>().ShowGameOver();
        }
    }

    public void setLayerFar()
    {
        heroSR.sortingOrder = farLayer;
        lastCooldown = Time.time;
    }
    public void setLayerNear()
    {
        heroSR.sortingOrder = nearLayer;
    }

}
