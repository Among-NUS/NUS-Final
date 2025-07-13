using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PressurePlateBehaviour : MonoBehaviour, ICondition
{
    [Tooltip("哪些Tag会触发压力板")]
    public string[] validTags = { "Player" , "Ghost" , "Enemy" };

    private int triggerCount = 0;

    public bool IsTrue => triggerCount > 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsValid(other))
        {
            triggerCount++;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsValid(other))
        {
            triggerCount = Mathf.Max(triggerCount - 1, 0);
        }
    }

    private bool IsValid(Collider2D other)
    {
        foreach (var tag in validTags)
        {
            if (other.CompareTag(tag))
                return true;
        }
        return false;
    }
}
