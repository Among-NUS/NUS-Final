using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ear : MonoBehaviour
{
    public delegate void OnHearing(Collider2D other);//在听到声音立刻将实体传递
    public OnHearing onHearing;

    private void OnTriggerStay2D(Collider2D collision)
    {
        onHearing(collision);
    }
}
