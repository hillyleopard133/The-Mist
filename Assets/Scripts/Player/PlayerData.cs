using System;

[Serializable]
public class PlayerData
{ 
    public int Level;

    public float Health;
    public float MaxHealth;

    public float Mana;
    public float MaxMana;

    public float CurrentExp;
    public float NextLevelExp;
    public float ExpMultiplier;
    
    public float BaseDamage;
    public float CriticalChance;
    public float CriticalDamage;

    public int Strength;
    public int Dexterity;
    public int Intelligence;
    public int AttributePoints;

    public float TotalExp;
    public float TotalDamage;
}
