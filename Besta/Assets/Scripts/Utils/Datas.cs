using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Define;

public class Datas
{
    [Serializable]
    public class Note : IComparable<Note>
    {
        public LaneNumber _laneNumber;
        public int _startTiming;
        public int _endTiming;
        public bool _isLongNote;

        public Note(LaneNumber laneNumber, int startTiming, int endTiming, bool isLongNote)
        {
            _laneNumber = laneNumber;
            _startTiming = startTiming;
            _endTiming = endTiming;
            _isLongNote = isLongNote;
        }

        public int CompareTo(Note other)
        {
            if (_startTiming > other._startTiming)
                return 1;
            else if (_startTiming < other._startTiming)
                return -1;
            else
                return 0;
        }
    }

    public class MusicPattern
    {
        public AudioClip _musicSource;
        public string _name;
        public int _bpm;
        public int _songOffset;
        public int _songLength;
        public List<Note> _noteDatas;

        public MusicPattern(string name)
        {
            _bpm = 120;
            _songOffset = 0;
            _name = name;
            _musicSource = Managers.Data.LoadMusicFile($"{Application.dataPath}/Patterns/{_name}/music.mp3");
            _songLength = _songOffset + _musicSource.samples;
            _noteDatas = new List<Note>();
        }

        public void ReloadMusic()
        {
            _musicSource = Managers.Data.LoadMusicFile($"{Application.dataPath}/Patterns/{_name}/music.mp3");
        }
    }
}
