using System.Collections;
using System.Collections.Generic;
using BayatGames.SaveGameFree;
using UnityEngine;

public enum AttributeType
{
    Strength,
    Dexterity,
    Intelligence
}

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Player Stats")]
public class PlayerStats : ScriptableObject
{
    [Header("Config")]
    public int Level;

    [Header("Health")]
    public float Health;
    public float MaxHealth;
    public float InitialMaxHealth;

    [Header("Mana")]
    public float Mana;
    public float MaxMana;
    public float InitialMaxMana;

    [Header("Exp")]
    public float CurrentExp;
    public float NextLevelExp;
    public float InitialNextLevelExp;
    [Range(1f, 100f)] public float ExpMultiplier;
    
    [Header("Attack")]
    public float BaseDamage;
    public float CriticalChance;
    public float CriticalDamage;

    [Header("Attributes")] 
    public int Strength;
    public int Dexterity;
    public int Intelligence;
    public int AttributePoints;

    [HideInInspector] public float TotalExp;
    [HideInInspector] public float TotalDamage;
    
    private readonly string PLAYER_STATS = "PLAYER_STATS";

    public void SavePlayerStats()
    {
        PlayerData playerData = new PlayerData();
        
        playerData.Level = Level;
        playerData.Health = Health;
        playerData.MaxHealth = MaxHealth;
        playerData.CurrentExp = CurrentExp;
        playerData.NextLevelExp = NextLevelExp;
        playerData.ExpMultiplier = ExpMultiplier;
        playerData.BaseDamage = BaseDamage;
        playerData.CriticalChance = CriticalChance;
        playerData.CriticalDamage = CriticalDamage;
        playerData.AttributePoints = AttributePoints;
        playerData.TotalExp = TotalExp;
        playerData.TotalDamage = TotalDamage;
        playerData.Mana = Mana;
        playerData.MaxMana = MaxMana;
        playerData.Strength = Strength;
        playerData.Dexterity = Dexterity;
        playerData.Intelligence = Intelligence;
        
        SaveGame.Save(PLAYER_STATS, playerData);
    }

    public void LoadPlayerStats()
    {
        if (SaveGame.Exists(PLAYER_STATS))
        {
            PlayerData playerData = SaveGame.Load<PlayerData>(PLAYER_STATS);
            Level = playerData.Level;
            Health = playerData.Health;
            MaxHealth = playerData.MaxHealth;
            CurrentExp = playerData.CurrentExp;
            NextLevelExp = playerData.NextLevelExp;
            ExpMultiplier = playerData.ExpMultiplier;
            BaseDamage = playerData.BaseDamage;
            CriticalChance = playerData.CriticalChance;
            CriticalDamage = playerData.CriticalDamage;
            AttributePoints = playerData.AttributePoints;
            TotalExp = playerData.TotalExp;
            TotalDamage = playerData.TotalDamage;
            Mana = playerData.Mana;
            MaxMana = playerData.MaxMana;
            Strength = playerData.Strength;
            Dexterity = playerData.Dexterity;
            Intelligence = playerData.Intelligence;
        }
    }

    public void ResetPlayer()
    {
        MaxHealth = InitialMaxHealth;
        MaxMana = InitialMaxMana;
        Health = MaxHealth;
        Mana = MaxMana;
        Level = 1;
        CurrentExp = 0;
        NextLevelExp = InitialNextLevelExp;
        TotalExp = 0f;
        BaseDamage = 2;
        CriticalChance = 10;
        CriticalDamage = 50;
        Strength = 0;
        Intelligence = 0;
        Dexterity = 0;
        AttributePoints = 0;
        SavePlayerStats();
    }

}
