using UnityEngine;

public class PlayerMana : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private PlayerStats stats;
    
    public float CurrentMana {get; private set;}

    private void Start()
    {
        ResetMana();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            UseMana(1f);
        }
    }

    public void UseMana(float amount)
    {
        //This method chooses the largest of 2 numbers. (if mana goes below 0 it will choose 0)
        stats.Mana = Mathf.Max(stats.Mana -= amount, 0);
        CurrentMana = stats.Mana;
    }

    public void RecoverMana(float amount)
    {
        stats.Mana += amount;
        //Alternative way to do it than was used in health potion
        stats.Mana = Mathf.Min(stats.Mana, stats.MaxMana);
    }

    public bool CanRecoverMana()
    {
        return stats.Mana >= 0 && stats.Mana < stats.MaxMana;
    }

    public void ResetMana()
    {
        CurrentMana = stats.MaxMana;
    }

}
