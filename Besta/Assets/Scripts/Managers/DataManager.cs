using System;
using System.IO;
using UnityEngine;

public class DataManager
{
    public void SavePatternAsJson<T>(T obj, string patternName)
    {
        if (!Directory.Exists($"{Application.dataPath}/Patterns/"))
        {
            Directory.CreateDirectory($"{Application.dataPath}/Patterns/");
            Debug.Log("Pattern directory does not exists, Pattern directory is added automatically");
        }
        if (!Directory.Exists($"{Application.dataPath}/Patterns/{patternName}"))
        {
            Directory.CreateDirectory($"{Application.dataPath}/Patterns/{patternName}");
            Debug.Log("This pattern's directory does not exists, Pattern directory is added automatically");
        }
        File.WriteAllText($"{Application.dataPath}/Patterns/{patternName}/data.json", JsonUtility.ToJson(obj));
        Debug.Log("File saved succesfully!");
    }

    public T LoadJsonData<T>(string path)
    {
        string loadedText;
        try
        {
            loadedText = File.ReadAllText(path);
            T loadedData = JsonUtility.FromJson<T>(loadedText);
            Debug.Log("File loaded successfully!");
            return loadedData;
        }
        catch (Exception e)
        {
            Debug.LogError($"File load failed from: {path}");
            return default(T);
        }
    }

    public AudioClip LoadMusicFile(string path)
    {
        WWW uri = new WWW(path);
        while (!uri.isDone) { }

        try
        {
            Debug.Log("Music file loaded completly!");
            return uri.GetAudioClip();
        }
        catch (Exception e)
        {
            Debug.Log("Failed to load music file!");
            return null;
        }
    }

    public bool CopyMusicFilesWithCheckDirectory(string origin)
    {
        bool isDirectoryCreated = false;
        string[] directorys = origin.Split("/");
        string name = directorys[directorys.Length - 1].Split(".")[0];
        if (File.Exists($"{Application.dataPath}/Patterns/{name}/music.mp3"))
            return false;
        if (!Directory.Exists($"{Application.dataPath}/Patterns/"))
        {
            Directory.CreateDirectory($"{Application.dataPath}/Patterns/");
            Debug.Log("Pattern directory does not exists, Pattern directory is added automatically");
        }
        if (!Directory.Exists($"{Application.dataPath}/Patterns/{name}"))
        {
            Directory.CreateDirectory($"{Application.dataPath}/Patterns/{name}");
            Debug.Log("This pattern's directory does not exists, Pattern directory is added automatically");
            isDirectoryCreated = true;
        }
        File.Copy(origin, $"{Application.dataPath}/Patterns/{name}/music.mp3");
        return isDirectoryCreated;
    }

    public bool ChangePatternDirectoryName(string origin, string target)
    {
        if (!Directory.Exists($"{Application.dataPath}/Patterns/{target}"))
            Directory.Move($"{Application.dataPath}/Patterns/{origin}", $"{Application.dataPath}/Patterns/{target}");
        else
        {
            Debug.LogError($"Directory name \"{target}\" already exists");
            return false;
        }
        return true;
    }
}
