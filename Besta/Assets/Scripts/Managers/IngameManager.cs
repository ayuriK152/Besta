using static Datas;

public class IngameManager
{
    public MusicPattern currentLoadedPattern;
    public int currentCombo;
    public int maxCombo;
    public double acurracy;
    public double progressByCombo;
    public int gainedScore;

    public void Init()
    {
        currentCombo = 0;
        maxCombo = 0;
        gainedScore = 0;
    }

    public void OnUpdate()
    {

    }
}
