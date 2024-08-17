using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridCell : MonoBehaviour
{
    public enum CellTypes
    {
        Buildable,
        Blocked,
    }

    public CellTypes CellType { get; set; } = CellTypes.Buildable;
    public RegionClass RegionClass { get; set; }
    public Dictionary<int, int> Influences { get; set; } = new Dictionary<int, int>();
    public Dictionary<int, int> AntiInfluences { get; set; } = new Dictionary<int, int>();
    public Building ConstructedBuilding { get; set; }

    private void Start()
    {
        for (int i = 1; i <= LevelManager.Instance.NumPlayers; i++)
        {
            Influences[i] = 0;
            AntiInfluences[i] = 0;
        }
    }
}
