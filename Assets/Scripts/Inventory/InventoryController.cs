using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public ItemGrid selectedItemGrid;
    private EquipmentItemSlot selectedItemSlot;

    private Vector2Int posOnGrid;
    private InventoryItem selectedItem;
    private RectTransform selectedItemRectTransform;

    [SerializeField] private List<ItemData> itemDatas;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private Transform targetCanvas;

    [SerializeField] private RectTransform selectedItemParent;

    public EquipmentItemSlot SelectedItemSlot
    {
        get => selectedItemSlot;
        set
        {
            selectedItemSlot = value;
        }
    }

    public ItemGrid SelectedItemGrid
    {
        get => selectedItemGrid;
        set
        {
            selectedItemGrid = value;
        }
        
    }
    
    private void Update()
    {
        ProcessMouseInput();

        if (selectedItemGrid == null) return;

        if (Input.GetKeyDown(KeyCode.Z))
        {
            CreateRandomItem();
        }
        
        if (Input.GetKeyDown(KeyCode.X))
        {
            InsertRandomItem();
        }
    }

    public void InsertRandomItem()
    {
        if (selectedItemGrid == null) return;
        
        CreateRandomItem();
        var itemToInsert = selectedItem;
        selectedItem = null;
        InsertItem(itemToInsert);
    }

    private void InsertItem(InventoryItem itemToInsert)
    {
        var posOnGrid = selectedItemGrid.FindFirstSlot();

        if (posOnGrid == null) return;
        
        selectedItemGrid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
    }

    private void CreateRandomItem()
    {
        if (selectedItem != null) return;
        var selectedItemID = UnityEngine.Random.Range(0, itemDatas.Count);
        var newItem = CreateNewInventoryItem(itemDatas[selectedItemID]);
        SelectItem(newItem);
    }

    public InventoryItem CreateNewInventoryItem(ItemData itemData)
    {
        var newItem = Instantiate(inventoryItemPrefab);

        var newInventoryItem = newItem.GetComponent<InventoryItem>();

        var newItemRectTransform = newItem.GetComponent<RectTransform>();
        newItemRectTransform.SetParent(targetCanvas);
        
        newInventoryItem.Set(itemData);

        return newInventoryItem;
    }
    public void SelectItem(InventoryItem inventoryItem)
    {
        selectedItem = inventoryItem;
        selectedItemRectTransform = inventoryItem.GetComponent<RectTransform>();
        selectedItemRectTransform.SetParent(selectedItemParent);
    }

    private void ProcessMouseInput()
    {
        if (selectedItem != null)
        {
            selectedItemRectTransform.position = Input.mousePosition;
        }

        if (selectedItemGrid == null && selectedItemSlot == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (selectedItemGrid != null)
            {
                ItemGridInput();
            }

            if (selectedItemSlot != null)
            {
                ItemSlotInput();
            }
        }
    }

    private void ItemSlotInput()
    {
        if (selectedItem != null)
        {
            PlaceItemIntoSlot();
        }
    }

    private void PlaceItemIntoSlot()
    {
        if (!selectedItemSlot.Check(selectedItem)) return;
        
        var replacedItem = selectedItemSlot.ReplaceItem(selectedItem);

        if (replacedItem == null)
        {
            NullSelectedItem();
        }
        else
        {
            SelectItem(replacedItem);
        }
    }

    private void NullSelectedItem()
    {
        selectedItem = null;
        selectedItemRectTransform = null;
    }

    private void ItemGridInput()
    {
        posOnGrid = selectedItemGrid.GetTileGridPos(Input.mousePosition);
        if (selectedItem == null)
        {
            if (!selectedItemGrid.ContainsItem(posOnGrid.x, posOnGrid.y)) return;
            selectedItem = selectedItemGrid.PickUpItem(posOnGrid);
            SelectItem(selectedItem);
        }
        else
        {
            if (selectedItemGrid.ContainsItem(posOnGrid.x, posOnGrid.y)) return;
            selectedItemGrid.PlaceItem(selectedItem, posOnGrid.x, posOnGrid.y);
            NullSelectedItem();
        }
    }
}
