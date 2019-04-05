using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditDisplay : Display
{
    private readonly int ShowStatusMessageHash = Animator.StringToHash("Show");

    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Transform _cameraPositionOffset;
    [SerializeField] private Camera _camera;
    [SerializeField] private Text _paintWhiteText;
    [SerializeField] private Text _paintBlackText;
    [SerializeField] private Color _isPaintingColor;
    [SerializeField] private Color _isNotPaintingColor;
    [SerializeField] private Text[] _statusMessageTexts;
    [SerializeField] private Animator _statusMessageAnimator;
    [SerializeField] private Color _errorColor;
    [SerializeField] private Color _successColor;



    public override void Show()
    {
        base.Show();
        
        _camera.transform.position += _cameraPositionOffset.localPosition;
    }

    public override void Hide()
    {
        base.Hide();
        _camera.transform.position -= _cameraPositionOffset.localPosition;
        _paintWhiteText.color = _paintBlackText.color = _isNotPaintingColor;
        _gameManager.PaintMode = PaintMode.PaintOff;        
    }

    public void WidthUp()
    {
        ChangeBoardDimensions(Settings.BoardDimensions + Vector2Int.right);
    }

    public void Widthdown()
    {
        if (Settings.BoardDimensions.x == 1)
            return;
        ChangeBoardDimensions(Settings.BoardDimensions + Vector2Int.left);
    }

    public void HeightUp()
    {
        ChangeBoardDimensions(Settings.BoardDimensions + Vector2Int.up);
    }

    public void HeightDown()
    {
        if (Settings.BoardDimensions.y == 1)
            return;
        ChangeBoardDimensions(Settings.BoardDimensions + Vector2Int.down);
    }

    public void ClearBoard()
    {
        _gameManager.ResetGame();
    }

    public void Import()
    {


        var boardString = GUIUtility.systemCopyBuffer;
        if (!Board.IsBoardStringValid(boardString))
        {
            ShowStatusMessage("Invalid board string found on clipboard!",_errorColor);
            return;
        }
        ShowStatusMessage("Board successfully imported!", _successColor);
        _gameManager.ImportGame(boardString);
        Settings.BoardDimensions = _gameManager.Board.Dimensions;
    }

    public void Export()
    {
        var boardString = _gameManager.Board.ToString();

        ShowStatusMessage("Board copied to clipboard!",_successColor);
        GUIUtility.systemCopyBuffer = boardString;
    }

    public void Done()
    {
        DisplaysManager.instance.ShowDisplay(DisplaysManager.instance.GameDisplay);
    }

    public void PaintWhite()
    {
        if (_gameManager.PaintMode == PaintMode.PaintOff)
        {
            _gameManager.PaintMode = PaintMode.PaintWhite;
            _paintWhiteText.color = _isPaintingColor;
            _paintBlackText.color = _isNotPaintingColor;
            return;
        }
        _gameManager.PaintMode = PaintMode.PaintOff;
        _paintWhiteText.color = _isNotPaintingColor;
    }

    public void PaintBlack()
    {
        if (_gameManager.PaintMode == PaintMode.PaintOff)
        {
            _gameManager.PaintMode = PaintMode.PaintBlack;
            _paintBlackText.color = _isPaintingColor;
            _paintWhiteText.color = _isNotPaintingColor;
            return;
        }
        _gameManager.PaintMode = PaintMode.PaintOff;
        _paintBlackText.color = _isNotPaintingColor;
    }

    public void Puzzles()
    {
        DisplaysManager.instance.ShowDisplay(DisplaysManager.instance.PuzzlesDisplay);
    }

    private void ChangeBoardDimensions(Vector2Int newDimensions)
    {
        Settings.BoardDimensions = newDimensions;
        _gameManager.ResetGameWithNewAgents(_gameManager.Agents);
    }

    private void ShowStatusMessage(string statusMessage,Color color)
    {
        foreach (var text in _statusMessageTexts)        
            text.text = statusMessage;

        _statusMessageTexts[0].color = color;
        

        _statusMessageAnimator.SetTrigger(ShowStatusMessageHash);
    }

}
