using Unity.VisualScripting;
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
    private GameObject soundSource = null;
    private float lastSound = 0f;
    public float soundCooldown = 0.1f;


    void Awake()
    {
        shooterAudioS = GetComponent<AudioSource>();
        soundCooldown = 0.5f;
    }
    void FixedUpdate()
    {
        if (soundSource != null&&Time.time-lastSound>soundCooldown)
        {
            Destroy(soundSource);
        }
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
        soundSource = (GameObject)Instantiate(Resources.Load("Prefabs/soundSource"), firePoint.position+new Vector3(3.1f,0f,0f),Quaternion.identity);
        lastSound = Time.time;
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