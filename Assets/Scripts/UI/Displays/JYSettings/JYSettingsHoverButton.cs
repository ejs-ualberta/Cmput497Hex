using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class JYSettingsHoverButton : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
{
    private const string ButtonText = "Black Move {0}/{1}";
    private readonly List<Vector2Int> _visualizedMoves = new List<Vector2Int>();
    [SerializeField] private BoardVisualizer _visualization;

    private string[] _blackMovesStrings;
    private int _numBlackMoves = 0;

    private void Start(){
        GetMoveInformation();
        index = Settings.JYSettings.BlackMovesIndex;
    }

    private int index{
        set{
            var str = string.Format(ButtonText,value + 1, _numBlackMoves);
            GetComponent<Text>().text = str;
            transform.GetChild(0).GetComponent<Text>().text = str;
        }
    }
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        GetMoveInformation();
        VisualizeMoves();
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        DevisualizeMoves();
    }

    public void Click(){
        Settings.JYSettings.BlackMovesIndex = (Settings.JYSettings.BlackMovesIndex + 1) % _numBlackMoves;
        index = Settings.JYSettings.BlackMovesIndex;
        DevisualizeMoves();
        VisualizeMoves();
    }

    private void VisualizeMoves(){
        for(int i = 1; i < _blackMovesStrings.Length;i++){
            if((i - 1)/2 != Settings.JYSettings.BlackMovesIndex)
                continue;
            if( i % 2 == 1){
                var move = BenzeneUtil.TryToParseMove(_blackMovesStrings[i]).Location;
                _visualizedMoves.Add(move);
                _visualization.SelectMove(move,TileState.Black);
            }
            else{
                var whiteMovesStrings = _blackMovesStrings[i].Trim().Split(',');
                for(int j = 0; j < whiteMovesStrings.Length;j++){
                    var move = BenzeneUtil.TryToParseMove(whiteMovesStrings[j]).Location;
                    _visualizedMoves.Add(move);
                    _visualization.SelectMove(move,TileState.White);   
                }
                break;
            }
        }
    }
    private void DevisualizeMoves(){
        foreach(var move in _visualizedMoves)
            _visualization.ClearSelectedMove(move);
        _visualizedMoves.Clear();
    }

    private void GetMoveInformation(){
        _blackMovesStrings = SolverParser.IssueCommand(BenzeneCommands.show_jyblackmoves_list).Trim().Split(' '); 
        _numBlackMoves = (_blackMovesStrings.Length - 1) / 2;
        if(Settings.JYSettings.BlackMovesIndex >= _numBlackMoves)
            Settings.JYSettings.BlackMovesIndex = 0;
    }
}
