using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    private Animator animator;
    private Rigidbody2D rb2d;
    private Collider2D playerCollder;
    private float jumpCD;
    private float horizontalInput;

    private bool isWallSliding;
    private float wallSlideSpeed;

    private bool isWallJumping;
    private float wallJumpDirection;
    private float wallJumpingTime = 0.2f; //time before being able to wall jump
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;


    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerCollder = GetComponent<Collider2D>();
        cam = Camera.main;

    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        //Flip player when facing left/right.
        if (rb2d.velocity.x > 0.01f)
            transform.localScale = Vector3.one;
        else if (rb2d.velocity.x < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);

        //Sets animation parameters
        animator.SetBool("Run", horizontalInput != 0);
        animator.SetBool("Grounded", IsGrounded());
        
        if (Input.GetKey(KeyCode.Space))
            Jump();

        WallSlide();
        WallJump();

    }

    private void FixedUpdate()
    {
        if (!isWallJumping)
        {
            rb2d.velocity = new Vector2(horizontalInput * speed, rb2d.velocity.y);
        }
    }

    private void Jump() {
        //Logic for Jumping while on the ground
        if (IsGrounded())
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce * 1.5f);
            animator.SetTrigger("Jump");
            Debug.Log("Ground Jump");
        }
    }

    private void WallSlide()
    {
        if (OnWall() && !IsGrounded() && horizontalInput != 0f)
        {
            isWallSliding = true;
            rb2d.velocity = new Vector2(rb2d.velocity.x, Mathf.Clamp(rb2d.velocity.y, -wallSlideSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb2d.velocity = new Vector2(wallJumpDirection * jumpForce * 0.8f, jumpForce * 1.6f);
            wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpDirection)
            {
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    private bool IsGrounded() {
        RaycastHit2D ray = Physics2D.BoxCast(playerCollder.bounds.center, playerCollder.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return ray.collider != null;
    }
    private bool OnWall()
    {
        RaycastHit2D ray = Physics2D.BoxCast(playerCollder.bounds.center, playerCollder.bounds.size, 0, new Vector2(transform.localScale.x, 0f), 0.1f, wallLayer);
        return ray.collider != null;
    }
    public bool CanAttack()
    {
        return horizontalInput == 0 && IsGrounded() || !OnWall() && !GetComponent<Health>().IsDead();
    }
}
