using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotGameDisplay : Display
{
    [SerializeField] GameManager _gameManager;
    public void Back()
    {
        DisplaysManager.instance.ShowDisplay(DisplaysManager.instance.BotsDisplay);
    }

    public void Undo(){
        //Since the bots will play when given the opportunity give the move back to the human
        _gameManager.UndoMoves(2);
    }
}
