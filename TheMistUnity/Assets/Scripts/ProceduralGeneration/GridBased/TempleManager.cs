using System;
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
    
    private bool[] templesCleared = new bool[3];
    
    public Temples currentTemple;

    public void ActivateRelic()
    {
        switch (currentTemple)
        {
            case Temples.Fire:
                fireRelics++;
                if (fireRelics >= fireRelicsAmount) UnlockBossRoom();
                break;
            case Temples.Ice:
                iceRelics++;
                if (iceRelics >= iceRelicsAmount) UnlockBossRoom();
                break;
            case Temples.Wind:
                windRelics++;
                if (windRelics >= windRelicsAmount) UnlockBossRoom();
                break;
        }
    }

    private void UnlockBossRoom()
    {
        TempleGeneration.Instance.UnlockBossRoom();
    }

    private void FailTemple()
    {
        fireRelics = 0;
        iceRelics = 0;
        windRelics = 0;
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
    }
}
