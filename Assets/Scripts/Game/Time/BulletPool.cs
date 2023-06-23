using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private GameObject bulletPrefab;

    [SerializeField] public int Size;
    [SerializeField] private float bulletSpeed;

    public int currentReadyBullet;

    public GameObject[] bulletPool;

    public static Vector2 restPosition = new Vector2(-100.0f, -100.0f);

    private void Awake()
    {
        Quaternion identity = Quaternion.identity;
        bulletPool = new GameObject[Size];
        for (int i = 0; i < Size; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, restPosition, identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            CapsuleCollider2D collider = bullet.GetComponent<CapsuleCollider2D>();
            rb.isKinematic = true;
            collider.enabled = false;
            bullet.transform.parent = gameObject.transform;
            bulletPool[i] = bullet;
        }
        currentReadyBullet = 0;
    }

    public void shootBullet(Vector2 position, float rotation, bool reverse = false)
    {
        GameObject bullet = bulletPool[currentReadyBullet];
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D> ();
        CapsuleCollider2D collider = bullet.GetComponent<CapsuleCollider2D>();
        rb.isKinematic = false;
        collider.enabled = true;
        rb.position = position;
        rb.rotation = rotation;
        int direction = 1;
        if (reverse)
        {
            direction = -1;
        }
        rb.velocity = direction * bulletSpeed * DegreeToVector2(rotation);
        currentReadyBullet = (currentReadyBullet + 1) % Size;

    }

    public static void SetToInactive(GameObject bullet)
    {
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        CapsuleCollider2D collider = bullet.GetComponent<CapsuleCollider2D>();
        rb.isKinematic = true;
        collider.enabled = false;
        rb.position = restPosition;
        rb.rotation = 0;
        rb.velocity = Vector2.zero;
    }

    public void ReactivateBullets()
    {
        for (int i = 0; i < Size; i++)
        {
            Rigidbody2D rb = bulletPool[i].GetComponent<Rigidbody2D>();
            if (rb.position != restPosition)
            {
                CapsuleCollider2D collider = bulletPool[i].GetComponent<CapsuleCollider2D>();
                rb.isKinematic = false;
                collider.enabled = true;
                rb.velocity = bulletSpeed * DegreeToVector2(rb.rotation);
            }
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
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
