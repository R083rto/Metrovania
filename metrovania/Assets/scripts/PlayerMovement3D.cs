using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement3D : MonoBehaviour
{
    public CharacterController3d controller;
    public Animator playerAnimator;

    public float runSpeed = 40f;
    [HideInInspector] public bool isDead = false;

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

       if(horizontalMove != 0)
        {
            playerAnimator.SetBool("IsRunning", true);
        }
       else
        {
            playerAnimator.SetBool("IsRunning", false);
        }

        if (Input.GetButtonDown("Jump"))
        {
            shouldJump = true;

        }
    }

    public void OnWallClimb()
    {
        playerAnimator.SetTrigger("ClingToWall");
        playerAnimator.SetBool("OnWall", true);
    }

    public void OnLetGoOfWall()
    {
        playerAnimator.SetTrigger("LetGoOfWall");
        playerAnimator.SetBool("IsJumping", true);
        playerAnimator.SetBool("OnWall", false);
    }

    public void OnLanding()
    {
        playerAnimator.SetBool("IsJumping", false);
    }

    public void OnJump()
    {
        playerAnimator.SetTrigger("TakeOf");
        playerAnimator.SetBool("IsJumping", true);
        
    }

    private void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, shouldJump);
        shouldJump = false;

    }
}
