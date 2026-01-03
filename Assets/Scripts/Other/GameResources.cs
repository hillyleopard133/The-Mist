using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "GameResources", menuName = "Scriptable Objects/GameResources")]
public class GameResources : ScriptableObject
{
    private static GameResources instance;
    public static GameResources Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<GameResources>("GameResources");
            }
            return instance;
        }
    }
    
    [Header("Procedural Generation")]
    public const int maxChildCorridors = 3;
    public const int maxTempleRebuildAttemptsForRoomGraph = 1000;
    public const int maxTempleBuildAttempts = 10;
    public RoomNodeTypeList roomNodeTypeList;
}