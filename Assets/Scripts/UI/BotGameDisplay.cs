using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotGameDisplay : Display
{
    [SerializeField] MoHexPlayer _moHexPlayer;
    [SerializeField] GameManager _gameManager;

    [SerializeField] GameObject[] _buttons;
    public void Back() 
    {
        DisplaysManager.instance.ShowDisplay(DisplaysManager.instance.BotsDisplay);
    }

    public void Undo(){
        //Since the bots will play when given the opportunity give the move back to the human
        _gameManager.UndoMoves(2);
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
