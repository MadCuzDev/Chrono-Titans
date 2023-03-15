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
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        Init(gridWidth, gridHeight);
        CreateTestItem(7, 0);
        CreateTestItem(0, 0);
        CreateTestItem(3, 3);
        CreateTestItem(4, 4);
    }

    private void CreateTestItem(int x, int y)
    {
        var itemTest = Instantiate(inventoryItemPrefab);
        var inventoryTestItem = itemTest.GetComponent<InventoryItem>();
        PlaceItem(inventoryTestItem, x, y);
    }

    private void Init(int width, int height)
    {
        inventoryItemGrid = new InventoryItem[width, height];
        var size = new Vector2
        {
            x = TileSizeWidth * width,
            y = TileSizeHeight * height
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

    public Vector2Int? FindFirstSlot(ItemData itemData)
    {
        var height = gridHeight - itemData.sizeHeight + 1;
        var width = gridWidth - itemData.sizeWidth + 1;

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                if (CheckAvailableSpace(x, y, itemData.sizeWidth, itemData.sizeHeight))
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        return null;
    }

    private bool CheckAvailableSpace(int posX, int posY, int sizeWidth, int sizeHeight)
    {
        for (var x = 0; x < sizeWidth; x++)
        {
            for (var y = 0; y < sizeHeight; y++)
            {
                if (inventoryItemGrid[posX + x, posY + y] != null)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public bool ContainsItem(int x, int y)
    {
        return inventoryItemGrid.GetValue(x, y) != null;
    }

    public Vector2Int GetTileGridPos(Vector2 mousePosition)
    {
        mousePosOnGrid.x = mousePosition.x - rectTransform.position.x;
        mousePosOnGrid.y = rectTransform.position.y - mousePosition.y;

        tileGridPos.Set((int)(mousePosOnGrid.x / TileSizeWidth), (int)(mousePosOnGrid.y / TileSizeHeight));

        return tileGridPos;
    }

    internal InventoryItem PickUpItem(Vector2Int tilePosOnGrid)
    {
        var pickedItem = inventoryItemGrid[tilePosOnGrid.x, tilePosOnGrid.y];
        inventoryItemGrid[tilePosOnGrid.x, tilePosOnGrid.y] = null;
        return pickedItem;
    }
}
