using System.Collections;
using System.Collections.Generic;
using BayatGames.SaveGameFree;
using UnityEngine;

public class NPCFollowerManager : Singleton<NPCFollowerManager>
{
    [SerializeField] private GameObject[] npcs;
    protected override void Awake()
    {
        base.Awake(); 
    }

    private void Start()
    {
        //InstantiateAppropriatePrefabs();
    }

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
        foreach (GameObject npc in npcs)
        {
            string isFollowing = "IS_FOLLOWING" + npc.name;
            if (SaveGame.Exists(isFollowing))
            {
                if (SaveGame.Load<bool>(isFollowing))
                {
                    GameObject newNpc = Instantiate(npc, transform);
                    newNpc.name = npc.name;
                }
            }
        }
    }
}