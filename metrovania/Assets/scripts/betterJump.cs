using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class betterJump : MonoBehaviour
{

    public float fallmultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    

    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        if(Physics2D.gravity.normalized.y != 0)
        {
            if(Physics2D.gravity.normalized.y < 0)
            {
                if (rb.velocity.y < 0)
                    rb.velocity += Vector2.up * Physics2D.gravity * (fallmultiplier - 1) * Time.deltaTime;
                else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
                    rb.velocity += Vector2.up * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
            else
            {
                if(rb.velocity.y > 0)
                    rb.velocity += Vector2.up * Physics2D.gravity * (fallmultiplier - 1) * Time.deltaTime;
                else if (rb.velocity.y < 0 && !Input.GetButton("Jump"))
                    rb.velocity += Vector2.up * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
        }
        else
        {
            if (Physics2D.gravity.normalized.x < 0)
            {
                if (rb.velocity.x < 0)
                    rb.velocity += Vector2.right * Physics2D.gravity * (fallmultiplier - 1) * Time.deltaTime;
                else if (rb.velocity.x > 0 && !Input.GetButton("Jump"))
                    rb.velocity += Vector2.right * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
            else
            {
                if (rb.velocity.x > 0)
                    rb.velocity += Vector2.right * Physics2D.gravity * (fallmultiplier - 1) * Time.deltaTime;
                else if (rb.velocity.x < 0 && !Input.GetButton("Jump"))
                    rb.velocity += Vector2.right * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
        }

    }
}
