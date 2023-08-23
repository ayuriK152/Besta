using System;
using System.IO;
using UnityEngine;

public class DataManager
{
    public void Init()
    {
        PlayerOptionInit();
    }

    // 채보 에디터의 작업물 저장은 어차피 외부 디렉토리에 되어야함. 수정 불필요
    public void SavePatternAsJson<T>(T obj, string patternName)
    {
#if UNITY_EDITOR
        if (!Directory.Exists($"{Application.dataPath}/Resources/Patterns/"))
        {
            Directory.CreateDirectory($"{Application.dataPath}/Resources/Patterns/");
            Debug.Log("Pattern directory does not exists, Pattern directory is added automatically");
        }
        if (!Directory.Exists($"{Application.dataPath}/Resources/Patterns/{patternName}"))
        {
            Directory.CreateDirectory($"{Application.dataPath}/Resources/Patterns/{patternName}");
            Debug.Log("This pattern's directory does not exists, Pattern directory is added automatically");
        }
        File.WriteAllText($"{Application.dataPath}/Resources/Patterns/{patternName}/data.json", JsonUtility.ToJson(obj));
#elif PLATFORM_STANDALONE_WIN
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
#endif
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

    public T LoadJsonData<T>(TextAsset data)
    {
        string loadedText;
        try
        {
            loadedText = data.ToString();
            T loadedData = JsonUtility.FromJson<T>(loadedText);
            Debug.Log("File loaded successfully!");
            return loadedData;
        }
        catch (Exception e)
        {
            Debug.LogError($"File load failed");
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

    public Texture2D LoadImageFile(string path)
    {
        byte[] textureBytes = File.ReadAllBytes(path);
        if (textureBytes == null)
        {
            Debug.LogError("Image file load fail!");
            return null;
        }
        Texture2D loadedTexture = new Texture2D(0, 0);
        loadedTexture.LoadImage(textureBytes);
        return loadedTexture;
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
        {
            Directory.Move($"{Application.dataPath}/Patterns/{origin}", $"{Application.dataPath}/Patterns/{target}");
            Directory.Delete($"{Application.dataPath}/Patterns/{origin}");
        }
        else
        {
            Debug.LogError($"Directory name \"{target}\" already exists");
            return false;
        }
        return true;
    }

    public void PlayerOptionInit()
    {
        if (!PlayerPrefs.HasKey("MusicSoundValue"))
            PlayerPrefs.SetInt("MusicSoundValue", 50);
        Managers.Sound.managerAudioSource.volume = PlayerPrefs.GetInt("MusicSoundValue") / 100.0f;

        if (!PlayerPrefs.HasKey("EffectSoundValue"))
            PlayerPrefs.SetFloat("EffectSoundValue", 0.5f);

        if (!PlayerPrefs.HasKey("LastSelectMusic"))
            PlayerPrefs.SetInt("LastSelectMusic", 0);

        if (!PlayerPrefs.HasKey("UserOffset"))
            PlayerPrefs.SetInt("UserOffset", 0);

        if (!PlayerPrefs.HasKey("JudgeLineHeight"))
            PlayerPrefs.SetInt("JudgeLineHeight", 0);

        if (!PlayerPrefs.HasKey("SlideSpeed"))
            PlayerPrefs.SetInt("SlideSpeed", 10);
    }
}
