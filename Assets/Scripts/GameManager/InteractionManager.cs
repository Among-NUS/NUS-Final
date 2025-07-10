using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.E;
    public Transform player;

    private List<IInteractable> nearbyInteractables = new(); // 当前碰撞范围内的

    public void RegisterNearby(IInteractable i)
    {
        if (!nearbyInteractables.Contains(i))
            nearbyInteractables.Add(i);
    }

    public void UnregisterNearby(IInteractable i)
    {
        nearbyInteractables.Remove(i);
    }

    void Update()
    {
        if (!Input.GetKeyDown(interactKey)) return;

        if (nearbyInteractables.Count == 0) return;

        // 找最近的交互对象（可选：如果多个）
        IInteractable nearest = null;
        float minDist = float.MaxValue;
        foreach (var i in nearbyInteractables)
        {
            float d = Vector2.Distance(i.GetTransform().position, player.position);
            if (d < minDist)
            {
                minDist = d;
                nearest = i;
            }
        }

        nearest?.Interact();
    }
}
