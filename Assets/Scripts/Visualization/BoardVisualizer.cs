using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class BoardVisualizer : MonoBehaviour
{
    //discovered empricially
    private const float CameraYOffsetPerTileSquared = 0.075f;
    private const float CameraBaseHeight = 5f;

    private readonly int FlashPiecesHash = Animator.StringToHash("Flash");

    [SerializeField] private GameObject _hexTilePrefab;
    [SerializeField] private GameObject _hexPiecePrefab;

    [SerializeField] private Material _defaultTileMaterial;
    [SerializeField] private Material _whiteTileBorderMaterial;
    [SerializeField] private Material _blackTileBorderMaterial;    
    [SerializeField] private Material _blackPieceMaterial;
    [SerializeField] private Material _whitePieceMaterial;

    //These gameobjects hold all the tiles and pieces as children respectively
    [SerializeField] private Transform _tilesRoot;
    [SerializeField] private Transform _piecesRoot;

    //These fields are used to define the distances between tiles. 
    [SerializeField] private Transform _originTile;
    [SerializeField] private Transform _rightTile;
    [SerializeField] private Transform _downTile;

    //This field governs the height of the pieces with respect to the board
    [SerializeField] private Transform _upPiece;

    //This camera must be the one used for showing the game.
    [SerializeField] private Camera _gameCamera;
    
    private Vector3 _downTileDistance;
    private Vector3 _rightTileDistance;
    private Vector3 _upPieceDistance;
    private Vector2Int _dimensionsWithBorders;
    private readonly List<GameObject> _tiles = new List<GameObject>();
    private readonly Dictionary<Vector2Int,GameObject> _selectedMoves = new Dictionary<Vector2Int, GameObject>();
    private Board _board;

    

    internal void VisualizeBoard(Board board)
    {
        GenerateNewBoard(board);
    }    
    
    //Dynamically builds a new empty board from tiles for a given size.
    private void GenerateNewBoard(Board board)
    {
        _tiles.Clear();
        var dimensions = board.Dimensions;

        _downTileDistance = _downTile.transform.position - _originTile.transform.position;
        _rightTileDistance = _rightTile.transform.position - _originTile.transform.position;
        _upPieceDistance = _upPiece.transform.position;        
        _dimensionsWithBorders = new Vector2Int(dimensions.x + 2 ,dimensions.y + 2);
        _board = board;

        foreach (Transform child in _tilesRoot) 
            Destroy(child.gameObject);

        foreach(Transform child in _piecesRoot)
            Destroy(child.gameObject);


        var boardMaxDimension = Mathf.Max(_dimensionsWithBorders.x,_dimensionsWithBorders.y);

        
        _gameCamera.transform.position = new Vector3(_gameCamera.transform.position.x, CameraYOffsetPerTileSquared * boardMaxDimension * boardMaxDimension + CameraBaseHeight, _gameCamera.transform.position.z) ;



        for (var y = 0; y < _dimensionsWithBorders.y; y++)
        {            
            for (var x = 0; x < _dimensionsWithBorders.x; x++)
            {
                if ((x == 0 && y == 0) || (x == _dimensionsWithBorders.x - 1 && y == _dimensionsWithBorders.y - 1) 
                    || (x== _dimensionsWithBorders.x - 1 && y == 0) || (x == 0 && y == _dimensionsWithBorders.y - 1))
                    continue;

                var location = new Vector2Int(x, y);
                var playableLocation = new Vector2Int(x - 1, y - 1);
                var newTile = Instantiate(_hexTilePrefab, _tilesRoot);
                newTile.transform.localPosition = GetWorldPositionFromGridLocation(location);
                

                Material tileMaterial = null;
                if(y == 0 || y == _dimensionsWithBorders.y - 1)
                    tileMaterial = _blackTileBorderMaterial;
                else if (x == 0 || x == _dimensionsWithBorders.x - 1)
                    tileMaterial = _whiteTileBorderMaterial;
                else
                    tileMaterial = _defaultTileMaterial;

                var isPlayable = x != 0 && y != 0 && x != _dimensionsWithBorders.x - 1 &&
                                 y != _dimensionsWithBorders.y - 1;

                newTile.GetComponentInChildren<MeshRenderer>().material = tileMaterial;
                newTile.GetComponent<Tile>().IsPlayable = isPlayable;
                newTile.GetComponent<Tile>().Location = playableLocation;

                if (isPlayable)
                {
                    _tiles.Add(newTile);
                    var tileState = board.BoardState[board.GetBoardIndexFromCoords(playableLocation.x, playableLocation.y)];                    
                    if (tileState != TileState.Empty)                    
                        GenerateNewPiece(playableLocation, tileState);
                    
                }
                    
            }
        }
    }

    internal GameObject GenerateNewPiece(Vector2Int location, TileState tileState)
    {        
        var newPiece = Instantiate(_hexPiecePrefab, _piecesRoot);
        newPiece.GetComponent<MeshRenderer>().material =
            (tileState == TileState.Black) ? _blackPieceMaterial : _whitePieceMaterial;
        newPiece.transform.position = GetWorldPositionFromGridLocation(location + Vector2Int.one) + _upPieceDistance;
        newPiece.GetComponent<Piece>().PlayerName =
            (tileState == TileState.Black) ? PlayerName.Black : PlayerName.White;
        return newPiece;
    }

    private Vector3 GetWorldPositionFromGridLocation(Vector2Int location)
    {
        var boardWidth = _dimensionsWithBorders.x * _rightTileDistance;
        var boardHeight = _dimensionsWithBorders.y * _downTileDistance;
        var baseTilePosition = -boardHeight / 2f + -boardWidth / 2f + _downTileDistance / 2f + _rightTileDistance / 2f;
        return baseTilePosition + location.x * _rightTileDistance + location.y * _downTileDistance;
    }

    public void SelectMove(Vector2Int move, TileState tileState)
    {
        ClearAllSelectedMoves();

        var newPiece = GenerateNewPiece(move,tileState);
        var material = newPiece.GetComponent<MeshRenderer>().material;
        material.color = new Color(material.color.r, material.color.g, material.color.b, material.color.a / 2f);
        newPiece.transform.localScale *= 0.9f;
        _selectedMoves[move] = newPiece;
    }

    public void ClearSelectedMove(Vector2Int move)
    {
        if (!_selectedMoves.ContainsKey(move))
            return;
        Destroy(_selectedMoves[move]);
        _selectedMoves.Remove(move);
    }

    public void ClearAllSelectedMoves()
    {
        foreach(var move in _selectedMoves.Values)
            Destroy(move);

        _selectedMoves.Clear();        
    }


    public void ShowWinner(List<Vector2Int> winningLine,PlayerName winnerName)
    {

        foreach (Transform piece in _piecesRoot)
        {
            if (piece.GetComponent<Piece>().PlayerName == winnerName)
                piece.GetComponent<Animator>().SetTrigger(FlashPiecesHash);
        }
        foreach (var location in winningLine)
            _tiles[_board.GetBoardIndexFromLocation(location)].GetComponent<MeshRenderer>().material =
                (winnerName == PlayerName.White) ? _whiteTileBorderMaterial : _blackTileBorderMaterial;



    }


}
