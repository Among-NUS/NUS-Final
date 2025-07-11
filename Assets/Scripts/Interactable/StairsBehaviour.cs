using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 挂在每一段楼梯上。玩家进入触发器并按 W / S 时
/// 立即把玩家传送到 linkedStairs 的 transform.position。
/// </summary>
public class StairsBehaviour : MonoBehaviour
{
    [Tooltip("这一段楼梯对应的另一段楼梯（必须互相指向）")]
    public StairsBehaviour stairsUp = null;
    public StairsBehaviour stairsDown = null;
    public Animator stairsAnimator;
    private GameObject enterPlayer;

    [Tooltip("站在这段楼梯时，按 W 是否算“上楼”")]
    //public bool upwardHere = true;

    // --- 私有状态 ---
    bool playerInside;
    Transform playerTf;
    float cooldown;            // 防止瞬时往返
    void Start()
    {
        Debug.Assert(stairsAnimator != null);
    }

    void Update()
    {
        if (cooldown > 0f)           // 冷却中
            cooldown -= Time.deltaTime;

        if (!playerInside || cooldown > 0f) // 玩家不在触发器内或冷却中
            return;

        // 根据方向检测输入

        if ((stairsUp != null && Input.GetKeyDown(KeyCode.W)))
        {
            TeleportTo(stairsUp);
            
            enterPlayer.GetComponent<HeroBehaviour>().setLayerFar();
            
        }
        if ((stairsDown != null && Input.GetKeyDown(KeyCode.S)))
        {
            TeleportTo(stairsDown);
            
            enterPlayer.GetComponent<HeroBehaviour>().setLayerFar();
            
        }
    }

    void TeleportPlayer()
    {
        if (stairsUp == null)
        {
            Debug.LogWarning($"{name} 没有 linkedStairs,无法传送");
            return;
        }

        // 目标坐标：直接用另一段楼梯的 transform.position
        playerTf.position = stairsUp.transform.position;

        // 给两端都加一点冷却，避免刚到就被判定回传
        cooldown = 0.15f;
        stairsUp.cooldown = 0.15f;
    }
    void TeleportTo(StairsBehaviour target)
    {
        playerTf.position   = target.transform.position;
        cooldown            = 0.15f;   // 自己冷却
        target.cooldown     = 0.15f;   // 目标端也冷却
    }

    // --- 触发器检测 ---
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")||other.CompareTag("Ghost"))
        {
            playerInside = true;
            playerTf = other.transform;
            stairsAnimator.SetBool("HeroIn", true);
            enterPlayer = other.gameObject;
            
            
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")||other.CompareTag("Ghost"))
        {
            playerInside = false;
            playerTf = null;
            stairsAnimator.SetBool("HeroIn", false);
        }
    }
    // 供 Ghost 调用的公共接口
    public void TryTeleport(Transform entity, bool commandUp)
    {
        // 冷却中直接忽略
        if (cooldown > 0f) return;
        StairsBehaviour target = commandUp ? stairsUp : stairsDown;
        // commandUp=true 表示来的是 “想往上”
        
        if (target==null) return;

        enterPlayer.GetComponent<GhostBehaviour>().setLayerFar();
        // 执行传送
        entity.position = target.transform.position;
        cooldown              = 0.15f;
        target.cooldown = 0.15f;
    }
}

