using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


// with the selected pathfinding algorithm returns a path to the desired position 

public class PathFindingState : BaseState
{

    // Structures needed for the algorithms to work
    public Vector2 startPos;
    public Vector2 goalPos;
    private Dictionary<Vector2,Vector2>  nodeParents = new Dictionary<Vector2,Vector2>();
    private List<Vector2> path = new List<Vector2>();
    private float pathOpacity = 0.7f;

    // Speed of the algorithm animation per iteration
    private float animationSpeed = 0.001f;


    public override void EnterState(GameManager game){
        
        path.Clear();
        nodeParents.Clear();

        startPos = game.player.transform.position;

        goalPos = game.food.transform.position;

        if(game.searchChoice.text=="Largura"){
            game.StartCoroutine(BFS(game));
        }else if(game.searchChoice.text=="Profundidade"){
            game.StartCoroutine(DFS(game));
        }else if(game.searchChoice.text=="Custo Uniforme"){
            game.StartCoroutine(Dijkstra(game));
        }else if(game.searchChoice.text=="Gulosa"){
            game.StartCoroutine(Greedy(game));
        }else if(game.searchChoice.text=="A*"){
            game.StartCoroutine(Astar(game));
        }else if(game.searchChoice.text=="Genetico"){
            game.StartCoroutine(Genetic(game));
        }

    }


