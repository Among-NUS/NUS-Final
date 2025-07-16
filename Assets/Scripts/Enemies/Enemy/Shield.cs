using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [Header("need to drag")]
    public GameObject followedEnemy;
    [Header("auto update")]
    public bool isHoldingShield = true;
    public bool isFaingLeft = false;
    public float distance = 1f;
    // Start is called before the first frame update
    void Start()
    {
        if (followedEnemy.GetComponent<Enemy>()==null)
        {
            Debug.LogError("orphan shield");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //isHoldingShield = 
        //    (EnemyBehaviour.EnemyState.CHASE == followedEnemy.GetComponent<Enemy>().enemyState)||
        //    (EnemyBehaviour.EnemyState.DISCOVER == followedEnemy.GetComponent<Enemy>().enemyState) ||
        //    (EnemyBehaviour.EnemyState.SHOOT == followedEnemy.GetComponent<Enemy>().enemyState);
        //在射击或者追击时持盾
        isFaingLeft = followedEnemy.GetComponent<Enemy>().facingLeft;
        Vector3 positionBias = new Vector2(isFaingLeft?-1:1,0);
        positionBias = isHoldingShield? -positionBias : positionBias;
        transform.position = followedEnemy.transform.position+positionBias*distance;
        transform.localScale = new Vector2 (positionBias.x>0?1:-1, 1);
    }
}
