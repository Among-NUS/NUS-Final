using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConditionalExitBehaviour : MonoBehaviour
{
    private Transform parent;
    public GameObject conditionObject;
    public bool condition = false;
    public string targetSceneName;

    // Start is called before the first frame update
    void Start()
    {
        parent = transform;
        gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (conditionObject != null)
        {
            condition = !conditionObject.activeSelf;
        }
        else
        {
            Debug.Log("No assigned level exit condition");
        }
        if (condition)
        {
            gameObject.SetActive(true);
        }
    }

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
