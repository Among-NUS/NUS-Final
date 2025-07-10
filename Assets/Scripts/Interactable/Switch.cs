using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour, IInteractable
{
    public bool isOn = false;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public Transform GetTransform() => transform;
    public void Interact()
    {
        isOn = !isOn;
    }
}
