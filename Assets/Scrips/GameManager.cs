using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{   

    //FSM related stuff
    private BaseState currentState;
    public CreateWorldState createWorld = new CreateWorldState();
    public PathFindingState pathFindig = new PathFindingState();

    //elements that the GameManager controls
    public GameObject map;
    public Grid_ grid;
    public float size;

    //Assets
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
        currentState.UpdateState(this);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        currentState.OnCollisionEnter(this,other);        
    }

    public void SwitchState(BaseState state){
        currentState = state;
        state.EnterState(this);
    }

}
