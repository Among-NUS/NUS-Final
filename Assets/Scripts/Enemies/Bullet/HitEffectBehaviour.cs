using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffectBehaviour : MonoBehaviour
{
    [Header("need to fill in")]
    public float lifeTime = 2f;
    [Header("auto update")]
    public float spawnTime;
    // Start is called before the first frame update
    void Start()
    {
        spawnTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Time.time-spawnTime)>lifeTime)
        {
            Destroy(gameObject);
        }
    }
}
