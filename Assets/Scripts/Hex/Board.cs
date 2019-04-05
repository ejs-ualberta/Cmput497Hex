using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using Random = UnityEngine.Random;

//This class holds the Hex game logic.
public class Board
{
    private const int InvalidBoardPosition = -1;

    //Hex connections as defined on a grid.
    private static readonly Vector2Int[] HexTileConnections = new[]
    {
        new Vector2Int(1, -1), new Vector2Int(1, 0),
        new Vector2Int(0,1), new Vector2Int(-1,1),
        new Vector2Int(-1, 0), new Vector2Int(0, -1)
    };
    
    public Vector2Int Dimensions { get; private set; }

    //This stores the current board state.
    public List<TileState> BoardState { get; private set; }

    public bool IsLeftPlayerMove { get; private set; } //i.e. left or right

    private List<Move> _cachedMoveList = null;
    
    private readonly List<Vector2Int> _winningLine = new List<Vector2Int>();

    private readonly Dictionary<Vector2Int,bool> _winningSearchState = new Dictionary<Vector2Int, bool>();

    private bool _isGameOver = false;

    public bool IsGameOver
    {
        get { return _isGameOver; }
    }

    public PlayerName PlayerToMove
    {
        get { return (IsLeftPlayerMove) ? PlayerName.Black : PlayerName.White; }
    }

    public List<Vector2Int> WinningLine
    {
        get
        {
            return _winningLine;
        }
    }

    public Board(Vector2Int dimensions)
    {
        Dimensions = dimensions;                        
        Reset();
    }

    public Board(string boardString)
    {
        var data = boardString.Split(' ');
        Dimensions = new Vector2Int(int.Parse(data[0]),int.Parse(data[1]));
        Reset();
        IsLeftPlayerMove = data[2] == "B";
        for (int i = 3; i < data.Length; i++)
        {
            var moveString = data[i];
            if (moveString.Length == 0)
                break;
            var tileState = moveString[0] == 'b' ? TileState.Black : TileState.White;
            var y = moveString[1] - 'A';
            var x = int.Parse(moveString.Substring(2)) - 1;
            ForceMove(new Move(new Vector2Int(x,y)), tileState );
        }

    }

    public void Reset()
    {
        _isGameOver = false;
        _cachedMoveList = null;
        BoardState = Enumerable.Repeat(TileState.Empty, Dimensions.x * Dimensions.y).ToList();
        IsLeftPlayerMove = true;
        
    }


    
    public static bool IsBoardStringValid(string boardString)
    {
        var data = boardString.Split(' ');
        int x,y;
        if (!int.TryParse(data[0], out x) || !int.TryParse(data[1], out y))
            return false;

        if (data[2] != "B" && data[2] != "W")
            return false;

        for (int i = 3; i < data.Length; i++)
        {
            var moveString = data[i];
            if (moveString.Length == 0)
                break;

            if (moveString[0] != 'b' && moveString[0] != 'w')
                return false;
            int _;
            if (!int.TryParse(moveString.Substring(2),out _))
                return false;
            
            var moveY = moveString[1] - 'A';
            var moveX = int.Parse(moveString.Substring(2)) - 1;
            if (moveX < 0 || moveX >= x || moveY < 0 || moveY > y)
                return false;
        }

        return true;
    }

    public override string ToString()
    {
        var boardString = "";
        boardString += string.Format("{0} {1} {2} ",Dimensions.x,Dimensions.y,IsLeftPlayerMove ? 'B' : 'W');
        for (var y = 0; y < Dimensions.y; y++)
        {
            for (var x = 0; x < Dimensions.x; x++)
            {
                var entry = "";

                switch (BoardState[y * Dimensions.x + x])
                {
                    case TileState.Black:
                        entry = "b";
                        break;
                    case TileState.White:
                        entry = "w";
                        break;
                    case TileState.Empty:
                        continue;                        
                }

                boardString += entry;
                boardString += (char)('A' + y);
                boardString += (char) ('0' + (x + 1));
                boardString += ' ';
            }            
        }        
        return boardString;
    }

    public List<Move> GetAllValidMoves()
    {
        if (_cachedMoveList != null)
            return _cachedMoveList;

        var validMoves = new List<Move>();
        

        //Only iterate to dimension size - 1 in either direction since a move uses the top left of a 1x2 domino
        for (var y = 0; y < Dimensions.y; y++)
        {
            for (var x = 0; x < Dimensions.x; x++)
            {
                if (!IsLocationEmpty(new Vector2Int(x, y)))
                    continue;

                validMoves.Add(new Move(new Vector2Int(x, y)));


            }
        }
        _cachedMoveList = validMoves;

        return _cachedMoveList;
    }



    public bool IsMoveValid(Move move)
    {
        if (IsGameOver) return false;
        return IsLocationEmpty(move.Location);
    }


    public bool IsLocationEmpty(Vector2Int location)
    {
        return BoardState[GetBoardIndexFromLocation(location)] == TileState.Empty;
    }

