using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplaysManager : MonoBehaviour
{
    [SerializeField] private Display _defaultDisplay;
    [SerializeField] internal GameDisplay GameDisplay;
    [SerializeField] internal SplashScreenDisplay SplashDisplay;
    [SerializeField] internal EditDisplay EditDisplay;
    [SerializeField] internal PuzzlesDisplay PuzzlesDisplay;
    [SerializeField] internal BotsDisplay BotsDisplay;
    [SerializeField] internal BotGameDisplay BotGameDisplay;
    [SerializeField] internal JYSettingsDisplay JYSettingsDisplay;
    
    public static DisplaysManager instance;

    public Display CurrentDisplay
    {
        get { return _currentDisplay; }
    }

    private Display _currentDisplay;

    private void Awake()
    {
        instance = this;
        _currentDisplay = _defaultDisplay;
        _currentDisplay.Show();
    }

    public void ShowDisplay(Display display)
    {
        _currentDisplay.Hide();
        display.Show();
        _currentDisplay = display;        
    }
}
