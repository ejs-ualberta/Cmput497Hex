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
    [SerializeField] private Material _fadedBlackPieceMaterial;
    [SerializeField] private Material _fadedWhitePieceMaterial;
    [SerializeField] private Material[] _patternHighlightMaterials;
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
    private readonly List<Tile> _highlightedTiles = new List<Tile>();
    private readonly Dictionary<Vector2Int,List<GameObject>> _stackedPieces = new Dictionary<Vector2Int, List<GameObject>>();
    private Board _board;

    

    internal void VisualizeBoard(Board board)
    {
    
        
        GenerateNewBoard(board);

    }    
    
    //Dynamically builds a new empty board from tiles for a given size.
    private void GenerateNewBoard(Board board)
    {
        _tiles.Clear();
        _highlightedTiles.Clear();
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


        foreach(var piece in _selectedMoves.Values){
            Destroy(piece);
        }
        _selectedMoves.Clear();

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

                newTile.GetComponent<Tile>().CanonicalMaterial = tileMaterial;
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
        newPiece.GetComponent<Piece>().PlayerColours =
            (tileState == TileState.Black) ? PlayerColours.Black : PlayerColours.White;
        newPiece.GetComponent<Piece>().Location = location;

        if(_selectedMoves.ContainsKey(location))
            Destroy(_selectedMoves[location]);
            _selectedMoves.Remove(location);

        return newPiece;
    }

    internal void RemovePiece(Vector2Int location)
    {
        foreach (Transform piece in _piecesRoot)
        {
            var pieceComp = piece.GetComponent<Piece>();
            if (pieceComp.Location.x == location.x && pieceComp.Location.y == location.y)
            {
                Destroy(pieceComp.gameObject);
                
                return;
            }
        }
            
    }

    private Vector3 GetWorldPositionFromGridLocation(Vector2Int location)
    {
        var boardWidth = _dimensionsWithBorders.x * _rightTileDistance;
        var boardHeight = _dimensionsWithBorders.y * _downTileDistance;
        var baseTilePosition = -boardHeight / 2f + -boardWidth / 2f + _downTileDistance / 2f + _rightTileDistance / 2f;
        return baseTilePosition + location.x * _rightTileDistance + location.y * _downTileDistance;
    }

    public void SelectMove(Vector2Int location, TileState tileState)
    {
        //At most one piece visualized per location
        if(_selectedMoves.ContainsKey(location)){
            var selectedMove = _selectedMoves[location];
            selectedMove.SetActive(true);
            selectedMove.GetComponent<MeshRenderer>().material = (tileState == TileState.Black) ? _fadedBlackPieceMaterial : _fadedWhitePieceMaterial;            
            return;
        }

        var newPiece = GenerateNewPiece(location,tileState);
        newPiece.GetComponent<MeshRenderer>().material  = (tileState == TileState.Black) ? _fadedBlackPieceMaterial : _fadedWhitePieceMaterial;
        
        newPiece.transform.localScale *= 0.9f;
        _selectedMoves[location] = newPiece;
    }

    public void ClearSelectedMove(Vector2Int move)
    {
        if (!_selectedMoves.ContainsKey(move))
            return;
        _selectedMoves[move].SetActive(false);
        //Destroy(_selectedMoves[move]);
        //_selectedMoves.Remove(move);
    }

    public void ClearAllSelectedMoves()
    {
        foreach(var piece in _selectedMoves.Values){            
            piece.SetActive(false);
        }

        //_selectedMoves.Clear();        
    }

    public void HighlightTile(Vector2Int location,int patternIndex){
        var tile = _tiles[_board.Dimensions.x * location.y + location.x];
        if(tile.GetComponent<Tile>().Location.x != location.x || tile.GetComponent<Tile>().Location.y != location.y){
            Debug.LogError("Wrong tile highlighted!");
        }
        if(patternIndex > _patternHighlightMaterials.Length)
            Debug.LogWarning("Insufficient materials to draw all pattens at once.");
        tile.GetComponent<Tile>().Material = _patternHighlightMaterials[patternIndex % _patternHighlightMaterials.Length];
        _highlightedTiles.Add(tile.GetComponent<Tile>());
    }

    public void RemoveHighlight(Vector2Int location){
        var tile = _tiles[_board.Dimensions.x * location.y + location.x];
        var tileComp = tile.GetComponent<Tile>();
        tileComp.Material = tileComp.CanonicalMaterial;
        _highlightedTiles.Remove(tileComp);
    }

    public void RemoveAllHighlights(){
        foreach(var tileComp in _highlightedTiles)
            tileComp.Material = tileComp.CanonicalMaterial;
        _highlightedTiles.Clear();
    }

    public void ShowWinner(List<Vector2Int> winningLine,PlayerColours winnerColours)
    {

        foreach (Transform piece in _piecesRoot)
        {
            if (piece.GetComponent<Piece>().PlayerColours == winnerColours)
                piece.GetComponent<Animator>().SetTrigger(FlashPiecesHash);
        }
        foreach (var location in winningLine)
            _tiles[_board.GetBoardIndexFromLocation(location)].GetComponent<MeshRenderer>().material =
                (winnerColours == PlayerColours.White) ? _whiteTileBorderMaterial : _blackTileBorderMaterial;



    }


}
