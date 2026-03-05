using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Items/Scroll", fileName = "ItemScroll")]
public class ItemScroll : ItemEquipment
{
    public int mana;
    public DamageType scrollDamageType;
    public AttackMove[] Attacks;
}
