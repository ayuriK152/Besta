public class Define
{
    public enum LaneNumber
    {
        None,
        First,
        Second,
        Third,
        Fourth
    }

    public enum EditorNoteMode
    {
        NormalNote,
        LongNote
    }

    public enum Beat
    {
        One,
        Two,
        Three,
        Four,
        Six,
        Eight,
        Nine,
        Sixteen,
        None
    }

    public enum MouseEvent
    {
        PointerDown,
        PointerUp,
        Press,
        Click
    }

    public enum MousePointer
    {
        Left,
        Right
    }

    public enum MouseScroll
    {
        Down,
        Up
    }

    public enum Scene
    {
        Unknown,
        Ingame,
        MusicSelect,
        StartMenu,
        PatternEditor,
        GameResult
    }

    public enum Judge
    {
        Besta,
        Good,
        Bad,
        Miss,
        None
    }

    public enum ScreenResolution
    {
        _1920x1080,
        _1600x900,
        _1440x810,
        _1366x768,
        _1280x720,
        _1024x576
    }

    public enum FullScreenMode
    {
        Fullscreen,
        Exclusive,
        Windowed
    }
}
