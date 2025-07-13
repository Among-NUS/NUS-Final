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
    private bool facingLeft = true;       // ← 记录当前朝向
    private Rigidbody2D heroRigidBody;
    private bool isOnGround = true;
    private bool isJumping = false;
    private int groundLayer = 13;
    void Awake()
    {
        shooter = GetComponentInChildren<Shooter>();
        heroSR = GetComponent<SpriteRenderer>();
        Debug.Assert(heroAnimator != null);
        heroRigidBody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (Time.time - lastCooldown > layerChangeCooldown)
        {
            setLayerNear();
        }
        HandleMovement();
        
        heroAnimator.SetBool("isJumping", heroRigidBody.velocity.y > 0);
        heroAnimator.SetBool("isFalling", heroRigidBody.velocity.y < 0);

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
        if (Input.GetKey(KeyCode.Space) && isOnGround)
        {
            heroRigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isOnGround = false;
            isJumping = true;
        }
        
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
        if (Input.GetKey(KeyCode.Space)) keys.Add(' ');
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
}
