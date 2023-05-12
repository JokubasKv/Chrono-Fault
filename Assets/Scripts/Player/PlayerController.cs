using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{

    PlayerInput inputActions;
    Vector2 input_Movement;
    Vector2 input_Mouse;
    Rigidbody2D rb;
    RewindManager rewindManager;
    [HideInInspector] public PlayerHealth health;

    [Header("Essential Parameters")]
    [SerializeField] Camera cam;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Animator animator;

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    //[SerializeField] private float slideSpeed = 5.0f;
    private float currentSetSpeed;
    private Vector2 smoothedMovementInput;
    private Vector2 movementInputSmoothVelocity;

    [Header("Shooting Parameters")]
    [SerializeField] private Transform rotationPoint;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private GameObject bullet;
    [SerializeField] public bool inheritMovement;
    Rigidbody2D rotationPointRb2d;


    [Header("Turn Back Time Parameters")]
    [SerializeField] float rewindIntensity = 0.02f;
    [SerializeField] AudioSource rewindSound;
    bool isRewinding = false;
    bool rewindPressed = false;
    float rewindValue = 0;

    [Header("Stats")]
    [SerializeField] public float attackSpeed = 1.0f;
    [SerializeField] public float extraMovement = 0.0f;
    [SerializeField] public float shootSpeed = 10.0f;
    [SerializeField] public float bulletCount = 1.0f;
    private float shootTimer = 0f;
    private float standStillTimer = 0f;


    [Header("Items")]
    public List<ItemList> items = new();




    
    private bool FacingRight = true;


    #region -Awake/OnEnable/OnDisable -
    private void Awake()
    {
        inputActions = new PlayerInput();
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<PlayerHealth>();
        rewindManager = FindObjectOfType<RewindManager>();
        health.InitializeHealth(100);
        currentSetSpeed = walkSpeed;

        rotationPointRb2d = rotationPoint.GetComponent<Rigidbody2D>();

        DontDestroyOnLoad(this.gameObject);
    }

    private void OnEnable()
    {
        inputActions.Enable();


        inputActions.Gameplay.Movement.performed        += e => input_Movement = e.ReadValue<Vector2>();
        inputActions.Gameplay.Movement.canceled         += e => input_Movement = e.ReadValue<Vector2>();
        inputActions.Gameplay.MousePosition.performed   += e => TrackMousePositon(e.ReadValue<Vector2>());
        inputActions.Gameplay.Sprint.performed          += e => StartSprint();
        inputActions.Gameplay.Sprint.canceled           += e => StopSprint();
        inputActions.Gameplay.MouseLeftClick.performed  += e => Shoot();
        inputActions.Gameplay.TurnTimeBack.started      += e => TurnBackTimePressed();
        inputActions.Gameplay.TurnTimeBack.canceled     += e => TurnBackTimeReleased();
        inputActions.UI.Escape.performed                += e => EscapePressed();
    }


    private void OnDisable()
    {
        inputActions.Disable();

        inputActions.Gameplay.Movement.performed            -= e => input_Movement = e.ReadValue<Vector2>();
        inputActions.Gameplay.Movement.canceled             -= e => input_Movement = e.ReadValue<Vector2>();
        inputActions.Gameplay.MousePosition.performed       -= e => TrackMousePositon(e.ReadValue<Vector2>());
        inputActions.Gameplay.Sprint.performed              -= e => StartSprint();
        inputActions.Gameplay.Sprint.canceled               -= e => StopSprint();
        inputActions.Gameplay.TurnTimeBack.started          -= e => TurnBackTimePressed();
        inputActions.Gameplay.TurnTimeBack.canceled         -= e => TurnBackTimeReleased();
        inputActions.UI.Escape.performed                    -= e => EscapePressed();
    }
    #endregion

    
    private void Start()
    {
        StartCoroutine(CallItemUpdate());
    }
    void OnLevelWasLoaded()
    {
        if(items.Count != 0)
            UIManagerSingleton.Instance.UpdateItemSlotsUi(items);
    }

    #region - Update/FixedUpdate -
    private void Update()
    {

    }


    private void FixedUpdate()
    {
        HandleTurnBackTime();
        HandleMovement();
        HandleTurningToMouse();
        HandleAnimations();
        HandleShootPointTurning();
        HandleTimer();
    }

    private void HandleTimer()
    {
        shootTimer += Time.fixedDeltaTime;
        if (input_Movement == Vector2.zero)
        {
            standStillTimer += Time.fixedDeltaTime;
            if(standStillTimer >= 2f)
            {

            }
        }
        else
        {
            standStillTimer = 0f;
        }
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
    private void TrackMousePositon(Vector2 mousepostion)
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
        input_Mouse = cam.ScreenToWorldPoint(mousepostion);
    }
    #endregion
    #region - Items -
    IEnumerator CallItemUpdate()
    {
        foreach (var item in items)
        {
            item.item.OnUpdate(this, item.stacks);
        }
        yield return new WaitForSeconds(1);
        StartCoroutine(CallItemUpdate());
    }
    public void CallItemOnHit(Enemy enemy)
    {
        foreach (var item in items)
        {
            item.item.OnHit(this, enemy, item.stacks);
        }
    }
    public void CallItemOnCreate(Transform target)
    {
        foreach (var item in items)
        {
            item.item.OnCreate(this, target);
        }
    }

    public void CallItemOnStandStill()
    {
        foreach (var item in items)
        {
            item.item.OnStandStill(this, item.stacks);
        }
    }
    #endregion
    #region - TurnBackTime -

    void TurnBackTimePressed()
    {
        rewindPressed = true;
    }

    void TurnBackTimeReleased()
    {
        rewindPressed = false;
    }

    void HandleTurnBackTime()
    {
        if (rewindPressed)
        {
            rewindValue += rewindIntensity;

            if (!isRewinding)
            {
                rewindManager.StartRewindTimeBySeconds(rewindValue);
                UIManagerSingleton.Instance.TimeTravelImageStart(1f);
                //rewindSound.Play();
            }
            else
            {
                if (rewindManager.HowManySecondsAvailableForRewind > rewindValue)
                    rewindManager.SetTimeSecondsInRewind(rewindValue);
            }
            isRewinding = true;
        }
        else
        {
            if (isRewinding)
            {
                rewindManager.StopRewindTimeBySeconds();
                UIManagerSingleton.Instance.TimeTravelImageStop(1f);
                //rewindSound.Stop();
                rewindValue = 0;
                isRewinding = false;
            }
        }
    }
    #endregion

    #region - Shooting -
    private void HandleShootPointTurning()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
        Vector2 facingDirection = input_Mouse - rb.position;
        float angle = Mathf.Atan2(facingDirection.y, facingDirection.x) * Mathf.Rad2Deg - 90;

        rotationPointRb2d.MoveRotation(angle);
    }

    private void Shoot()
    {
        if (UIManagerSingleton.Instance.paused)
        {
            return;
        }
        if (shootTimer >= 1f / attackSpeed)
        {
            GameObject gameObject = Instantiate(bullet, shootPoint.position, shootPoint.rotation);
            Rigidbody2D rbG = gameObject.GetComponent<Rigidbody2D>();
            Vector2 direction = shootPoint.position - rotationPoint.position;
            animator.SetTrigger("Shoot");
            shootTimer = 0f;

            rbG.AddForce(direction * shootSpeed + (inheritMovement ? Vector2.zero : Vector2.zero), ForceMode2D.Impulse);
            CallItemOnCreate(gameObject.transform);
        }
    }
    #endregion
    #region - UI -
    private void EscapePressed()
    {
        if (UIManagerSingleton.Instance.paused)
        {
            UIManagerSingleton.Instance.UnpauseGame();
        }
        else
        {
            UIManagerSingleton.Instance.PauseGame();
        }
    }
    #endregion

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
