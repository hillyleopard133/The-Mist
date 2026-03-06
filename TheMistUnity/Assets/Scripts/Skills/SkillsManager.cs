using System;
using System.Collections;
using System.Collections.Generic;
using BayatGames.SaveGameFree;
using UnityEngine;

public enum AttributeType
{
    Health,
    Defence,
    Attack,
    CritChance,
    Mana
}

public class SkillsManager : Singleton<SkillsManager>
{
    [HideInInspector] public int Level;
    private int currentExp;
    private int nextLevelExp;
    [SerializeField] [Range(1f, 100f)] public float ExpMultiplier;
    [SerializeField] private int firstLevelExp;

    [SerializeField] public int healthIncreasePerLevel;
    [SerializeField] public int defenceIncreasePerLevel;
    [SerializeField] public int attackIncreasePerLevel;
    [SerializeField] public int critIncreasePerLevel;
    [SerializeField] public int manaIncreasePerLevel;
    
    [SerializeField] public PartyMember[] partyMembers;
    [HideInInspector] public int[] availableAttributePoints;
    [HideInInspector] public int[] pendingAttributePoints;
    
    [HideInInspector] public int[] pointsToAddHealth;
    [HideInInspector] public int[] pointsToAddDefence;
    [HideInInspector] public int[] pointsToAddAttack;
    [HideInInspector] public int[] pointsToAddCritChance;
    [HideInInspector] public int[] pointsToAddMana;
    
    [SerializeField] public Skill[] skills;
    [HideInInspector] public int skillOrbs;
    
    private UIManager uIManager;
    private CombatManager combatManager;
    
    private readonly string PARTY_DATA = "PARTY_DATA";
    private readonly string ATTRIBUTE_POINTS = "ATTRIBUTE_POINTS";
    private readonly string SKILL_ORBS = "SKILL_ORBS";
    private readonly string SKILLS_UNLOCKED = "SKILLS_UNLOCKED";
    private readonly string PLAYER_LEVEL_DATA = "PLAYER_LEVEL_DATA";

    private void Start()
    {
        uIManager = UIManager.Instance;
        combatManager = CombatManager.Instance;
        nextLevelExp = firstLevelExp;
        
        availableAttributePoints = new int[partyMembers.Length];
        pendingAttributePoints = new int[partyMembers.Length];
        
        pointsToAddHealth = new int[partyMembers.Length];
        pointsToAddDefence = new int[partyMembers.Length];
        pointsToAddAttack = new int[partyMembers.Length];
        pointsToAddCritChance = new int[partyMembers.Length];
        pointsToAddMana = new int[partyMembers.Length];
    }

    public List<PartyMember> GetUnlockedPartyMembers()
    {
        List<PartyMember> unlockedPartyMembers = new List<PartyMember>();

        foreach (PartyMember partyMember in partyMembers)
        {
            if (partyMember.IsUnlocked) unlockedPartyMembers.Add(partyMember);
        }
        
        return unlockedPartyMembers;
    }

    public Skill GetSkill(SkillTreeSkills skillTreeSkill)
    {
        foreach (Skill skill in skills)
        {
            if(skill.SkillTreeSkill == skillTreeSkill) return skill;
        }
        return null;
    }

    public int GetSkillIndex(Skill skill)
    {
        return Array.IndexOf(skills,skill);
    }

    public void UnlockSkill(Skill skill)
    {
        skill.IsUnlocked = true;
        skillOrbs -= skill.OrbCost;
        SaveSkills();
    }

    public void AddSkillOrb(int amount)
    {
        skillOrbs += amount;
        uIManager.UpdateSkillOrbsAmount();
    }

    public float GetExpBarPercentage()
    {
        return (float)currentExp / nextLevelExp;
    }

    public void AddExp(int exp)
    {
        currentExp += exp;
        
        //Needs to be while incase take multiple lvls in one go
        while (currentExp >= nextLevelExp)
        {
            currentExp -= nextLevelExp;
            LevelUp();
        }
        
        uIManager.UpdateLevelBar();
        SaveSkills();
    }
    
    private void LevelUp()
    {
        Level++;
        for (int i = 0; i < partyMembers.Length; i++)
        {
            availableAttributePoints[i]++;
        }
        
        int newNextLevelExp = (int)Mathf.Round(nextLevelExp * (1 + ExpMultiplier / 100f));
        nextLevelExp = newNextLevelExp;

        combatManager.LevelUp();
    }

    public bool HasSkillPointsLeft(int partyMemberIndex)
    {
        return availableAttributePoints[partyMemberIndex] - pendingAttributePoints[partyMemberIndex] > 0;
    }

