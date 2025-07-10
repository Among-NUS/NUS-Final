using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    [Header("need to drag")]
    public List<GameObject> switchs = new List<GameObject>();
    public SwitchType st = SwitchType.OR;
    [Header("auto update")]
    [SerializeField]
    bool isOn = false;
    [SerializeField]
    List<bool> switchState = new List<bool>();
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < switchs.Count; i++)
        {
            switchState.Add(switchs[i].GetComponent<Switch>().isOn);
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (st) 
        { 
            case SwitchType.AND:
                isOn = allSwitchOn();
                break;
            case SwitchType.OR:
                isOn = oneSwitchOn();
                break;
            case SwitchType.CHANGE:
                if (switchChange())
                {
                    isOn = !isOn;
                }
                break;
            default:
                isOn = false;
                break;
        }
        if (!isOn^GetComponent<BoxCollider2D>().enabled)
        {
            GetComponent<BoxCollider2D>().enabled = !isOn;
        }
        GetComponent<SpriteRenderer>().color = isOn?Color.red : Color.green;
    }
    bool allSwitchOn()
    {
        for (int i = 0;i<switchs.Count;i++)
        {
            if (!switchs[i].GetComponent<Switch>().isOn)
            {
                return false;
            }
        }
        return true;
    }
    bool oneSwitchOn()
    {
        for (int i = 0; i < switchs.Count; i++)
        {
            if (switchs[i].GetComponent<Switch>().isOn)
            {
                return true;
            }
        }
        return false;
    }
    bool switchChange()
    {
        for (int i = 0; i < switchs.Count; i++)
        {
            if (switchState[i]^switchs[i].GetComponent<Switch>().isOn)
            {
                switchState[i] = switchs[i].GetComponent<Switch>().isOn;
                return true;
            }
        }
        return false;
    }
    public enum SwitchType
    {
        AND,
        OR,
        CHANGE
    }
}
