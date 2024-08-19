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
    public Dictionary<int, int> Influences { get; set; } = new Dictionary<int, int>();
    public Dictionary<int, bool> Buildable { get; set; } = new Dictionary<int, bool>();
    public Building ConstructedBuilding { get; set; }

    public int GetInfluence(int targetPlayer)
    {
        return Influences[targetPlayer];
    }

    public int GetTotalInfluence()
    {
        return Influences.Sum(item => item.Value);
    }

    public float GetInfluencePercentage(int targetPlayer)
    {
        if (GetTotalInfluence() == 0)
        {
            return 0f;
        }
        return GetInfluence(targetPlayer) / GetTotalInfluence();
    }

    private void Start()
    {
        for (int i = 1; i <= LevelManager.Instance.NumPlayers; i++)
        {
            Buildable[i] = false;
        }

    }
}
