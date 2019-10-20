using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JingYangDebug : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private JingYangPlayer _jingYangPlayer;
    [SerializeField] private Agent _opponent;

    [SerializeField] private BoardVisualizer _visualizer;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            Application.Quit();
        }

    }
}
