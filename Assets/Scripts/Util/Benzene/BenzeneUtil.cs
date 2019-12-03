using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;
using UnityEngine;

public static class BenzeneUtil
{   

    public static Vector2Int HexPointToLocation(string hexpoint){
        return new Vector2Int(hexpoint[0] - 'a',int.Parse(hexpoint.Substring(1)) - 1);
    }

    public static Move TryToParseMove(string commandStr){
    
        if(commandStr.Contains("="))
            commandStr = commandStr.Split('=')[1].Trim();

        if(commandStr == "resign")
            return null;

        return new Move(HexPointToLocation(commandStr));
    }

    public static string MoveToHexPoint(Move move){
        return LocationToHexPoint(move.Location);
    }

    public static string LocationToHexPoint(Vector2Int location){
        string hexPoint = "";
        hexPoint += (char)('a' + location.x);
        hexPoint += (char)('0' + location.y + 1);
        return hexPoint;
    }

    public static string PlayerColourToStr(PlayerColours colour){
        return (colour == PlayerColours.Black) ? "b" : "w";
    }
}
