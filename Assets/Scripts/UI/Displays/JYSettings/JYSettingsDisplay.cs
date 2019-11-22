using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class JYSettingsDisplay : Display
{
    [SerializeField] private Transform _cameraPositionOffset;
    [SerializeField] private Camera _camera;
    [SerializeField] private Color _trueColour;
    [SerializeField] private Color _falseColour;

    [SerializeField] private GameObject _brainButton;
    [SerializeField] private GameObject _RNsButton;
    [SerializeField] private GameObject _VCsButton;
    [SerializeField] private GameObject _branchesButton;
    public override void Show()
    {
        base.Show();
        
        _camera.transform.position += _cameraPositionOffset.localPosition;
        ReflectValue(ref Settings.JYSettings.Brain,_brainButton.GetComponent<Text>());
        ReflectValue(ref Settings.JYSettings.RuleNumbers,_RNsButton.GetComponent<Text>());
        ReflectValue(ref Settings.JYSettings.VirtualConnections,_VCsButton.GetComponent<Text>());
        ReflectValue(ref Settings.JYSettings.Branches,_branchesButton.GetComponent<Text>());
    }

    public override void Hide()
    {
        base.Hide();
        _camera.transform.position -= _cameraPositionOffset.localPosition;
    }

    public void Click(GameObject gameObject){
        var foregroundText = gameObject.GetComponent<Text>();
        if(gameObject == _brainButton)
            HandleClick(ref Settings.JYSettings.Brain,foregroundText);
        else if(gameObject == _RNsButton)            
            HandleClick(ref Settings.JYSettings.RuleNumbers,foregroundText);
        else if (gameObject == _VCsButton)
            HandleClick(ref Settings.JYSettings.VirtualConnections,foregroundText);
        else if (gameObject == _branchesButton)
            HandleClick(ref Settings.JYSettings.Branches,foregroundText);
        else
            Debug.LogErrorFormat("{0} received unsupported click from {1}.",transform.name,gameObject.name);
    }

    private void HandleClick(ref bool settingField, Text buttonText){
        settingField =!settingField;
        ReflectValue(ref settingField,buttonText);
    }

    private void ReflectValue(ref bool settingField,Text buttonText){
        if(settingField)
            buttonText.color = _trueColour;
        else
            buttonText.color = _falseColour;
    }

    public void Done(){
        DisplaysManager.instance.ShowDisplay(DisplaysManager.instance.BotGameDisplay);
    }
}