    public void ApplyPoints(int partyMemberIndex)
    {
        partyMembers[partyMemberIndex].IncreaseSkillPoints(AttributeType.Health, pointsToAddHealth[partyMemberIndex]);
        partyMembers[partyMemberIndex].IncreaseSkillPoints(AttributeType.Defence, pointsToAddDefence[partyMemberIndex]);
        partyMembers[partyMemberIndex].IncreaseSkillPoints(AttributeType.Attack, pointsToAddAttack[partyMemberIndex]);
        partyMembers[partyMemberIndex].IncreaseSkillPoints(AttributeType.CritChance, pointsToAddCritChance[partyMemberIndex]);
        partyMembers[partyMemberIndex].IncreaseSkillPoints(AttributeType.Mana, pointsToAddMana[partyMemberIndex]);
        
        combatManager.AddHealth(partyMemberIndex, pointsToAddHealth[partyMemberIndex] * healthIncreasePerLevel);
        combatManager.AddMana(partyMemberIndex, pointsToAddMana[partyMemberIndex] * manaIncreasePerLevel);
        uIManager.UpdatePartyMemberInfo();

        availableAttributePoints[partyMemberIndex] -= pendingAttributePoints[partyMemberIndex];
        pendingAttributePoints[partyMemberIndex] = 0;

        pointsToAddHealth[partyMemberIndex] = 0;
        pointsToAddDefence[partyMemberIndex] = 0;
        pointsToAddAttack[partyMemberIndex] = 0;
        pointsToAddCritChance[partyMemberIndex] = 0;
        pointsToAddMana[partyMemberIndex] = 0;
        
        UIManager.Instance.UpdateSkillCards();
        SaveSkills();
    }

    public void ClearPendingPoints()
    {
        pendingAttributePoints = new int[partyMembers.Length];
        pointsToAddHealth = new int[partyMembers.Length];
        pointsToAddDefence = new int[partyMembers.Length];
        pointsToAddAttack = new int[partyMembers.Length];
        pointsToAddCritChance = new int[partyMembers.Length];
        pointsToAddMana = new int[partyMembers.Length];
        
        UIManager.Instance.UpdateSkillCards();
    }
    
    private void AttributeCallback(AttributeType attributeType, int partyMemberIndex, bool isIncrease)
    {
        switch (attributeType)
        {
            case AttributeType.Health:
                pointsToAddHealth[partyMemberIndex] += isIncrease ? 1 : -1;
                break;
            case AttributeType.Defence:
                pointsToAddDefence[partyMemberIndex] += isIncrease ? 1 : -1;
                break;
            case AttributeType.Attack:
                pointsToAddAttack[partyMemberIndex] += isIncrease ? 1 : -1;
                break;
            case AttributeType.CritChance:
                pointsToAddCritChance[partyMemberIndex] += isIncrease ? 1 : -1;
                break;
            case AttributeType.Mana:
                pointsToAddMana[partyMemberIndex] += isIncrease ? 1 : -1;
                break;
        }

        pendingAttributePoints[partyMemberIndex] += isIncrease ? 1 : -1;
        
        UIManager.Instance.UpdateSkillCards();
    }

    public void ResetSkills()
    {
        foreach (PartyMember partyMember in partyMembers)
        {
            partyMember.ResetPartyMember();
        }
        partyMembers[0].UnlockPartyMember();

        Level = 0;
        currentExp = 0;
        nextLevelExp = firstLevelExp;

        foreach (Skill skill in skills)
        {
            skill.IsUnlocked = false;
        }
        skillOrbs = 0;
        
        SaveSkills();
    }

    public void LoadSkills()
    {
        if (SaveGame.Exists(PARTY_DATA))
        {
            PartyMemberData[] partyData  = SaveGame.Load<PartyMemberData[]>(PARTY_DATA);
            
            for(int i = 0; i < partyMembers.Length; i++)
            {
                partyMembers[i].SetData(partyData[i]);
            }
        }

        if (SaveGame.Exists(ATTRIBUTE_POINTS)) availableAttributePoints = SaveGame.Load<int[]>(ATTRIBUTE_POINTS);
        if (SaveGame.Exists(SKILL_ORBS)) skillOrbs = SaveGame.Load<int>(SKILL_ORBS);

        if (SaveGame.Exists(SKILLS_UNLOCKED))
        {
            bool[] skillsUnlocked = SaveGame.Load<bool[]>(SKILLS_UNLOCKED);
            for (int i = 0; i < skillsUnlocked.Length; i++)
            {
                skills[i].IsUnlocked = skillsUnlocked[i];
            }
        }
        
        if (SaveGame.Exists(PLAYER_LEVEL_DATA))
        {
            PlayerLevelData levelData = SaveGame.Load<PlayerLevelData>(PLAYER_LEVEL_DATA);
            Level = levelData.level;
            currentExp = levelData.currentExp;
            nextLevelExp = levelData.nextLevelExp;
        }
    }

    public void SaveSkills()
    {
        PartyMemberData[] partyData = new PartyMemberData[partyMembers.Length];

        for(int i = 0; i < partyMembers.Length; i++)
        {
            partyData[i] = partyMembers[i].GetData();
        }
        SaveGame.Save(PARTY_DATA, partyData);
        
        SaveGame.Save(ATTRIBUTE_POINTS, availableAttributePoints);
        SaveGame.Save(SKILL_ORBS, skillOrbs);
        
        bool[] isSkillUnlocked = new bool[skills.Length];
        for (int i = 0; i < skills.Length; i++)
        {
            isSkillUnlocked[i] = skills[i].IsUnlocked;
        }
        SaveGame.Save(SKILLS_UNLOCKED, isSkillUnlocked);
        
        SaveGame.Save(PLAYER_LEVEL_DATA, new PlayerLevelData(Level, currentExp, nextLevelExp));
    }

    private void OnEnable()
    {
        AttributeButton.OnAttributeSelectedEvent += AttributeCallback;
    }

    private void OnDisable()
    {
        AttributeButton.OnAttributeSelectedEvent -= AttributeCallback;
        
    }
}
