using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public bool isCollidingWithFood = false;
    public Sprite[] sprites;
    public bool isMoving = false;
    private int index;
    private int count=0;
    private int animationSpeed = 10;
    private int animationOffset=4;
    public int previousdirection=0;
    public int direction=0;
    private SpriteRenderer spriteRenderer;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag=="Food"){
            isCollidingWithFood = true;
        }
    }

    private void FixedUpdate()
    {
        if(isMoving){
            
            if(direction !=previousdirection){
                count = animationSpeed;
                previousdirection = direction;
            }       
            count++;
            if(index>3){
                index=0;
            }
            if(count>animationSpeed){

                spriteRenderer.sprite = sprites[(direction*animationOffset) + index];
            
                count = 0;
                index++;
            }
            

        }


    }


}
