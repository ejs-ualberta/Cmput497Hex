using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;
using UnityEngine;

public static class BenzeneUtil
{   

    private static readonly string _jingyangPathSuffix = "/jingyang"; 

    private static Process _jingyang;

    public static Process JingYang{ 
        get{
            if(_jingyang == null)
                LaunchJingYang();
            return _jingyang;
        }
    }

    private static void LaunchJingYang(){
        

        if(!File.Exists(Application.streamingAssetsPath + _jingyangPathSuffix))
            UnityEngine.Debug.LogError("Couldn't find JingYang executable.");

        _jingyang = new Process();
        
        _jingyang.StartInfo.UseShellExecute = false;
        _jingyang.StartInfo.RedirectStandardOutput = true;
        _jingyang.StartInfo.RedirectStandardInput = true;
        _jingyang.StartInfo.CreateNoWindow = true;
        _jingyang.StartInfo.FileName = Application.streamingAssetsPath + _jingyangPathSuffix;
        _jingyang.Start();
    }

    public static string IssueCommand(Process process,string command){
        var stdIn = process.StandardInput;
        stdIn.WriteLine(command);
        stdIn.Flush();

        string result = "";

        while (!process.StandardOutput.EndOfStream) {
            var line = process.StandardOutput.ReadLine (); 
            //Sometimes the process believes it never reachers the EndOfStream so in the case that line is nonempty and we have results then escape.
            if(result != "" && line == "")    
                break;
            
            result += line;            
        }

        return result.Trim();
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
}
