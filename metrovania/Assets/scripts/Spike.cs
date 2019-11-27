using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Spike : MonoBehaviour
{
   private BoxCollider2D boxCollider;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject == CharacterController2D.ControlledPlayer())
        {
            other.gameObject.GetComponent<Respawn>().ThePlayerHasDied();
        }
    }

}
