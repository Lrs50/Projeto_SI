using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


// with the selected pathfinding algorithm returns a path to the desired position 

public class PathFindingState : BaseState
{

    // Structures needed for the algorithms to work
    public Vector3 startPos;
    public Vector3 goalPos;
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

    //Checks if any agent has reached the goal
    private bool ReachedGoal(List<TrainingAgent> students){

        foreach(TrainingAgent student in students){
            if(student.reachedGoal){
                return true;
            }
        }

        return false;
    }
    //Checks if all the agents have finished to real all their genetic code
    private bool EvaluationDone(List<TrainingAgent> students){

        foreach(TrainingAgent student in students){
            if(!student.hasFinished){
                return false;
            }
        }

        return true;
    }

    //Creates the next generation based on the previous generation
    private  List<TrainingAgent> NextGen(List<TrainingAgent> students,int populationSize,GameManager game){
        
        List<TrainingAgent> roulette = new List<TrainingAgent>();
        List<TrainingAgent> newStudents = new List<TrainingAgent>();
        

        //normalizing the fitness
        float maxFitness = GetFittestValue(students);

        //Creates the russian roulette 
        foreach(TrainingAgent student in students){
            student.fitness /= maxFitness;

            int count = Mathf.RoundToInt(student.fitness*100f);
            count = (count==0)? 1:count;
            for(int i=0;i<count;i++){
                roulette.Add(student);
            }
        }
        Debug.Log(roulette.Count);
        
        for(int i=0;i<students.Count;i++){
            newStudents.Add(new TrainingAgent(new DNA(roulette[Random.Range(0,roulette.Count)].dna,roulette[Random.Range(0,roulette.Count)].dna),
            GetMappedVec(goalPos,game),GetMappedVec(startPos,game),game));
        }

        return newStudents;
    }

    //Returns the maximum fittness
    private float GetFittestValue(List<TrainingAgent> students){
        float maxFittness = float.MinValue;
        int index =0;
        for(int i=0;i<students.Count;i++){
            students[i].CalculateFitness();
            if(students[i].fitness>maxFittness){
                index = i;
                maxFittness = students[i].fitness;
            }
        }

        return maxFittness;
    }

    //Return the index of the fittest agent
    private int GetFittestIndex(List<TrainingAgent> students){
        float maxFittness = float.MinValue;
        int index =0;
        for(int i=0;i<students.Count;i++){
            students[i].CalculateFitness();
            if(students[i].fitness>maxFittness){
                index = i;
                maxFittness = students[i].fitness;
            }
        }

        return index;
    }

    // Genetic algorithm implementation
    private IEnumerator Genetic(GameManager game){
        
        int generation = 0;
        float currentBestDistance = -1;
        int populationSize = (int)game.createWorld.squareCount*5;
        float pathSize=Heuristic(GetMappedVec(goalPos,game),GetMappedVec(startPos,game))*2f;
        
        List<TrainingAgent> students = new List<TrainingAgent>();
        HashSet<Vector2> visited = new HashSet<Vector2>(); 

        game.generationBox.SetActive(true);

        for(int i=0;i<populationSize;i++){
            students.Add(new TrainingAgent(new DNA(Mathf.RoundToInt(pathSize)),GetMappedVec(goalPos,game),GetMappedVec(startPos,game),game));
        }

    
        while(!ReachedGoal(students)){

            if(game.searchChoice.text != "Genetico"){
                game.grid.resetColors();
                game.generationBox.SetActive(false);
                game.SwitchState(game.pathFinding); 
                yield break;
            }

            while(!EvaluationDone(students)){
                if(game.searchChoice.text != "Genetico"){
                    game.grid.resetColors();
                    game.generationBox.SetActive(false);
                    game.SwitchState(game.pathFinding); 
                    yield break;
                }

                foreach(TrainingAgent student in students){
                    if(!visited.Contains(student.position)){
                        visited.Add(student.position);
                    }
                    student.Update();
                } 
                ShowExploredNodes(visited,game,0.7f);
                yield return new WaitForSeconds(animationSpeed);
                visited.Clear();    
            }

            if(ReachedGoal(students)){
                int index = GetFittestIndex(students);
                bool validFittest = false;
                while(!validFittest){
                    if(students[index].reachedGoal){
                        validFittest=true;
                    }else{
                        students.Remove(students[index]);
                        index = GetFittestIndex(students);
                    }
                }
                path = students[index].path;
                break;
            }
            game.grid.resetColors();
            int index_ = GetFittestIndex(students);
            currentBestDistance = students[index_].distance;
            game.distanceGenerationText.text = "Distance +" +currentBestDistance.ToString();
            students = NextGen(students,populationSize,game);
            generation++;
            game.generationText.text = "Generation "+generation.ToString();

           
        }
        int index__ = GetFittestIndex(students);
        currentBestDistance = students[index__].distance;
        game.distanceGenerationText.text = "Distance +" +currentBestDistance.ToString();
        ShowPath(game,pathOpacity);
        yield return new WaitForSeconds(0.5f);
        path.Reverse();
        game.path = path;
        //game.grid.resetColors();
        game.SwitchState(game.movingState);

        yield return new WaitForSeconds(0.5f);


    }


    // All the ShowExploredNodes shows in the grid the given information, All UndoShowExploredNodes 
    // undo those previously made changes by ShowExploredNodes

    private void ShowExploredNodes(HashSet<Vector3> exploredNodes,GameManager game,float fadeRate){
        // Displays the explored nodes to far
        foreach(Vector3 node in exploredNodes){

            Vector2 pos = GetMappedVec(node,game);
            if(game.grid.gridarray[(int)pos.x,(int)pos.y].spriteRenderer.color.a>fadeRate){
                game.grid.gridarray[(int)pos.x,(int)pos.y].spriteRenderer.color*=fadeRate;
            }
        }
    }
    private void ShowExploredNodes(HashSet<Vector2> exploredNodes,GameManager game,float fadeRate){
        // Displays the explored nodes to far
        foreach(Vector2 node in exploredNodes){

            if(game.grid.gridarray[(int)node.x,(int)node.y].spriteRenderer.color.a>fadeRate){
                game.grid.gridarray[(int)node.x,(int)node.y].spriteRenderer.color*=fadeRate;
            }
        }
    }
    private void UndoShowExploredNodes(HashSet<Vector2> exploredNodes,GameManager game,float fadeRate){
        // Displays the explored nodes to far
        foreach(Vector2 node in exploredNodes){

            if(game.grid.gridarray[(int)node.x,(int)node.y].spriteRenderer.color.a>fadeRate){
                game.grid.gridarray[(int)node.x,(int)node.y].spriteRenderer.color/=fadeRate;
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

    public override void FixedUpdateState(GameManager game){}

    // Shows the final Path highlighted in red
    
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

