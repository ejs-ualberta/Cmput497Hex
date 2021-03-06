﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDisplay : Display
{
    
    [SerializeField] private GameManager _gameManager;

    public void Edit()
    {
        DisplaysManager.instance.ShowDisplay(DisplaysManager.instance.EditDisplay);
    }

    public void Bots(){
        DisplaysManager.instance.ShowDisplay(DisplaysManager.instance.BotsDisplay);
    }

    public void Undo()
    {
        _gameManager.UndoMove();
    }

    public void Redo()
    {
        _gameManager.RedoMove();
    }
}
