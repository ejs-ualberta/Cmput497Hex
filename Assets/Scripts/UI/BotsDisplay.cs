using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotsDisplay : Display
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private JingYangPlayer _jingYangPlayer;
    [SerializeField] private JingYangOpponent _jingYangOpponent;
    private Agent[] _agents = new Agent[0];
    private string _state = "";
    private bool _inBotGame = false;

    public override void Show(){
        base.Show();

        if(_inBotGame){
            RestoreState();
            _inBotGame = false;
        }

    }

    public void JingYang(){
        StartBotGame();
        DisplaysManager.instance.ShowDisplay(DisplaysManager.instance.BotGameDisplay);
        Settings.BoardDimensions = new Vector2Int(9,9);
        _gameManager.ResetGameWithNewAgents(new Agent[]{_jingYangPlayer,_jingYangOpponent});
    }
    public void Back()
    {
        DisplaysManager.instance.ShowDisplay(DisplaysManager.instance.GameDisplay);
    }

    private void StartBotGame(){
        _inBotGame = true;
        SaveState();
    }
    private void SaveState(){
        _state = _gameManager.Board.ToString();
        _agents = _gameManager.Agents;
    }

    private void RestoreState(){
        _gameManager.ResetGameWithNewAgents(_agents);
        _gameManager.ImportGame(_state);
    }
}
