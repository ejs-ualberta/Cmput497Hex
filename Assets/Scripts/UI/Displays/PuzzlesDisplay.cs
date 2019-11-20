using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlesDisplay : Display
{
    [SerializeField] private Transform _cameraPositionOffset;
    [SerializeField] private Camera _camera;
    public override void Show()
    {
        base.Show();

        _camera.transform.position += _cameraPositionOffset.localPosition;
    }

    public override void Hide()
    {
        base.Hide();
        _camera.transform.position -= _cameraPositionOffset.localPosition;
    }

    public void Back()
    {
        DisplaysManager.instance.ShowDisplay(DisplaysManager.instance.EditDisplay);
    }


}
