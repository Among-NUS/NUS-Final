using UnityEngine;
public interface IInteractable
{
    Transform GetTransform(); // 用于距离判断
    void Interact();          // 实际交互逻辑
}
