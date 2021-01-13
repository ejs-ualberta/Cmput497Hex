using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
public class SolverFileLoader : MonoBehaviour
{
    //WebGL and android require that files within StreamingAsset be loaded via webrequest thus simply calling File.ReadAllLines is not sufficient for all platforms.
    //Instead this class must load the files in a coroutine and provide the data when ready. 
    //Therefore this class caches the contents of all files listed in _strategyFiles. This class can then be queried to request the status and data of a given file.

    public static SolverFileLoader instance;
    private string[] _strategyFiles;
    [SerializeField] private string sf_path = Application.streamingAssetsPath + "/strategy_files.txt";

    private static Dictionary<string,string[]> _fileContents = new Dictionary<string, string[]>();


    public string[] GetFileContent(string fileName){
        if(_fileContents.ContainsKey(fileName)){
            return _fileContents[fileName];
        }
        return new string[]{};
    }

    public bool IsFileReady(string fileName ){
        return _fileContents.ContainsKey(fileName);
    }

    void Awake()
    {
        instance = this;
#if UNITY_WEBGL
        StartCoroutine(GetRequest(sf_path));
        while (!_fileContents.ContainsKey(sf_path)){
            Debug.Log("...");
        }
        _strategyFiles = _fileContents[sf_path];
#else
        _strategyFiles = File.ReadAllLines(sf_path);
#endif
        foreach(var file in _strategyFiles){
            var fileName = Application.streamingAssetsPath + "/" + file;
#if UNITY_WEBGL
            StartCoroutine(GetRequest(fileName));
#else
            _fileContents[fileName] = File.ReadAllLines(fileName);
            Debug.Log(_fileContents[fileName][0]);
            Debug.LogFormat("{0} {1}",fileName,_fileContents[fileName].Length);
#endif
        }

    }

    private IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                _fileContents[uri] = webRequest.downloadHandler.text.Split('\n');
            }
        }
    }

    public static List<string> GetNxNStrategyFileNames(uint n){
        List<string> fnames = new List<string>();
        foreach(KeyValuePair<string, string[]> kvp in _fileContents){
            string[] path = kvp.Key.Split('/');
            string name = path[path.Length - 1];
            if (name.Contains(string.Format("{0}_{1}", n, n))){
                fnames.Add(name);
            }
        }
        return fnames;
    }
}
