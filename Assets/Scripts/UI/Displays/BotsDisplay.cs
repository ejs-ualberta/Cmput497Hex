using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotsDisplay : Display
{

    [SerializeField] private GameManager _gameManager;
    

    [SerializeField] private JingYangPlayer _jingYangPlayer;
    [SerializeField] private Vector2Int _jyBoardSize;
    [SerializeField] private JingYangOpponent _jingYangOpponent;
    [SerializeField] private MoHexPlayer _moHexPlayer;
    [SerializeField] private Player _moHexOpponent;
    private Agent[] _agents = new Agent[0];
    private string _state = "";
    private bool _inBotGame = false;

    public override void Show(){
        base.Show();

        if(_inBotGame){
            RestoreState();
            _inBotGame = false;
        }
        if(_gameManager.AgentOne == _jingYangPlayer || _gameManager.AgentOne == _moHexPlayer){
            Settings.BoardDimensions = _jyBoardSize;
            _gameManager.ResetGameWithNewAgents(_gameManager.DefaultAgents);
            return;
        }

    }

    public void JingYang(){
        StartBotGame();
        Settings.BoardDimensions = _jyBoardSize;
        _gameManager.ResetGameWithNewAgents(new Agent[]{_jingYangPlayer,_jingYangOpponent});
    }

    public void MoHex(){
        StartBotGame();
        _gameManager.ResetGameWithNewAgents(new Agent[]{_moHexPlayer,_moHexOpponent});
    }
    public void Back()
    {
        DisplaysManager.instance.ShowDisplay(DisplaysManager.instance.GameDisplay);
    }

    private void StartBotGame(){
        _inBotGame = true;
        SaveState();
        DisplaysManager.instance.ShowDisplay(DisplaysManager.instance.BotGameDisplay);
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
