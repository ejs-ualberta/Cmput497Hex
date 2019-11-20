using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SerializationManager : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;

    public static SerializationManager instance;

    private readonly String FileName = "Settings";
    private readonly BinaryFormatter _binaryFormatter = new BinaryFormatter();

    private void Awake()
    {
        instance = this;
        if (File.Exists(GetSettingsPath()))
        {
            Deserialize();
        }        
    }

    private void OnApplicationQuit()
    {
        Settings.BoardString = _gameManager.Board.ToString();
        Serialize();
    }

    internal void Serialize()
    {
        var settingsFile = new FileStream(GetSettingsPath(), FileMode.OpenOrCreate);
        _binaryFormatter.Serialize(settingsFile,Settings.BoardString);
        _binaryFormatter.Serialize(settingsFile,Settings.JYSettings);
        settingsFile.Close();
    }

    internal void Deserialize()
    {
        var settingsFile = new FileStream(GetSettingsPath(), FileMode.Open);
        Settings.BoardString = (string)_binaryFormatter.Deserialize(settingsFile);        
        Settings.JYSettings = (JingYangSettings) _binaryFormatter.Deserialize(settingsFile);
        settingsFile.Close();
    }

    private string GetSettingsPath()
    {
        return Application.persistentDataPath + "/" + FileName;
    }
}
