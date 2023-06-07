using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : TimeControlledCharacter
{
    [SerializeField] private float _speed;

    [SerializeField] private float _rotationSpeed; // in degrees / frame


    private Vector2 _movementInput;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    public override void TimeUpdate()
    {
        MovePlayerPosition();
        RotateTowardsMouse();
    }

    private void FixedUpdate()
    {
        //SetPlayerVelocity();
        //MovePlayerPosition();
        //RotateTowardsMouse();
    }

    private void SetPlayerVelocity()
    {
        rigidbody.velocity = _movementInput * _speed;
    }

    private void MovePlayerPosition()
    {
        rigidbody.MovePosition(rigidbody.position + _movementInput * _speed * Time.fixedDeltaTime);
    }

    private void OnMove(InputValue inputValue)
    {
        _movementInput = inputValue.Get<Vector2>();
    }

    private void RotateTowardsMouse()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseDir = mousePos - rigidbody.position;
        float desiredAngle = Mathf.Atan2(mouseDir.y, mouseDir.x) * Mathf.Rad2Deg;
        float angleDiff = desiredAngle - rigidbody.rotation;
        int rotationDirection = (angleDiff > 0) ? 1 : -1;
        if ((rigidbody.rotation > 90 && desiredAngle < -90) || (rigidbody.rotation < -90 && desiredAngle > 90))
        {
            rotationDirection *= -1;
        }
        if (Mathf.Abs(angleDiff) > 360)
        {
            rigidbody.rotation += 360 * Mathf.Sign(angleDiff);
            angleDiff = desiredAngle - rigidbody.rotation;
            rotationDirection = (angleDiff > 0) ? 1 : -1;
        }
        if (Mathf.Abs(angleDiff) > _rotationSpeed * Time.fixedDeltaTime)
        {
            rigidbody.rotation += _rotationSpeed * rotationDirection * Time.fixedDeltaTime;
        } else {
            rigidbody.rotation = desiredAngle;
        }

        /*
        if (Mathf.Abs(_rigidbody.rotation) >= 360)
        {
            _rigidbody.rotation -= 360 * Mathf.Sign(_rigidbody.rotation);
        }
        Debug.Log("curr: " + _rigidbody.rotation + " diff: " + angleDiff);
        */
    }

}
