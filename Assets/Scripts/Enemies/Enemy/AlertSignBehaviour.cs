using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertSignBehaviour : MonoBehaviour
{
    public float existInterval = 2f;
    private float lastShowTime = 0f;
    public SpriteRenderer alertSR;
    public AudioClip alertClip;
    private AudioSource alertAS;
    // Start is called before the first frame update
    void Start()
    {
        alertSR = GetComponent<SpriteRenderer>();
        alertSR.enabled = false;
        alertAS = GetComponent<AudioSource>();
        alertAS.clip = alertClip;
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
        if (Time.time - lastShowTime < existInterval)
        {
            return;
        }
        alertSR.enabled = true;
        lastShowTime = Time.time;
        alertAS.PlayOneShot(alertClip);
    }
    
}
