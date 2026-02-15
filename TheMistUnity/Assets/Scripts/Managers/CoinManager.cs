using BayatGames.SaveGameFree;
using UnityEngine;

public class CoinManager : Singleton<CoinManager>
{
    [SerializeField] float startingCoins;
    public float Coins { get; private set; }
    private const string COIN_KEY = "Coins";

    
    protected override void Awake()
    {
        base.Awake(); 
    }

    public void AddCoins(float amount)
    {
        Coins += amount;
        SaveGame.Save(COIN_KEY, Coins);
    }

    public void RemoveCoins(float amount)
    {
        if (Coins >= amount)
        {
            Coins -= amount;
            SaveGame.Save(COIN_KEY, Coins);
        }
    }
    
    public void LoadCoins(){
        Coins = SaveGame.Load<float>(COIN_KEY);
    }

    public void ResetCoins()
    {
        Coins = startingCoins;
        SaveGame.Save(COIN_KEY, Coins);
    }
    
}