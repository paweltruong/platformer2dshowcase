using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class CharacterMove : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed = 2;

    //Overall Jump/Fall settings
    [SerializeField] float jumpPower = 4;
    [Tooltip("Constrain jump speed (useful when set seconJumpPowerMultiplier)")]
    [SerializeField] float jumpMaxSpeed = 6;
    [SerializeField] float fallMaxSpeed = 10;
    //First Jump
    [Tooltip("Interval for first jump input detection")]
    [SerializeField] float firstJumpCooldown = 0.5f;
    //Second Jump
    [Tooltip("If we want second jump to be stronger then value should be greater than 1")]
    [Range(0.01f, 10f)]
    [SerializeField] float secondJumpPowerMultiplier = 1.3f;
    [Tooltip("time in seconds in which we can jump for second time after first jump (time window after first jump that allow to use second jump)")]
    [SerializeField] float secondJumpAvailableTime = 0.5f;
    //Wall Slide
    [SerializeField] float wallSlideSpeed = 6;
    //Wall Jump
    [SerializeField] float wallJumpPowerMultiplier = 1.3f;
    [SerializeField] float wallBounceOffPower = 2f;
    [Tooltip("Assist player when bouncing (invert input for shot time when bouncing of the wall")]
    [Range(0, 1f)]
    [SerializeField] float wallBounceInputCooldown = 0.25f;
    [Tooltip("When player is bouncing in opposite direction of the wall, smooth the bounce back <0.1;1) to amplify (1;10> to dampen")]
    [Range(0.1f, 10f)]
    [SerializeField] float wallBounceSmoothing = 1.5f;
    [Tooltip("After wall jump is triggered, disable wall sliding for some time to be able to take off from the wall and not be dragged down")]
    [SerializeField] float wallSlideCooldown = 0.25f;


    [Header("Animations")]
    [Tooltip("Check when sprite character is by default looking left")]
    [SerializeField] bool isFacingRightSprite = true;

    [Header("GroundCheck & Wall Detection")]
    [SerializeField] LayerMask groundCheckLayers;
    [SerializeField] bool isGrounded;
    [SerializeField] float groundCheckBonusRange = 0.01f;
    [SerializeField] Collider2D groundCheckCollider;
    [SerializeField] LayerMask wallCheckLayers;
    [SerializeField] bool isFacingWall;
    [SerializeField] Collider2D frontWallCheckCollider;

    Animator animator;
    Rigidbody2D rb;
    Collider2D col;

    bool facingRight = true;
    float previousVelocity;
    float velocity;
    [Header("Debug")]
    //For now made public - for debugging purposes
    public bool firstJumpRequested;
    public bool secondJumpRequested;
    public bool wallJumpRequested;
    public bool secondJumpOnCooldown;
    /// <summary>
    /// Interval for first jump input detection
    /// </summary>
    public float firstJumpCooldownCurrent;
    /// <summary>
    /// timespan in seconds when second jump is available to trigger after first jump
    /// </summary>
    public float secondJumpAvailableTimeCurrent;
    /// <summary>
    /// After wall jump is triggered, disable wall sliding for some time to be able to take off from the wall and not be dragged down
    /// </summary>
    public float wallSlideCooldownCurrent;
    public float wallBounceInputCooldownCurrent;
    /// <summary>
    /// -1 for left, 1 for 0 & right
    /// </summary>
    public float wallBounceDirection;

    bool isFrozen;

    bool IsDirectionChanged
    {
        get
        {
            if (previousVelocity != 0 && velocity != 0)
            {
                return Mathf.Sign(previousVelocity) != Mathf.Sign(velocity);
            }
            else if ((facingRight && velocity < 0)//need to change from right to left
                || (!facingRight && velocity > 0))//need to change from left to right
                return true;
            return false;
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        SetInitialDirection();
    }

    void Start()
    {
        if (groundCheckCollider == null || frontWallCheckCollider == null)
            Debug.LogError("Ground & Wall check colliders need to be set up!");
        if (fallMaxSpeed < wallSlideSpeed)
            Debug.LogError("Wall slide speed is greater than overall max fall speed");

    }

    void Update()
    {
        //set input & state
        previousVelocity = velocity;
        velocity = Input.GetAxisRaw("Horizontal");

        UpdateFirstJumpCooldown();
        UpdateSecondJumpAvailablilityTimer();
        UpdateWallSlideCooldown();
        UpdateWallBounceInputCooldown();

        HandleJumpInputAndFields();

        AdjustSpriteDirectionIfNeeded();
        SetAnimation();
    }

    void FixedUpdate()
    {
        isGrounded = IsGrounded();
        isFacingWall = IsFrontFacingWall();
        Move();
        Jump();
        ApplyWallSlide();
        WallJump();
        CheckFreeze();
    }

    void CheckFreeze()
    {
        if (isFrozen)
            rb.MovePosition(transform.position);
    }

    void HandleJumpInputAndFields()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded && firstJumpCooldownCurrent <= 0)
            {
                //Debug.Log("Update FirstJumpRequested");
                firstJumpRequested = true;
                firstJumpCooldownCurrent = firstJumpCooldown;
                secondJumpRequested = false;
                secondJumpOnCooldown = false;
            }
            else if (!isGrounded && isFacingWall)
            {
                //Debug.Log("Update WallJumpRequested");
                wallJumpRequested = true;
                wallSlideCooldownCurrent = wallSlideCooldown;
                wallBounceInputCooldownCurrent = wallBounceInputCooldown;
            }
            else if (secondJumpAvailableTimeCurrent > 0 && !secondJumpOnCooldown)
            {
                //Debug.Log("Update SecondJumpRequested");
                secondJumpRequested = true;
                secondJumpOnCooldown = true;
            }
        }
    }

    void SetInitialDirection()
    {
        //set up proper facingRight field value
        if (!isFacingRightSprite)
        {
            SwitchDirection();
            facingRight = true;
            //Debug.Log($"Direction flipped, current scale: {transform.localScale.x}");
        }
    }
    void Move()
    {
        if (velocity != 0)
        {
            //help player bouncing off the wall, to make this move pleasant and not struggle
            if (wallBounceInputCooldownCurrent > 0)
            {
                var newVelocity = new Vector2(velocity * speed, rb.velocity.y);
                //when bounce direction differ from input and bounceCooldown active, apply wallbounce assistance to player
                if (Mathf.Sign(newVelocity.x) != wallBounceDirection)
                {
                    newVelocity.x *= -1f / wallBounceSmoothing;
                    //Debug.Log("Bounce assistance applied");
                }
                rb.velocity = newVelocity;
            }
            else
                rb.velocity = new Vector2(velocity * speed, rb.velocity.y);
        }
        else
            rb.velocity = new Vector2(0, rb.velocity.y);//stop immediately
    }
    void AdjustSpriteDirectionIfNeeded()
    {
        if (IsDirectionChanged)
        {
            SwitchDirection();
            //Debug.Log($"Direction change: pv:{previousVelocity} v:{velocity}");
        }
    }
    void SwitchDirection()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y);
        facingRight = !facingRight;
    }

    /// <summary>
    /// used when loading data to scene to set player/sprite state to adjust direction on move
    /// </summary>
    /// <param name="scaleX"></param>
    public void SetDirection(float scaleX)
    {
        if (Mathf.Sign(transform.localScale.x) != scaleX)
        {
            SwitchDirection();
        }
    }


    void UpdateFirstJumpCooldown()
    {
        if (firstJumpCooldownCurrent > 0)
            firstJumpCooldownCurrent -= Time.deltaTime;
        else
            firstJumpCooldownCurrent = 0;
    }
    private void UpdateSecondJumpAvailablilityTimer()
    {
        if (secondJumpAvailableTimeCurrent > 0)
            secondJumpAvailableTimeCurrent -= Time.deltaTime;
        else
            secondJumpAvailableTimeCurrent = 0;
    }
    void UpdateWallSlideCooldown()
    {
        if (wallSlideCooldownCurrent > 0)
            wallSlideCooldownCurrent -= Time.deltaTime;
        else
            wallSlideCooldownCurrent = 0;
    }

    void UpdateWallBounceInputCooldown()
    {
        if (wallBounceInputCooldownCurrent > 0)
            wallBounceInputCooldownCurrent -= Time.deltaTime;
        else
            wallBounceInputCooldownCurrent = 0;
    }


    void Jump()
    {
        if (firstJumpRequested || secondJumpRequested)
        {
            if (firstJumpRequested)
                secondJumpAvailableTimeCurrent = secondJumpAvailableTime;

            var newVelocity = rb.velocity;
            var jumpPowerMultiplier = secondJumpRequested ? secondJumpPowerMultiplier : 1;
            //setup new value to ignore falling velocity (dont add jumping force -> replace existing y force)
            newVelocity.y = Mathf.Clamp(Vector2.up.y * jumpPower * jumpPowerMultiplier, -fallMaxSpeed, jumpMaxSpeed);
            rb.velocity = newVelocity;

            firstJumpRequested = false;
            secondJumpRequested = false;
        }
    }

    void WallJump()
    {
        if (wallJumpRequested)
        {
            var newVelocity = rb.velocity;
            newVelocity.x = -velocity * wallBounceOffPower;
            wallBounceDirection = Mathf.Sign(newVelocity.x);
            newVelocity.y = Mathf.Clamp(Vector2.up.y * jumpPower * wallJumpPowerMultiplier, -fallMaxSpeed, jumpMaxSpeed);
            rb.velocity = newVelocity;
            //Debug.Log($"WallJump: {wallJumpRequested} vel:{rb.velocity}");
            wallJumpRequested = false;
        }
    }

    void ApplyWallSlide()
    {
        if (isFacingWall && !isGrounded && wallSlideCooldownCurrent <= 0)
        {
            var newVelocity = rb.velocity;
            //force slide down a wall (instead of stucking to it - if we add slippery physics material it will work but for spriteshape it will also slide on the top suface)
            newVelocity.y = Mathf.Clamp(Vector2.down.y * wallSlideSpeed, -fallMaxSpeed, -wallSlideSpeed);
            rb.velocity = newVelocity;
        }
    }



    void SetAnimation()
    {
        animator.SetFloat("Velocity", velocity);
        animator.SetBool("IsMoving", velocity != 0);
        animator.SetBool("IsGrounded", isGrounded);
        if (firstJumpRequested || secondJumpRequested)
            animator.SetTrigger("Jump");
    }


    bool IsGrounded()
    {
        Collider2D[] results = new Collider2D[1];
        groundCheckCollider.OverlapCollider(new ContactFilter2D { layerMask = groundCheckLayers, useLayerMask = true }, results);
        if (results.Any(r => r != null))
            return true;
        return false;
    }

    bool IsFrontFacingWall()
    {
        Collider2D[] results = new Collider2D[1];
        frontWallCheckCollider.OverlapCollider(new ContactFilter2D { layerMask = wallCheckLayers, useLayerMask = true }, results);
        if (results.Any(r => r != null))
            return true;
        return false;
    }

    public void Freeze()
    {
        isFrozen = true;
    }
}
