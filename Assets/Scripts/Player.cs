using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public bool isCollidingWithFood = false;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag=="Food"){
            isCollidingWithFood = true;
        }
    }



}
