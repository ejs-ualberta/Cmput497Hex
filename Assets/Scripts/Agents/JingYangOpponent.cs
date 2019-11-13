using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JingYangOpponent : Player
{
    private List<List<Vector2Int>> _virtualConnections = new List<List<Vector2Int>>();
    private int _vcIndex = 0;

    public float VCIndex = 0f;
    protected Move _visualizedPatternForMove = null;
    protected Move _visualizedCounterMove = null;
    public override void VisualizeMove(){
        if(!_isSelectingMove)
            return;

            
        base.VisualizeMove();

        if(_visualizedMove == _visualizedPatternForMove)
            return;

        if(_visualizedCounterMove != null )
            if(_visualizedMove == null || _visualizedCounterMove.Location != _visualizedMove.Location)
                _visualization.ClearSelectedMove(_visualizedCounterMove.Location);
                _visualization.ClearAllVCs();

        if(_visualizedMove == null){
            _visualizedPatternForMove = null;
            
            _visualization.RemoveAllHighlights();
            return;
        }
        BenzeneUtil.IssueCommand(BenzeneUtil.JingYang,
            BenzeneCommands.play(PlayerColours.White,_visualizedMove));
        _visualizedCounterMove = BenzeneUtil.TryToParseMove(BenzeneUtil.IssueCommand(BenzeneUtil.JingYang,
            BenzeneCommands.genmove(PlayerColours.Black)));
        VisualizePatterns();
        ParseVCs();
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
        var lastPatternIndex = -1;
        for(int i = 2; i < points.Length; i += 1){
            if(i % 2 == 0)
                location = BenzeneUtil.HexPointToLocation(points[i]);
            else{
                var newIndex = int.Parse(points[i].Split('@')[1]);
                if(lastPatternIndex != newIndex){
                    patternIndex++;
                    lastPatternIndex = newIndex;
                }
                _visualization.HighlightTile(location,patternIndex);
            }                
        }  
    }

    private void ParseVCs(){
        _virtualConnections.Clear();
        BenzeneUtil.IssueCommand(BenzeneUtil.JingYang,BenzeneCommands.vc_build(PlayerColours.Black));
        var str = BenzeneUtil.IssueCommand(BenzeneUtil.JingYang,
            BenzeneCommands.vc_between_cells_full(PlayerColours.Black,_visualizedCounterMove,_visualizedCounterMove.Location.y >= 4 ? "South" : "North"));
        //Debug.Log(str);

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

    public override void OnMyMoveEvent(Board board, MoveChoiceCallback moveChoiceCallback)
    {
        base.OnMyMoveEvent(board,moveChoiceCallback);
        VisualizePatterns();
        
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
    }

    public void LateUpdate(){
        if(_virtualConnections.Count == 0)
            return;

        int _newVCIndex = (int) Mathf.Floor(VCIndex * _virtualConnections.Count);

        if(_vcIndex == _newVCIndex)
            return;

        _vcIndex = _newVCIndex;
        _visualization.ClearAllVCs();
        _visualization.SelectVC(_virtualConnections[_vcIndex],_vcIndex);
    }
}
