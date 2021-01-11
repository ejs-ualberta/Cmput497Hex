using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotsDisplay : Display
{
    private readonly string _treeStrategyFile = Application.streamingAssetsPath + "/hex33-1.txt";
    private readonly string _3x3StrategyFile = Application.streamingAssetsPath + "/hex33.txt";
    private readonly string _4x4StrategyFile =  Application.streamingAssetsPath + "/hex44.txt";
    private readonly string _9x9StrategyFile =  Application.streamingAssetsPath + "/hex99-3.txt";


    [SerializeField] private GameManager _gameManager;
    [SerializeField] private JingYangPlayer _jingYangPlayer;
    [SerializeField] private JingYangOpponent _jingYangOpponent;
    [SerializeField] private MohexPlayer _mohexPlayer;
    
    private Agent[] _agents = new Agent[0];
    private string _state = "";
    private bool _inBotGame = false;

    public override void Show(){
        base.Show();

        if(_inBotGame){
            RestoreState();
            _inBotGame = false;
        }
        if(_gameManager.AgentOne == _jingYangPlayer){
            Settings.BoardDimensions = new Vector2Int(9,9);
            _gameManager.ResetGameWithNewAgents(_gameManager.DefaultAgents);
            return;
        }

    }

    public void JingYang(){
        StartBotGame();
        Settings.BoardDimensions = new Vector2Int(9,9);
        SolverParser.Main(_9x9StrategyFile);
        _gameManager.ResetGameWithNewAgents(new Agent[]{_jingYangPlayer,_jingYangOpponent});
    }

    public void FourByFour(){
        StartBotGame();
        Settings.BoardDimensions = new Vector2Int(4,4);
        SolverParser.Main(_4x4StrategyFile);
        _gameManager.ResetGameWithNewAgents(new Agent[]{_jingYangPlayer,_jingYangOpponent});
    }

    public void ThreeByThree(){
        StartBotGame();
        Settings.BoardDimensions = new Vector2Int(3,3);
        SolverParser.Main(_3x3StrategyFile);
        _gameManager.ResetGameWithNewAgents(new Agent[]{_jingYangPlayer, _jingYangOpponent});
    }

    public void TreeByTree(){
        _inBotGame = true;
        SaveState();
        DisplaysManager.instance.ShowDisplay(DisplaysManager.instance.BotGameDisplay);
        Settings.BoardDimensions = new Vector2Int(3,3);
        //SolverParser.Main(_treeStrategyFile);
        //TODO: make this automatic.
        _mohexPlayer.SetValidFirstMoves(new Dictionary<Vector2Int, string>(){
            [new Vector2Int(1, 1)] = "hex33.txt",
            [new Vector2Int(0, 2)] = "hex33-1.txt"
        });
        _gameManager.ResetGameWithNewAgents(new Agent[]{_mohexPlayer,_jingYangOpponent});
    }

    public void Back()
    {
        DisplaysManager.instance.ShowDisplay(DisplaysManager.instance.GameDisplay);
    }

    public void Quit()
    {   
        Application.Quit();
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
