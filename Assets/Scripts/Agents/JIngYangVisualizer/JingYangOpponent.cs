using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JingYangOpponent : Player
{
    //Highlight bridges(2) and 432s(5) in separate colours despite having same rule number. Simple patterns is mapping of rule number to rule size
    private Dictionary<int,int> SimplePatterns = new Dictionary<int, int>(){{2,2},{5,9}};
    
    private List<List<Vector2Int>> _virtualConnections = new List<List<Vector2Int>>();
    private readonly Dictionary<Vector2Int,List<Vector2Int>> _blackResponses = new Dictionary<Vector2Int, List<Vector2Int>>();
    private readonly List<Vector2Int> _blackMoves = new List<Vector2Int>();
    private int _vcIndex = 0;
 

    public float clockedIndex = 0f;
    protected Move _visualizedPatternForMove = null;
    protected Move _visualizedCounterMove = null;
    public override void VisualizeMove(){
        VisualizeVCs();


        if(!_isSelectingMove)
            return;

            
        base.VisualizeMove();

        if(_visualizedMove == _visualizedPatternForMove)
            return;

        if(_visualizedCounterMove != null ){
            if(_visualizedMove == null || _visualizedCounterMove.Location != _visualizedMove.Location){
                _visualization.ClearSelectedMove(_visualizedCounterMove.Location);
                _visualization.ClearAllVCs();
                _virtualConnections.Clear();
            }
        }

        if(_visualizedMove == null){
            _visualizedPatternForMove = null;            
            VisualizeBrain();
            return;
        }
        BenzeneUtil.IssueCommand(BenzeneUtil.JingYang,
            BenzeneCommands.play(PlayerColours.White,_visualizedMove));
        _visualizedCounterMove = BenzeneUtil.TryToParseMove(BenzeneUtil.IssueCommand(BenzeneUtil.JingYang,
            BenzeneCommands.genmove(PlayerColours.Black)));
        VisualizeBrain();
        if(Settings.JYSettings.VirtualConnections)
            ParseVCs();

        //VisualizeWhiteMoveRegions(_visualizedCounterMove.Location);
        for(int i = 0; i < 2 ;i++)
            BenzeneUtil.IssueCommand(BenzeneUtil.JingYang,
                BenzeneCommands.undo);
        _visualization.SelectMove(_visualizedCounterMove.Location,TileState.Black);
        _visualizedPatternForMove = _visualizedMove;
    }

    private void VisualizePatterns(){
        _visualization.RemoveAllHighlights();
        var patterns = BenzeneUtil.IssueCommand(BenzeneUtil.JingYang,BenzeneCommands.show_jypattern_list).Trim();     
        var points = patterns.Split(' ');
        var location = Vector2Int.zero;
        var patternIndex = 0;
        var _seenLocals = new HashSet<int>();
        for(int i = 2; i < points.Length; i += 1){
            if(i % 2 == 0){
                if(points[i].Equals("invalid")){
                    Debug.LogWarningFormat("Player generated invalid move!\n {0}",patterns);
                    i++;
                    continue;
                }

                location = BenzeneUtil.HexPointToLocation(points[i]);
            }
            else{
                var ruleInfo = points[i].Split('@');
                var newIndex = int.Parse(ruleInfo[1]);
                int local = int.Parse(ruleInfo[0]);
                if(_seenLocals.Contains(local)){
                    patternIndex++;
                    _seenLocals.Clear();
                }
                _seenLocals.Add(local);
                _visualization.HighlightTile(location,patternIndex,(Settings.JYSettings.RuleNumbers) ? newIndex : -1);
            }                
        }  
    }

    private void VisualizeBranches(){
        _visualization.RemoveAllHighlights();
        var patterns = BenzeneUtil.IssueCommand(BenzeneUtil.JingYang,BenzeneCommands.show_jybranch_list).Trim();     
        var points = patterns.Split(' ');
        var location = Vector2Int.zero;
        var _seenLocals = new HashSet<int>();
        var patternIndex = 0;
        var oldBranch = 0;
        for(int i = 2; i < points.Length; i += 1){
            if(i % 2 == 0){
                if(points[i].Equals("invalid")){
                    Debug.LogWarningFormat("Player generated invalid move!\n {0}",patterns);
                    i++;
                    continue;
                }

                location = BenzeneUtil.HexPointToLocation(points[i]);
            }
            else{
                var info = points[i].Split('@');
                var branchNumber = int.Parse(info[1]);
                if(oldBranch != branchNumber){
                    oldBranch = branchNumber;
                    patternIndex++;
                }
                var ruleNumber = int.Parse(info[0]);
                _visualization.HighlightTile(location, patternIndex,(Settings.JYSettings.RuleNumbers) ? ruleNumber: -1);
            }                
        }  
    }

    private void VisualizeBrain(){
        if(!Settings.JYSettings.Brain){
            _visualization.RemoveAllHighlights();
            return;
        }
        if(Settings.JYSettings.Branches){
            VisualizeBranches();
        }
        else{
            VisualizePatterns();
        }
    }


    private void ParseVCs(){

        _virtualConnections.Clear();
        _visualization.ClearAllVCs();
        BenzeneUtil.IssueCommand(BenzeneUtil.JingYang,BenzeneCommands.vc_build(PlayerColours.Black));
        var str = BenzeneUtil.IssueCommand(BenzeneUtil.JingYang,
            BenzeneCommands.vc_between_cells_full(PlayerColours.Black,_visualizedCounterMove,_visualizedCounterMove.Location.y >= 4 ? "South" : "North"));
        var vcs = str.Split(new string[] {"black"},System.StringSplitOptions.None);
        for(int j = 1; j < vcs.Length; j++){
            var vcStr = vcs[j];
            var len = vcStr.IndexOf("]") - vcStr.IndexOf("[") - 1;
            if(len <= 0)
                return;
            var movesStr = vcStr.Substring(vcStr.IndexOf("[") + 1,len).Trim();
            var points = movesStr.Split(' ');
            var locations = new List<Vector2Int>();
            for(int i = 0; i < points.Length; i++){
                locations.Add(BenzeneUtil.HexPointToLocation(points[i]));
            }
            _virtualConnections.Add(locations);
        }
        if(_virtualConnections.Count >= 1){
            _vcIndex = 0;
            _visualization.SelectVC(_virtualConnections[_vcIndex],_vcIndex);
        }
    }

    private void VisualizeVCs(){
        if(_virtualConnections.Count == 0 || !Settings.JYSettings.VirtualConnections)
            return;

        int _newVCIndex = (int) Mathf.Floor(clockedIndex * _virtualConnections.Count);

        if(_vcIndex == _newVCIndex)
            return;

        _vcIndex = _newVCIndex;
        _visualization.ClearAllVCs();
        _visualization.SelectVC(_virtualConnections[_vcIndex],_vcIndex);
    }

  
    public override void OnMyMoveEvent(Board board, MoveChoiceCallback moveChoiceCallback)
    {
        base.OnMyMoveEvent(board,moveChoiceCallback);
        VisualizeBrain();

    }

    public override void OnUndoEvent(){
        base.Reset();
        _visualization.ClearAllSelectedMoves();
        _visualization.ClearAllVCs();
        _visualization.RemoveAllHighlights();
        _visualizedPatternForMove = null;
        _visualizedCounterMove = null;
    }

    public override void Reset(){
        base.Reset();
        _visualizedPatternForMove = null;
        _visualizedCounterMove = null;
        _virtualConnections.Clear();
    }


}
