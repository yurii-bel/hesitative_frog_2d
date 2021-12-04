using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // storing variables
    private Rigidbody2D rb; //rigid body variable
    private BoxCollider2D coll; 
    private SpriteRenderer sprite;
    private Animator anim;

    [SerializeField] private LayerMask jumpableGround;


    private float dirX = 0f;
    [SerializeField] private float moveSpeed = 7f; // [SerializeField] - to see this var in Unity
    [SerializeField] private float jumpForce = 14f;

    private enum MovementState { idle, running, jumping, falling } // create own data type. in unity converted to int values..

    [SerializeField] private AudioSource jumpSourceEffect;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        dirX = Input.GetAxisRaw("Horizontal"); // press right +1 press left -1. we back to 0 immidiately
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y); //detect speed and direction a frame before. rb.velocity.y

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            jumpSourceEffect.Play();
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); // x, y 
        }

        UpdateAnimationState();
    }
    
    private void UpdateAnimationState()
    {
        MovementState state;

        if (dirX > 0f)
        {
            // anim.SetBool("running", true);
            state = MovementState.running;
            sprite.flipX = false;
        }
        else if (dirX < 0f)
        {
            // anim.SetBool("running", true);
            state = MovementState.running;
            sprite.flipX = true;
        }
        else 
        {
            // anim.SetBool("running", false);
            state = MovementState.idle;
        }

        // if check for jumping animation. we don't want to see running animation in the air.

        if (rb.velocity.y > .1f) // value is never exactly equals 0 even if we are staying... we user very small value.. 
        {
            state = MovementState.jumping;
        }
        // if check we might falling.
        else if (rb.velocity.y < -.1f) // downward force applied
        {
            state = MovementState.falling;
        }


        anim.SetInteger("state", (int)state); // turns enum to int representation
    }

    // method checks if we staying on the ground or not
    private bool IsGrounded()
    {
        // create a box around player that has same shape as box collider 
        // 0f- rotation (0 rotation)
        // Vector2.down, .1f moves the box a little bit down
        // checks if we overlaping jumpableGround true / false
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }
}
