using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("need to drag")]
    public List<Transform> pathWayPoint;
    public Monitor enemyMonitor;
    public GameObject bullet;
    [Header("auto initialize")]
    public float speed = 5f;
    public float shotInterval = 0.5f;
    [Header("auto update")]
    public int currentTarget;
    [SerializeField]
    float lastShot;

    // Start is called before the first frame update
    void Start()
    {
        currentTarget = 0;
        lastShot = Time.time;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += (pathWayPoint[currentTarget].position-transform.position).normalized * speed * 0.02f;
        transform.rotation = Quaternion.Euler
            (
            0, 0, Mathf.Sign(Vector2.Dot(
                -Vector2.right, (pathWayPoint[currentTarget].position - transform.position)
                ))*90
            );
        if (enemyMonitor.getCapturedTarget().Count!=0)
        {
            Vector2 direction = (enemyMonitor.getCapturedTarget()[0].transform.position - transform.position);
            if ((Time.time-lastShot)>shotInterval)
            {
                Instantiate(bullet, transform.position, Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90));
                lastShot = Time.time;
            }
        }
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {   
        if (collision.gameObject == pathWayPoint[currentTarget].gameObject)
        {
            currentTarget = (currentTarget+1)%pathWayPoint.Count;
        }
    }

}
