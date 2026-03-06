using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CombatGridType
{
    DarkGrass,
    Bedroom,
    Temple
}

public class EnemyArea : MonoBehaviour
{
    [SerializeField] private float padding;
    [SerializeField] private CombatGridType gridType;
    private BoxCollider2D spawnArea;
    private List<EnemyDetails> enemies = new List<EnemyDetails>();

    [HideInInspector] public bool hasEntered;
    
    private CombatManager combatManager;

    private void Awake()
    {
        spawnArea = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        combatManager = CombatManager.Instance;
    }

    public void ClearEnemies()
    {
        enemies.Clear();
    }

    public void AddEnemy(EnemyDetails enemy)
    {
        enemies.Add(enemy);
    }
    
    private void StartCombat()
    {
        combatManager.EnterCombat(enemies, gridType);
        hasEntered = true;
    }

    public void SpawnEnemies()
    {
        foreach (EnemyDetails enemy in enemies)
        {
            Vector2 randomPos = GetRandomPositionInsideBox();
            Instantiate(enemy.enemyPrefab, randomPos, Quaternion.identity, transform);
        }
    }
    
    private Vector2 GetRandomPositionInsideBox()
    {
        Vector2 center = spawnArea.bounds.center;
        Vector2 size = spawnArea.bounds.size;

        float minX = center.x - size.x / 2f + padding;
        float maxX = center.x + size.x / 2f - padding;
        float minY = center.y - size.y / 2f + padding;
        float maxY = center.y + size.y / 2f - padding;

        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        return new Vector2(randomX, randomY);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if(!hasEntered) StartCombat();
        }
    }
}
