using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Datas;

public class MusicSelectUI : MonoBehaviour
{
    public void OnTempLoadClick()
    {
        Managers.Game.currentLoadedPattern = Managers.Data.LoadJsonData<MusicPattern>(EditorUtility.OpenFilePanel("Choose music pattern","","json"));
    }
}
