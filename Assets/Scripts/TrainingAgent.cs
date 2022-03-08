using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingAgent 
{
    public float pathMultiplier;
    private int pathIndex;
    public DNA dna;
    public bool hasFinished= false;
    private Vector2 target;
    public Vector2 position {get;private set;}
    private GameManager game;
    public bool reachedGoal=false;
    public List<Vector2> path {get;private set;}
    public float fitness = 0;
    public float distance = 0;

    public TrainingAgent(DNA newDna,Vector2 target,Vector2 startPos,GameManager game){
        dna = newDna;
        this.target = target;
        position = startPos;
        this.game = game;
        path = new List<Vector2>();
        path.Add(position);
    }


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

    private void End(){
        if(position == target){
            reachedGoal =true;
        }
        hasFinished = true;
    }

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

        fitness = Mathf.Pow(10f/(dist),4) + Mathf.Pow(1f/(totalCost),2);
    }

}
