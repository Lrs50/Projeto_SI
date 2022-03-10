using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//State that recieves the mpath and makes the player follow it 
public class MovingState : BaseState
{

    private Rigidbody2D playerbody;
    private List<Vector2> path;
    private Vector3 offset;
    Player player;

    private float speed = 25;
    private int pathIndex;
    private float originalZoom;
    private float zoom = 0.2f;

    public override void EnterState(GameManager game){

        game.cost = 0;

        
        if(game.path.Count==0){
            player.isCollidingWithFood=false;
            game.food.transform.position = game.GetRandomValidPos() + new Vector3(game.createWorld.squareSize/2,game.createWorld.squareSize/2);
            playerbody.velocity=Vector3.zero;
            game.grid.resetColors();
            game.score++;
            game.scoreText.text = "Score: "+game.score.ToString();

            game.SwitchState(game.pathFinding);
        }

        //Sets the zoom 
        game.zoomBox.SetActive(true);
        originalZoom = Camera.main.orthographicSize;
        if(game.zoom){
            Camera.main.orthographicSize = originalZoom*zoom;
            Camera.main.transform.position = player.transform.position;
        }

        playerbody = game.player.GetComponent<Rigidbody2D>();
        path = game.path;
        offset = new Vector3(game.createWorld.squareSize/2,game.createWorld.squareSize/2);
        pathIndex = path.Count-1;
        player = game.player.GetComponent<Player>();
        player.isMoving = true;
        
        game.costText.text = "Cost: "+game.cost.ToString();

        
    }

    private void Move(GameManager game){

        //Checks if the player has collided with the moves, otherwise move
        if(player.isCollidingWithFood){

            game.food.SetActive(false);
            player.isCollidingWithFood=false;
            
            game.food.transform.position = game.GetRandomValidPos() + new Vector3(game.createWorld.squareSize/2,game.createWorld.squareSize/2);
            while(game.pathFinding.GetMappedVec(game.food.transform.position,game)==game.pathFinding.GetMappedVec(game.player.transform.position,game)){
                game.food.transform.position = game.GetRandomValidPos() + new Vector3(game.createWorld.squareSize/2,game.createWorld.squareSize/2);
            }

            game.food.SetActive(true);
            playerbody.velocity=Vector3.zero;
            game.grid.resetColors();
            game.score++;
            game.scoreText.text = "Score: "+game.score.ToString();
            game.scoreSound.Play();
            
            player.isMoving = false;
                

            Camera.main.orthographicSize = originalZoom;
            Camera.main.transform.position = new Vector3(0,0,-10);

            game.zoomBox.SetActive(false);
            game.SwitchState(game.pathFinding);
                
        }else{

            //gets the foodPos and playerPos and calculates the direction vector
            Vector3 nextPos = game.grid.GetWorldPosition((int)path[pathIndex].x,(int)path[pathIndex].y) + offset;
            Vector3 playerpos = game.player.transform.position;
            Vector3 direction = nextPos - playerpos;

            //If the vector is small enough go to the next path possition
            if(direction.magnitude < 1f){
            
                pathIndex --;
                game.cost += game.grid.gridarray[(int)path[pathIndex].x,(int)path[pathIndex].y].cost;
                game.costText.text = "Cost: "+game.cost.ToString();
                
            }else{
                //Checks player orientation and moves the character 
                if (pathIndex >= 0){
                    direction.Normalize();
                    if(Mathf.RoundToInt(direction.x) == 1){
                        //direita
                        player.direction = 3;
                    }else if(Mathf.RoundToInt(direction.x) == -1){
                        //esquerda
                        player.direction = 2;
                    }else if(Mathf.RoundToInt(direction.y) == 1){
                        //cima
                        player.direction = 1;
                    }else if(Mathf.RoundToInt(direction.y) == -1){
                        //baixo
                        player.direction = 0;
                    }
                    
                    player.isOnWater = false;
                    if (game.grid.gridarray[(int)path[pathIndex].x, (int)path[pathIndex].y].type == "Water"){
                        playerbody.velocity = direction * speed * 0.4f;
                        player.isOnWater = true;
                    }
                    else if (game.grid.gridarray[(int)path[pathIndex].x, (int)path[pathIndex].y].type == "Mud")
                        playerbody.velocity = direction * speed * 0.7f;
                    else
                        playerbody.velocity = direction * speed;

                }       
            }
        }    
    }

    public override void FixedUpdateState(GameManager game){
        //Constrols the zoom
        if(game.zoom){
            Camera.main.orthographicSize = originalZoom*zoom;
            float x = Mathf.MoveTowards(game.cam.transform.position.x, player.transform.position.x, 0.6f);
            float y = Mathf.MoveTowards(game.cam.transform.position.y, player.transform.position.y, 0.6f);
            game.cam.transform.position = new Vector3(x,y,-10);
        }else{
            Camera.main.orthographicSize = originalZoom;
        }
        //Moves the player
        Move(game);
    }

}
