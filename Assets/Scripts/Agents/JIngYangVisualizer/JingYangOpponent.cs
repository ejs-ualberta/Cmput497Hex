using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JingYangOpponent : Player
{
    //This class provides the bulk of the visualization, only the white hovered move is taken from the base human agent.
    //Highlight bridges(2) and 432s(5) in separate colours despite having same rule number. Simple patterns is mapping of rule number to rule size
    
    
    private List<List<Vector2Int>> _virtualConnections = new List<List<Vector2Int>>();

    protected Move _visualizedPatternForMove = null;
    protected Move _visualizedCounterMove = null;
    public override void VisualizeMove(){


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
        //To get the countermove visualization we must actually play the white move and then get the black respones then undo them.
        SolverParser.IssueCommand(
            BenzeneCommands.play(PlayerColours.White,_visualizedMove));
        _visualizedCounterMove = BenzeneUtil.TryToParseMove(SolverParser.IssueCommand(
            BenzeneCommands.genmove(PlayerColours.Black)));

        VisualizeBrain();

        for(int i = 0; i < 2 ;i++)
            SolverParser.IssueCommand(
                BenzeneCommands.undo);
        _visualization.SelectMove(_visualizedCounterMove.Location,TileState.Black);
        _visualizedPatternForMove = _visualizedMove;
    }

    private void VisualizePatterns(){
        _visualization.RemoveAllHighlights();
        var patterns = SolverParser.IssueCommand(BenzeneCommands.show_jypattern_list).Trim();     
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
        var patterns = SolverParser.IssueCommand(BenzeneCommands.show_jybranch_list).Trim();     
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
