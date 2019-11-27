using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    private Vector3 RespawnPoint;
    public float hurtTime = 1f;
    Animator playerAnimator;

    bool isRespawning = false;

    CharacterController2D charController;
    PlayerMovement playerMove;

    private void Start()
    {
       playerAnimator = GetComponent<Animator>();
       RespawnPoint = transform.position;
       charController = GetComponent<CharacterController2D>();
       playerMove = GetComponent<PlayerMovement>();
    }

    public void StartDeathAnimation()
    {
        playerAnimator.SetBool("IsDead", true);
    }

    public void RespawnThePlayer()
    {
        playerAnimator.SetBool("IsDead", false);
        transform.position = RespawnPoint;

    }

    public void ThePlayerHasDied()
    {
        StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        //TODO replace this 
        if (!isRespawning)
        {
            isRespawning = true;
            StartDeathAnimation();
            charController.AddForceToControlledPawn();
            playerMove.isDead = true;
            yield return new WaitForSeconds(hurtTime);
            playerMove.isDead = false;
            RespawnThePlayer();
            isRespawning = false;
        }
    }
}
