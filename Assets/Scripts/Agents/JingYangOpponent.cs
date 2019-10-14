using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JingYangOpponent : Player
{

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

    public override void OnMyMoveEvent(Board board, MoveChoiceCallback moveChoiceCallback)
    {
        base.OnMyMoveEvent(board,moveChoiceCallback);
        VisualizePatterns();
    }

    public override void Reset(){
        base.Reset();
        _visualizedPatternForMove = null;
        _visualizedCounterMove = null;
    }
}
