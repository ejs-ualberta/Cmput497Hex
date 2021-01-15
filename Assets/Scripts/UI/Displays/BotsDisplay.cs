using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotsDisplay : Display
{
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
    public void SixBySix(){
        NewStrategyGame(6);
    }

    public void FiveByFive(){
        NewStrategyGame(5);
    }

    public void FourByFour(){
        NewStrategyGame(4);
    }

/*    public void ThreeByThree(){
        StartBotGame();
        Settings.BoardDimensions = new Vector2Int(3,3);
        SolverParser.Main(_3x3StrategyFile);
        _gameManager.ResetGameWithNewAgents(new Agent[]{_jingYangPlayer, _jingYangOpponent});
    }
*/

    public void ThreeByThree(){
        NewStrategyGame(3);
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

    private void NewStrategyGame(int board_size){
        StartBotGame();
        _strategyPlayer.SetBoardSize(board_size);
        _gameManager.ResetGame();
        Settings.BoardDimensions = new Vector2Int(board_size, board_size);
        _gameManager.ResetGameWithNewAgents(new Agent[]{_strategyPlayer,_jingYangOpponent});
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
