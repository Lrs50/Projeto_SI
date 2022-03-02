using UnityEngine;
using System.Collections;


//This class is responsible for creating the map based on the Perlin noise

public  class CreateWorldState : BaseState
{
    //Map properties
    private float size;
    private float org;
    private float squareSize;
    private float offset;

    // User defined properties 
    private float squareCount = 50;
    private float scale = 10;

    // Structure that holds all of the map entities
    private Grid_ grid;

    public override void EnterState(GameManager game){
        // Setting dinamically the map properties
        
        size = Camera.main.orthographicSize;
        game.size = size;
        squareSize = (size*2) / squareCount;
        org = - (size) + (squareSize/2);

        grid = new Grid_((int)squareCount,(int)squareCount,squareSize,new Vector3(org-squareSize/2,org-squareSize/2),game.baseSquare,game.map.transform);
        
        CreateMap(game);
    }

    public override void UpdateState(GameManager game){

    }

    public override void OnCollisionEnter(GameManager game,Collision2D other){

    }

    private void CreateMap(GameManager game)
    {
        // Creates a map based on Perlin noise

        offset = Random.Range(0f,squareCount*1000);
        for(float y=0;y<squareCount;y++){
            for(float x=0;x<squareCount;x++){
                
                float xCoord = (x / squareCount) +offset;
                float yCoord = (y / squareCount) +offset;

                float sample = Mathf.PerlinNoise(xCoord*scale,yCoord*scale);

                grid.gridarray[(int)x,(int)y].SetType(sample);

            }

        }
        game.grid = grid;
        game.SwitchState(game.pathFindig);
    }

}
