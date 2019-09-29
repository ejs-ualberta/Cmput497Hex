using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JingYangPlayer : Agent
{

    private int _wins = 0;
    private int _games = 0;

    private void Start(){
        Debug.Log(BenzeneUtil.IssueCommand(BenzeneUtil.JingYang,BenzeneCommands.name));
        if("JY" != BenzeneUtil.IssueCommand(BenzeneUtil.JingYang,BenzeneCommands.name)){
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

        Debug.LogFormat("Move of opponent: {0}",board.LastMove);
        //Ensure states are consistent
        if(board.LastMove != null)
            BenzeneUtil.IssueCommand(BenzeneUtil.JingYang,BenzeneCommands.play(PlayerColours.White, board.LastMove));

        var moveStr = BenzeneUtil.IssueCommand(BenzeneUtil.JingYang,BenzeneCommands.genmove(PlayerColours.Black));   
        Debug.LogFormat("Move of jingyang: {0}",moveStr);
        //This code assumes board is 9x9 therefore 1 digit values for x,y
        var x =  moveStr[0] - 'a';
        var y = moveStr[1] - '0' - 1;
        
        var move = new Move(new Vector2Int(x,y));

        Debug.Log(move);

        moveChoiceCallback(move);
    }

}
