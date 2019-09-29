using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BenzeneCommands {
    public readonly static string list_commands = "list_commands";
    public readonly static string all_legal_moves = "all_legal_moves";
    public static string boardsize(Vector2Int dimensions){ return string.Format("boardsize {0}x{1}",dimensions.x,dimensions.y);}
    public readonly static string clear_board = "clear_board";
    public readonly static string compute_dominated = "compute-dominated";
    public readonly static string compute_dominated_cell = "compute-dominated-cell";
    public readonly static string compute_fillin = "compute-fillin";
    public readonly static string compute_inferior = "compute-inferior";
    public readonly static string compute_reversible = "compute-reversible";
    public readonly static string compute_vulnerable = "compute-vulnerable";

    public static string play(PlayerColours colour, Move move){
        string moveStr = "";
        moveStr += (char)('a' + move.Location.x);
        moveStr += (char)('0' + move.Location.y + 1);
        Debug.LogFormat("Sent {0} to jingyang",moveStr);
        return string.Format("play {0} {1}",(colour == PlayerColours.Black) ? "b" : "w",moveStr);
    } 

    public static string genmove(PlayerColours colour){ 
        return string.Format("genmove {0}", (colour == PlayerColours.Black) ? "b" : "w");
    }

    public readonly static string showboard = "showboard";
    public readonly static string name = "name";
}

