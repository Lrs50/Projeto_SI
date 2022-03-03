using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingState : BaseState
{

    private Rigidbody2D playerbody;
    private List<Vector2> path;
    private Vector3 offset;
    private float speed = 50;
    private int pathIndex;

    public override void EnterState(GameManager game){
        playerbody = game.player.GetComponent<Rigidbody2D>();
        path = game.path;
        offset = new Vector3(game.createWorld.squareSize/2,game.createWorld.squareSize/2);
        pathIndex = path.Count-1;
        
    }

    private void Move(GameManager game){


        Vector3 foodpos = game.grid.GetWorldPosition((int)path[pathIndex].x,(int)path[pathIndex].y) + offset;
        Vector3 playerpos = game.player.transform.position;
        //Debug.Log($"predicted {foodpos} real {game.food.transform.position}");
        Vector3 direction = foodpos - playerpos;
        if(direction.magnitude < 1f){
            pathIndex --;
            foodpos = game.grid.GetWorldPosition((int)path[pathIndex].x,(int)path[pathIndex].y) + offset;
            direction = foodpos - playerpos;
            playerbody.velocity=Vector3.zero;
        }

        if(pathIndex>=0){
            direction.Normalize();
            playerbody.velocity = direction*speed;
        }

    }

    public override void FixedUpdateState(GameManager game){
        Move(game);
    }

    public override void UpdateState(GameManager game){
        
    }

    public override void OnCollisionEnter(GameManager game,Collision2D other){
        if(other.gameObject.tag=="Food"){
            
        }
    }
}
