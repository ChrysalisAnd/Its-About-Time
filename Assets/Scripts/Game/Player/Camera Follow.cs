using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject cursor;
    [SerializeField] private float viewDistance;

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPosition = player.transform.position;
        Vector3 position = (playerPosition + cursor.transform.position) / 2;
        if (Vector3.Distance(playerPosition, position) > viewDistance)
        {
            position = playerPosition + (cursor.transform.position - playerPosition).normalized * viewDistance;
        }
        transform.position = position;
    }
}
