using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;							// Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround = 0;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck = null;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck = null;
    [SerializeField] private Transform m_RightCheck = null;                        
    [SerializeField] private Transform m_LeftCheck = null;
    [SerializeField] private SpriteRenderer CharacterSprite = null;
    [SerializeField] private Collider2D m_CrouchDisableCollider = null;                // A collider that will be disabled when crouching
    [SerializeField] private float wallJumpForceX = 1f;
    [SerializeField] private float wallJumpForceY = 1f;
    [SerializeField] private float wallSlideForce = 1f;
    [SerializeField] private float wallJumptimer = 2f;

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded = true;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D playerRigidbody;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;

    private static GameObject contolledPlayer;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;
    public UnityEvent OnJumpEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;
    private bool canDubbleJump = false;

    float tickUp = 0f;
    bool shouldTick = false;
    Vector3 velocityToAply;
    public float speed;

    private void Awake()
	{
        contolledPlayer = gameObject;

		playerRigidbody = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}

    public static GameObject ControlledPlayer()
    {
        return contolledPlayer;
    }

    public void AddForceToControlledPawn()
    {
        playerRigidbody.velocity = Vector2.zero;
        playerRigidbody.AddForce(-Physics2D.gravity.normalized * m_JumpForce);
    }

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}
	}
    private void Update()
    {
        float step = speed * Time.deltaTime;

        if(Physics2D.gravity.normalized == Vector2.down)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, 0), step);
        }
        else if (Physics2D.gravity.normalized == Vector2.up)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, 180), step);
        }
        else if (Physics2D.gravity.normalized == Vector2.left)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, -90), step);
        }
        else if (Physics2D.gravity.normalized == Vector2.right)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, 90), step);
        }
    }


    public void Move(float move, bool crouch, bool jump)
	{
		// If crouching, check to see if the character can stand up
		if (!crouch)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				crouch = true;
			}
		}

		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{

			// If crouching
			if (crouch)
			{
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= m_CrouchSpeed;

				// Disable one of the colliders when crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			}
            else
			{
				// Enable the collider when not crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

            // Move the character by finding the target velocity while taking in consideration for at what the angle gravity pulls
            Vector3 targetVelocity;
            if (Physics2D.gravity.normalized.y != 0)
			    targetVelocity = new Vector2(-Physics2D.gravity.normalized.y * move * 10f, playerRigidbody.velocity.y);
            else
                targetVelocity = new Vector2(playerRigidbody.velocity.x, Physics2D.gravity.normalized.x * move * 10f);

            // And then smoothing it out and applying it to the character
            if (!shouldTick)
			    playerRigidbody.velocity = Vector3.SmoothDamp(playerRigidbody.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}

        if(m_Grounded)
        {
            canDubbleJump = true;
        }

        // oposit of gravity
        Vector2 jumpDir = -Physics2D.gravity.normalized;

        // If the player should jump...
        if (m_Grounded && jump)
		{
			// Add a vertical force to the player.
			m_Grounded = false;
			playerRigidbody.AddForce(jumpDir * m_JumpForce);
            OnJumpEvent.Invoke();
        }
        else if(!m_Grounded && jump)
        {
            //wall and duoble jump
            if(Physics2D.OverlapCircle(m_RightCheck.position, k_GroundedRadius, m_WhatIsGround))
            {
                tickUp = 0f;
                canDubbleJump = true;
                shouldTick = true;
                playerRigidbody.velocity = Vector3.zero;
                
                if(Physics2D.gravity.normalized.y != 0)
                {
                    velocityToAply = (new Vector2(-Physics2D.gravity.normalized.y * -wallJumpForceX, -Physics2D.gravity.normalized.y * wallJumpForceY));
                }
                else
                {
                    velocityToAply = (new Vector2(-Physics2D.gravity.normalized.x * wallJumpForceY, Physics2D.gravity.normalized.x * -wallJumpForceX));
                }
                OnJumpEvent.Invoke();
            }
            else if(Physics2D.OverlapCircle(m_LeftCheck.position, k_GroundedRadius, m_WhatIsGround))
            {
                tickUp = 0f;
                canDubbleJump = true;
                shouldTick = true;
                playerRigidbody.velocity = Vector2.zero;
                if (Physics2D.gravity.normalized.y != 0)
                {
                    velocityToAply = (new Vector2(-Physics2D.gravity.normalized.y * wallJumpForceX, -Physics2D.gravity.normalized.y * wallJumpForceY));
                }
                else
                {
                    velocityToAply = (new Vector2(-Physics2D.gravity.normalized.x * wallJumpForceY, Physics2D.gravity.normalized.x * wallJumpForceX));
                }
                OnJumpEvent.Invoke();
            }
            else if(canDubbleJump)
            {
                if(jumpDir.y != 0f)
                    playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x,0f);
                else
                    playerRigidbody.velocity = new Vector2(0f, playerRigidbody.velocity.y);

                OnJumpEvent.Invoke();
                playerRigidbody.AddForce(jumpDir * m_JumpForce);
                canDubbleJump = false;
            }
        }

        //stick to walls
        if(!m_Grounded && !jump)
        {
            if (Physics2D.OverlapCircle(m_RightCheck.position, k_GroundedRadius/5, m_WhatIsGround) && move > 0f)
            {
                tickUp = 0f;
                shouldTick = true;
                playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, 0f);
                if (Physics2D.gravity.normalized.y != 0)
                    velocityToAply = (new Vector2(0f, wallSlideForce));
                else
                {
                    velocityToAply = (new Vector2(wallSlideForce, 0f));
                }
                OnJumpEvent.Invoke();
            }
            else if (Physics2D.OverlapCircle(m_LeftCheck.position, k_GroundedRadius/5, m_WhatIsGround) && move < 0f)
            {
                tickUp = 0f;
                shouldTick = true;
                playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, 0f);
                if (Physics2D.gravity.normalized.y != 0)
                    velocityToAply = (new Vector2(0f, wallSlideForce));
                 else
                {
                    velocityToAply = (new Vector2(wallSlideForce, 0f));
                }
                OnJumpEvent.Invoke();
            }
        }
        
        if (shouldTick)
        {
            tickUp += Time.deltaTime;
            playerRigidbody.velocity = velocityToAply;
            if (tickUp>wallJumptimer)
            {
               
                tickUp = 0f;
                shouldTick = false;
            }
        }
    }

	private void Flip()
	{
		m_FacingRight = !m_FacingRight;
        CharacterSprite.flipX = !CharacterSprite.flipX;
    }
}
