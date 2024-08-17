using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class GridController : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap buildingTilemap;
    [SerializeField] private GameObject _blocker;
    [SerializeField] private GameObject _buildWheel;

    private GridCell[,] cells = new GridCell[20,10];


    public void SetBuilding(Vector2Int pos, Building building)
    {

    }

    public bool CanBuild(Vector2Int pos, Building building)
    {
        return cells[pos.x,pos.y].CellType == GridCell.CellTypes.Buildable && cells[pos.x, pos.y].ConstructedBuilding == null;
    }

    private void Start()
    {
        groundTilemap.GetComponent<DetectGroundClick>().OnGroundClick += HandleGroundClick;
        for (int x = 0; x < cells.GetLength(0); x++)
        {
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                cells[x,y] = new GridCell();
            }
        }
    }

    private Vector2Int GetTile()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int gridPos = (Vector2Int)grid.WorldToCell(mouseWorldPos);
        gridPos.x = Mathf.Clamp(gridPos.x, 0, cells.GetLength(0) - 1);
        gridPos.y = Mathf.Clamp(gridPos.y, 0, cells.GetLength(1) - 1);
        return gridPos;
    }

    private void HandleGroundClick()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Vector2Int gridPos = GetTile();
            
            if (cells[gridPos.x, gridPos.y].CellType == GridCell.CellTypes.Buildable)
            {
                _blocker.SetActive(true);
                _buildWheel.GetComponent<RectTransform>().position = Input.mousePosition;
                Debug.Log("O terreno está vazio!");
            }
        }
    }
}