    public void PlayMove(Move move)
    {
        if (!IsMoveValid(move))
        {
            Debug.LogErrorFormat("Board received invalid move! {0}",move);
            return;
        }

        ForceMove(move, (IsLeftPlayerMove ? TileState.Black : TileState.White));

        foreach (var key in _winningSearchState.Keys.ToList())
            _winningSearchState[key] = false;

        _winningLine.Clear();

        var result = DetectWin(move.Location);
        if (result[0] && result[1])
            _isGameOver = true;

        IsLeftPlayerMove = !IsLeftPlayerMove;
    }

    public void ForceMove(Move move,TileState tileState)
    {
        var moveIndex = GetBoardIndexFromLocation(move.Location);

        BoardState[moveIndex] = tileState;

        RemoveMovesFromCache(move);

    }

    public void ForcePlayerToPlay(PlayerName player)
    {
        IsLeftPlayerMove = (player == PlayerName.Black);
    }

    private void RemoveMovesFromCache(Move move)
    {
        //Update cached moves
        if (_cachedMoveList == null)
            return;


        _cachedMoveList.Remove(move);
    }

    public bool IsLocationWithinBoard(Vector2Int location)
    {        
        return location.x >= 0 && location.x < Dimensions.x && location.y >= 0 && location.y < Dimensions.y;
    }

    internal int GetBoardIndexFromLocation(Vector2Int location)
    {        
        return Dimensions.x * location.y + location.x;
    }

    internal int GetBoardIndexFromCoords(int x, int y)
    {        
        return Dimensions.x * y + x;
    } 

    public Board DeepCopy()
    {
      Board copy = new Board(Dimensions);
      copy.BoardState = new List<TileState>(BoardState.ToArray());
      copy.IsLeftPlayerMove = IsLeftPlayerMove;
      return copy;
    }

    #region  WinDetection
        

    private bool[] DetectWinBFS(Vector2Int start)
    {
        var hasConnectionOne = false;
        var hasConnectionTwo = false;

        var queue = new Queue<Vector2Int>();
        _winningSearchState[start] = true;
        queue.Enqueue(start);
        _winningLine.Add(start);
        while (queue.Count != 0)
        {
            var candidate = queue.Dequeue();
            _winningLine.Add(candidate);
            if (candidate.x == 0 && !IsLeftPlayerMove)
            {
                hasConnectionOne = true;

            }
            if (candidate.x == Dimensions.x - 1 && !IsLeftPlayerMove)
            {
                hasConnectionTwo = true;

            }

            if (candidate.y == 0 && IsLeftPlayerMove)
            {
                hasConnectionOne = true;
            }

            if (candidate.y == Dimensions.y - 1 && IsLeftPlayerMove)
            {
                hasConnectionTwo = true;
            }

            //Debug.LogFormat("{0},{1},{2}", start, hasConnectionOne,hasConnectionTwo);

            if (hasConnectionOne && hasConnectionTwo)
                return new[] {true, true};

            foreach (var offset in HexTileConnections)
            {
                
                var newLocation = candidate + offset;

                if (!IsLocationWithinBoard(newLocation) || BoardState[GetBoardIndexFromLocation(newLocation)] !=
                    (IsLeftPlayerMove ? TileState.Black : TileState.White))
                    continue;

                if (_winningSearchState.ContainsKey(newLocation) && _winningSearchState[newLocation])
                    continue;
                
                _winningSearchState[newLocation] = true;
                queue.Enqueue(newLocation);

            }


        }



        return new[] { hasConnectionOne, hasConnectionTwo };

    }
    
    private bool[] DetectWin(Vector2Int start, bool hasConnectionOne = false, bool hasConnectionTwo = false)
    {

        _winningLine.Add(start);
        _winningSearchState[start] = true;

        if (start.x == 0 && !IsLeftPlayerMove)
        {
            hasConnectionOne = true;
        }
        if (start.x == Dimensions.x - 1 && !IsLeftPlayerMove)
        {
            hasConnectionTwo = true;
        }

        else if (start.y == 0 && IsLeftPlayerMove)
        {
            hasConnectionOne = true;
        }

        if (start.y == Dimensions.y - 1&& IsLeftPlayerMove)
        {
            hasConnectionTwo = true;
        }

        foreach (var offset in HexTileConnections)
        {
            var newLocation = start + offset;

            if (hasConnectionOne && hasConnectionTwo)
                break;

            if (IsLocationWithinBoard(newLocation) && BoardState[GetBoardIndexFromLocation(newLocation)] == (IsLeftPlayerMove ? TileState.Black : TileState.White))
            {

                //Debug.LogFormat("{0},{1}", start, newLocation);
                if (_winningSearchState.ContainsKey(newLocation) && _winningSearchState[newLocation])
                    continue;


                var result = DetectWin(newLocation,hasConnectionOne,hasConnectionTwo);
                if (result[0] && ! hasConnectionOne)
                {
                    _winningLine.Add(newLocation);
                    hasConnectionOne = true;
                }

                if (result[1] && !hasConnectionTwo)
                {
                    _winningLine.Add(newLocation);
                    hasConnectionTwo = true;
                }




                if (hasConnectionOne && hasConnectionTwo)
                    break;
            }
        }

        if(!hasConnectionOne || !hasConnectionTwo)
            _winningLine.Remove(start);

        return new[]{ hasConnectionOne, hasConnectionTwo };
    }
    #endregion
}
