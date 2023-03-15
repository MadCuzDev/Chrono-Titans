using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] ItemGrid mainInventoryItemGrid;
    [SerializeField] InventoryController inventoryController;

    public bool AddItem(ItemData itemData)
    {
        var positionToPlace = mainInventoryItemGrid.FindFirstSlot(itemData);

        if (positionToPlace == null) return false;
        
        var newItem = inventoryController.CreateNewInventoryItem(itemData);
        mainInventoryItemGrid.PlaceItem(newItem, positionToPlace.Value.x, positionToPlace.Value.y);

        return true;
    }
}
