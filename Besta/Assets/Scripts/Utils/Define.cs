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
        Four,
        Eight,
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
        StartMenu
    }

    public enum Judge
    {
        Besta,
        Good,
        Bad,
        Miss
    }
}
