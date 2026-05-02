using UnityEngine;

public class EnemyBoss : MonoBehaviour
{
    [SerializeField] private EnemyDetails boss;
    void Start()
    {
        boss.enemyCombatBrain = GetComponent<EnemyCombatBrain>();
    }
}
