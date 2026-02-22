using BayatGames.SaveGameFree;
using UnityEngine;

public class CoinManager : Singleton<CoinManager>
{
    [SerializeField] int startingCoins;
    public int Coins { get; private set; }
    private const string COIN_KEY = "Coins";

    private void UpdateCoinAmountUI()
    {
        UIManager.Instance.UpdateCoinAmount(Coins);
    }
    
    public void AddCoins(int amount)
    {
        Coins += amount;
        UpdateCoinAmountUI();
        SaveGame.Save(COIN_KEY, Coins);
    }

    public void RemoveCoins(int amount)
    {
        if (Coins >= amount)
        {
            Coins -= amount;
            UpdateCoinAmountUI();
            SaveGame.Save(COIN_KEY, Coins);
        }
    }
    
    public void LoadCoins(){
        Coins = SaveGame.Load<int>(COIN_KEY);
        UpdateCoinAmountUI();
    }

    public void ResetCoins()
    {
        Coins = startingCoins;
        UpdateCoinAmountUI();
        SaveGame.Save(COIN_KEY, Coins);
    }
    
}