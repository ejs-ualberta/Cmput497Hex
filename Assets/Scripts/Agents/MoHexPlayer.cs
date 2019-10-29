using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MoHexPlayer : Agent
{

    public bool IsThinking{get; private set;}
    private MoveChoiceCallback _callback;
    private int _wins = 0;
    private int _games = 0;

    private Thread _moveFetchThread;
    private Coroutine _moveFetchCoroutine;

    private void Start(){
        if("= MoHex" != BenzeneUtil.IssueCommand(BenzeneUtil.MoHex,BenzeneCommands.name)){
            Debug.LogError("Failed to start MoHex player");
            return;
        }

    }

    public override void Reset(){
        BenzeneUtil.IssueCommand(BenzeneUtil.MoHex,BenzeneCommands.boardsize(Settings.BoardDimensions));
        BenzeneUtil.IssueCommand(BenzeneUtil.MoHex,BenzeneCommands.clear_board);
        StopCoroutine("WaitForMove");
        IsThinking = false;
    }

    

    public override AgentType Type
    {
        get { return AgentType.MoHex; }
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
            BenzeneUtil.IssueCommand(BenzeneUtil.MoHex,BenzeneCommands.play(PlayerColours.White, board.LastMove));
        
        _callback = moveChoiceCallback;
        //Spawn a thread to fetch move and start a coroutine to wait for the result
        BenzeneUtil.ProcessOutput = null;
        IsThinking = true;
        _moveFetchThread = new Thread(() => BenzeneUtil.IssueCommandAsync(BenzeneUtil.MoHex,BenzeneCommands.genmove(PlayerColours.Black)));   
        _moveFetchThread.Start();
        _moveFetchCoroutine = StartCoroutine("WaitForMove");
    }

    public override void OnUndoEvent(){
        BenzeneUtil.IssueCommand(BenzeneUtil.MoHex, BenzeneCommands.undo);
    }

    private void OnApplicationExit(){
        BenzeneUtil.MoHex.Kill();
    }

    private IEnumerator WaitForMove(){

        //Debug.Log(BenzeneUtil.ProcessOutput);
        while(BenzeneUtil.ProcessOutput == null){
            yield return null;
        }
        //Debug.Log(BenzeneUtil.ProcessOutput);
        _callback(BenzeneUtil.TryToParseMove(BenzeneUtil.ProcessOutput));
        IsThinking = false;
    }
}
