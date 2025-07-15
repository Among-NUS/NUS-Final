using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BriefcaseBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    private SpriteRenderer briefcaseSR;
    public bool isPicked = false;
    public GameObject[] targetExit;
    void Start()
    {
        briefcaseSR = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPicked = true;
            foreach (var exit in targetExit) {
                exit.SetActive(true);
            }

            gameObject.SetActive(false);
        }
    }
}
