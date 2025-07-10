using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Bullets : RecordableObject
{
    [Header("auto initialize")]
    public float speed = 5f;
    public int damage = 10;
    BoxCollider2D bulletCollider;
    public float lifeTime = 3f;
    public float spawnTime;

    public bool isFriendly = false;
    // Start is called before the first frame update
    void Start()
    {
        bulletCollider = GetComponent<BoxCollider2D>();
        if (bulletCollider!=null)
        {
            Debug.Log("Ã»ÓÐÅö×²Ïä");
        }
        spawnTime = Time.time;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.time-spawnTime>lifeTime)
        {
            Destroy(gameObject);
        }
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, speed * 0.02f);
        if (hit.collider != null)
        {
            HittingSomething(hit);
        }
        Move();
    }
    void Move()
    {
        transform.position += transform.up * speed * 0.02f;
    }
    //TODO
    void HittingSomething(RaycastHit2D hit)
    {
        
        if (hit.collider.GetComponent<Hero>()!=null&&!isFriendly)
        {
            hit.collider.GetComponent<Hero>().isHurt = true;
            Destroy(gameObject);
        }
        else if(hit.collider.GetComponent<Ghost>() != null&&!isFriendly)
        {
            hit.collider.GetComponent<Ghost>().isHurt = true;
            Destroy(gameObject);
        }
        else if (hit.collider.GetComponent<TilemapCollider2D>() != null)
        {
            Destroy(gameObject);
        }
        
    }
}
