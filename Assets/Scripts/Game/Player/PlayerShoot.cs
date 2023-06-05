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
    
    void Start()
    {
        lastFireTime = Time.time;
    }


    private void FireBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab,
            gunOffset.position, transform.rotation);
        Rigidbody2D bulletRigidBody = bullet.GetComponent<Rigidbody2D>();
        Rigidbody2D playerRigidBody = GetComponent<Rigidbody2D>();
        bulletRigidBody.velocity = bulletSpeed * DegreeToVector2(playerRigidBody.rotation);
    }

    private void OnFire(InputValue inputValue)
    {
        float timeSinceLastFire = Time.time - lastFireTime;
        if (timeSinceLastFire > timeBetweenShots)
        {
            FireBullet();
            lastFireTime = Time.time;
        }

    }

    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }

}
