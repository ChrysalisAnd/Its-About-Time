using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update

    public BulletPool pool { get; set; }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    { // at the end https://www.youtube.com/watch?v=JcfNFoeuzUk&list=PLx7AKmQhxJFajrXez-0GJgDlKELabQQHT&index=9 how to kill enemies
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        Rigidbody2D rb2 = gameObject.GetComponent<Rigidbody2D>();
        Debug.Log("collision");
        if (other.GetComponent<PlayerMovement>() == null /*|| other.GetComponent<Bullet>() == null*/) // if it is not player. TODO: change this in case bullet is fired by enemy
        {
            //Destroy(gameObject); // for test (or not?)
            //gameObject.parent
            BulletPool.SetToInactive(gameObject);
        }
    }


}
