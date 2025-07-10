using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroCollider : MonoBehaviour
{
    [Header("need to drag")]
    public GameObject father;
    [Header("need update")]
    public int colliderType;//决定碰撞箱种类，-1左，0下，1右,2上 


    Hero hero;
    // Start is called before the first frame update
    void Start()
    {
        hero = father.GetComponent<Hero>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (colliderType == -1)
        {
            hero.isTouchLeft = true;
        }
        if (colliderType == 0)
        {
            hero.isTouchGround = true;
        }
        if (colliderType == 1)
        {
            hero.isTouchRight = true;
        }
        if (colliderType == 2)
        {
            hero.isTouchTop = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (colliderType == -1)
        {
            hero.isTouchLeft = false;
        }
        if (colliderType == 0)
        {
            hero.isTouchGround = false;
        }
        if (colliderType == 1)
        {
            hero.isTouchRight = false;
        }
        if (colliderType == 2)
        {
            hero.isTouchTop = false;
        }
    }
}
