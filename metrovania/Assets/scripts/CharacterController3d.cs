using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController3d : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400f;
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f; // How much to smooth out the movement
    [SerializeField] private bool m_Aircontrol = true;      // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround = 0;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck = null;                           // A position marking where to check if the player is grounded.
    [SerializeField] private Transform m_CeilingCheck = null;
    [SerializeField] private Transform m_RightCheck = null;
    [SerializeField] private Transform m_LeftCheck = null;
    [SerializeField] private SpriteRenderer CharacterSprite = null;
    [SerializeField] private float wallJumpForceX = 3f;
    [SerializeField] private float wallJumpForceY = 12f;
    [SerializeField] private float wallSlideForce = 0.01f;
    [SerializeField] private float wallJumptimer = 0.15f;

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    [HideInInspector]public bool m_Grounded;            // Whether or not the player is grounded.
    const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
    Rigidbody playerRigidbody;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private Vector3 m_Velocity = Vector3.zero;

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;
    public UnityEvent OnJumpEvent;
    
    private bool canDubbleJump = false;

    float tickUp = 0f;
    bool shouldTick = false;
    Vector3 velocityToAply;
    public float speed;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnJumpEvent == null)
            OnJumpEvent = new UnityEvent();
    }

    private void FixedUpdate()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        Collider[] colliders = Physics.OverlapSphere(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        if(colliders.Length > 0)
        {
            m_Grounded = true;
            if (!wasGrounded)
                OnLandEvent.Invoke();
        }
    }

    private void Update()
    {

        float step = speed * Time.deltaTime;

        if (Physics.gravity.normalized == Vector3.down)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, 0), step);
        }
        else if (Physics.gravity.normalized == Vector3.up)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, 180), step);
        }
        else if (Physics.gravity.normalized == Vector3.left)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, -90), step);
        }
        else if (Physics.gravity.normalized == Vector3.right)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, 90), step);
        }
    }

    public void Move(float move, bool crouch, bool jump)
    {
        // add crouching functionality here
        

        if(m_Grounded || m_Aircontrol)
        {
            // more crouching stuff here

            Vector3 targetVelocity;
            if (Physics.gravity.normalized.y != 0)
                targetVelocity = new Vector3(-Physics.gravity.normalized.y * move * 10f, playerRigidbody.velocity.y);
            else
                targetVelocity = new Vector3(playerRigidbody.velocity.x, Physics.gravity.normalized.x * move * 10f);

            if (!shouldTick)
                playerRigidbody.velocity = Vector3.SmoothDamp(playerRigidbody.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

            if(move > 0 && !m_FacingRight)
            {
                Flip();
            }
            else if (move < 0 && m_FacingRight)
            {
                Flip();
            }
        }
        
        if(m_Grounded)
        {
            canDubbleJump = true;
        }

        Vector3 jumpDir = -Physics.gravity.normalized;

        if(m_Grounded && jump)
        {
            playerRigidbody.AddForce(jumpDir * m_JumpForce);
            OnJumpEvent.Invoke();
        }
        else if(!m_Grounded && jump)
        {
            //wall and double jump
            
            if (IsPlayerWithinInteractionDistanceToWall(m_RightCheck.position))
            {
                tickUp = 0f;
                canDubbleJump = true;
                shouldTick = true;
                playerRigidbody.velocity = Vector3.zero;

                if(Physics.gravity.normalized.y != 0f)
                {
                    velocityToAply = (new Vector3(-Physics.gravity.normalized.y * -wallJumpForceX, -Physics.gravity.normalized.y * wallJumpForceY, 0f));
                }
                else
                {
                    velocityToAply = (new Vector3(-Physics.gravity.normalized.x * wallJumpForceY, Physics.gravity.normalized.x * -wallJumpForceX, 0f));
                }
                OnJumpEvent.Invoke();
            }
            else if (IsPlayerWithinInteractionDistanceToWall(m_LeftCheck.position))
            {
                tickUp = 0f;
                canDubbleJump = true;
                shouldTick = true;
                playerRigidbody.velocity = Vector2.zero;
                if (Physics.gravity.normalized.y != 0)
                {
                    velocityToAply = (new Vector3(-Physics.gravity.normalized.y * wallJumpForceX, -Physics.gravity.normalized.y * wallJumpForceY, 0f));
                }
                else
                {
                    velocityToAply = (new Vector3(-Physics.gravity.normalized.x * wallJumpForceY, Physics.gravity.normalized.x * wallJumpForceX, 0f));
                }
                OnJumpEvent.Invoke();
            }
            else  if(canDubbleJump)
            {
                if (jumpDir.y != 0f)
                    playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, 0f, 0f);
                else
                    playerRigidbody.velocity = new Vector3(0f, playerRigidbody.velocity.y, 0f);

                playerRigidbody.AddForce(jumpDir * m_JumpForce);
                OnJumpEvent.Invoke();
                canDubbleJump = false;
            }
        }

        //stick to walls

        
        if(!m_Grounded && !jump)
        {
            if(IsPlayerWithinInteractionDistanceToWall(m_RightCheck.position) && move >0f || IsPlayerWithinInteractionDistanceToWall(m_LeftCheck.position) && move < 0f)
            {
                tickUp = 0f;
                shouldTick = true;
                playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, 0f);
                if (Physics.gravity.normalized.y != 0f)
                    velocityToAply = (new Vector3(0f, wallSlideForce));
                else
                {
                    velocityToAply = (new Vector3(wallSlideForce, 0f));
                }
                OnJumpEvent.Invoke();
            }
        }
        

        if (shouldTick)
        {
            tickUp += Time.deltaTime;
            playerRigidbody.velocity = velocityToAply;
            if(tickUp > wallJumptimer)
            {
                tickUp = 0f;
                shouldTick = false;
            }
        }
        
    }

    bool IsPlayerWithinInteractionDistanceToWall(Vector3 position)
    {
        if (Physics.CheckSphere(position, k_GroundedRadius / 5, m_WhatIsGround))
            return true;

        return false;
    }

    private void Flip()
    {
        m_FacingRight = !m_FacingRight;
        CharacterSprite.flipX = !CharacterSprite.flipX;
    }
}
