using System;
using BayatGames.SaveGameFree;
using Mono.Cecil;
using UnityEngine;

public enum Temples
{
    Fire,
    Ice,
    Wind
}

public class TempleManager : Singleton<TempleManager>
{
    [SerializeField] private int fireRelicsAmount = 3;
    [SerializeField] private int iceRelicsAmount = 3;
    [SerializeField] private int windRelicsAmount = 3;
    private int fireRelics;
    private int iceRelics;
    private int windRelics;
    
    [SerializeField] private int fireChestsAmount = 3;
    [SerializeField] private int iceChestsAmount = 3;
    [SerializeField] private int windChestsAmount = 3;
    private int fireChests;
    private int iceChests;
    private int windChests;
    
    private bool[] templesCleared = new bool[3];
    
    [HideInInspector] public Temples currentTemple = Temples.Fire;
    
    private UIManager uiManager;
    
    private const string TEMPLE_DATA = "TEMPLE_DATA";

    private void Start()
    {
        uiManager = UIManager.Instance;
    }

    public bool IsTempleCleared(Temples temple)
    {
        switch (temple)
        {
            case Temples.Fire:
                return templesCleared[0];
            case Temples.Ice:
                return templesCleared[1];
            case Temples.Wind:
                return templesCleared[2];
        }
        return false;
    }

    public void OpenChest()
    {
        switch (currentTemple)
        {
            case Temples.Fire:
                fireChests++;
                uiManager.UpdateChestAmount(fireChests, fireChestsAmount);
                break;
            case Temples.Ice:
                uiManager.UpdateChestAmount(iceChests, iceChestsAmount);
                iceChests++;
                break;
            case Temples.Wind:
                uiManager.UpdateChestAmount(windChests, windChestsAmount);
                windChests++;
                break;
        }
        
        SaveTemples();
    }

    public void ActivateRelic()
    {
        switch (currentTemple)
        {
            case Temples.Fire:
                fireRelics++;
                uiManager.UpdateRelicAmount(fireRelics, fireRelicsAmount);
                if (fireRelics >= fireRelicsAmount) UnlockBossRoom();
                break;
            case Temples.Ice:
                iceRelics++;
                uiManager.UpdateRelicAmount(iceRelics, iceRelicsAmount);
                if (iceRelics >= iceRelicsAmount) UnlockBossRoom();
                break;
            case Temples.Wind:
                windRelics++;
                uiManager.UpdateRelicAmount(windRelics, windRelicsAmount);
                if (windRelics >= windRelicsAmount) UnlockBossRoom();
                break;
        }
        SaveTemples();
    }

    private void UnlockBossRoom()
    {
        TempleGeneration.Instance.UnlockBossRoom();
    }

    public void FailTemple()
    {
        switch (currentTemple)
        {
            case Temples.Fire:
                fireRelics = 0;
                uiManager.UpdateRelicAmount(fireRelics, fireRelicsAmount);
                break;
            case Temples.Ice:
                iceRelics = 0;
                uiManager.UpdateRelicAmount(iceRelics, iceRelicsAmount);
                break;
            case Temples.Wind:
                windRelics = 0;
                uiManager.UpdateRelicAmount(windRelics, windRelicsAmount);
                break;
        }
        SaveTemples();
    }

    public void CompleteTemple()
    {
        switch (currentTemple)
        {
            case Temples.Fire:
                templesCleared[0] = true;
                break;
            case Temples.Ice:
                templesCleared[1] = true;
                break;
            case Temples.Wind:
                templesCleared[2] = true;
                break;
        }
        SaveTemples();
    }

    public void ResetTemples()
    {
        fireRelics = 0;
        iceRelics = 0;
        windRelics = 0;
        fireChests = 0;
        iceChests = 0;
        windChests = 0;
        
        templesCleared = new bool[3];
    }

    public void LoadTemples()
    {
        if (SaveGame.Exists(TEMPLE_DATA))
        {
            TempleData templeData = SaveGame.Load<TempleData>(TEMPLE_DATA);
            int[] relics = templeData.relics;
            int[] chests = templeData.chests;
            templesCleared = templeData.completed;
            
            fireRelics = relics[0];
            iceRelics = relics[1];
            windRelics = relics[2];
            
            fireChests = chests[0];
            iceChests = chests[1];
            windChests = chests[2];
        }
    }

    public void SaveTemples()
    {
        int[] relics = { fireRelics, iceRelics, windRelics };
        int[] chests = { fireChests, iceChests, windChests };
        
        TempleData templeData = new TempleData();
        templeData.relics = relics;
        templeData.chests = chests;
        templeData.completed = templesCleared;
        
        SaveGame.Save(TEMPLE_DATA, templeData);
    }
}
