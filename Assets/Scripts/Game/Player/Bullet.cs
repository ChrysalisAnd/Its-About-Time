using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    { // at the end https://www.youtube.com/watch?v=JcfNFoeuzUk&list=PLx7AKmQhxJFajrXez-0GJgDlKELabQQHT&index=9 how to kill enemies
        Debug.Log("collision");
        if (other.GetComponent<PlayerMovement>() == null) // if it is not player
        {
            //Destroy(other.gameObject);
            Destroy(gameObject); // for test (or not?)
        }
    }
}
