using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Basic class that holds the information of one position in the grid
public class Node
{

    private GameObject gameObject;
    public SpriteRenderer spriteRenderer;

    //Element Sprites
    private Sprite[] landSprites;
    private Sprite[] waterSprites;
    private Sprite[] mudSprites;
    private Sprite[] wallSprites;

    //Node properties
    public string type;
    public int cost;
    public int animSpeed = 50; //FPS = 50/animSpeed
    public int animCount = 0;

    public Node (Vector3 pos, float size, Transform parent, Sprite[] landSprites, Sprite[] waterSprites, Sprite[] mudSprites, Sprite[] wallSprites){
        gameObject = new GameObject("node");
        gameObject.transform.parent = parent;
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

        gameObject.transform.position = pos;
        gameObject.transform.localScale = new Vector3(size * 5, size * 5, 1);

        this.landSprites = landSprites;
        this.waterSprites = waterSprites;
        this.mudSprites = mudSprites;
        this.wallSprites = wallSprites;
    } 

    // Chooses a random terrain with custom randomness
    private int terrainRandom(){
        float x = Random.Range(0, 100);
        if (x < 35)
            return 0;
        if (x < 70)
            return 1;
        if (x < 85)
            return 2;
        else
            return 3;
    }

    // Selects a type for the node based on the value assigned 
    public void SetType(float val){
        if (val < 0.25f){
            type = "Water";
            cost = 10;
            spriteRenderer.sprite = waterSprites[Random.Range(0, 3)];
        }else if(val < 0.35f){
            type = "Mud";
            spriteRenderer.sprite = mudSprites[terrainRandom()];
            cost = 5;
        }else if(val < 0.5f){
            type = "Land";
            spriteRenderer.sprite = landSprites[terrainRandom()];
            cost = 1;
        }else{
            type ="Wall";
            spriteRenderer.sprite = wallSprites[terrainRandom()];
            cost = int.MaxValue;
        }
    }

    public Vector3 GetPos(){
        return gameObject.transform.position;
    }

    public void FixedUpdateNode(){
        if (type == "Water"){
            animCount++;
            if (animCount > animSpeed){
                animCount = 0;
                spriteRenderer.sprite = waterSprites[Random.Range(0, 3)];
            }
        }
    }
}
