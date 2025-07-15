using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySensorBehaviour : MonoBehaviour
{
    public int enemyIn = 0;
    // Start is called before the first frame update
    void Start()
    {
        enemyIn = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            enemyIn++;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            enemyIn--;
        }
    }
}
