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
            
            VisualizePatterns(BenzeneCommands.compute_dominated(PlayerColours.Black));
            
        }
        else if(Input.GetKeyDown(KeyCode.B)){
            VisualizePatterns(BenzeneCommands.compute_inferior(PlayerColours.Black));
        }
        else if(Input.GetKeyDown(KeyCode.C)){
            VisualizePatterns(BenzeneCommands.compute_fillin(PlayerColours.Black));
        }
    }

    private void VisualizePatterns(string command){
        _visualization.RemoveAllHighlights();
        var patterns = BenzeneUtil.IssueCommand(BenzeneUtil.MoHex, command).Trim();            
        var points = patterns.Split(' ');
        var location = Vector2Int.zero;
        var patternIndex = 0;
        var lastPatternIndex = -1;
        for(int i = 2; i < points.Length; i += 1){
            if(i % 2 == 0){
                location = BenzeneUtil.HexPointToLocation(points[i]);
                _visualization.HighlightTile(location,0);
            }
            else{
                
                var len = points[i].IndexOf("]") - points[i].IndexOf("[") - 1;
                var moveStr = points[i].Substring(points[i].IndexOf("[") + 1,len);
                Debug.Log(moveStr);
                location = BenzeneUtil.HexPointToLocation(moveStr);
                _visualization.HighlightTile(location,1);
            }
            
        }  

    }
}
