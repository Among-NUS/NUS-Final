using UnityEngine;

public class Monitor : MonoBehaviour
{
    public bool heroInSight = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            heroInSight = true;
            Debug.Log("Hero ENTERED camera view!");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            heroInSight = false;
            Debug.Log("Hero LEFT camera view.");
        }
    }
}
