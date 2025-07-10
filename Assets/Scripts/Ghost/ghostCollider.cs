using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostCollider : MonoBehaviour
{
    [Header("need to drag")]
    public GameObject father;
    [Header("need update")]
    public int colliderType;//������ײ�����࣬-1��0�£�1��,2�� 


    Ghost ghost;
    // Start is called before the first frame update
    void Start()
    {
        ghost = father.GetComponent<Ghost>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (colliderType == 0)
        {
            ghost.isTouchGround = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (colliderType == 0)
        {
            ghost.isTouchGround = false;
        }
    }
}
