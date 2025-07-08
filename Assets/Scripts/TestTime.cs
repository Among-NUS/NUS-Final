using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTime : MonoBehaviour
{
    float angle = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //let the object rotate around(0,0,0) z is rotate axis
        transform.RotateAround(Vector3.zero, Vector3.forward, 1f);
        
    }
}
