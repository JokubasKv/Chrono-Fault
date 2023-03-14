using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootingScript : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] private Transform rotationPoint;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private GameObject bullet;
    [SerializeField] public float shootPower;

    [SerializeField] public bool inheritMovement;

    PlayerInput inputActions;
    Rigidbody2D rb;
    #region -Awake/OnEnable/OnDisable -
    private void Awake()
    {
        inputActions = new PlayerInput();
        inputActions.Gameplay.MouseLeftClick.performed += e => Shoot();

        rb = rotationPoint.GetComponent<Rigidbody2D>();
        inputActions.Enable();
    }
    #endregion
    private void FixedUpdate()
    {
        HandleTurning();
    }

    private void HandleTurning()
    {
        Vector2 input_Mouse = cam.ScreenToWorldPoint(inputActions.Gameplay.MousePosition.ReadValue<Vector2>());
        Vector2 facingDirection = input_Mouse - rb.position;
        float angle = Mathf.Atan2(facingDirection.y, facingDirection.x) * Mathf.Rad2Deg - 90;

        rb.MoveRotation(angle);
    }

    private void Shoot()
    {
        GameObject gameObject = Instantiate(bullet, shootPoint.position, shootPoint.rotation);
        Rigidbody2D rbG = gameObject.GetComponent<Rigidbody2D>();
        Vector2 direction = shootPoint.position - rotationPoint.position;

        rbG.AddForce(direction * shootPower + (inheritMovement ? Vector2.zero : Vector2.zero), ForceMode2D.Impulse);
    }
}
