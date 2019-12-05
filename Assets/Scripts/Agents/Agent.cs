using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Agent : MonoBehaviour
{
    //This is the class used to broker interactions between the GameManager and whatever entity is generating moves.
    public virtual AgentType Type
    {
        get { return AgentType.Unimplemented; }
    }

    public delegate void MoveChoiceCallback(Move move);
    //Important to offer a callback for choosing moves since humans and searching bots have very long latencies when generating moves.
    public abstract void OnMyMoveEvent(Board board, MoveChoiceCallback moveChoiceCallback);

    public abstract void OnGameOverEvent(bool isWinner);

    public abstract void OnUndoEvent();

    public abstract void Reset();
    

    public void OnEnable(){
        Reset();
    }
}
