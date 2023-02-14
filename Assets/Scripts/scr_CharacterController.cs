using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.RuleTile.TilingRuleOutput;

[RequireComponent(typeof(Rigidbody2D))]
public class scr_CharacterController : MonoBehaviour
{

    PlayerInput inputActions;
    Vector2 input_Movement;
    Vector2 input_Mouse;
    Rigidbody2D rb;

    [Header("Essential Parameters")]
    [SerializeField] Camera cam;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Animator animator;

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    //[SerializeField] private float slideSpeed = 5.0f;

    private float currentSetSpeed;
    private bool FacingRight = true;

    private Vector2 smoothedMovementInput;
    private Vector2 movementInputSmoothVelocity;


    #region -Awake/OnEnable/OnDisable -
    private void Awake()
    {
        inputActions = new PlayerInput();
        rb = GetComponent<Rigidbody2D>();
        currentSetSpeed = walkSpeed;
    }

    private void OnEnable()
    {
        inputActions.Enable();


        inputActions.Gameplay.Movement.performed        += e => input_Movement = e.ReadValue<Vector2>();
        inputActions.Gameplay.Movement.canceled         += e => input_Movement = e.ReadValue<Vector2>();
        inputActions.Gameplay.MousePosition.performed   += e => input_Mouse = cam.ScreenToWorldPoint(e.ReadValue<Vector2>());
        inputActions.Gameplay.Sprint.performed          += e => StartSprint();
        inputActions.Gameplay.Sprint.canceled           += e => StopSprint();
        inputActions.Gameplay.MouseLeftClick.performed  += e => Shoot();



    }


    private void OnDisable()
    {
        inputActions.Disable();

        inputActions.Gameplay.Movement.performed            -= e => input_Movement = e.ReadValue<Vector2>();
        inputActions.Gameplay.Movement.canceled             -= e => input_Movement = e.ReadValue<Vector2>();
        inputActions.Gameplay.MousePosition.performed       -= e => input_Mouse = cam.ScreenToWorldPoint(e.ReadValue<Vector2>());
        inputActions.Gameplay.Sprint.performed              -= e => StartSprint();
        inputActions.Gameplay.Sprint.canceled               -= e => StopSprint();
    }
    #endregion

    
    private void Start()
    {

    }

    #region - Update/FixedUpdate -
    private void Update()
    {
        
    }
    private void FixedUpdate()
    {
        HandleMovement();
        HandleTurningToMouse();
        HandleAnimations();
    }


    #endregion
    #region - Movement -
    private void HandleMovement()
    {
        smoothedMovementInput = Vector2.SmoothDamp(smoothedMovementInput,
            input_Movement,
            ref movementInputSmoothVelocity,
            0.1f);

        rb.velocity = smoothedMovementInput * currentSetSpeed;
    }
    #endregion

    #region - Sprinting -
    private void StopSprint()
    {
        currentSetSpeed = walkSpeed;
    }

    private void StartSprint()
    {
        currentSetSpeed = sprintSpeed;
    }
    #endregion
    #region - Mouse  -
    private void HandleTurningToMouse()
    {
/*        Vector2 facingDirection = input_Mouse - rb.position;
        float angle = Mathf.Atan2(facingDirection.y, facingDirection.x) * Mathf.Rad2Deg;

        rb.MoveRotation(angle);*/

        if (input_Mouse.x < transform.position.x && FacingRight)
        {
            Flip();
        }
        else if (input_Mouse.x > transform.position.x && !FacingRight)
        {
            Flip();
        }
    }
    #endregion


    private void Shoot()
    {
        animator.SetTrigger("Shoot");
    }


    private void HandleAnimations()
    {
        animator.SetBool("isMoving", input_Movement != Vector2.zero);
    }

    void Flip()
    {
        Vector3 tmpScale = transform.localScale;
        tmpScale.x = -tmpScale.x;
        transform.localScale = tmpScale;
        FacingRight = !FacingRight;

    }

}
