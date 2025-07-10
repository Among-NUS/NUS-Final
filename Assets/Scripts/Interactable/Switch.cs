using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : RecordableObject
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
        bool isInteract = false;
        if (collision.GetComponent<Hero>()!=null)
        {
            isInteract = collision.GetComponent<Hero>().isInteracting;
        }
        else if (collision.GetComponent<Ghost>()!=null)
        {
            isInteract = collision.GetComponent<Ghost>().isInteracting;
        }
        if (isInteract ^ isPressInteract && isInteract)
        {
            isOn = !isOn;
        }
        isPressInteract = isInteract;
    }
    public override Dictionary<string, object> CaptureExtraData()
    {
        return new Dictionary<string, object>() 
        {
            {"isOn",isOn}
        };
    }
    public override void RestoreExtraData(Dictionary<string, object> data)
    {
        this.isOn = (bool)data["isOn"];
    }
}
