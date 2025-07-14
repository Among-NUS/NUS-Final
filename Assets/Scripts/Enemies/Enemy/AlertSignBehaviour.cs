using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertSignBehaviour : MonoBehaviour
{
    public float existInterval = 2f;
    private float lastShowTime = 0f;
    public SpriteRenderer alertSR;
    // Start is called before the first frame update
    void Start()
    {
        alertSR = GetComponent<SpriteRenderer>();
        alertSR.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastShowTime > existInterval)
        {
            alertSR.enabled = false;
        }
    }

    public void showAlert()
    {
        alertSR.enabled = true;
        lastShowTime = Time.time;
    }
    
}
