using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SinglePuzzleButton : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
{

    [SerializeField] private GameDefinition _puzzleDefinition;
    [SerializeField] private Text[] _puzzleNameTexts;
    [SerializeField] private GameManager _gameManager;

    private string _currentBoardString;

    private void Awake()
    {
        foreach (var text in _puzzleNameTexts)
            text.text = _puzzleDefinition.GameName;
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        _currentBoardString = _gameManager.Board.ToString();
        _gameManager.ImportGame(_puzzleDefinition.BoardString);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        _gameManager.ImportGame(_currentBoardString);
    }

    public void ImportPuzzle()
    {
        DisplaysManager.instance.ShowDisplay(DisplaysManager.instance.GameDisplay);
    }
}
