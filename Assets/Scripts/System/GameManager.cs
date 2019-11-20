using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BoardVisualizer _boardVisualization;
    [SerializeField] private Agent[] _agents;
    [SerializeField] private float _minimumTimeBetweenMoves;
    [SerializeField] private float _minimumTimeBetweenGames;

    private Board _currentBoard;
    private float _lastMoveTime;
    private float _lastGameTime;
    private Move _nextMove;
    private int _agentIndex;
    private PaintMode _paintMode = PaintMode.PaintOff;


    [SerializeField] internal Agent[] DefaultAgents;
    
    internal PaintMode PaintMode
    {
        get { return _paintMode; }
        set
        {
            _paintMode = value;
            if (_paintMode == PaintMode.PaintWhite)
                Board.ForcePlayerToPlay(PlayerColours.White);
            else if(_paintMode == PaintMode.PaintBlack)
                Board.ForcePlayerToPlay(PlayerColours.Black);
        }
    }

    internal Board Board
    {
        get { return _currentBoard; }
    }

    internal Agent AgentOne
    {
        get { return _agents[0]; }
    }

    internal Agent AgentTwo
    {
        get { return _agents[1]; }
    }

    internal Agent[] Agents
    {
        get { return _agents; }
    }

	void Start ()
	{	 
        if(_agents.Length != 2)
            Debug.LogError("GameManager does not have 2 agents!");
        ImportGame(Settings.BoardString);
	}
	
	
	void Update ()
	{
	    if (Time.time - _lastMoveTime >= _minimumTimeBetweenMoves && _nextMove != null && !_currentBoard.IsGameOver)
	    {
            CommitMove();
	    }

	    if (Time.time - _lastGameTime >= _minimumTimeBetweenGames && _currentBoard.IsGameOver)
	    {
            ResetGame();
	    }
    }    

    public void ResetGameWithNewAgents(Agent[] agents)
    {
        foreach(var agent in _agents){
            agent.Reset();
            agent.gameObject.SetActive(false);

        }
        _agents = agents;
        foreach(var agent in _agents){
            agent.gameObject.SetActive(true);
            agent.Reset();
        }
        ResetGame();
    }

    public void ResetGame()
    {
        _currentBoard = new Board(Settings.BoardDimensions);
        Init();

    }

    public void ImportGame(string boardString)
    {
        _currentBoard = new Board(boardString);
        Init();
        _currentBoard.InvalidateUndoableMoves();
    }

    public void UndoMove()
    {
        if(TryUndo() == false)
            return;
        foreach(var agent in _agents)
            agent.OnUndoEvent();
        GetNextMoveFromAgent();
    }

    public void UndoMoves(int numToUndo){
        while(numToUndo > 0 && TryUndo()){
            numToUndo--;
            foreach(var agent in _agents)
                agent.OnUndoEvent();
        }

        GetNextMoveFromAgent();
    }

    public void RedoMove()
    {
        var redoneMove = _currentBoard.RedoMove();
        if (redoneMove == null)
            return;        
        _boardVisualization.GenerateNewPiece(redoneMove.Location,
            _currentBoard.IsLeftPlayerMove ? TileState.White : TileState.Black);

    }

    private void Init()
    {
        Settings.BoardDimensions = _currentBoard.Dimensions;
        _boardVisualization.VisualizeBoard(_currentBoard);
        _lastMoveTime = Time.time + _minimumTimeBetweenMoves * 2;
        //Agents didn't change so reset
        _agents[0].Reset();
        _agents[1].Reset();
        _nextMove = null;
        _agentIndex = 0;
        GetNextMoveFromAgent();
        if (PaintMode == PaintMode.PaintWhite)
            _currentBoard.ForcePlayerToPlay(PlayerColours.White);
    }

    private void GetNextMoveFromAgent()
    {
        _agents[_currentBoard.IsLeftPlayerMove ? 0 : 1].OnMyMoveEvent(_currentBoard, ReceiveMoveFromAgent);
    }

    private void CommitMove()
    {

        _currentBoard.PlayMove(_nextMove);
        _boardVisualization.GenerateNewPiece(_nextMove.Location,_currentBoard.IsLeftPlayerMove ? TileState.White : TileState.Black);
        _nextMove = null;


        if (_currentBoard.IsGameOver)
        {
            var winner = _currentBoard.IsLeftPlayerMove ? PlayerColours.White : PlayerColours.Black;
            var winnerIndex = _currentBoard.IsLeftPlayerMove ? 1 : 0;



            _boardVisualization.ShowWinner(_currentBoard.WinningLine, winner);

            _agents[winnerIndex].OnGameOverEvent(true);
            _agents[1 - winnerIndex].OnGameOverEvent(false);
            _lastGameTime = Time.time;
            return;
        }

        switch (PaintMode)
        {
            case PaintMode.PaintBlack:
                _currentBoard.ForcePlayerToPlay(PlayerColours.Black);
                break;
            case PaintMode.PaintWhite:
                _currentBoard.ForcePlayerToPlay(PlayerColours.White);
                break;
        }
        GetNextMoveFromAgent();
        
        _lastMoveTime = Time.time;
        
    }

    private bool TryUndo(){
        var undoneMove = _currentBoard.UndoMove();
        if (undoneMove == null)
            return false;
        _boardVisualization.RemovePiece(undoneMove.Location);
        return true;
    }
    private void ReceiveMoveFromAgent(Move move)
    {
        _nextMove = move;
    }
}
