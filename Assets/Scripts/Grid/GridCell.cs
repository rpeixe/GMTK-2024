using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridCell : MonoBehaviour
{
    public enum CellTypes
    {
        Buildable,
        Blocked,
    }

    public CellTypes CellType { get; set; } = CellTypes.Blocked;
    public Vector2Int Position { get; set; }
    public RegionClass RegionClass { get; set; } = LevelManager.Instance.GridController.ClassB;
    public Dictionary<int, bool> Buildable { get; set; } = new Dictionary<int, bool>();
    public Building ConstructedBuilding { get; set; }
}
