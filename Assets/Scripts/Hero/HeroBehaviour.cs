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
    public float jumpForce = 5f;
    private bool heroIsWalking = false;  // ← 用于记录是否在行走

    private Shooter shooter;
    private StairsBehaviour currentStairs = null;
    public bool facingLeft = true;       // ← 记录当前朝向
    private Rigidbody2D heroRigidBody;
    private bool isOnGround = true;
    private bool isJumping = false;
    private int groundLayer = 13;
    public bool jumpBuffer = false;

    void Awake()
    {
        shooter = GetComponentInChildren<Shooter>();
        heroSR = GetComponent<SpriteRenderer>();
        Debug.Assert(heroAnimator != null);
        heroRigidBody = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBuffer = true;
        }
    }

    void FixedUpdate()
    {
        if (Time.time - lastCooldown > layerChangeCooldown)
        {
            setLayerNear();
        }
        HandleMovement();
        bool consumedJump = HandleJump();

        heroAnimator.SetBool("isJumping", heroRigidBody.velocity.y > 0.01);
        heroAnimator.SetBool("isFalling", heroRigidBody.velocity.y < -0.01);

        // J 键开火（与上次示例一致）
        if (Input.GetKey(KeyCode.J) && shooter != null && shooter.CanFire() && GameManager.Instance.currentPhase != GameManager.GamePhase.TimeStop)
        {
            shooter.faceLeft = facingLeft;
            shooter.Fire();
            heroAnimator.SetTrigger("shootAnim");
        }
        if (currentStairs)
        {
            if (Input.GetKey(KeyCode.W)) currentStairs.TryTeleport(transform, true);
            if (Input.GetKey(KeyCode.S)) currentStairs.TryTeleport(transform, false);
        }


        if (GameManager.Instance.currentPhase == GameManager.GamePhase.Recording)
            GameManager.Instance.RecordKeyInput(CollectKeyInputs(consumedJump));
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


    List<char> CollectKeyInputs(bool jumpThisFrame)
    {
        List<char> keys = new();
        if (Input.GetKey(KeyCode.A)) keys.Add('a');
        if (Input.GetKey(KeyCode.D)) keys.Add('d');
        if (Input.GetKey(KeyCode.W)) keys.Add('w');
        if (Input.GetKey(KeyCode.S)) keys.Add('s');
        if (Input.GetKey(KeyCode.E)) keys.Add('e');
        if (Input.GetKey(KeyCode.J)) keys.Add('j');
        if (jumpThisFrame) keys.Add(' ');
        return keys;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out StairsBehaviour stairs))
            currentStairs = stairs;
        BulletBehaviour bullet = collision.GetComponent<BulletBehaviour>();
        if (bullet != null && bullet.isEnemy && GameManager.Instance?.currentPhase != GameManager.GamePhase.TimeStop)
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == groundLayer) // Check if collision is with the ground layer
        {
            isOnGround = true;  // Reset grounded state when hitting the ground
            isJumping = false;  // Reset jumping state when on the ground


        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == groundLayer) // Check if collision is with the ground layer
        {
            isOnGround = false; // Set isOnGround to false when exiting the ground layer
        }
    }

    bool HandleJump()
    {
        bool didJump = false;
        bool wantJump = jumpBuffer;
        jumpBuffer = false;
        if (wantJump && isOnGround)
        {
            heroRigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isOnGround = false;
            isJumping = true;
            didJump = true;
        }
        return didJump;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out StairsBehaviour stairs) && stairs == currentStairs)
            currentStairs = null;
    }
}
