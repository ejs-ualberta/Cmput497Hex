using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JingYangPlayer : Agent
{
    //This player interfaces with the solver class to generate moves based on the content of a strategy file.

    //True if using benzene integrated jingyang player, false if using standalone 'main.cxx' executable
    [SerializeField] private bool _isUsingStandaloneExecutable = false;
    [SerializeField] private string _defaultStrategyFile;
    private int _wins = 0;
    private int _games = 0;

    private bool _hasInitialized = false;
    private MoveChoiceCallback _callback = null;


    public void Initialize(){
        SolverParser.Main(Application.streamingAssetsPath + "/" + _defaultStrategyFile);
        var name = SolverParser.IssueCommand(BenzeneCommands.name);
        var expectedName = (_isUsingStandaloneExecutable) ? "= jingyang" : "= JY"; 
        if(expectedName != name){
            Debug.LogError("Failed to start jingyang player. Instead got: " + name);
            return;
        }
        if(!_isUsingStandaloneExecutable){
            //Standalone executables automatically determine boardsize but benzene defaults to 13x13
            SolverParser.IssueCommand(BenzeneCommands.boardsize(new Vector2Int(9,9)));
        }
        if(_callback != null){
            _callback(new Move(new Vector2Int((Settings.BoardDimensions.x)/2,(Settings.BoardDimensions.y - 1) /2)));
        }
        
    }

    public override void Reset(){
        if(_hasInitialized)
            SolverParser.IssueCommand(BenzeneCommands.clear_board);
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
        if(!_hasInitialized){
            _callback = moveChoiceCallback;
            return;
        }

        if(_isUsingStandaloneExecutable && board.LastMove == null){        
            //Benzene behaviour differs here. The standalones automatically play the first move but in benzene must send command 'genmove b' to get opening move.
            moveChoiceCallback(new Move(new Vector2Int((Settings.BoardDimensions.x)/2,(Settings.BoardDimensions.y - 1) /2)));
            return;            
        }

        //Ensure states are consistent
        if(board.LastMove != null){
            SolverParser.IssueCommand(BenzeneCommands.play(PlayerColours.White, board.LastMove));            
        }

        var moveStr = SolverParser.IssueCommand(BenzeneCommands.genmove(PlayerColours.Black));   
        var move = BenzeneUtil.TryToParseMove(moveStr);
        moveChoiceCallback(move);
    }

    public override void OnUndoEvent(){
        SolverParser.IssueCommand( BenzeneCommands.undo);
    }

    private void Update(){
        
        if(!_hasInitialized && SolverFileLoader.instance.IsFileReady(Application.streamingAssetsPath + '/' + _defaultStrategyFile)){
            //In webgl and android it is possible that files are not available at application start so this condition must be met before the bot can initialize.
            Initialize();
            _hasInitialized = true;
        }
    }
}
