using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySwithcer3D : MonoBehaviour
{
    float grav;

    private void Start()
    {
        grav = Mathf.Abs(Physics.gravity.y);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Vector3 v = VectorRotator(Physics.gravity.normalized, -90);

            if(v == new Vector3(-1f, 0f))
            {
                Physics.gravity = new Vector3(-grav, 0f, 0f);
            }
            else if(v == new Vector3(1f, 0f))
            {
                Physics.gravity = new Vector3(grav, 0f, 0f);
            }
            else if (v == new Vector3(0f, 1f))
            {
                Physics.gravity = new Vector3(0f, grav,0f);
            }
            else if(v == new Vector3(0f,-1f))
            {
                Physics.gravity = new Vector3(0f, -grav,0f);
            }
        }

        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            Vector3 v = VectorRotator(Physics.gravity.normalized, 90);

            if(v == new Vector3(-1f, 0f))
            {
                Physics.gravity = new Vector3(-grav, 0f, 0f);
            }
            else if (v == new Vector3(1f, 0f))
            {
                Physics.gravity = new Vector3(grav, 0f,0f);
            }
            else if (v == new Vector3(0f, 1f))
            {
                Physics.gravity = new Vector3(0f, grav,0f);
            }
            else if (v == new Vector3(0f, -1f))
            {
                Physics.gravity = new Vector3(0f, -grav, 0f);
            }
        }
    }

    Vector3 VectorRotator(Vector3 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        return new Vector3(cos * tx - sin * ty, sin * tx + cos * ty);
    }
}
