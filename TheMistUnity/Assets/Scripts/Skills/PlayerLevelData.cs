using System;

[Serializable]
public class PlayerLevelData
{
    public int level;
    public int currentExp;
    public int nextLevelExp;

    public PlayerLevelData(int level, int currentExp, int nextLevelExp)
    {
        this.level = level;
        this.currentExp = currentExp;
        this.nextLevelExp = nextLevelExp;
    }
}