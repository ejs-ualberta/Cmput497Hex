using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreenDisplay : Display
{

    [SerializeField] private Agent[] _leftOptions;
    [SerializeField] private Agent[] _rightOptions;

    [SerializeField] private Player _leftHuman;
    [SerializeField] private Player _rightHuman;

    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Camera _rotatingCamera;
 

    public void ZeroPlayer()
    {
        Debug.Log("Zero player");        
        _gameManager.ResetGameWithNewAgents(new Agent[]{ _leftOptions[0], _rightOptions[0] });        
        DisplaysManager.instance.ShowDisplay(DisplaysManager.instance.GameDisplay);
        _rotatingCamera.enabled = false;
    }

    public void OnePlayer()
    {
        Debug.Log("One player");        
        _gameManager.ResetGameWithNewAgents(new Agent[] { _leftOptions[0] ,_rightHuman});        
        DisplaysManager.instance.ShowDisplay(DisplaysManager.instance.GameDisplay);
        _rotatingCamera.enabled = false;
    }

    public void TwoPlayer()
    {
        Debug.Log("Two player");                
        _gameManager.ResetGameWithNewAgents(new Agent[]{_leftHuman,_rightHuman});        
        DisplaysManager.instance.ShowDisplay(DisplaysManager.instance.GameDisplay);
        _rotatingCamera.enabled = false;
    }


    public override void Show()
    {
        _rotatingCamera.enabled = true;
        base.Show();
        _gameManager.ResetGameWithNewAgents(new Agent[] {_leftOptions[0],_rightOptions[0]});
}


}