    //BFS implementation
    private IEnumerator BFS(GameManager game){

        Queue<Vector3> queue = new Queue<Vector3>();
        HashSet<Vector3> exploredNodes = new HashSet<Vector3>();
        HashSet<Vector3> visited = new HashSet<Vector3>();

        queue.Enqueue(startPos);

        while(queue.Count!=0){

            if(game.searchChoice.text!="Largura"){
                game.grid.resetColors();
                game.SwitchState(game.pathFinding); 
                yield break;
            }

            Vector3 currentNode = queue.Dequeue();
            visited.Add(currentNode);

            int cx,cy;
            game.grid.GetXY(currentNode,out cx,out cy);
            int gx,gy;
            game.grid.GetXY(goalPos,out gx,out gy);

            if((cx == gx) && (cy == gy)){
                break;
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

        ShowPath(game,pathOpacity);
        yield return new WaitForSeconds(0.5f);
        game.path = path;
        //game.grid.resetColors();
        game.SwitchState(game.movingState);

    }

    //DFS implementation
    private IEnumerator DFS(GameManager game){

        Stack<Vector3> stack = new Stack<Vector3>();
        HashSet<Vector3> exploredNodes = new HashSet<Vector3>();
        List<Vector3> toView = new List<Vector3>();
        HashSet<Vector3> visited = new HashSet<Vector3>();

        stack.Push(startPos);

        while(stack.Count!=0){

            if(game.searchChoice.text!="Profundidade"){
                game.grid.resetColors();
                game.SwitchState(game.pathFinding); 
                yield break;
            }

            Vector3 currentNode = stack.Pop();
            visited.Add(currentNode);

            int cx,cy;
            game.grid.GetXY(currentNode,out cx,out cy);
            int gx,gy;
            game.grid.GetXY(goalPos,out gx,out gy);

            if((cx == gx) && (cy == gy)){
                break;
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

        ShowPath(game,pathOpacity);
        yield return new WaitForSeconds(0.5f);
        game.path = path;
        //game.grid.resetColors();
        game.SwitchState(game.movingState);
    }

    private bool avaliate(int x1,int x2){
        return x1<x2;
    }
    private bool avaliate2(float x1,float x2){
        return x1<x2;
    }

    // Dijkstra implementation
    private IEnumerator Dijkstra(GameManager game){

        Heap<int,Vector3> priorityQueue = new Heap<int, Vector3>(avaliate);
        HashSet<Vector3> exploredNodes = new HashSet<Vector3>();
        HashSet<Vector3> visited = new HashSet<Vector3>();
        Dictionary<Vector3,int>  costs = new Dictionary<Vector3,int>();
        
        //seta distancia infinita para todos os outros nodes
        // foreach(Vector2 pos in game.validPos){
            
        //     Vector3 posWorld =  game.grid.GetWorldPosition((int)pos.x, (int)pos.y);
        //     //costs.Add(posWorld,int.MaxValue);
        //     nodeParents.Add(pos, -Vector2.one);

        // }
        //Vector2 a = GetMappedVec(startPos,game);
        //Debug.Log(startPos);
        //Debug.Log(game.grid.GetWorldPosition((int)a.x, (int)a.y));

        costs[startPos] = 0; //setar a posicao inicial como zero :)
        
        Vector2 index;
        priorityQueue.Add(costs[startPos],startPos);
        

        while(!priorityQueue.Empty()){

            if(game.searchChoice.text!="Custo Uniforme"){
                game.grid.resetColors();
                game.SwitchState(game.pathFinding); 
                yield break;
            }

            KeyValuePair<int,Vector3> pair = priorityQueue.Pop();
            Vector3 currentNode = pair.Value;//pair.value eh o node
            visited.Add(currentNode);
            

            List<Vector3> nodes = game.grid.GetNodeNeighbors(currentNode);

            foreach(Vector3 node in nodes){
                if(!exploredNodes.Contains(node)){
                    exploredNodes.Add(node);
                }
                if(!visited.Contains(node)){  
                    index = GetMappedVec(node,game);
                    try{
                        if((costs[currentNode]+ game.grid.gridarray[(int)index.x,(int)index.y].cost) < costs[node]){
                            costs[node]=costs[currentNode]+ game.grid.gridarray[(int)index.x,(int)index.y].cost;
                            nodeParents[GetMappedVec(node,game)]=GetMappedVec(currentNode,game);
                            priorityQueue.Add(costs[node],node);
                        }
                    }catch{
                            costs[node]=costs[currentNode]+ game.grid.gridarray[(int)index.x,(int)index.y].cost;
                            nodeParents[GetMappedVec(node,game)]=GetMappedVec(currentNode,game);
                            priorityQueue.Add(costs[node],node);
                    }  
                }
            }
            ShowExploredNodes(visited,game,0.7f);
            ShowExploredNodes(exploredNodes,game,0.9f);
            yield return new WaitForSeconds(animationSpeed);
            UndoShowExploredNodes(visited,game,0.9f);
        }

        ShowPath(game,pathOpacity);
        yield return new WaitForSeconds(0.5f);
        game.path = path;
        //game.grid.resetColors();
        game.SwitchState(game.movingState);

        yield return new WaitForSeconds(0.5f);
    }

    public float Heuristic(Vector2 a, Vector2 b){  
        return Mathf.Abs(a.x - b.x) +Mathf.Abs(a.y - b.y);
    } 

    // Greedy implementação
    private IEnumerator Greedy(GameManager game){

        Heap<float,Vector3> priorityQueue = new Heap<float, Vector3>(avaliate2);
        HashSet<Vector3> exploredNodes = new HashSet<Vector3>();
        HashSet<Vector3> visited = new HashSet<Vector3>();
        
        Vector2 neighborMapPosition, currentMapPosition;
        

        priorityQueue.Add(Heuristic((Vector2)startPos,(Vector2)goalPos),startPos);

        while(!priorityQueue.Empty()){

            if(game.searchChoice.text != "Gulosa"){
                game.grid.resetColors();
                game.SwitchState(game.pathFinding); 
                yield break;
            }

            KeyValuePair<float, Vector3> pair = priorityQueue.Pop();
            Vector3 currentNode = pair.Value;//pair.value eh o node
            visited.Add(currentNode);
        
            int cx,cy;
            game.grid.GetXY(currentNode,out cx,out cy);
            int gx,gy;
            game.grid.GetXY(goalPos,out gx,out gy);

            if((cx == gx) && (cy == gy)){
                break;
            }

            List<Vector3> nodes = game.grid.GetNodeNeighbors(currentNode);

            foreach(Vector3 node in nodes){
                
                if(!exploredNodes.Contains(node)){
                    exploredNodes.Add(node);
                    neighborMapPosition = GetMappedVec(goalPos, game);
                    currentMapPosition = GetMappedVec(node, game);

                    float neighborCost = Heuristic(currentMapPosition, neighborMapPosition);
                
                    nodeParents[GetMappedVec(node, game)] = GetMappedVec(currentNode, game);
                    priorityQueue.Add(neighborCost, node);

                }

            }

            ShowExploredNodes(visited,game,0.7f);
            ShowExploredNodes(exploredNodes,game,0.9f);
            yield return new WaitForSeconds(animationSpeed);
            UndoShowExploredNodes(visited,game,0.9f);
        }

        ShowPath(game,pathOpacity);
        yield return new WaitForSeconds(0.5f);
        game.path = path;
        //game.grid.resetColors();
        game.SwitchState(game.movingState);

        yield return new WaitForSeconds(0.5f);
    }
    
    // A* implementação
    private IEnumerator Astar(GameManager game){

        Dictionary<Vector3,int>  costSoFar = new Dictionary<Vector3,int>();
        Heap<float,Vector3> priorityQueue = new Heap<float, Vector3>(avaliate2);
        HashSet<Vector3> exploredNodes = new HashSet<Vector3>();
        HashSet<Vector3> visited = new HashSet<Vector3>();
        
        Vector2 index;
        
        costSoFar[startPos] = 0;
        priorityQueue.Add(Heuristic((Vector2)startPos,(Vector2)goalPos),startPos);

        while(!priorityQueue.Empty()){

            if(game.searchChoice.text != "A*"){
                game.grid.resetColors();
                game.SwitchState(game.pathFinding); 
                yield break;
            }

            KeyValuePair<float, Vector3> pair = priorityQueue.Pop();
            Vector3 currentNode = pair.Value;//pair.value eh o node
            visited.Add(currentNode);
        
            int cx,cy;
            game.grid.GetXY(currentNode,out cx,out cy);
            int gx,gy;
            game.grid.GetXY(goalPos,out gx,out gy);

            if((cx == gx) && (cy == gy)){
                break;
            }

            List<Vector3> nodes = game.grid.GetNodeNeighbors(currentNode);

            foreach(Vector3 node in nodes){
                if(!exploredNodes.Contains(node)){
                    exploredNodes.Add(node);
                }

                if(!visited.Contains(node)){  
                    index = GetMappedVec(node,game);
                    try{
                        if((costSoFar[currentNode]+ game.grid.gridarray[(int)index.x,(int)index.y].cost) < costSoFar[node]){
                            costSoFar[node]=costSoFar[currentNode]+ game.grid.gridarray[(int)index.x,(int)index.y].cost;
                            nodeParents[GetMappedVec(node,game)]=GetMappedVec(currentNode,game);
                            priorityQueue.Add(costSoFar[node]+Heuristic(GetMappedVec(node,game),GetMappedVec(goalPos,game)),node);
                        }
                    }catch{
                            costSoFar[node]=costSoFar[currentNode]+ game.grid.gridarray[(int)index.x,(int)index.y].cost;
                            nodeParents[GetMappedVec(node,game)]=GetMappedVec(currentNode,game);
                            priorityQueue.Add(costSoFar[node]+Heuristic(GetMappedVec(node,game),GetMappedVec(goalPos,game)),node);
                    }  
                }

            }

            ShowExploredNodes(visited,game,0.7f);
            ShowExploredNodes(exploredNodes,game,0.9f);
            yield return new WaitForSeconds(animationSpeed);
            UndoShowExploredNodes(visited,game,0.9f);
        }

        ShowPath(game,pathOpacity);
        yield return new WaitForSeconds(0.5f);
        game.path = path;
        //game.grid.resetColors();
        game.SwitchState(game.movingState);

        yield return new WaitForSeconds(0.5f);

    }

    private IEnumerator Genetic(GameManager game){
        //Não implementado

        yield return new WaitForSeconds(0.5f);
        game.searchChoice.text="Largura";
        game.path = path;
        //game.grid.resetColors();
        game.SwitchState(game.pathFinding);

    }

    private void ShowExploredNodes(HashSet<Vector3> exploredNodes,GameManager game,float fadeRate){
        // Displays the explored nodes to far
        foreach(Vector3 node in exploredNodes){

            Vector2 pos = GetMappedVec(node,game);
            if(game.grid.gridarray[(int)pos.x,(int)pos.y].spriteRenderer.color.a>fadeRate){
                game.grid.gridarray[(int)pos.x,(int)pos.y].spriteRenderer.color*=fadeRate;
            }
        }
    }
    
    private void UndoShowExploredNodes(HashSet<Vector3> exploredNodes,GameManager game,float fadeRate){
        // Erases from the grid the changes in color
        foreach(Vector3 node in exploredNodes){

            Vector2 pos = GetMappedVec(node,game);
            if(game.grid.gridarray[(int)pos.x,(int)pos.y].spriteRenderer.color.a>fadeRate){
                game.grid.gridarray[(int)pos.x,(int)pos.y].spriteRenderer.color/=fadeRate;
            }
        }
    }

    public Vector2 GetMappedVec(Vector3 node ,GameManager game){
        // Returns a grid position from the world position 
        int x,y;
        game.grid.GetXY(node,out x,out y);
        return new Vector2(x,y);
    }
    

    public override void UpdateState(GameManager game){

    }

    public override void FixedUpdateState(GameManager game){}

    public override void OnCollisionEnter(GameManager game,Collision2D other){
    }

    private void ShowPath(GameManager game,float opacity){
        // shows the final path
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
            //Alpha (0 to 1) works by: Color = (Top Color * Alpha) + (Base Color * (1 - Alpha))
            game.grid.gridarray[(int)i.x,(int)i.y].spriteRenderer.color = Color.white * (1 - opacity) + Color.magenta * opacity;
        }
    }

}

