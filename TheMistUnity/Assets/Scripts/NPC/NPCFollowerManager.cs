using System.Collections;
using System.Collections.Generic;
using BayatGames.SaveGameFree;
using UnityEngine;

public class NPCFollowerManager : Singleton<NPCFollowerManager>
{
    [SerializeField] private GameObject[] npcs;

    public void ResetFollowing()
    {
        foreach (GameObject npc in npcs)
        {
            string isFollowing = "IS_FOLLOWING" + npc.name;
            if (SaveGame.Exists(isFollowing))
            {
                SaveGame.Delete(isFollowing);
            }
        }
        
        foreach (Transform npc in this.transform)
        {
            Destroy(npc.gameObject);
        }
    }

    public void InstantiateAppropriateNPCPrefabs()
    {
        for(int i = 0; i < npcs.Length; i++)
        {
            string isFollowing = "IS_FOLLOWING" + npcs[i].name;
            if (SaveGame.Exists(isFollowing))
            {
                if (SaveGame.Load<bool>(isFollowing))
                {
                    GameObject newNpc = Instantiate(npcs[i], transform);
                    newNpc.name = npcs[i].name;
                    if (i < 2)
                    {
                        SkillsManager.Instance.partyMembers[i + 1].UnlockPartyMember();
                    }
                }
            }
        }
        DialogueManager.Instance.SelectNPC(null);
    }
}