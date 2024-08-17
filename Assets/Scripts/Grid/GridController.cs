using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridController : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap buildingTilemap;

    private GridCell[,] cells = new GridCell[10,20];

    private Vector2Int GetTile()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return (Vector2Int)grid.WorldToCell(mouseWorldPos);
    }
}
