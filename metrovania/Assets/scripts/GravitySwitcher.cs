using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySwitcher : MonoBehaviour
{

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.LeftArrow))
        //{
        //    Physics2D.gravity = new Vector2(-9.81f, 0f);
        //    Debug.Log(Physics2D.gravity);
        //}
        //if (Input.GetKeyDown(KeyCode.DownArrow))
        //{
        //    Physics2D.gravity = new Vector2(0f, -9.81f);
        //    Debug.Log(Physics2D.gravity);
        //}
        //if (Input.GetKeyDown(KeyCode.UpArrow))
        //{
        //    Physics2D.gravity = new Vector2(0f, 9.81f);
        //    Debug.Log(Physics2D.gravity);
        //}
        //if (Input.GetKeyDown(KeyCode.RightArrow))
        //{
        //    Physics2D.gravity = new Vector2(9.81f, 0f);
        //    Debug.Log(Physics2D.gravity);
        //}

           if (Input.GetKeyDown(KeyCode.LeftArrow))
           {
               Vector2 v = GravityRotator(Physics2D.gravity.normalized, -90);
        
               if (v == new Vector2(-1f, 0f))
               {
                   Physics2D.gravity = new Vector2(-9.81f, 0f);
               }
               else if(v == new Vector2(1f, 0f))
               {
                   Physics2D.gravity = new Vector2(9.81f, 0f);
               }
               else if (v == new Vector2(0f, 1f))
               {
                   Physics2D.gravity = new Vector2(0f, 9.81f);
               }
               else if (v == new Vector2(0f, -1f))
               {
                   Physics2D.gravity = new Vector2(0f, -9.81f);
               }
           }
        
           if (Input.GetKeyDown(KeyCode.RightArrow))
           {
               Vector2 v = GravityRotator(Physics2D.gravity.normalized, 90);
        
               if (v == new Vector2(-1f, 0f))
               {
                   Physics2D.gravity = new Vector2(-9.81f, 0f);
               }
               else if (v == new Vector2(1f, 0f))
               {
                   Physics2D.gravity = new Vector2(9.81f, 0f);
               }
               else if (v == new Vector2(0f, 1f))
               {
                   Physics2D.gravity = new Vector2(0f, 9.81f);
               }
               else if (v == new Vector2(0f, -1f))
               {
                   Physics2D.gravity = new Vector2(0f, -9.81f);
               }
           }
    }

    Vector2 GravityRotator(Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
    }
}
