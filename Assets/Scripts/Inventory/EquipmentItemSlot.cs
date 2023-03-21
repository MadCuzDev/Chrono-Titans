using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentItemSlot : MonoBehaviour
{
    [SerializeField] private EquipmentType equipmentType;

    InventoryItem itemInSlot;

    private RectTransform slotRectTransform;

    private void Awake()
    {
        slotRectTransform = GetComponent<RectTransform>();
    }

    public bool Check(InventoryItem itemToPlace)
    {
        return equipmentType == itemToPlace.itemData.equipmentType;
    }

    public InventoryItem ReplaceItem(InventoryItem itemToPlace)
    {
        var replaceItem = itemInSlot;
        
        PlaceItem(itemToPlace);

        return replaceItem;
    }

    public void PlaceItem(InventoryItem itemToPlace)
    {
        itemInSlot = itemToPlace;
        var rectTransform = itemToPlace.GetComponent< RectTransform>();
        rectTransform.SetParent(slotRectTransform);
        rectTransform.position = slotRectTransform.position;
    }
}
