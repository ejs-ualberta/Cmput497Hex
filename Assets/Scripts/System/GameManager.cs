﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BoardVisualizer _boardVisualization;
    [SerializeField] private Agent[] _agents;
    [SerializeField] private float _minimumTimeBetweenMoves;
    [SerializeField] private float _minimumTimeBetweenGames;

    internal PaintMode PaintMode
    {
        get { return _paintMode; }
        set
        {
            _paintMode = value;
            if (_paintMode == PaintMode.PaintWhite)
                Board.ForcePlayerToPlay(PlayerName.White);
            else if(_paintMode == PaintMode.PaintBlack)
                Board.ForcePlayerToPlay(PlayerName.Black);
        }
    }

    private Board _currentBoard;
    private float _lastMoveTime;
    private float _lastGameTime;
    private Move _nextMove;
    private int _agentIndex;
    private PaintMode _paintMode = PaintMode.PaintOff;

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
        _agents[0].gameObject.SetActive(false);
        _agents[1].gameObject.SetActive(false);
        _agents = agents;
        _agents[0].gameObject.SetActive(true);
        _agents[1].gameObject.SetActive(true);
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
    }

    private void Init()
    {
        Settings.BoardDimensions = _currentBoard.Dimensions;
        _boardVisualization.VisualizeBoard(_currentBoard);
        _lastMoveTime = Time.time + _minimumTimeBetweenMoves * 2;
        _agents[0].gameObject.SetActive(false);
        _agents[1].gameObject.SetActive(false);
        _agents[0].gameObject.SetActive(true);
        _agents[1].gameObject.SetActive(true);
        _nextMove = null;
        _agentIndex = 0;
        GetNextMoveFromAgent();
        if (PaintMode == PaintMode.PaintWhite)
            _currentBoard.ForcePlayerToPlay(PlayerName.White);
    }

    private void GetNextMoveFromAgent()
    {
        _agents[_agentIndex++ % _agents.Length].OnMyMoveEvent(_currentBoard, ReceiveMoveFromAgent);
    }

    private void CommitMove()
    {

        _currentBoard.PlayMove(_nextMove);
        _boardVisualization.GenerateNewPiece(_nextMove.Location,_currentBoard.IsLeftPlayerMove ? TileState.White : TileState.Black);
        _nextMove = null;


        if (_currentBoard.IsGameOver)
        {
            var winner = _currentBoard.IsLeftPlayerMove ? PlayerName.White : PlayerName.Black;
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
                _currentBoard.ForcePlayerToPlay(PlayerName.Black);
                break;
            case PaintMode.PaintWhite:
                _currentBoard.ForcePlayerToPlay(PlayerName.White);
                break;
        }
        GetNextMoveFromAgent();
        
        _lastMoveTime = Time.time;
        
    }


    private void ReceiveMoveFromAgent(Move move)
    {
        _nextMove = move;
    }
}