using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Agent
{    
    //This is the basic human player, it shows speculative moves determined by the mouse position as faded pieces.
    //The human may commit a move by clicking 

    [SerializeField] protected Camera _camera;
    [SerializeField] protected BoardVisualizer _visualization;    
    [SerializeField] protected Board _board;
    protected Vector2Int _visualizedLocation = new Vector2Int(-1,-1);
    protected List<Move> _validMoves = null;
    protected Move _visualizedMove = null;

    protected bool _isSelectingMove = false;

    protected MoveChoiceCallback _moveChoiceCallback = null;

    public override AgentType Type
    {
        get { return AgentType.Human; }
    }

    void OnEnable()
    {        

    }

    public override void Reset(){
        _visualizedMove = null;
        _isSelectingMove = false;
        _validMoves = null;
        _moveChoiceCallback = null;
    }

    

    void Update ()
	{
    

        VisualizeMove();
	}

    public virtual void VisualizeMove(){
            


	    //Only visualize if allowed to move.
        if (!_isSelectingMove)
	        return;


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
        else{
            //No longer mousing over a valid move
            if(_visualizedMove != null){
                _visualization.ClearSelectedMove(_visualizedMove.Location);
                _visualizedMove = null;
            }
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

    public override void OnUndoEvent(){
        
    }

}
