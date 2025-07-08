using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTime : MonoBehaviour
{
    void FixedUpdate()
    {
        if(GameManager.Instance.currentPhase == GameManager.GamePhase.TimeStop) return;
        transform.RotateAround(Vector3.zero, Vector3.forward, 1f);
    }
}
