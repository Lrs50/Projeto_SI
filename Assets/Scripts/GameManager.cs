using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{   

    //FSM related stuff
    private BaseState currentState;
    public CreateWorldState createWorld = new CreateWorldState();
    public PathFindingState pathFinding = new PathFindingState();
    public MovingState movingState = new MovingState();

    //GameObjects that the manager controls
    public GameObject map;
    public GameObject player;
    public GameObject food;
    public GameObject scoreBox;
    public GameObject selectBox;
    public GameObject cam;
    public GameObject zoomChoice;
    public GameObject zoomBox;
    public GameObject generationBox;
    public GameObject loading;

    //Text elements that the manager controls

    public Text generationText;
    public Text distanceGenerationText;
    public Text searchChoice;
    public Text scoreText;
    public Text costText;
    
    //Sprites for the map
    public Sprite[] zoomSprites;
    public Sprite[] landSprites;
    public Sprite[] waterSprites;
    public Sprite[] mudSprites;
    public Sprite[] wallSprites;
    
    //Sound to play
    public AudioSource scoreSound; 
    //General game assets or properties

    [HideInInspector]public float size;
    [HideInInspector]public int score = 0;
    [HideInInspector]public int cost = 0;
    [HideInInspector]public bool zoom = false;
    [HideInInspector] public Grid_ grid;
    [HideInInspector]public List<Vector2> validPos;
    [HideInInspector]public List<Vector2> path;


    //This function is called by unity when the object is created
    private void Start()
    {
        //FSM related stuff
        scoreSound.Pause();
        currentState = createWorld;  
        currentState.EnterState(this);
    }

    //This function is called each fixed frame time (0.02s)
    private void FixedUpdate()
    {
        //FSM related stuff
        currentState.FixedUpdateState(this);
        if (grid != null)
            grid.FixedUpdateGrid();
        
    }

    //Function is called by states when they want to switch states
    public void SwitchState(BaseState state){
        //FSM related stuff
        currentState = state;
        state.EnterState(this);
    }

    //This function returns a random valid position in the world
    public Vector3 GetRandomValidPos(){

        int len = validPos.Count;
        Vector2 validGridPos = validPos[Random.Range(0,len)];

        return grid.GetWorldPosition((int) validGridPos.x,(int) validGridPos.y);

    }

    //Function that controls the zoom configuration when the button is pressed

    public void Zoom(){
        zoom = !zoom;
        Image choiceOP = zoomChoice.GetComponent<Image>();
        if(zoom){
            Camera.main.transform.position=new Vector3(player.transform.position.x,player.transform.position.y,-10);
            choiceOP.sprite = zoomSprites[1];
        }else{
            Camera.main.transform.position=new Vector3(0,0,-10);
            choiceOP.sprite = zoomSprites[0];
        }
    }
}
