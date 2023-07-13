using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameUI : MonoBehaviour
{
    public void OnStartButtonClick()
    {
        GameController.isPlaying = true;
    }
}
