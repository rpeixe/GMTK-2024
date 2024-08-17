using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridController : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap buildingTilemap;
    public GridCell[,] cells = new GridCell[10,20];



    private Vector2Int GetTilePos()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return (Vector2Int)groundTilemap.WorldToCell(mouseWorldPos);
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2Int tilePos = GetTilePos();
            GridCell cell = cells[tilePos.x, tilePos.y];
            Debug.Log($"Cell = {tilePos}");
        }
    }

}
