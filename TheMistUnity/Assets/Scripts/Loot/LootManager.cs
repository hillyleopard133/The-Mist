using UnityEngine;

public class LootManager : Singleton<LootManager>
{
    [Header("Config")]
    [SerializeField] private GameObject lootPanel;
    [SerializeField] private LootButton lootButtonPrefab;
    [SerializeField] private Transform container;

    [HideInInspector] public EnemyLoot CurrentEnemyLoot;

    public void ShowLoot(EnemyLoot enemyLoot)
    {
        UIManager.Instance.CloseAllPanels();
        lootPanel.SetActive(true);
        CurrentEnemyLoot = enemyLoot;
        if (LootPanelItemCount() > 0)
        {
            for (int i = 0; i < LootPanelItemCount(); i++)
            {
                Destroy(container.GetChild(i).gameObject);
            }
        }

        foreach (DropItem item in enemyLoot.Items)
        {
            if (item.PickedItem)
            {
                continue;
            }
            LootButton lootButton = Instantiate(lootButtonPrefab, container);
            lootButton.ConfigLootButton(item);
        }

    }

    public void TakeAllLoot()
    {
        for (int i = 0; i < LootPanelItemCount(); i++)
        {
            container.GetChild(i).GetComponent<LootButton>().CollectItem();
            ClosePanel();
            Destroy(CurrentEnemyLoot.gameObject);
        }
    }
    
    public void ClosePanel()
    {
        lootPanel.SetActive(false);
    }

    public int LootPanelItemCount()
    {
        return container.childCount;
    }

}