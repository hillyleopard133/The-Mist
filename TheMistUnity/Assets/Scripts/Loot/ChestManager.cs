using System.Collections.Generic;
using BayatGames.SaveGameFree;

public class ChestManager : Singleton<ChestManager>
{
    private List<string> chests = new List<string>();
    
    private const string CHESTS = "CHESTS";

    public void OpenChest(string chestID)
    {
        chests.Add(chestID);
        SaveChests();
    }

    public bool IsOpened(string chestID)
    {
        return chests.Contains(chestID);
    }

    public void ResetChests()
    {
        chests.Clear();
    }

    public void LoadChests()
    {
        if (SaveGame.Exists(CHESTS))
        {
            chests = SaveGame.Load<List<string>>(CHESTS);
        }
    }

    public void SaveChests()
    {
        SaveGame.Save(CHESTS, chests);
    }
}