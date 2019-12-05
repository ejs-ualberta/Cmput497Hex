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
    [SerializeField] private string[] _strategyFiles;

    private readonly Dictionary<string,string[]> _fileContents = new Dictionary<string, string[]>();
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
        foreach(var file in _strategyFiles){
            var fileName = Application.streamingAssetsPath + "/" + file;
#if UNITY_WEBGL
            StartCoroutine(GetRequest(fileName));
#else
            _fileContents[fileName] = File.ReadAllLines(fileName);
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
}
