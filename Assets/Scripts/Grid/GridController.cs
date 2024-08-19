using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class GridController : MonoBehaviour
{
    [SerializeField] private Grid _grid;
    [SerializeField] private TileBase rangeTile;
    [Header("Tilemaps")]
    [SerializeField] private Tilemap _groundTilemap;
    [SerializeField] private Tilemap _buildingTilemap;
    [SerializeField] private Tilemap _roadTilemap;
    [SerializeField] private Tilemap _targetTilemap;
    [SerializeField] private Tilemap _classATilemap;
    [SerializeField] private Tilemap _classBTilemap;
    [SerializeField] private Tilemap _classCTilemap;
    [SerializeField] private Tilemap _rangeTilemap;
    [Header("Region Types")]
    [SerializeField] private RegionClass _classA;
    [SerializeField] private RegionClass _classB;
    [SerializeField] private RegionClass _classC;

    public RegionClass ClassA => _classA;
    public RegionClass ClassB => _classB;
    public RegionClass ClassC => _classC;
    public GridCell[,] Cells { get; set; }
    public bool TerrainView { get; private set; } = false;


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

    public bool CanBuild(GridCell cell)
    {
        return (cell.CellType == GridCell.CellTypes.Buildable && cell.Buildable[1] > 0);
    }

    public void SetTileColor(Vector2Int pos, Color color)
    {
        _buildingTilemap.SetTileFlags((Vector3Int)pos, TileFlags.None);
        _buildingTilemap.SetColor((Vector3Int)pos, color);
    }

    public void SetRangeTile(Vector2Int pos, bool boolean)
    {
        if (boolean)
        {
            _rangeTilemap.SetTile((Vector3Int)pos, rangeTile);
        }
        else
        {
            _rangeTilemap.SetTile((Vector3Int)pos, null);
        }
    }

    public void ChangeViewType()
    {
        TerrainView = !TerrainView;

        _rangeTilemap.GetComponent<ToggleTilemap>().ToggleRenderer();
        _classATilemap.GetComponent<ToggleTilemap>().ToggleRenderer();
        _classBTilemap.GetComponent<ToggleTilemap>().ToggleRenderer();
        _classCTilemap.GetComponent<ToggleTilemap>().ToggleRenderer();
    }

    private void Start()
    {
        _groundTilemap.GetComponent<DetectGroundClick>().OnGroundClick += HandleGroundClick;
        _groundTilemap.GetComponent<DetectGroundClick>().OnGroundClick += HandleBuildingClick;
        
    }

    public void InitializeCells()
    {
        for (int x = 0; x < Cells.GetLength(0); x++)
        {
            for (int y = 0; y < Cells.GetLength(1); y++)
            {
                Cells[x, y] = new GameObject("Grid Cell").AddComponent<GridCell>();
                Cells[x, y].Position = new Vector2Int(x, y);

                if (_classATilemap.GetTile(new Vector3Int(x, y)))
                {
                    Cells[x, y].CellType = GridCell.CellTypes.Buildable;
                    Cells[x, y].RegionClass = _classA;
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

                for (int i = 0; i <= LevelManager.Instance.NumPlayers; i++)
                {
                    Cells[x, y].Buildable[i] = 0;
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

        if (cell.ConstructedBuilding != null && cell.ConstructedBuilding.Owner == 1)
        {
            UIManager.Instance.OpenSelectedMenu(cell.ConstructedBuilding);
        }
    }

    private void HandleGroundClick(Vector2 mousePosition)
    {
        Vector2Int gridPos = GetTilePos(mousePosition);
        GridCell cell = Cells[gridPos.x, gridPos.y];

        if (cell.CellType == GridCell.CellTypes.Buildable && cell.Buildable[1] > 0)
        {
            if (cell.ConstructedBuilding == null)
            {
                UIManager.Instance.OpenBuildMenu(Cells[gridPos.x, gridPos.y]);
            }
        }
    }
}
