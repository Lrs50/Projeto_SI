using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//This class is responsible for creating the map based on the Perlin noise

public  class CreateWorldState : BaseState
{
    //Map properties
    private float size;
    private float org;
    public float squareSize;
    private float offset;

    // User defined properties 
    private float squareCount = 50;
    private float scale = 12;

    // Structure that holds all of the map entities
    private Grid_ grid;

    // List with all the valid positions
    public List<Vector2> validPos = new List<Vector2>();

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

        bool validMap = false;
        while(!validMap){
            validPos.Clear();
            offset = Random.Range(0f,squareCount*1000);
            for(float y=0;y<squareCount;y++){
                for(float x=0;x<squareCount;x++){
                    
                    float xCoord = (x / squareCount) +offset;
                    float yCoord = (y / squareCount) +offset;

                    float sample = Mathf.PerlinNoise(xCoord*scale,yCoord*scale);

                    grid.gridarray[(int)x,(int)y].SetType(sample);
                    if( grid.gridarray[(int)x,(int)y].type!="Wall"){
                        validPos.Add(new Vector2(x,y));
                    }
                }
            }

            validMap = validateMap(game);
        }
        

        game.grid = grid;
        game.validPos = validPos;
        
        game.player.transform.localScale = new Vector3(squareSize*1.2f,squareSize*1.2f,1);
        game.player.transform.position = game.GetRandomValidPos() + new Vector3(squareSize/2,squareSize/2);
        game.food.transform.localScale = new Vector3(squareSize*1.75f,squareSize*1.75f,1) ;
        game.food.transform.position = game.GetRandomValidPos() + new Vector3(squareSize/2,squareSize/2);

        game.player.SetActive(true);
        game.food.SetActive(true);

        game.SwitchState(game.pathFinding);
    }

    private bool validateMap(GameManager game){
        
        Queue<Vector3> queue = new Queue<Vector3>();
        HashSet<Vector3> exploredNodes = new HashSet<Vector3>();
        List<Vector2> testNodes = new List<Vector2>(validPos);
        
        //gets the first valid position
        queue.Enqueue(grid.GetWorldPosition((int)validPos[0].x,(int)validPos[0].y));

        while(queue.Count!=0){
            Vector3 currentNode = queue.Dequeue();

            //removes the position of the node from testNodes and goes with the normal BSF
            int x,y;
            grid.GetXY(currentNode,out x,out y);
            Vector2 currentPos = new Vector2(x,y);
            testNodes.Remove(currentPos);

            List<Vector3> nodes = grid.GetNodeNeighbors(currentNode);

            foreach(Vector3 node in nodes){
                if(!exploredNodes.Contains(node)){
                    exploredNodes.Add(node);

                    queue.Enqueue(node);
                }
            }
        }

        // if i have visited all valid nodes it's a valid map
        //Debug.Log(testNodes.Count);
        return (testNodes.Count==0);
    }
    
    public override void FixedUpdateState(GameManager game){}

}
