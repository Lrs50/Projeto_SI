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

    //the Training Agent contructor 
    public TrainingAgent(DNA newDna,Vector2 target,Vector2 startPos,GameManager game){
        dna = newDna;
        this.target = target;
        position = startPos;
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
            reachedGoal =true;
        }
        hasFinished = true;
    }

    //Calculates the agent fitness
    public void CalculateFitness(){

        distance = game.pathFinding.Heuristic(position,target);
        float dist = Vector2.Distance(position,target);
        
        if(dist == 0){
            fitness = 0.001f;
        }
        float totalCost = 0;

        foreach(Vector2 pos in path){
            totalCost += (float)game.grid.gridarray[(int)pos.x,(int)pos.y].cost;
        }

        //The smaller the distance bigger the fitness, bigger the cost smaller the fittness
        // The distance marker is more important than the cost 
        fitness = Mathf.Pow(10f/(dist),4) + Mathf.Pow(1f/(totalCost),2);
    }

}
