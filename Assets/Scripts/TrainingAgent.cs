using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingAgent 
{

    public DNA dna;
    private Vector2 target;
    public Vector2 position {get;private set;}
    private GameManager game;
    public List<Vector2> path {get;private set;}

    private int pathIndex;
    public bool hasFinished= false;
    public bool reachedGoal=false;
    public float fitness = 0;
    public float distance = 0;
    public Vector2 startPos;

    //the Training Agent contructor 
    public TrainingAgent(DNA newDna,Vector2 target,Vector2 startPos,GameManager game){
        dna = newDna;
        this.target = target;
        position = startPos;
        this.startPos = startPos;
        this.game = game;
        path = new List<Vector2>();
        path.Add(position);
    }

    //the Training Agent moves based on the DNA
    public  void Update()
    {
        if(!hasFinished){
            if(pathIndex >= dna.genes.Count || position == target){
                End();
            }else{
        
                List<Vector2> neighbors = game.grid.GetNodeNeighbors(position);
                position = neighbors[dna.genes[pathIndex]%(neighbors.Count)];
                pathIndex++;
                path.Add(position);
            }
        }
    }

    //Checks if the agent has finished its dna
    private void End(){
        if(position == target){
            reachedGoal = true;
        }
        hasFinished = true;
    }

    private int CountRepeated(){

        Dictionary<Vector2,int> count = new Dictionary<Vector2, int>();

        foreach(Vector2 pos in path){
            if(count.ContainsKey(pos)){
                count[pos]++;
            }else{
                count[pos]=0;
            }
        }

        int amount =0;
        foreach(int i in count.Values){
            amount +=i;
        }
        return amount;
    }

    //Calculates the agent fitness
    public void CalculateFitness(){

        distance = game.pathFinding.Heuristic(position,target);
        float totalWalkedDistance = game.pathFinding.Heuristic(startPos,position);
        float dist = distance;
        float predictionDistance = game.pathFinding.Heuristic(startPos,target);
        
        if(distance == 0){
            dist = 0.1f;
        }

        float totalCost = 0;

        foreach(Vector2 pos in path){
            totalCost += (float)game.grid.gridarray[(int)pos.x,(int)pos.y].cost;
        }

        //The smaller the distance bigger the fitness, bigger the cost smaller the fittness
        // The distance marker is more important than the cost 

        float repeatedMultiplier = (float)CountRepeated();
        repeatedMultiplier = (repeatedMultiplier==0f)? 1f:1f/repeatedMultiplier;
        float distanceMultiplier = totalWalkedDistance/predictionDistance;
        distanceMultiplier = (distanceMultiplier>1)? 1:distanceMultiplier;
        float Last10distMultiplier=1;

        if(path.Count>10){
            Last10distMultiplier = game.pathFinding.Heuristic(path[path.Count-10],position)/10f;
            if(Last10distMultiplier<=0.4f){
                Last10distMultiplier=0;
            }
        }
        
        float costMultiplier = 1/totalCost;

        fitness = Mathf.Pow(100/(dist),4)*distanceMultiplier*Mathf.Pow(repeatedMultiplier,3)*Last10distMultiplier*Mathf.Pow(costMultiplier,2);

    }

}
