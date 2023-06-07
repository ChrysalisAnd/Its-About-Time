using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControlledCharacter : MonoBehaviour
{
    public new Rigidbody2D rigidbody;
    public Vector2 position;
    public float rotation;

    public virtual void TimeUpdate()
    {

    }
}
