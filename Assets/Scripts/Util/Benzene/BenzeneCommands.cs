using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BenzeneCommands {
    //This class defines commands to interface with benzene/c++/c# backends.

    public static string boardsize(Vector2Int dimensions){ return string.Format("boardsize {0}x{1}",dimensions.x,dimensions.y);}
    public readonly static string clear_board = "clear_board";
    
    public static string play(PlayerColours colour, Move move){
        return string.Format("play {0} {1}", BenzeneUtil.PlayerColourToStr(colour),BenzeneUtil.MoveToHexPoint(move));
    } 

    public static string genmove(PlayerColours colour){ 
        return string.Format("genmove {0}", BenzeneUtil.PlayerColourToStr(colour));
    }

    public readonly static string show_jypattern_list = "show_jypattern_list";
    public readonly static string show_jybranch_list = "show_jybranch_list";
    public readonly static string show_jyblackmoves_list = "show_jyblackmoves_list";
    public readonly static string showboard = "showboard";
    public readonly static string name = "name";
    public readonly static string undo = "undo";

}

