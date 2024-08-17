using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class GridController : MonoBehaviour
{
    [SerializeField] private Grid _grid;
    [SerializeField] private Tilemap _groundTilemap;
    [SerializeField] private Tilemap _buildingTilemap;
    [SerializeField] private Tilemap _roadTilemap;
    [SerializeField] private GameObject _blocker;
    [SerializeField] private GameObject _buildWheel;
    [Header("Region Types")]
    [SerializeField] private RegionClass _classA;
    [SerializeField] private RegionClass _classB;
    [SerializeField] private RegionClass _classC;

    public RegionClass ClassA => _classA;
    public RegionClass ClassB => _classB;
    public RegionClass ClassC => _classC;
    public GridCell[,] Cells { get; set; } = new GridCell[20, 10];


    public void SetBuilding(Vector2Int pos, Building building)
    {
        SetBuilding(Cells[pos.x, pos.y], building);
    }

    public void SetBuilding(GridCell cell, Building building)
    {
        cell.ConstructedBuilding = building;
        _buildingTilemap.SetTile((Vector3Int)cell.Position, building.BuildingInformation.Tile);
    }

    public bool CanBuild(Vector2Int pos, Building building)
    {
        return Cells[pos.x,pos.y].CellType == GridCell.CellTypes.Buildable && Cells[pos.x, pos.y].ConstructedBuilding == null;
    }

    private void Start()
    {
        _groundTilemap.GetComponent<DetectGroundClick>().OnGroundClick += HandleGroundClick;
        for (int x = 0; x < Cells.GetLength(0); x++)
        {
            for (int y = 0; y < Cells.GetLength(1); y++)
            {
                Cells[x,y] = new GameObject("Grid Cell").AddComponent<GridCell>();
                Cells[x,y].Position = new Vector2Int(x, y);
            }
        }
    }


    private Vector2Int GetTilePos(Vector2 worldPosition)
    {
        Vector2Int gridPos = (Vector2Int)_grid.WorldToCell(worldPosition);
        gridPos.x = Mathf.Clamp(gridPos.x, 0, Cells.GetLength(0) - 1);
        gridPos.y = Mathf.Clamp(gridPos.y, 0, Cells.GetLength(1) - 1);
        return gridPos;
    }

    private void HandleGroundClick(Vector2 mousePosition)
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Vector2Int gridPos = GetTilePos(mousePosition);
            Debug.Log(gridPos);

            if (Cells[gridPos.x, gridPos.y].CellType == GridCell.CellTypes.Buildable)
            {
                UIManager.Instance.OpenBuildMenu(Cells[gridPos.x, gridPos.y]);
            }
        }
    }
}
