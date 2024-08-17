using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class GridController : MonoBehaviour
{
    [SerializeField] private Grid _grid;
    [SerializeField] private Tilemap _groundTilemap;
    [SerializeField] private Tilemap _buildingTilemap;
    [SerializeField] private GameObject _blocker;
    [SerializeField] private GameObject _buildWheel;

    public GridCell[,] Cells { get; set; } = new GridCell[20,10];


    public void SetBuilding(Vector2Int pos, Building building)
    {

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
                Cells[x,y] = new GridCell();
            }
        }
    }


    private Vector2Int GetTilePos()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int gridPos = (Vector2Int)_grid.WorldToCell(mouseWorldPos);
        gridPos.x = Mathf.Clamp(gridPos.x, 0, Cells.GetLength(0) - 1);
        gridPos.y = Mathf.Clamp(gridPos.y, 0, Cells.GetLength(1) - 1);
        return gridPos;
    }

    private void HandleGroundClick()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Vector2Int gridPos = GetTilePos();
            
            if (Cells[gridPos.x, gridPos.y].CellType == GridCell.CellTypes.Buildable)
            {
                _blocker.SetActive(true);
                _buildWheel.GetComponent<RectTransform>().position = Input.mousePosition;
                Debug.Log("O terreno está vazio!");
            }
        }
    }
}
