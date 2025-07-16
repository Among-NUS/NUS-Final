using System.Collections;
using System.Collections.Generic;
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

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BossWalk(float duration,bool facingLeft)
    {

    }

    public void BossStand()
    {

    }

    public void BossDie()
    {
        
    }



}
