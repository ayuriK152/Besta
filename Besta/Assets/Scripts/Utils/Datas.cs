using UnityEngine;

public class Datas
{
    public class Note
    {
        public double _startTiming;
        public double _endTiming;
        public bool _isLongNote;

        public Note(double startTiming, double endTiming, bool isLongNote)
        {
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
