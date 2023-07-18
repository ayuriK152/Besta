using static Datas;

public class IngameManager
{
    public MusicPattern currentLoadedPattern;
    public int currentCombo;
    public int maxCombo;
    public double acurracy;

    public void Init()
    {
        currentCombo = 0;
        maxCombo = 0;
    }

    public void OnUpdate()
    {

    }
}
