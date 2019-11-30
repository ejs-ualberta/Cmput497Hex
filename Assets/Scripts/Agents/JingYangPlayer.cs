using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JingYangPlayer : Agent
{
    //True if using benzene integrated jingyang player, false if using standalone 'main.cxx' executable
    [SerializeField] private bool _isUsingStandaloneExecutable = false;

    private int _wins = 0;
    private int _games = 0;
    private void Awake(){
        var name = BenzeneUtil.IssueCommand(BenzeneUtil.JingYang,BenzeneCommands.name);
        var expectedName = (_isUsingStandaloneExecutable) ? "= jingyang" : "= JY"; 
        if(expectedName != name){
            Debug.LogError("Failed to start jingyang player. Instead got: " + name);
            return;
        }
        if(!_isUsingStandaloneExecutable)
            BenzeneUtil.IssueCommand(BenzeneUtil.JingYang,BenzeneCommands.boardsize(new Vector2Int(9,9)));
    }

    public override void Reset(){
        BenzeneUtil.IssueCommand(BenzeneUtil.JingYang,BenzeneCommands.clear_board);
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
        if(_isUsingStandaloneExecutable && board.LastMove == null){           
            moveChoiceCallback(new Move(new Vector2Int((Settings.BoardDimensions.x)/2,(Settings.BoardDimensions.y - 1) /2)));
            return;            
        }

        //Ensure states are consistent
        if(board.LastMove != null){
            BenzeneUtil.IssueCommand(BenzeneUtil.JingYang,BenzeneCommands.play(PlayerColours.White, board.LastMove));            
        }

        var moveStr = BenzeneUtil.IssueCommand(BenzeneUtil.JingYang,BenzeneCommands.genmove(PlayerColours.Black));   
        var move = BenzeneUtil.TryToParseMove(moveStr);
        moveChoiceCallback(move);
    }

    public override void OnUndoEvent(){
        BenzeneUtil.IssueCommand(BenzeneUtil.JingYang, BenzeneCommands.undo);
    }
}
