using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JingYangDebug : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;

    [SerializeField] private BoardVisualizer _visualization;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A)){
            BenzeneUtil.IssueCommand(BenzeneUtil.JingYang,BenzeneCommands.showboard);
            return;
            VisualizePatterns(BenzeneCommands.compute_dominated(PlayerColours.Black));
            
        }
        else if(Input.GetKeyDown(KeyCode.B)){
            VisualizePatterns(BenzeneCommands.compute_inferior(PlayerColours.Black));
        }
        else if(Input.GetKeyDown(KeyCode.C)){
            VisualizePatterns(BenzeneCommands.compute_fillin(PlayerColours.Black));
        }
        else if(Input.GetKeyDown(KeyCode.D)){
            VisualizePV();
        }
        else if(Input.GetKeyDown(KeyCode.E)){
            VisualizePatterns(BenzeneCommands.vc_build(PlayerColours.Black));
        }
    }

    private void VisualizePV(){
        _visualization.ClearAllSelectedMoves();
        var pvStr = BenzeneUtil.IssueCommand(BenzeneUtil.MoHex, BenzeneCommands.mohex_get_pv).Trim();
        if(pvStr[0] == '?')
            return;
        var colour = (_gameManager.Board.PlayerToMove == PlayerColours.Black) ? TileState.Black : TileState.White;
        var points = pvStr.Split(' ');
        
            
        for(int i = 2; i < points.Length; i += 1){
            var point = points[i];
            _visualization.SelectMove(BenzeneUtil.HexPointToLocation(point),colour);
            colour = (colour == TileState.Black) ? TileState.White : TileState.Black;
        }
    }

//=  a1 iv[a2] b1 iv[b2] c1 id[b2] d1 id[c2] e1 id[d2] f1 id[e2] g1 id[f2] h1 id[g2] i1 id[h2] a2 id[b2] b2 id[c2] g2 id[f2] i2 iv[i1-h3] a3 id[b3] i3 fpw h4 iv[h3-g5] i4 fcw i5 fcw h6 fcw i6 fcw g7 iv[f7-g8] h7 fpw i7 fpw d8 id[c8] f8 id[e8] g8 id[f8] h8 fcw i8 fcw a9 id[b8] b9 id[c8] c9 id[d8] d9 id[e8] e9 id[f8] f9 iv[f8] g9 iv[g8] h9 fcw i9 fcw
    private void VisualizePatterns(string command){
        _visualization.RemoveAllHighlights();
        var patterns = BenzeneUtil.IssueCommand(BenzeneUtil.MoHex, command).Trim();            
        var points = patterns.Split(' ');
        var location = Vector2Int.zero;
        var patternIndex = 0;
        var lastPatternIndex = -1;
        Debug.Log(patterns);
        for(int i = 2; i < points.Length; i += 1){
            if(i % 2 == 0){
                location = BenzeneUtil.HexPointToLocation(points[i]);
                _visualization.HighlightTile(location,0);
            }
            else{
                
                var len = points[i].IndexOf("]") - points[i].IndexOf("[") - 1;
                if(len < 0)
                    continue;
                var moveStr = points[i].Substring(points[i].IndexOf("[") + 1,len);
                
                location = BenzeneUtil.HexPointToLocation(moveStr);
                _visualization.HighlightTile(location,1);
            }
            
        }  

    }
}
