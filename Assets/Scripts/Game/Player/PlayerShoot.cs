using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Transform gunOffset;
    [SerializeField] private float timeBetweenShots;

    private float lastFireTime;

    private TimeController timeController;
    private TimeKeeper timeKeeper;
    private BulletPool bulletPool;
    private Rigidbody2D playerBody;
    
    void Start()
    {
        lastFireTime = Time.time;
        timeController = GameObject.FindObjectOfType<TimeController>();
        timeKeeper = GameObject.FindObjectOfType<TimeKeeper>();
        bulletPool = GameObject.FindObjectOfType<BulletPool>();
        playerBody = GetComponent<Rigidbody2D>();
    }



    private void OnFire(InputValue inputValue)
    {
        float timeSinceLastFire = Time.time - lastFireTime;
        if (timeSinceLastFire > timeBetweenShots)
        {
            bulletPool.shootBullet(gunOffset.position, playerBody.rotation);
            lastFireTime = Time.time;

        }

    }


}
