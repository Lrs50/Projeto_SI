using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{   

    //FSM related stuff
    private BaseState currentState;
    public CreateWorldState createWorld = new CreateWorldState();
    public PathFindingState pathFinding = new PathFindingState();

    //elements that the GameManager controls
    public GameObject map;
    public Player player;
    public GameObject foodPrefab;

    //General game assets or properties

    [HideInInspector] public Grid_ grid;
    [HideInInspector] public float size;
    public Sprite baseSquare;


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

}
