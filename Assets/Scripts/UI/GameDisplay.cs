using System;
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
}
