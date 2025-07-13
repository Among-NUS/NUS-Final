using UnityEngine;

public class Shooter : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public bool isEnemy = false;          // �Ƿ�Ϊ�з��ӵ�
    public bool faceLeft = true;          // �泯����������䷽��
    public float fireInterval = 0.5f;

    private float fireCooldown = 0f;
    public AudioClip gunshot;
    private AudioSource shooterAudioS;

    void Awake()
    {
        shooterAudioS = GetComponent<AudioSource>();
    }
    void FixedUpdate()
    {
        if (fireCooldown > 0f)
            fireCooldown -= Time.fixedDeltaTime;
    }

    public bool CanFire() => fireCooldown <= 0f;

    public void Fire()
    {
        if (!CanFire()) return;
        shooterAudioS.clip = gunshot;
        shooterAudioS.Play();
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