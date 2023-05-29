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
        public AudioSource _musicSource;
        public int _bpm;
        public double _songOffset = 0.0f;

        public MusicPattern()
        {
            _bpm = 120;
            _songOffset = 0.0f;
        }
    }
}
