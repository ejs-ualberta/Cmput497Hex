using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public static class SolverParser
{
    //This is a port from a c++ implentation to an static class emulating the behaviour of the executable.
    //Therefore this file can be imported into a c# codebase and used in place of running an executable

    //In principle it is the same but due to differences in how the languages handle List vs Vector the c++ implentation is more performant.
    //The bottleneck of performance for 9x9 is loading the file and parsing the patterns so this performance difference is not yet significant.

    // This should always be true as to not break things, unless it is set to false 
    // directly before a call to Main and then set back to true. See BotsDisplay.cs for examples.
    public static bool firstMoveInCentre = true;
    public static int BOARDSIZE;
    public static int Y_WIDTH;

    public struct PMOVE
    {
        public int local_move; //[1, ]
        public int global_move;//in the original board, i.e., in range [0, boardsize**2)
    };

    public struct Pattern
    {
        public Pattern(Pattern pattern){
            RN = pattern.RN;
            BT = pattern.BT;
            vc_BN = pattern.vc_BN.ToList();
            vc_WMs = new List<List<PMOVE>>();
            foreach(var pmoves in pattern.vc_WMs){
                vc_WMs.Add(pmoves.ToList());
            }
            vc_BM = pattern.vc_BM.ToList();
            vc_ND = pattern.vc_ND.ToList();
            vc_PSs = pattern.vc_PSs.ToList();
            vc_vc_PPs = pattern.vc_vc_PPs.ToList();
        }
        public int RN;//rule number, i.e., pattern index
        public int BT;//branch total
        public List<int> vc_BN;
        public List<List<PMOVE>> vc_WMs; //for each branch, white move list
        public List<PMOVE> vc_BM;
        //for each branch, for any move in white move list, black's counter move
        //Note the pair means local point move and its corresponding global point move
        //global point move means the move in the original board, i.e., in [0, boardsize*boardisze)
        public List<int> vc_ND; //for each branch, number of decompositions after white's play and black's response
        public List<List<int>> vc_PSs;//for each branch, list of decomposed pattern IDs
        public List<List<List<int>>> vc_vc_PPs; //for each branch, for each decomposed pattern, moves to update
    };

    private static string strip(string line)
    {
        
        int j = line.Length - 1;
        while (j >= 0 && line[j] != '"') j--;
        Debug.Assert(line[0] == '"' && line[j] == '"');
        string line2 = line.Substring(1, j - 1);
        return line2;
    }

    private static List<string> split(string str, char delimiter)
    {
        List<string> ret;
        string ss = ""; // Turn the string into a stream.        
        ret = new List<string>(str.Split(delimiter)); 
        return ret;
    }

    private static int cell_to_point(string move)
    {
        int x = move[0] - 'a';
        int y = int.Parse(move.Substring(1, Y_WIDTH)) - 1;
        return y * BOARDSIZE + x; //point integer starts from 0
    }

    private static int reflect_point(int point)
    {
        return BOARDSIZE * BOARDSIZE - point - 1;
    }

    private static string point_to_cell(int point)
    {
        string ret = "";
        int x, y;
        x = point % BOARDSIZE;
        y = point / BOARDSIZE;
        ret += (char)('a' + x);
        ret += (1+y).ToString();
        return ret;
    }

    private static bool empty(List<string> board, int point)
    {
        int x, y;
        x = point % BOARDSIZE;
        y = point / BOARDSIZE;
        return board[y][x] == '.';
    }

    private static void next(List<string> vc_str, ref int idx, ref List<string> tokens)
    {
        tokens = split(vc_str[idx], ' ');
        idx++;
    }

    private static PMOVE init_PMOVE(int m, int RN)
    {
        PMOVE p = new PMOVE();
        p.local_move = m;
        p.global_move = -1;//will calculate later
        if (RN == 1)
        {
            p.global_move = m;
            if (m <= BOARDSIZE * BOARDSIZE / 2)
                p.global_move = m - 1;
        }
        return p;
    }


    private static void parse_patterns(List<string> vc_str, ref Dictionary<int, Pattern> patterns)
    {
        string line;
        List<string> tokens = new List<string>();
        int i, j, k;
        int idx = 0;
        int m;
        while (idx < vc_str.Count)
        {
            next(vc_str, ref idx,ref tokens);
            if (tokens[0] == "RN")
            {
                //a new pattern
                Pattern pattern = new Pattern(){
                    vc_BM = new List<PMOVE>(),
                    vc_ND = new List<int>(),
                    vc_PSs = new List<List<int>>(),
                    vc_BN = new List<int>(),
                    vc_WMs = new List<List<PMOVE>>(),
                    vc_vc_PPs = new List<List<List<int>>>()
                };
                pattern.RN = int.Parse(tokens[1]);
                int RN = pattern.RN;
                next(vc_str,ref idx, ref tokens);
                Debug.Assert(tokens[0] == "BT");
                pattern.BT = int.Parse(tokens[1]);
                for (int bn = 0; bn < pattern.BT; bn++)
                {
                    next(vc_str,ref idx,ref tokens);
                    Debug.Assert(tokens[0] == "BN");
                    pattern.vc_BN.Add(int.Parse(tokens[1]));
                    next(vc_str,ref idx, ref tokens); Debug.Assert(tokens[0] == "WM");
                    //print_tokens(tokens);
                    List<PMOVE> WMs = new List<PMOVE>();
                    for (j = 1; j < tokens.Count; j++)
                    {
                        m = int.Parse(tokens[j]);
                        WMs.Add(init_PMOVE(m, RN));
                    }
                    pattern.vc_WMs.Add(WMs);
                    next(vc_str,ref idx, ref tokens); Debug.Assert(tokens[0] == "BM");
                    //print_tokens(tokens);
                    m = int.Parse(tokens[1]);
                    pattern.vc_BM.Add(init_PMOVE(m, RN));
                    next(vc_str,ref idx,ref tokens); Debug.Assert(tokens[0] == "ND");
                    int ND = int.Parse(tokens[1]);
                    pattern.vc_ND.Add(ND);
                    //print_tokens(tokens);
                    List<int> PSs = new List<int>();
                    List<List<int>> vcs = new List<List<int>>();
                    if (ND == 0)
                    {
                        
                        pattern.vc_PSs.Add(PSs);
                       
                        pattern.vc_vc_PPs.Add(vcs);
                        continue;
                    }
                    next(vc_str, ref idx, ref tokens); Debug.Assert(tokens[0] == "PS");
                    PSs = new List<int>();
                    for (j = 1; j < tokens.Count; j++)
                    {
                        m = int.Parse(tokens[j]);
                        PSs.Add(m);
                    }   
                    pattern.vc_PSs.Add(PSs);
                    vcs = new List<List<int>>();
                    for (k = 0; k < ND; k++)
                    {
                        next(vc_str,ref  idx, ref tokens);
                        Debug.Assert(tokens[0] == "PP");
                        List<int> vc = new List<int>();
                        for (j = 1; j < tokens.Count; j++)
                        {
                            m = int.Parse(tokens[j]);
                            vc.Add(m);
                        }
                        vcs.Add(vc);
                    }
                    pattern.vc_vc_PPs.Add(vcs);
                }
                patterns[pattern.RN] = pattern;
            }

        }
    }
    private static void add_new_patterns(ref Dictionary<int, Pattern> all_patterns, Pattern cur_pattern,
    int bn,ref List<Pattern> working_patterns)
    {
        int i, j, k;
        Dictionary<int, int> local_global = new Dictionary<int, int>();
        for (i = 0; i < cur_pattern.vc_WMs.Count; i++)
        {
            for (j = 1; j < cur_pattern.vc_WMs[i].Count; j++)
            {
                local_global[cur_pattern.vc_WMs[i][j].local_move] = cur_pattern.vc_WMs[i][j].global_move;
            }
        }
        if (cur_pattern.RN == 1)
        {
            for (i = 1; i <= BOARDSIZE * BOARDSIZE; i++)
            {
                local_global[i] = i - 1;
            }
        }

        if (cur_pattern.vc_ND[bn] == 0) return; //nothing to add

        int ND = cur_pattern.vc_vc_PPs[bn].Count;
        
        for (i = 0; i < ND; i++)
        {
            int ch_id = cur_pattern.vc_vc_PPs[bn][i][0];
            Dictionary<int, int> ch_pa = new Dictionary<int, int>();
            for (j = 1; j < cur_pattern.vc_vc_PPs[bn][i].Count; j++)
            {
                int m = cur_pattern.vc_vc_PPs[bn][i][j];
                ch_pa[j] = local_global[m];
            }
            Pattern p = new Pattern(all_patterns[ch_id]);
            
            for (j = 0; j < p.vc_WMs.Count; j++)
            {
                int l;
                PMOVE pmove;
                for (k = 1; k < p.vc_WMs[j].Count; k++)
                {
                    pmove = new PMOVE();
                    
                    l = p.vc_WMs[j][k].local_move;                    
                    //p.vc_BM[j].global_move = ch_pa[l];
                    pmove.global_move = ch_pa[l];
                    pmove.local_move = p.vc_WMs[j][k].local_move;
                    p.vc_WMs[j][k] = pmove;
                }
                pmove = new PMOVE();
                l = p.vc_BM[j].local_move;
                pmove.global_move = ch_pa[l];
                pmove.local_move = p.vc_BM[j].local_move;
                //p.vc_BM[j].global_move = ch_pa[l];
                p.vc_BM[j] = pmove;
            }
            working_patterns.Add(p);
        }
    }

    private static string genmove(string white_move, List<Pattern> working_patterns,ref  Dictionary<int, Pattern> all_patterns)
    {
        //genmove black move by computer
        //first transoform white's move into local move
        int white = cell_to_point(white_move);
        int i, j, k, index;
        int BM = -1;
        string black_move = "";
        //cerr << "white move: " << white_move << " =>" << white << endl;
        Pattern pattern = working_patterns[0];
        for (index = 0; index < working_patterns.Count; index++)
        {
            
            pattern = working_patterns[index];            
            for (i = 0; i < pattern.vc_WMs.Count; i++)
            {
                for (j = 1; j < pattern.vc_WMs[i].Count; j++)
                {
                    //Contains the white move
                    //cerr<<pattern.vc_WMs[i][j].local_move<<"=>"<<pattern.vc_WMs[i][j].global_move<<"; ";
                    if (pattern.vc_WMs[i][j].global_move == white)
                    {
                        BM = pattern.vc_BM[i].global_move;
                        add_new_patterns(ref all_patterns, pattern, i, ref working_patterns);
                        //vector<Pattern>::iterator it = working_patterns.begin();
                        //it += index;  
                        working_patterns.Remove(pattern);
                        black_move = point_to_cell(BM);
                        return black_move;
                    }
                }
            }
        }
        //white move didn't Contains, so play a move in the working_patterns
        //cerr << "no matching white move, select the last pattern " << pattern.RN << "\n";
        BM = pattern.vc_BM[0].global_move;
        working_patterns.Remove(working_patterns[working_patterns.Count - 1]);
        add_new_patterns(ref all_patterns,pattern, 0, ref working_patterns);
        black_move = point_to_cell(BM);
        return black_move;
    }

    private static void play(ref List<string> board, char color, string move)
    {
        //Debug.Log(System.String.Join("\n", board));
        int x = move[0] - 'a';
        int y = int.Parse(move.Substring(1)) - 1;
        StringBuilder sb = new StringBuilder(board[y]);
        sb[x] = color;
        board[y] = sb.ToString();
        //Debug.Log(System.String.Join("\n", board));
    }


    //variables from in gtp_loop. We must emulate the 'stateful' aspect of the executable here
    private static Dictionary<int, Pattern> all_patterns;
    private static string text;
    private static int mid_point;
    private static int toplay;
    private static string white_move;
    private static List<Pattern> working_patterns;
    private static Stack<List<Pattern>> previous_working_patterns;

    private static Pattern p;
    //working_patterns.push_back(p);
    private static Stack<int> history;
    private static bool reflect;
    private static bool can_reflect;
    private static int point;
    private static List<string> board;

    public static string IssueCommand(string command)
    {
        text = command;
        
        if (text.Contains("genmove b")  || text.Contains("genmove black") )
        {
            if (working_patterns.Count == 0)
            {
                return "= invalid";
                
            }


            previous_working_patterns.Push(working_patterns.ToList());
            string black_move = genmove(white_move, working_patterns, ref all_patterns);
            point = cell_to_point(black_move);
            history.Push(point);
            if (reflect && can_reflect)
            {
                point = reflect_point(point);
                black_move = point_to_cell(point);
            }

            play(ref board, 'b', black_move);
            toplay = 1 - toplay;
            /*cerr << "Reflect:" << reflect << " Patterns now:";
            for (auto & p2: working_patterns)
                cerr << p2.RN << " ";
            cerr << endl;*/
            return "= " + black_move;
        }
        if (text.Contains("play ") )
        {
            List<string> tokens = split(text, ' ');
            if (tokens[1][0] == 'B' || tokens[1][0] == 'b')
            {
                if (!firstMoveInCentre){
                    play(ref board, 'b', tokens[2]);
                    return "= " + tokens[2];
                }
                return "= \n";
            }
            white_move = tokens[2];
            if (white_move.Length != 2)
            {
                Debug.LogError("Wrong move");
                return "";
            }
            point = cell_to_point(white_move);
            if (point < 0 || point > BOARDSIZE * BOARDSIZE - 1)
            {
                Debug.LogError("move out of range");
                return "";
            }
            //cerr << "white move: " << point << "\n" << endl;
            if (!empty(board, point))
            {
                //Debug.Log(System.String.Join("\n", board));
                Debug.LogError("occupied cell");
                return "";
            }
            play(ref board, tokens[1][0], white_move);
            if (history.Count == 0 && point > BOARDSIZE * BOARDSIZE / 2)
            {                
                reflect = true && can_reflect;
            }

            if (reflect)
            {
                point = reflect_point(point);//symmetric move
                white_move = point_to_cell(point);
            }
            history.Push(point);
            toplay = 1 - toplay;
            return "= \n";
        }

        if (text.Contains("version") )
        {
            return "= 2.0";
        }
        if (text.Contains("name") )
        {
            return "= jingyang";
        }
        if (text.Contains("hexgui-analyze_commands") )
        {
            return "= \n";
        }
        if (text.Contains("boardsize") )
        {
            return "= " + BOARDSIZE;
        }
        if (text.Contains("clear_board") )
        {
            for (int i = 0; i < BOARDSIZE; i++)
            {
                for (int j = 0; j < BOARDSIZE; j++)
                {
                    StringBuilder sb = new StringBuilder(board[i]);
                    sb[j] = '.';
                    board[i] = sb.ToString();
                }
            }
            white_move = "";
            working_patterns.Clear();            
            previous_working_patterns.Clear();          
            Pattern p = all_patterns[1];
            working_patterns.Add(p);            
            history.Clear();            
            reflect = false;
            point = 0;
            if (firstMoveInCentre){
                play(ref board, 'b', point_to_cell(mid_point));
            }
            toplay = 1;
            return "= \n";
        }
        if (text.Contains("undo") )
        {   //Debug.LogError(System.String.Join("\n", board));
            if (history.Count > 0)
            {
                if (toplay == 1)
                {
                    working_patterns = previous_working_patterns.Pop();                
                    toplay = 0;
                }
                else if (toplay == 0)
                {
                    toplay = 1;
                }
                string lastmove = point_to_cell((reflect) ? reflect_point(history.Peek()) : history.Peek());
                //Debug.Log("Undoing: " + lastmove);
                play(ref board, '.', lastmove);
                history.Pop();
                //Debug.Log("History size: " + history.Count);
                if (history.Count == 0 && reflect)
                {
                    //cerr << "Unreflecting\n" << endl;
                    reflect = false;
                }
            }
            //Debug.LogError(System.String.Join("\n", board));
            return "= \n";
        }                
        return "Invalid Command";
    }

    public static void Main(string file_name)
    {
        List<string> vc_str = new List<string>();

        //Add this behaviour to emulate killing and reoping an executable
        previous_working_patterns = new Stack<List<Pattern>>();;
        text = "";
        white_move = "";
        all_patterns = new Dictionary<int, Pattern>();
        working_patterns = new List<Pattern>();
        history = new Stack<int>();
        point = 0;
        board = new List<string>();
        reflect = false;

        string[] lines = SolverFileLoader.instance.GetFileContent(file_name);
        //Debug.Log(lines.Length);
        foreach(var line in lines)
        {
            if (line.Length == 0)
                continue;

            if (line[0] == '#')
            {
                BOARDSIZE = int.Parse(split(line, ' ')[1]);
                Y_WIDTH = BOARDSIZE >= 10 ? 2 : 1;
                can_reflect = (BOARDSIZE == 9);
                Debug.Log("Found boardsize:" + BOARDSIZE);
                
            }
            if (line[0] != '"')
                continue;
            

            vc_str.Add(strip(line));
        }
        //cerr << vc_str.size() << " lines in total" << endl;
        all_patterns = new Dictionary<int, Pattern>();
        parse_patterns(vc_str, ref all_patterns);
        board = new List<string>(BOARDSIZE);
        mid_point = ((BOARDSIZE - 1) / 2) * BOARDSIZE + (BOARDSIZE / 2);
       
       
        Debug.LogFormat("Found {0} patterns",all_patterns.Count);

        for (int i = 0; i < BOARDSIZE; i++)
        {                        
            board.Insert(i,"");
            for (int j = 0; j < BOARDSIZE; j++)
            {
                board[i] += '.';
            }
        }
        p = all_patterns[1];
        working_patterns.Add(p);
        if (firstMoveInCentre){
           play(ref board, 'b', point_to_cell(mid_point));
        }
        int toplay = 1;

    }
}
