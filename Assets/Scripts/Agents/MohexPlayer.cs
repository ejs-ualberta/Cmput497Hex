using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MohexPlayer : Agent
{
    private string StrategyFile;
    private bool _hasInitialized = false;
    private MoveChoiceCallback _callback = null;
    [SerializeField] protected Camera _camera;
    [SerializeField] protected BoardVisualizer _visualization;    
    [SerializeField] protected Board _board;
    protected Vector2Int _visualizedLocation = new Vector2Int(-1,-1);
    protected List<Move> _validMoves = null;
    protected Move _visualizedMove = null;

    private Dictionary<Vector2Int, string> valid_first_moves = new Dictionary<Vector2Int, string>();
    private int n_moves = 0;

    protected bool _isSelectingMove = false;


    public void SetStrategyFile(string FileName){
        StrategyFile = FileName;
    }

    public void SetValidFirstMoves(Dictionary<Vector2Int, string> dict){
        valid_first_moves = dict;
    }

    public void Initialize(){
        SolverParser.IssueCommand(BenzeneCommands.clear_board);
        _hasInitialized = true;
    }

    public override void Reset(){
        if(_hasInitialized){
            _hasInitialized = false;
            _isSelectingMove = true;
            n_moves = 0;
        }
    }


    public override AgentType Type
    {
        get { return AgentType.MoHex; }
    }

    public override void OnGameOverEvent(bool isWinner)
    {
        _visualizedMove = null;
        _isSelectingMove = true;
        _validMoves = _board.GetAllValidMoves();
        _hasInitialized = false;
        n_moves = 0;
    }

    public virtual void VisualizeMove(){
        if (!_isSelectingMove)
	        return;

	    if (_visualizedMove != null)
	    {
	        if (InputManager.GetMouseButtonDown(0)){
                if (!valid_first_moves.ContainsKey(_visualizedMove.Location)){return;}
                StrategyFile = valid_first_moves[_visualizedMove.Location];
                SolverParser.firstMoveInCentre = false;
                SolverParser.Main(Application.streamingAssetsPath + '/' + StrategyFile);
                SolverParser.IssueCommand(BenzeneCommands.clear_board);
                SolverParser.IssueCommand(BenzeneCommands.play(PlayerColours.Black, _visualizedMove));
                SolverParser.firstMoveInCentre = true;
	            _callback(_visualizedMove);
	            _isSelectingMove = false;
                _visualization.ClearSelectedMove(_visualizedMove.Location);
	            _visualizedMove = null;
                ++n_moves;
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
        if(!_hasInitialized){
            _isSelectingMove = true;
            _validMoves = board.GetAllValidMoves();
            _board = board;
            _callback = moveChoiceCallback;
            return;
        }

        Debug.Log(n_moves);
        if (n_moves <= 0){
            return;
        }

        //Ensure states are consistent
        if(board.LastMove != null){
            SolverParser.IssueCommand(BenzeneCommands.play(PlayerColours.White, board.LastMove));
            n_moves += 1;
        }

        var mv = BenzeneCommands.genmove(PlayerColours.Black);
        var moveStr = SolverParser.IssueCommand(mv);
        var move = BenzeneUtil.TryToParseMove(moveStr);
        moveChoiceCallback(move);
        n_moves += 1;
    }

    //TODO: Fix undo functionality.
    public override void OnUndoEvent(){
        Debug.Log(n_moves);
        if (n_moves > 1){
            SolverParser.IssueCommand(BenzeneCommands.undo);
            n_moves -= 1;
        }else{
            SolverParser.IssueCommand(BenzeneCommands.clear_board);
            n_moves = 0;
            _isSelectingMove = true;
        }
    }

    private void Update(){
        if(!_hasInitialized){
            //In webgl and android it is possible that files are not available at application start so this condition must be met before the bot can initialize.
            foreach(KeyValuePair<Vector2Int, string> kvp in valid_first_moves){
                if (!SolverFileLoader.instance.IsFileReady(Application.streamingAssetsPath + '/' + kvp.Value)){return;}
            }
            Initialize();
        }
        if (n_moves == 0){
            VisualizeMove();
        }
    }
}
