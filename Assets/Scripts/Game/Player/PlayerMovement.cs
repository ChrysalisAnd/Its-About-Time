using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _speed;

    [SerializeField] private float _rotationSpeed; // in degrees, but they are multiplied by fixedDeltaTime

    private Rigidbody2D _rigidbody;
    private Vector2 _movementInput;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        //SetPlayerVelocity();
        MovePlayerPosition();
        RotateTowardsMouse();
    }

    private void SetPlayerVelocity()
    {
        _rigidbody.velocity = _movementInput * _speed;
    }

    private void MovePlayerPosition()
    {
        _rigidbody.MovePosition(_rigidbody.position + _movementInput * _speed * Time.fixedDeltaTime);
    }

    private void OnMove(InputValue inputValue)
    {
        _movementInput = inputValue.Get<Vector2>();
    }

    private void RotateTowardsMouse()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseDir = mousePos - _rigidbody.position;
        float desiredAngle = Mathf.Atan2(mouseDir.y, mouseDir.x) * Mathf.Rad2Deg;
        float angleDiff = desiredAngle - _rigidbody.rotation;
        int rotationDirection = (angleDiff > 0) ? 1 : -1;
        if ((_rigidbody.rotation > 90 && desiredAngle < -90) || (_rigidbody.rotation < -90 && desiredAngle > 90))
        {
            rotationDirection *= -1;
        }
        if (Mathf.Abs(angleDiff) > 360)
        {
            _rigidbody.rotation += 360 * Mathf.Sign(angleDiff);
            angleDiff = desiredAngle - _rigidbody.rotation;
            rotationDirection = (angleDiff > 0) ? 1 : -1;
        }
        if (Mathf.Abs(angleDiff) > _rotationSpeed * Time.fixedDeltaTime)
        {
            _rigidbody.rotation += _rotationSpeed * rotationDirection * Time.fixedDeltaTime;
        } else {
            _rigidbody.rotation = desiredAngle;
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
