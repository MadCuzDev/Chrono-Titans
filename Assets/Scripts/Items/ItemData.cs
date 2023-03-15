using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    None,
    Weapons,
    Offhand,
    Armour,
    Helmet,
    Belt,
    Boots,
    Ring,
    Amulet
}

[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    public EquipmentType equipmentType;
    public int sizeWidth = 1;
    public int sizeHeight = 1;

    public Sprite icon;
}
