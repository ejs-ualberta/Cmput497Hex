using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JingYangDebug : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private JingYangPlayer _jingYangPlayer;
    [SerializeField] private Agent _opponent;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            Settings.BoardDimensions = new Vector2Int(9,9);
            _gameManager.ResetGameWithNewAgents(new Agent[]{_jingYangPlayer,_opponent});
        }
    }
}
