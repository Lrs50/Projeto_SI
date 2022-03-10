using UnityEngine;


//Abstract class used for polimorfism used in the game manager
public abstract class BaseState  
{

    public abstract void EnterState(GameManager game);

    public abstract void FixedUpdateState(GameManager game);

}
