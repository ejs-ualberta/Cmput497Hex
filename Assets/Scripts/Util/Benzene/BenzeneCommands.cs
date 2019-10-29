using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BenzeneCommands {
    public readonly static string list_commands = "list_commands";
    public readonly static string all_legal_moves = "all_legal_moves";
    public static string boardsize(Vector2Int dimensions){ return string.Format("boardsize {0}x{1}",dimensions.x,dimensions.y);}
    public readonly static string clear_board = "clear_board";
    public static string compute_dominated(PlayerColours colour){ 
        return string.Format("compute-dominated {0}",colour == PlayerColours.Black ? "b" : "w");
    }
    public readonly static string compute_dominated_cell = "compute-dominated-cell";
    public static string compute_fillin(PlayerColours colour){ 
        return string.Format("compute-fillin {0}",colour == PlayerColours.Black ? "b" : "w");
    }
    public static string compute_inferior(PlayerColours colour){ 
        return string.Format("compute-inferior {0}",colour == PlayerColours.Black ? "b" : "w");
    }
    public readonly static string compute_reversible = "compute-reversible";
    public readonly static string compute_vulnerable = "compute-vulnerable";

    public static string play(PlayerColours colour, Move move){
        string moveStr = "";
        moveStr += (char)('a' + move.Location.x);
        moveStr += (char)('0' + move.Location.y + 1);
        return string.Format("play {0} {1}",(colour == PlayerColours.Black) ? "b" : "w",moveStr);
    } 

    public static string genmove(PlayerColours colour){ 
        return string.Format("genmove {0}", (colour == PlayerColours.Black) ? "b" : "w");
    }

    public readonly static string show_jypattern_list = "show_jypattern_list";
    public readonly static string showboard = "showboard";
    public readonly static string name = "name";
    public readonly static string undo = "undo";
    public readonly static string mohex_get_pv = "mohex-get-pv";
}

