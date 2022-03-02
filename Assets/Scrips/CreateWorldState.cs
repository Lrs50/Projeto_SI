using UnityEngine;
using System.Collections;


public  class CreateWorldState : BaseState
{

    public float size;

    public float org;

    public float squareSize;
    public float squareCount = 50;
    public float scale = 10;
    private float offset;
    private Grid_ grid;

    public override void EnterState(GameManager game){
        
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
