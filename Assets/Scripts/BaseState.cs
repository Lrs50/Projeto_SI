using UnityEngine;

public abstract class BaseState  
{


    public abstract void EnterState(GameManager game);

    public abstract void UpdateState(GameManager game);

    public abstract void FixedUpdateState(GameManager game);

    public abstract void OnCollisionEnter(GameManager game,Collision2D other);


}
