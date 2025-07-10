using UnityEngine;

public class Shooter : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public bool isEnemy = false;          // 是否为敌方子弹
    public bool faceLeft = true;          // 面朝方向决定发射方向
    public float fireInterval = 0.5f;

    private float fireCooldown = 0f;

    void FixedUpdate()
    {
        if (fireCooldown > 0f)
            fireCooldown -= Time.fixedDeltaTime;
    }

    public bool CanFire() => fireCooldown <= 0f;

    public void Fire()
    {
        if (!CanFire()) return;

        Vector2 dir = faceLeft ? Vector2.left : Vector2.right;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        BulletBehaviour bb = bullet.GetComponent<BulletBehaviour>();
        if (bb != null)
        {
            bb.direction = dir;
            bb.isEnemy = isEnemy;
        }

        fireCooldown = fireInterval;
    }
}