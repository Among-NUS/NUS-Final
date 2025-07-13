using UnityEngine;

public class HeroDetectedCondition : MonoBehaviour, ICondition
{
    public MonitorBehaviour monitor;

    public bool IsTrue => monitor != null && monitor.getCapturedTarget().Count > 0;
}
