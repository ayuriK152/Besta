using UnityEngine;

public class Datas
{
    public class Note
    {
        public int _laneNumber;
        public int _startTiming;
        public int _endTiming;
        public bool _isLongNote;

        public Note(int laneNumber, int startTiming, int endTiming, bool isLongNote)
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

        public MusicPattern()
        {
            _bpm = 120;
            _songOffset = 0;
            _musicSource = Resources.Load("Sounds/Grievous_Lady") as AudioClip;    // 테스트용 임시 노래파일, 선택 가능하도록 변경 요망
            _songLength = _songOffset + _musicSource.samples;
        }
    }
}
