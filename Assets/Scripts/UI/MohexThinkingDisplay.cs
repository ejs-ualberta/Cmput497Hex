using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MohexThinkingDisplay : Display
{
    [SerializeField] private MoHexPlayer _moHexPlayer;
    [SerializeField] private GameObject _icon;
    [SerializeField] private GameManager _gameManager;
    void Update()
    {
        if(_gameManager.Agents[0] == _moHexPlayer || _gameManager.Agents[1] == _moHexPlayer)
            _icon.SetActive(_moHexPlayer.IsThinking); 
        else
            _icon.SetActive(false); 
    }
}
