using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;
using UnityEngine;

public static class BenzeneUtil
{   


#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
    private static readonly string _pathSuffix = "-linux"; 
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
    private static readonly string _pathSuffix = "-mac";     
#endif

    private static readonly string _jingyangPatternsPath = Application.streamingAssetsPath + "/hex99-3.txt";
    private static readonly string _jingyangPath = Application.streamingAssetsPath + "/jingyang" + _pathSuffix;
    private static readonly string _moHexPath = Application.streamingAssetsPath + "/mohex" + _pathSuffix;
    private static Process _jingyang;
    private static Process _moHex;

    public static Process JingYang{ 
        get{
            if(_jingyang == null)
                _jingyang = LaunchProcess(_jingyangPath,_jingyangPatternsPath);
            return _jingyang;
        }
        set{
            _jingyang = value;
        }
    }

    public static Process MoHex{
        get{
            if(_moHex == null)
                _moHex = LaunchProcess(_moHexPath);
            return _moHex;
        }
    }

    public static string ProcessOutput {get;set;}

    private static Process LaunchProcess(string processPath, string args=""){
        
        if(!File.Exists(processPath))
            UnityEngine.Debug.LogError("Couldn't find process executable.");

        var proc = new Process();
        proc.StartInfo.UseShellExecute = false;
        proc.StartInfo.RedirectStandardOutput = true;
        proc.StartInfo.RedirectStandardInput = true;
        proc.StartInfo.CreateNoWindow = true;
        proc.StartInfo.FileName = processPath;
        if(args != "")
            proc.StartInfo.Arguments = args;
        proc.Start();
        return proc;
    }

    public static void RestartProcess(Process process, string args){
        string path;
        if(process == _jingyang){
            path = _jingyangPath;
            process.Kill();
            _jingyang = LaunchProcess(_jingyangPath,args);
        }

    }

    public static string IssueCommand(Process process,string command){
        var stdIn = process.StandardInput;

        stdIn.WriteLine(command);
        stdIn.Flush();

        string result = "";
        int tries = 0;
        while (!process.StandardOutput.EndOfStream) {
            var line = process.StandardOutput.ReadLine (); 
            //Sometimes the process believes it never reachers the EndOfStream so in the case that line is nonempty and we have results then escape.
            if(result != "" && line == "" && tries < 1000)    
                break;
            
            result += line;            
            tries ++;
        }


        return result.Trim();
    }

    //For long latency commands such as genmove in MoHex, it is useful to perform them asynchronously
    public static void IssueCommandAsync(Process process,string command){
        var stdIn = process.StandardInput;
        stdIn.WriteLine(command);
        stdIn.Flush();
        string result = "";

        while (!process.StandardOutput.EndOfStream) {
            var line = process.StandardOutput.ReadLine (); 
            //Sometimes the process believes it never reachers the EndOfStream so in the case that line is nonempty and we have results then escape.
            if(result != "" && line == ""){
                break;
            }

            result += line;            
        }
        ProcessOutput = result;
    }

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
