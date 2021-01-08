using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MohexPlayer : Agent
{
    private string StrategyFile;
    private bool _isFirstMove = true;
    private bool _hasInitialized = false;
    private MoveChoiceCallback _callback = null;
    [SerializeField] protected Camera _camera;
    [SerializeField] protected BoardVisualizer _visualization;    
    [SerializeField] protected Board _board;
    protected Vector2Int _visualizedLocation = new Vector2Int(-1,-1);
    protected List<Move> _validMoves = null;
    protected Move _visualizedMove = null;

    protected bool _isSelectingMove = false;


    public void SetStrategyFile(string FileName){
        StrategyFile = FileName;
    }

    public void Initialize(){
        SolverParser.IssueCommand(BenzeneCommands.clear_board);
        _hasInitialized = true;
    }

    public override void Reset(){
        if(_hasInitialized){
            _hasInitialized = false;
            _isSelectingMove = true;
            _isFirstMove = true;
        }
        SolverParser.IssueCommand(BenzeneCommands.clear_board);
    }


    public override AgentType Type
    {
        get { return AgentType.MoHex; }
    }

    public override void OnGameOverEvent(bool isWinner)
    {
        _visualizedMove = null;
        _isFirstMove = true;
        _isSelectingMove = true;
        _validMoves = _board.GetAllValidMoves();
        _hasInitialized = false;
    }

    public virtual void VisualizeMove(){
        if (!_isSelectingMove)
	        return;

	    if (_visualizedMove != null)
	    {
	        if (InputManager.GetMouseButtonDown(0))
	        {   Debug.Log("First Move");
                SolverParser.firstMoveInCentre = false;
                SolverParser.IssueCommand(BenzeneCommands.clear_board);
                Debug.Log(SolverParser.IssueCommand(BenzeneCommands.play(PlayerColours.Black, _visualizedMove)));
                SolverParser.firstMoveInCentre = true;
                Debug.Log("After First Move");
	            _callback(_visualizedMove);
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
	                _visualization.SelectMove(candidateMove.Location, TileState.Black);
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

        if(!_hasInitialized){
            _callback = moveChoiceCallback;
            return;
        }

        //Ensure states are consistent
        if(board.LastMove != null){
            SolverParser.IssueCommand(BenzeneCommands.play(PlayerColours.White, board.LastMove));
        }

        var mv = BenzeneCommands.genmove(PlayerColours.Black);
        Debug.Log(mv);
        var moveStr = SolverParser.IssueCommand(mv);
        var move = BenzeneUtil.TryToParseMove(moveStr);
        moveChoiceCallback(move);
    }

    public override void OnUndoEvent(){
        SolverParser.IssueCommand( BenzeneCommands.undo);
    }

    private void Update(){
        if(!_hasInitialized && SolverFileLoader.instance.IsFileReady(Application.streamingAssetsPath + '/' + StrategyFile)){
            //In webgl and android it is possible that files are not available at application start so this condition must be met before the bot can initialize.
            Initialize();
        }
        if (_hasInitialized && _isFirstMove){
            VisualizeMove();
        }
    }
}
