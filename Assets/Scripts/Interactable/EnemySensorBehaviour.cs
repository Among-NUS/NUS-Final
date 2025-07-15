using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySensorBehaviour : MonoBehaviour
{
    [SerializeField]private List<Enemy> enemiesIn = new List<Enemy>();
    public int aliveCount = 0;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        enemiesIn.RemoveAll(e => e == null || !e.isAlive);
        aliveCount = enemiesIn.Count;

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy thisEnemy = collision.GetComponent<Enemy>();
            if (thisEnemy != null && !enemiesIn.Contains(thisEnemy))
            {
                enemiesIn.Add(thisEnemy);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null && enemiesIn.Contains(enemy))
            {
                enemiesIn.Remove(enemy);
            }
        }
    }
}
