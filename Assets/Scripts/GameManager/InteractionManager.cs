using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.E;
    public Transform player;

    private List<IInteractable> nearbyInteractables = new(); // ��ǰ��ײ��Χ�ڵ�

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

        // ������Ľ������󣨿�ѡ����������
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
