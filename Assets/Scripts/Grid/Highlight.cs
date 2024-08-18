using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class Highlight : MonoBehaviour
{
    [SerializeField] TileBase _highlitTile;

    private bool _tracking = false;
    private Vector2Int previousMousePos = new Vector2Int(-1, -1);

    private void OnMouseEnter()
    {
        _tracking = true;
    }

    private void OnMouseExit()
    {
        LevelManager.Instance.GridController.SetTarget(previousMousePos, null);
        previousMousePos = new Vector2Int(-1, -1);
        _tracking = false;
    }

    private void Update()
    {
        if (_tracking && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector2Int mousePos  = LevelManager.Instance.GridController.GetTilePos((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (mousePos != previousMousePos)
            {
                LevelManager.Instance.GridController.SetTarget(previousMousePos, null);
                if (LevelManager.Instance.GridController.Cells[mousePos.x, mousePos.y].CellType == GridCell.CellTypes.Buildable)
                {
                    LevelManager.Instance.GridController.SetTarget(mousePos, _highlitTile);
                }
                previousMousePos = mousePos;
            }
        }
    }
}
