using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Define;

public class Datas
{
    [Serializable]
    public class Note
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
    }

    public class MusicPattern
    {
        public AudioClip _musicSource;
        public int _bpm;
        public int _songOffset;
        public int _songLength;
        public List<Note> _noteDatas;

        public MusicPattern()
        {
            _bpm = 210;
            _songOffset = 0;
            _musicSource = Resources.Load("Sounds/Test") as AudioClip;    // �׽�Ʈ�� �ӽ� �뷡����, ���� �����ϵ��� ���� ���
            _songLength = _songOffset + _musicSource.samples;
            _noteDatas = new List<Note>();
        }
    }
}
