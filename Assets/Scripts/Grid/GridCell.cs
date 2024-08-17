using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridCell
{
    public enum EconomicClasses
    {
        A,
        B,
        C,
        Monument,
        None
    }

    public EconomicClasses EconomicClass { get; set; }
    public Dictionary<int, int> Influences { get; set; } = new Dictionary<int, int>();
    public IBuilding ConstructedBuilding { get; set; }
}
