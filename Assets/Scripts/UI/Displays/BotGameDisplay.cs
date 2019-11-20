using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotGameDisplay : Display
{
    [SerializeField] MoHexPlayer _moHexPlayer;
    [SerializeField] JingYangOpponent _jingYangOpponent;
    [SerializeField] JingYangPlayer _jingYangPlayer;
    [SerializeField] GameManager _gameManager;

    [SerializeField] GameObject[] _buttons;

    public void Start(){
        if (_gameManager.Agents[0] == _moHexPlayer || _gameManager.Agents[1] == _moHexPlayer)
            return;
        if (_gameManager.Agents[0] == _jingYangPlayer || _gameManager.Agents[1] == _jingYangPlayer)
            return;
        Settings.BoardDimensions = new Vector2Int(9,9);
        _gameManager.ResetGameWithNewAgents(new Agent[]{_jingYangPlayer,_jingYangOpponent});
        
    }

    public void Back() 
    {
        DisplaysManager.instance.ShowDisplay(DisplaysManager.instance.BotsDisplay);
    }

    public void Undo(){
        //Since the bots will play when given the opportunity give the move back to the human
        _gameManager.UndoMoves(2);
    }

    public void Options(){
        DisplaysManager.instance.ShowDisplay(DisplaysManager.instance.JYSettingsDisplay);
    }

    public void Update(){
        //Disallow the user to press the back/undo buttons if MoHex is thinking to avoid hangs caused by process IO semaphore

        if (_gameManager.Agents[0] != _moHexPlayer && _gameManager.Agents[1] != _moHexPlayer)
            return;

        if(_buttons[0].activeSelf && _moHexPlayer.IsThinking)
            foreach(var button in _buttons)
                button.SetActive(false);
        else if(!_buttons[0].activeSelf && !_moHexPlayer.IsThinking )
            foreach(var button in _buttons)
                button.SetActive(true);
    }
}
