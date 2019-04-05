using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Agent
{    
    [SerializeField] private Camera _camera;
    [SerializeField] private BoardVisualizer _visualization;    
    [SerializeField] private Board _board;
    private Vector2Int _visualizedLocation = new Vector2Int(-1,-1);
    private List<Move> _validMoves = null;
    private Move _visualizedMove = null;

    private bool _isSelectingMove = false;

    private MoveChoiceCallback _moveChoiceCallback = null;

    public override AgentType Type
    {
        get { return AgentType.Human; }
    }

    void OnEnable()
    {        
        _visualizedMove = null;
        _isSelectingMove = false;
        _validMoves = null;
        _moveChoiceCallback = null;
    }

    void Update ()
	{
    

	    if (_visualizedMove != null)
	    {
	        if (InputManager.GetMouseButtonDown(0))
	        {                
	            _moveChoiceCallback(_visualizedMove);
	            _isSelectingMove = false;
                _visualization.ClearSelectedMove(_visualizedMove.Location);
	            _visualizedMove = null;
	        }
	    }

	    //Only visualize if allowed to move.
        if (!_isSelectingMove)
	        return;

        var ray = _camera.ScreenPointToRay(InputManager.mousePosition);
	    RaycastHit hit;
	    if (Physics.Raycast(ray, out hit,100f))
	    {

	        if (hit.transform == null || hit.transform.GetComponent<Tile>() == null)
	            return;
	        var hitLocation = hit.transform.GetComponent<Tile>().Location;

            
	        if (hitLocation == _visualizedLocation)
	            return;
	       
            if(_visualizedMove != null)
                _visualization.ClearSelectedMove(_visualizedMove.Location);
            _visualizedLocation = hitLocation;  

	        var candidateMove = new Move(_visualizedLocation);

	        _visualizedMove = candidateMove;

	        foreach (var move in _validMoves)
	        {	            
	            if (move.Equals(candidateMove))
	            {
	                _visualization.SelectMove(candidateMove.Location,_board.IsLeftPlayerMove ? TileState.Black :TileState.White);
	                return;
	            }
	        }

	        _visualizedMove = null;
	    }   
	}

    public override void OnMyMoveEvent(Board board, MoveChoiceCallback moveChoiceCallback)
    {
        _isSelectingMove = true;
        _validMoves = board.GetAllValidMoves();
        _board = board;
        _moveChoiceCallback = moveChoiceCallback;
    }


    public override void OnGameOverEvent(bool isWinner)
    {
        if(isWinner)
            Debug.LogFormat("{0} is the winner!",name);

        _visualizedMove = null;
        _isSelectingMove = false;
        _validMoves = null;
        _moveChoiceCallback = null;
    }


}
