using System.IO;
using UnityEngine;

public class DataManager
{
    public void SavePatternAsJson<T>(T obj, string patternName)
    {
        if (!Directory.Exists(Application.dataPath + "/Patterns/"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Patterns/");
            Debug.Log("Pattern directory does not exists, Pattern directory is added automatically");
        }
        File.WriteAllText(Application.dataPath + "/Patterns/" + patternName + ".json", JsonUtility.ToJson(obj));
        Debug.Log("File saved succesfully!");
    }
}
