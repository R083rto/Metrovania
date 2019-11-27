using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController2D controller;
    public Animator playerAnimator;

    public float runSpeed = 40f;
    [HideInInspector]public bool isDead = false;

    float horizontalMove;
    bool shouldJump = false;


    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            horizontalMove = 0;
            return;
        }

       horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

       playerAnimator.SetFloat("speed", Mathf.Abs(horizontalMove));

        if (Input.GetButtonDown("Jump"))
        {
            shouldJump = true;
           
        }
    }

    public void OnLanding()
    {
        playerAnimator.SetBool("IsJumping", false);
    }

    public void OnJump()
    {
        if (!playerAnimator.GetBool("IsJumping"))
            playerAnimator.SetBool("IsJumping", true);
        else
        {
            playerAnimator.SetBool("IsJumping", true);
            playerAnimator.Play("placeholder_jump", -1, 0f);
        }
    }

    private void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, shouldJump);
        shouldJump = false;

    }
}
