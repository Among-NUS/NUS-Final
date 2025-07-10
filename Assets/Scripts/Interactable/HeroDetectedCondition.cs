using UnityEngine;

public class HeroDetectedCondition : MonoBehaviour, ICondition
{
    public Monitor monitor;

    public bool IsTrue => monitor != null && monitor.getCapturedTarget().Count > 0;
}
