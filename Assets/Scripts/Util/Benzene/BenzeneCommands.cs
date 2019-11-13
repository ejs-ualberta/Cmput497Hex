using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BenzeneCommands {
    public readonly static string list_commands = "list_commands";
    public readonly static string all_legal_moves = "all_legal_moves";
    public static string boardsize(Vector2Int dimensions){ return string.Format("boardsize {0}x{1}",dimensions.x,dimensions.y);}
    public readonly static string clear_board = "clear_board";
    public static string compute_dominated(PlayerColours colour){ 
        return string.Format("compute-dominated {0}", BenzeneUtil.PlayerColourToStr(colour));
    }
    public readonly static string compute_dominated_cell = "compute-dominated-cell";
    public static string compute_fillin(PlayerColours colour){ 
        return string.Format("compute-fillin {0}", BenzeneUtil.PlayerColourToStr(colour));
    }
    public static string compute_inferior(PlayerColours colour){ 
        return string.Format("compute-inferior {0}", BenzeneUtil.PlayerColourToStr(colour));
    }
    public readonly static string compute_reversible = "compute-reversible";
    public readonly static string compute_vulnerable = "compute-vulnerable";

    public static string play(PlayerColours colour, Move move){

        return string.Format("play {0} {1}", BenzeneUtil.PlayerColourToStr(colour),BenzeneUtil.MoveToHexPoint(move));
    } 

    public static string genmove(PlayerColours colour){ 
        return string.Format("genmove {0}", BenzeneUtil.PlayerColourToStr(colour));
    }

    public readonly static string show_jypattern_list = "show_jypattern_list";
    public readonly static string showboard = "showboard";
    public readonly static string name = "name";
    public readonly static string undo = "undo";
    public readonly static string mohex_get_pv = "mohex-get-pv";

    public static string vc_build(PlayerColours colour){ 
        return string.Format("vc-build {0}",  BenzeneUtil.PlayerColourToStr(colour));
    }

    public static string vc_between_cells_full(PlayerColours colour, Move move1, Move move2){
        return string.Format("vc-between-cells-full {0} {1} {2}",BenzeneUtil.PlayerColourToStr(colour), BenzeneUtil.MoveToHexPoint(move1), BenzeneUtil.MoveToHexPoint(move2));
    }

    
    public static string vc_between_cells_full(PlayerColours colour, Move move, string moveStr){
        return string.Format("vc-between-cells-full {0} {1} {2}", BenzeneUtil.PlayerColourToStr(colour), BenzeneUtil.MoveToHexPoint(move), moveStr);
    }
}

