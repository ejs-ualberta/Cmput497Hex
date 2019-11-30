using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationManager : MonoBehaviour
{
    public static ApplicationManager instance;

    [SerializeField] private SplashScreenDisplay _splashScreenDisplay;        
    
    private void Awake()
    {
        instance = this;                
    }

    void Start()
    {
        ShowSplashScreen();                
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();            
    }


    private void ShowSplashScreen()
    {
        _splashScreenDisplay.Show();        
    }

    private void OnApplicationExit(){
        BenzeneUtil.JingYang.Kill();
    }

}
