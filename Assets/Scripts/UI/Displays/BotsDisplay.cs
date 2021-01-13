using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotsDisplay : Display
{
    private readonly string _4x4StrategyFile =  Application.streamingAssetsPath + "/4_4_c2.txt";
    private readonly string _9x9StrategyFile =  Application.streamingAssetsPath + "/9_9_e5.txt";


    [SerializeField] private GameManager _gameManager;
    [SerializeField] private JingYangPlayer _jingYangPlayer;
    [SerializeField] private JingYangOpponent _jingYangOpponent;
    [SerializeField] private StrategyPlayer _strategyPlayer;
    
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
        _strategyPlayer.SetValidFirstMoves(GetStrategyFileDict(4));
        _gameManager.ResetGameWithNewAgents(new Agent[]{_strategyPlayer,_jingYangOpponent});
    }

/*    public void ThreeByThree(){
        StartBotGame();
        Settings.BoardDimensions = new Vector2Int(3,3);
        SolverParser.Main(_3x3StrategyFile);
        _gameManager.ResetGameWithNewAgents(new Agent[]{_jingYangPlayer, _jingYangOpponent});
    }
*/

    public void ThreeByThree(){
        StartBotGame();
        Settings.BoardDimensions = new Vector2Int(3,3);
        _strategyPlayer.SetValidFirstMoves(GetStrategyFileDict(3));
        _gameManager.ResetGameWithNewAgents(new Agent[]{_strategyPlayer,_jingYangOpponent});
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

    private Dictionary<Vector2Int, string> GetStrategyFileDict(uint n){
        Dictionary<Vector2Int, string> dict = new Dictionary<Vector2Int, string>();
        foreach (string name in SolverFileLoader.GetNxNStrategyFileNames(n)){
            string[] components = name.Split('_');
            string pos = components[components.Length - 1];
            int x = pos[0] - 'a';
            int y = pos[1] - '0' - 1;
            dict.Add(new Vector2Int(x, y), name);
        }
        return dict;
    }
}
