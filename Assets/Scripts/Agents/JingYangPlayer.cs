using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JingYangPlayer : Agent
{

    private int _wins = 0;
    private int _games = 0;

    private void Start(){
        Debug.Log(BenzeneUtil.IssueCommand(BenzeneUtil.JingYang,BenzeneCommands.name));
        if("= JY" != BenzeneUtil.IssueCommand(BenzeneUtil.JingYang,BenzeneCommands.name)){
            Debug.LogError("Failed to start jingyang player");
            return;
        }
        BenzeneUtil.IssueCommand(BenzeneUtil.JingYang,BenzeneCommands.boardsize(new Vector2Int(9,9)));
    }

    public override AgentType Type
    {
        get { return AgentType.JingYang; }
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

        
        //Ensure states are consistent
        if(board.LastMove != null)
            BenzeneUtil.IssueCommand(BenzeneUtil.JingYang,BenzeneCommands.play(PlayerColours.White, board.LastMove));

        var moveStr = BenzeneUtil.IssueCommand(BenzeneUtil.JingYang,BenzeneCommands.genmove(PlayerColours.Black));   
        

        var move = BenzeneUtil.TryToParseMove(moveStr);

        Debug.Log(move);

        moveChoiceCallback(move);
    }

    private void OnApplicationExit(){
        BenzeneUtil.JingYang.Kill();
    }
}
