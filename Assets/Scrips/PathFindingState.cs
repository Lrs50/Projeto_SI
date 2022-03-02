using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingState : BaseState
{

    
    private Vector2 startPos;
    private Vector2 goalPos;
    public Dictionary<Vector2,Vector2>  nodeParents = new Dictionary<Vector2,Vector2>();
    private List<Vector2> path = new List<Vector2>();
    private bool thereIsPath = false;
    private float animationSpeed = 0.005f;
    private Grid_ originalGrid;

    public override void EnterState(GameManager game){
        originalGrid = game.grid;

        startPos = GetRandomValidPos(game);

        goalPos = GetRandomValidPos(game);

        int x,y;
        game.grid.GetXY(startPos,out x,out y);
        game.grid.gridarray[x,y].spriteRenderer.color = Color.white;
        game.grid.GetXY(goalPos,out x,out y);
        game.grid.gridarray[x,y].spriteRenderer.color = Color.red;

        game.StartCoroutine(BSF(game));


        // foreach(Vector2 i in path){
        //     game.grid.gridarray[(int)i.x,(int)i.y].spriteRenderer.color *= 0.9f;
        // }

        // game.grid.GetXY(goalPos,out x,out y);
        // game.grid.gridarray[x,y].spriteRenderer.color = Color.yellow;

    }

    private IEnumerator BSF(GameManager game){
        Queue<Vector3> queue = new Queue<Vector3>();
        HashSet<Vector3> exploredNodes = new HashSet<Vector3>();
        HashSet<Vector3> visited = new HashSet<Vector3>();

        queue.Enqueue(startPos);

        while(queue.Count!=0 && !thereIsPath){
            Vector3 currentNode = queue.Dequeue();
            visited.Add(currentNode);

            int cx,cy;
            game.grid.GetXY(currentNode,out cx,out cy);
            int gx,gy;
            game.grid.GetXY(goalPos,out gx,out gy);

            if((cx == gx) && (cy == gy)){
                thereIsPath = true;
            }

            List<Vector3> nodes = game.grid.GetNodeNeighbors(currentNode);

            foreach(Vector3 node in nodes){
                if(!exploredNodes.Contains(node)){
                    exploredNodes.Add(node);

                    nodeParents.Add(GetMappedVec(node,game),GetMappedVec(currentNode,game));

                    queue.Enqueue(node);
                }
            }
            ShowExploredNodes(visited,game,0.7f);
            ShowExploredNodes(exploredNodes,game,0.9f);
            yield return new WaitForSeconds(animationSpeed);
            UndoShowExploredNodes(visited,game,0.9f);
        }

        ShowPath(game);
    }

    private void ShowExploredNodes(HashSet<Vector3> exploredNodes,GameManager game,float fadeRate){
        foreach(Vector3 node in exploredNodes){

            Vector2 pos = GetMappedVec(node,game);
            if(game.grid.gridarray[(int)pos.x,(int)pos.y].spriteRenderer.color.a>fadeRate){
                game.grid.gridarray[(int)pos.x,(int)pos.y].spriteRenderer.color*=fadeRate;
            }
        }
    }
    private void UndoShowExploredNodes(HashSet<Vector3> exploredNodes,GameManager game,float fadeRate){

        foreach(Vector3 node in exploredNodes){

            Vector2 pos = GetMappedVec(node,game);
            if(game.grid.gridarray[(int)pos.x,(int)pos.y].spriteRenderer.color.a>fadeRate){
                game.grid.gridarray[(int)pos.x,(int)pos.y].spriteRenderer.color/=fadeRate;
            }
        }
    }

    private IEnumerator DSF(GameManager game){
        Stack<Vector3> stack = new Stack<Vector3>();
        HashSet<Vector3> exploredNodes = new HashSet<Vector3>();
        List<Vector3> toView = new List<Vector3>();
        HashSet<Vector3> visited = new HashSet<Vector3>();

        stack.Push(startPos);

        while(stack.Count!=0 && !thereIsPath){
            Vector3 currentNode = stack.Pop();
            visited.Add(currentNode);

            int cx,cy;
            game.grid.GetXY(currentNode,out cx,out cy);
            int gx,gy;
            game.grid.GetXY(goalPos,out gx,out gy);

            if((cx == gx) && (cy == gy)){
                thereIsPath = true;
            }

            List<Vector3> nodes = game.grid.GetNodeNeighbors(currentNode);

            foreach(Vector3 node in nodes){
                if(!exploredNodes.Contains(node)){
                    exploredNodes.Add(node);

                    nodeParents.Add(GetMappedVec(node,game),GetMappedVec(currentNode,game));

                    stack.Push(node);
                }
            }
            ShowExploredNodes(visited,game,0.7f);
            ShowExploredNodes(exploredNodes,game,0.9f);
            yield return new WaitForSeconds(animationSpeed);
            UndoShowExploredNodes(visited,game,0.9f);
        }


        //game.grid.resetColors();
    }

    private Vector2 GetMappedVec(Vector3 node ,GameManager game){
        int x,y;
        game.grid.GetXY(node,out x,out y);
        return new Vector2(x,y);
    }

    public override void UpdateState(GameManager game){

    }

    public override void OnCollisionEnter(GameManager game,Collision2D other){

    }

    private void GetValidRandomPos(out int x, out int y, GameManager game){
        x = (int)Random.Range(-game.size,game.size);
        y = (int)Random.Range(-game.size,game.size);
        while(game.grid.gridarray[x,y].type=="Wall"){
            x = (int)Random.Range(-game.size,game.size);
            y = (int)Random.Range(-game.size,game.size);
        }
    }

    private Vector3 GetRandomValidPos(GameManager game){
        Vector3 pos = new Vector3(Random.Range(-game.size,game.size),Random.Range(-game.size,game.size));

        int x,y;
        game.grid.GetXY(startPos,out x,out y);
        while(game.grid.gridarray[x,y].type=="Wall"){
            pos = new Vector3(Random.Range(-game.size,game.size),Random.Range(-game.size,game.size));
            game.grid.GetXY(startPos,out x,out y);
        }

        return pos;

    }

    private void ShowPath(GameManager game){
        game.grid.resetColors();

        Vector2 current = GetMappedVec(goalPos,game);
        while(!current.Equals(GetMappedVec(startPos,game))){
            path.Add(current);
            try{
                current = nodeParents[current];
            }catch{
                break;
            }
        }
        foreach(Vector2 i in path){
            game.grid.gridarray[(int)i.x,(int)i.y].spriteRenderer.color = Color.red;
        }
    }

}
