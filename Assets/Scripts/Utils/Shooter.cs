using Unity.VisualScripting;
using UnityEngine;
using System;
using System.Collections.Generic;

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
    private Queue<GameObject> soundSource;
    private Queue<float> lastSound;
    public float soundCooldown;


    void Awake()
    {
        shooterAudioS = GetComponent<AudioSource>();
    }
    void Start()
    {
        soundSource = new Queue<GameObject>();
        lastSound = new Queue<float>();
        soundCooldown = 0.05f;
    }
    void FixedUpdate()
    {
        if (soundSource.Count>0&&Time.time-lastSound.Peek()>soundCooldown)
        {
            lastSound.Dequeue();
            Destroy(soundSource.Dequeue());
        }
        if (fireCooldown > 0f)
            fireCooldown -= Time.fixedDeltaTime;
    }

    public bool CanFire() => fireCooldown <= 0f;

    public void Fire()
    {
        if (!CanFire()) return;
        shooterAudioS.clip = gunshot;
        shooterAudioS.PlayOneShot(gunshot);
        Vector2 dir = faceLeft ? Vector2.left : Vector2.right;
        soundSource.Enqueue((GameObject)Instantiate(Resources.Load("Prefabs/soundSource"), firePoint.position,Quaternion.identity));
        lastSound.Enqueue(Time.time);
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