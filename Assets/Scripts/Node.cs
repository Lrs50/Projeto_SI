using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{

    private GameObject gameObject;
    public SpriteRenderer spriteRenderer;
    public Color originalColor = Color.black;

    public string type;
    public int cost;

    public  Node(Vector3 pos,float size,Sprite baseSquare,Transform parent){
        gameObject = new GameObject("node");
        gameObject.transform.parent = parent;
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

        gameObject.transform.position = pos;
        spriteRenderer.sprite = baseSquare;
        gameObject.transform.localScale = new Vector3(size,size,1);
    } 

    public void SetType(float val){
        if (val < 0.25f){
            type = "Water";
            cost = 10;
            originalColor = new Color(108f/255f,207f/255f,227f/255f);
        }else if(val < 0.35f){
            type = "Mud";
            cost = 5;
            originalColor = new Color(207f/255f,139f/255f,74f/255f);
        }else if(val < 0.65f){
            type = "Land";
            originalColor = new Color(253f/255f,228f/255f,172f/255f);
            cost = 1;
        }else{
            type ="Wall";
            originalColor = new Color(33f/255f,19f/255f,13f/255f);
            cost = int.MaxValue;
        }
        spriteRenderer.color = originalColor;
        
    }

    public Vector3 GetPos(){
        return gameObject.transform.position;
    }
}
