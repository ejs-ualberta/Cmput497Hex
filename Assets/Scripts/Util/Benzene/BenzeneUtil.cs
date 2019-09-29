using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public static class BenzeneUtil
{   
    //TODO: Use system vars to find benzene installation
    private static readonly string _jingyangPath = "/home/d/benzene-vanilla-cmake/build/src/jingyang/jingyang";

    private static Process _jingyang;

    public static Process JingYang{
        get{
            if(_jingyang == null)
                LaunchJingYang();
            return _jingyang;
        }
    }

    private static void LaunchJingYang(){
        
        
        if(!File.Exists(_jingyangPath))
            UnityEngine.Debug.LogError("Couldn't find JingYang executable.");

        _jingyang = new Process();
        
        _jingyang.StartInfo.UseShellExecute = false;
        _jingyang.StartInfo.RedirectStandardOutput = true;
        _jingyang.StartInfo.RedirectStandardInput = true;
        _jingyang.StartInfo.CreateNoWindow = true;
        _jingyang.StartInfo.FileName = _jingyangPath;
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
        if(result.Contains("="))
            return result.Split('=')[1].Trim();
        return "";
    }
}
