using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ItemGrid : MonoBehaviour
{
    private InventoryItem[,] inventoryItemGrid;
    
    private const float TileSizeWidth = 32f;
    private const float TileSizeHeight = 32f;

    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;

    private RectTransform rectTransform;

    private Vector2 mousePosOnGrid;
    private Vector2Int tileGridPos = new Vector2Int();

    [SerializeField] private GameObject inventoryItemPrefab;

    private void CreateTestItem(int x, int y)
    {
        rectTransform = GetComponent<RectTransform>();
        var itemTest = Instantiate(inventoryItemPrefab);
        var inventoryTestItem = itemTest.GetComponent<InventoryItem>();
        PlaceItem(inventoryTestItem, x, y);
    }

    public void Init()
    {
        rectTransform = GetComponent<RectTransform>();
        inventoryItemGrid = new InventoryItem[gridWidth, gridHeight];
        var size = new Vector2
        {
            x = TileSizeWidth * gridWidth,
            y = TileSizeHeight * gridHeight
        };
        rectTransform.sizeDelta = size;
    }

    public void PlaceItem(InventoryItem itemToPlace, int x, int y)
    {
        var itemRectTransform = itemToPlace.GetComponent<RectTransform>();
        itemRectTransform.SetParent(transform);

        inventoryItemGrid[x, y] = itemToPlace;
        
        var posOnGrid = new Vector2
        {
            x = TileSizeWidth * x + TileSizeWidth / 2,
            y = -(TileSizeHeight * y + TileSizeHeight / 2)
        };
        itemRectTransform.localPosition = posOnGrid;
    }

    public Vector2Int? FindFirstSlot()
    {
        for (var y = 0; y < gridHeight; y++)
        {
            for (var x = 0; x < gridWidth; x++)
            {
                if (!ContainsItem(x, y)) return new Vector2Int(x, y);
            }
        }

        return null;
    }

    public bool ContainsItem(int x, int y)
    {
        return inventoryItemGrid.GetValue(x, y) != null;
    }

    public Vector2Int GetTileGridPos(Vector2 mousePosition)
    {
        mousePosOnGrid.x = mousePosition.x - rectTransform.position.x;
        mousePosOnGrid.y = rectTransform.position.y - mousePosition.y;

        tileGridPos.x = (int)(mousePosOnGrid.x / TileSizeWidth);
        tileGridPos.y = (int)(mousePosOnGrid.y / TileSizeHeight);

        return tileGridPos;
    }

    internal InventoryItem PickUpItem(Vector2Int tilePosOnGrid)
    {
        var pickedItem = inventoryItemGrid[tilePosOnGrid.x, tilePosOnGrid.y];
        inventoryItemGrid[tilePosOnGrid.x, tilePosOnGrid.y] = null;
        return pickedItem;
    }
}
