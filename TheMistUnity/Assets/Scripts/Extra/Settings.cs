using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    //AStar Pathfinding
    public const int defaultAStarMovementPenalty = 40;
    public const int preferredPathAStarMovementPenalty = 1;
    public const float playerMoveDistanceToRebuildPath = 1f;
    public const float enemyPathRebuildCooldown = 2f;
    public const int targetFrameRateToSpreadPathFindingOver = 60;

    public const int defaultEnemyRespawnTime = 300;
}
