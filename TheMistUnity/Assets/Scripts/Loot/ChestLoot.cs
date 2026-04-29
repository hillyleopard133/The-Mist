using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu (menuName = "ScriptableObjects/ChestLoot", fileName = "Loot")]
public class ChestLoot : ScriptableObject
{
    public int minCoins;
    public int maxCoins;
    public InventoryItem[] items;
    public bool giveAllItems;
    public int maxItems;

    public int GetCoins()
    {
        return Random.Range(minCoins, maxCoins + 1);
    }

    public InventoryItem[] GetLoot()
    {
        if (giveAllItems) return items.ToArray();
        if(items.Length <= maxItems) return items.ToArray();

        List<InventoryItem> itemsList = items.ToList();
        List<InventoryItem> loot = new List<InventoryItem>();
        for (int i = 0; i < maxItems; i++)
        {
            int index = Random.Range(0, itemsList.Count);
            loot.Add(itemsList[index]);
            itemsList.RemoveAt(index);
        }
        return loot.ToArray();
    }
}