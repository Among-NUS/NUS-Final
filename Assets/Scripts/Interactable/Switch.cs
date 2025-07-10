using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    [Header("need to drag")]
    [Header("auto update")]
    [SerializeField]
    bool isPressInteract =false;
    public bool isOn = false;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        isPressInteract = false;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetKey(KeyCode.E)^isPressInteract&& Input.GetKey(KeyCode.E))
        {
            isOn = !isOn;
        }
        isPressInteract = Input.GetKey(KeyCode.E);
    }
}
