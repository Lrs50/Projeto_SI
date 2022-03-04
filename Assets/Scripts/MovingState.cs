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
    Player player;

    public override void EnterState(GameManager game){
        playerbody = game.player.GetComponent<Rigidbody2D>();
        path = game.path;
        offset = new Vector3(game.createWorld.squareSize/2,game.createWorld.squareSize/2);
        pathIndex = path.Count-1;
        player = game.player.GetComponent<Player>();

        game.cost = 0;
        game.costText.text = "Cost: "+game.cost.ToString();
        
    }

    private void Move(GameManager game){


        Vector3 foodpos = game.grid.GetWorldPosition((int)path[pathIndex].x,(int)path[pathIndex].y) + offset;
        Vector3 playerpos = game.player.transform.position;
        //Debug.Log($"predicted {foodpos} real {game.food.transform.position}");
        Vector3 direction = foodpos - playerpos;
        if(direction.magnitude < 1f){
            pathIndex --;
            game.cost += game.grid.gridarray[(int)path[pathIndex].x,(int)path[pathIndex].y].cost;
            game.costText.text = "Cost: "+game.cost.ToString();
        }

        if (pathIndex >= 0){
            direction.Normalize();
            if (game.grid.gridarray[(int)path[pathIndex].x, (int)path[pathIndex].y].type == "Water")
                playerbody.velocity = direction * speed * 0.4f;

            else if (game.grid.gridarray[(int)path[pathIndex].x, (int)path[pathIndex].y].type == "Mud")
                playerbody.velocity = direction * speed * 0.7f;
            else
                playerbody.velocity = direction * speed;
        }       

        if(player.isCollidingWithFood){

            player.isCollidingWithFood=false;
            game.food.transform.position = game.GetRandomValidPos() + new Vector3(game.createWorld.squareSize/2,game.createWorld.squareSize/2);
            playerbody.velocity=Vector3.zero;
            game.grid.resetColors();
            game.score++;
            game.scoreText.text = "Score: "+game.score.ToString();

            game.SwitchState(game.pathFinding);
            
        }

        

    }

    public override void FixedUpdateState(GameManager game){
        Move(game);
    }

    public override void UpdateState(GameManager game){
        
    }

    public override void OnCollisionEnter(GameManager game,Collision2D other){
    }


}
