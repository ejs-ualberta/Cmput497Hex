using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPlayer : Agent
{

    private int _wins = 0;
    private int _games = 0;

    public override AgentType Type
    {
        get { return AgentType.Random; }
    }

    public override void OnGameOverEvent(bool isWinner)
    {
        if (isWinner)
            _wins++;

        _games++;

        Debug.LogFormat("{0} has winrate {1} / {2}!",name, _wins, _games);
    }

    public override void OnMyMoveEvent(Board board, MoveChoiceCallback moveChoiceCallback)
    {
         var allMoves = board.GetAllValidMoves();
         moveChoiceCallback(allMoves[Random.Range(0, allMoves.Count)]);
        
        
    }

    public override void Reset(){
        
    }
}
