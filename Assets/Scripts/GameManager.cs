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

    //elements that the GameManager controls
    public GameObject map;
    public GameObject player;
    public GameObject food;
    public GameObject scoreBox;
    public GameObject selectBox;
    public GameObject cam;
    public GameObject zoomChoice;
    public GameObject zoomBox;
    public Sprite[] zoomSprites;
    public Text searchChoice;
    public Text scoreText;
    public Text costText;
    public GameObject loadingText;
    public int score = 0;
    public int cost = 0;
    public bool zoom = false;
    
    //General game assets or properties

    [HideInInspector] public Grid_ grid;
    [HideInInspector] public float size;
    public Sprite baseSquare;
    public List<Vector2> validPos;
    public List<Vector2> path;


    private void Start()
    {
        //Assents 
        //FSM related stuff
        currentState = createWorld;  
        currentState.EnterState(this);
    }

    private void Update()
    {
        //FSM related stuff
        currentState.UpdateState(this);
    }

    private void FixedUpdate()
    {
        //FSM related stuff
        currentState.FixedUpdateState(this);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        //FSM related stuff
        currentState.OnCollisionEnter(this,other);        
    }

    public void SwitchState(BaseState state){
        //FSM related stuff
        currentState = state;
        state.EnterState(this);
    }

    public Vector3 GetRandomValidPos(){

        int len = validPos.Count;
        Vector2 validGridPos = validPos[Random.Range(0,len)];

        return grid.GetWorldPosition((int) validGridPos.x,(int) validGridPos.y);

    }

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
