using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class Datas
{
    [Serializable]
    public class Note : IComparable<Note>
    {
        public LaneNumber laneNumber;
        public int startTiming;
        public int endTiming;
        public bool isLongNote;

        public Note(LaneNumber _laneNumber, int _startTiming, int _endTiming, bool _isLongNote)
        {
            laneNumber = _laneNumber;
            startTiming = _startTiming;
            endTiming = _endTiming;
            isLongNote = _isLongNote;
        }

        public int CompareTo(Note other)
        {
            if (startTiming > other.startTiming)
                return 1;
            else if (startTiming < other.startTiming)
                return -1;
            else
                return 0;
        }
    }

    public class MusicPattern
    {
        public AudioClip musicSource;
        public string name;
        public string artist;
        public string designer;
        public int bpm;
        public int songOffset;
        public int songLength;
        public int totalCombo;
        public List<Note> noteDatas;

        public MusicPattern(string _name)
        {
            bpm = 120;
            songOffset = 0;
            name = _name;
            artist = "";
            designer = "";
            ReloadMusic();
            songLength = songOffset + musicSource.samples;
            noteDatas = new List<Note>();
            totalCombo = 0;
        }

        public void ReloadMusic()
        {
#if UNITY_EDITOR
            musicSource = Managers.Data.LoadMusicFile($"{Application.dataPath}/Resources/Patterns/{name}/music.mp3");
#elif UNITY_STANDALONE_WIN
            if (Managers.Scene.currentScene == Scene.PatternEditor)
                musicSource = Managers.Data.LoadMusicFile($"{Application.dataPath}/Patterns/{name}/music.mp3");
            else
                musicSource = Resources.Load<AudioClip>($"Patterns/{name}/music");
#endif
        }
    }
}
