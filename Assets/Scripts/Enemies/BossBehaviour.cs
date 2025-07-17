using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class BossBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isWalking;
    public enum BossState { Walking, Standing, Dying }
    public bool isDead;
    public float generalCooldown;
    public float speed = 0.1f;
    public Animator bossAnimator;
    public AlertSignBehaviour bossAlert;
    public Rigidbody2D bossRigidBody;
    public float jumpForce = 5f;
    public bool bossFacingRight = true;

    void Start()
    {
        bossAlert = GetComponentInChildren<AlertSignBehaviour>();
        bossRigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MakeWalk()
    {
        bossAnimator.SetBool("isWalking", true);
    }

    public void MakeStand()
    {
        bossAnimator.SetBool("isWalking", false);
    }

    public void BossDie()
    {

    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        BulletBehaviour bullet = collision.GetComponent<BulletBehaviour>();
        if (bullet != null && !bullet.isEnemy)
        {
            DestroyBoss();
        }
    }

    public void DestroyBoss()
    {
        bossAnimator.SetBool("isDead", true);
    }

    public void BossTurn()
    {
        bossFacingRight = !bossFacingRight;
        Vector3 turnTo = transform.localScale;
        turnTo.x *= -1f;
        transform.localScale = turnTo;
    }

    public void BossAlert()
    {
        bossAlert.PurelyShowAlert();
    }

    public void BossUnAlert()
    {
        bossAlert.PurelyUnshowAlert();
    }

    public void Jump()
    {
        bossRigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }


}
