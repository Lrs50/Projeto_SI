using System.Collections.Generic;
using UnityEngine;


// Class that holds the map elements
public class Grid_
{

    // Grid properties
    private int width;
    private int height;
    private float size;
    private Vector3 origin;
    private Sprite baseSquare;
    private Transform parent;
    public bool instanciated {get;private set;}

    // Matrix that holds the grid elements
    public Node[,] gridarray {get; private set;}

    public Grid_(int width, int height,float size,Vector3 origin,Sprite baseSquare,Transform parent){

        this.origin = origin;
        this.width = width;
        this.height = height;
        this.size = size;
        this.baseSquare = baseSquare;
        this.parent = parent;
        instanciated = false;

        gridarray = new Node[width,height];

        GenerateDefaultGrid();
        
    }

    public void GenerateDefaultGrid(){
        //Generates an empty grid

        for(int x=0;x<gridarray.GetLength(0);x++){
            for(int y=0;y<gridarray.GetLength(1);y++){
                if(!instanciated){
                    Debug.DrawLine(GetWorldPosition(x,y),GetWorldPosition(x,y+1),Color.white,100f);
                    Debug.DrawLine(GetWorldPosition(x,y),GetWorldPosition(x+1,y),Color.white,100f);
                    gridarray[x,y] = new Node(GetWorldPosition(x,y) + new Vector3(size/2,size/2),size,baseSquare,parent);
                }
                
                gridarray[x,y].SetType(0f);
            }
        }
        if(!instanciated){
            Debug.DrawLine(GetWorldPosition(0,height),GetWorldPosition(width,height),Color.white,100f);
            Debug.DrawLine(GetWorldPosition(width,0),GetWorldPosition(width,height),Color.white,100f);
        }

        if(!instanciated){
            instanciated = true;
        }
        
    }

    public void GetXY(Vector3 worldPosition,out int x,out int y){
        //Transforms a vector3 world position to its grid position
        x = Mathf.FloorToInt((worldPosition-origin).x/size);
        y = Mathf.FloorToInt((worldPosition-origin).y/size);
        
    }

    public Vector3 GetWorldPosition(int x,int y){
        //Transforms its grid possition to its world position
        return new Vector3(x,y) * size + origin;
    }

    public List<Vector3> GetNodeNeighbors(Vector3 pos){
        // returns all the valid neighbors of a given node
        int x,y;
        GetXY(pos,out x,out y);
        List<Vector3> neighbors = new List<Vector3>();

        if(x-1>=0 && gridarray[x-1,y].type!="Wall"){
            neighbors.Add(gridarray[x-1,y].GetPos());

        }
        if(y+1<height && gridarray[x,y+1].type!="Wall"){
            neighbors.Add(gridarray[x,y+1].GetPos());

        }
        if(x+1<width && gridarray[x+1,y].type!="Wall"){
            neighbors.Add(gridarray[x+1,y].GetPos());

        }
        if(y-1>=0 && gridarray[x,y-1].type!="Wall"){
            neighbors.Add(gridarray[x,y-1].GetPos());
        }

        return neighbors;
    }

    public void resetColors(){
        //resets the grid to it's original non empty previous state
        for(int x=0;x<gridarray.GetLength(0);x++){
            for(int y=0;y<gridarray.GetLength(1);y++){
                gridarray[x,y].spriteRenderer.color = gridarray[x,y].originalColor;
            }
        }
    }
}
