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
    [SerializeField] private GameObject _blocker;
    [SerializeField] private GameObject _buildWheel;
    [Header("Tilemaps")]
    [SerializeField] private Tilemap _groundTilemap;
    [SerializeField] private Tilemap _buildingTilemap;
    [SerializeField] private Tilemap _roadTilemap;
    [SerializeField] private Tilemap _targetTilemap;
    [SerializeField] private Tilemap _classATilemap;
    [SerializeField] private Tilemap _classBTilemap;
    [SerializeField] private Tilemap _classCTilemap;
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

    public void SetTarget(Vector2Int pos, TileBase tile)
    {
        _targetTilemap.SetTile((Vector3Int)pos, tile);
    }

    public bool CanBuild(Vector2Int pos, Building building)
    {
        return Cells[pos.x,pos.y].CellType == GridCell.CellTypes.Buildable && Cells[pos.x, pos.y].ConstructedBuilding == null;
    }

    public void SetTileColor(Vector2Int pos, Color color)
    {
        _buildingTilemap.SetTileFlags((Vector3Int)pos, TileFlags.None);
        _buildingTilemap.SetColor((Vector3Int)pos, color);
    }

    public void SetGroundTileColor(Vector2Int pos, Color color)
    {
        _groundTilemap.SetTileFlags((Vector3Int)pos, TileFlags.None);
        _groundTilemap.SetColor((Vector3Int)pos, color);
    }

    private void Start()
    {
        _groundTilemap.GetComponent<DetectGroundClick>().OnGroundClick += HandleGroundClick;
        _groundTilemap.GetComponent<DetectGroundClick>().OnGroundClick += HandleBuildingClick;
        for (int x = 0; x < Cells.GetLength(0); x++)
        {
            for (int y = 0; y < Cells.GetLength(1); y++)
            {
                Cells[x,y] = new GameObject("Grid Cell").AddComponent<GridCell>();
                Cells[x,y].Position = new Vector2Int(x, y);

                if (_classATilemap.GetTile(new Vector3Int(x, y)))
                {
                    Cells[x,y].CellType = GridCell.CellTypes.Buildable;
                    Cells[x,y].RegionClass = _classA;
                }
                else if (_classBTilemap.GetTile(new Vector3Int(x, y)))
                {
                    Cells[x, y].CellType = GridCell.CellTypes.Buildable;
                    Cells[x, y].RegionClass = _classB;
                }
                else if (_classCTilemap.GetTile(new Vector3Int(x, y)))
                {
                    Cells[x, y].CellType = GridCell.CellTypes.Buildable;
                    Cells[x, y].RegionClass = _classC;
                }
            }
        }
    }


    public Vector2Int GetTilePos(Vector2 worldPosition)
    {
        Vector2Int gridPos = (Vector2Int)_grid.WorldToCell(worldPosition);
        gridPos.x = Mathf.Clamp(gridPos.x, 0, Cells.GetLength(0) - 1);
        gridPos.y = Mathf.Clamp(gridPos.y, 0, Cells.GetLength(1) - 1);
        return gridPos;
    }

    private void HandleBuildingClick(Vector2 mousePosition)
    {
        Vector2Int gridPos = GetTilePos(mousePosition);
        GridCell cell = Cells[gridPos.x, gridPos.y];

        if (cell.ConstructedBuilding != null)
        {
            UIManager.Instance.OpenSelectedMenu(cell.ConstructedBuilding);
        }
    }

    private void HandleGroundClick(Vector2 mousePosition)
    {
        Vector2Int gridPos = GetTilePos(mousePosition);
        GridCell cell = Cells[gridPos.x, gridPos.y];
        Debug.Log(gridPos);
        Debug.Log(cell.Buildable[1]);

        if (cell.CellType == GridCell.CellTypes.Buildable)
        {
            if (cell.ConstructedBuilding == null)
            {
                UIManager.Instance.OpenBuildMenu(Cells[gridPos.x, gridPos.y]);
            }
        }
    }
}
