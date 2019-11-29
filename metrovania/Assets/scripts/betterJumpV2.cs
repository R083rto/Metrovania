using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class betterJumpV2 : MonoBehaviour
{

    public float fallmultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    Rigidbody rb;


    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Physics.gravity.normalized.y != 0)
        {
            if (Physics.gravity.normalized.y < 0)
            {
                if(rb.velocity.y < 0)
                    rb.velocity += Vector3.up * Physics.gravity.y * (fallmultiplier - 1) * Time.deltaTime;
                else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
                    rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
             
        }
    }
}
