using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move{

    //Note that the move location is the upper left location of the tile.
    //i.e. if the tile is vertical is the location of the top half of the tile.
    public Vector2Int Location { get; private set; }        

    public Move(Vector2Int location)
    {
        Location = location;
    }

    public override string ToString()
    {
        return string.Format("{0}", Location);
    }

    public override bool Equals(object obj)
    {
        var move = obj as Move;
        if (move == null)
            return false;
        return move.Location.x == Location.x && move.Location.y == Location.y ;
    }



}
