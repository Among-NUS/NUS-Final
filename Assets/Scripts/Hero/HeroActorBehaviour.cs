using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroActorBehaviour : MonoBehaviour
{
    public Animator heroAnimator;
    private Shooter shooter;
    private Rigidbody2D heroRigidBody;
    private bool isOnGround = true;
    private bool isJumping = false;
    private int groundLayer = 13;
    public bool heroFacingRight;
    public float speed = 0.1f;
    void Awake()
    {
        shooter = GetComponentInChildren<Shooter>();
        heroRigidBody = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        heroAnimator.SetBool("isJumping", false);
        heroAnimator.SetBool("isFalling", false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    public void MakeFaceRight(bool faceRightCommand)
    {
        heroFacingRight = faceRightCommand;
        float targetSign = heroFacingRight ? 1f : -1f;
        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * targetSign;
        transform.localScale = s;
    }

    public void MakeWalk()
    {
        heroAnimator.SetBool("isWalking", true);
    }
    public void MakeStand()
    {
        heroAnimator.SetBool("isWalking", false);
    }

    public void MakeShoot()
    {
        shooter.faceLeft = false;
        shooter.Fire();
        heroAnimator.SetTrigger("shootAnim");
    }
    void OnColliEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == groundLayer)
        {
            isOnGround = true;
            isJumping = false;
        }
    }
    void OisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == groundLayer)
        {
            isOnGround = false;
        }
    }

}
