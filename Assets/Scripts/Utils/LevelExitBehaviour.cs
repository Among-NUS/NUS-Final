using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExitBehaviour : MonoBehaviour
{
    // The name of the scene to load when the player touches the exit.
    public string targetSceneName;

    // Trigger the scene change when a Player collides with this object.
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("exit collided");
        // Check if the object colliding is tagged as Player
        if (other.CompareTag("Player"))
        {
            Debug.Log("Teleporting to level");
            // Load the specified scene
            SceneManager.LoadScene(targetSceneName);
        }
    }
}

