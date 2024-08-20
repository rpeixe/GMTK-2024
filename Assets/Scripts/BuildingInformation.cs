using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "BuildingInformation")]
public class BuildingInformation : ScriptableObject
{
    public enum BuildingType
    {
        hq,
        office,
        ornamental,
        billboard,
        entertainment,
        monument,
    }

    [SerializeField] private TileBase[] _tiles;
    [SerializeField] private string _name;
    [SerializeField] private float _baseCost;
    [SerializeField] private int _buildingTime;
    [SerializeField] private int _influenceRadius;
    [SerializeField] private float _influenceValue;
    [SerializeField] private float _income;
    [SerializeField] private BuildingInformation _evolution;
    [SerializeField] private BuildingInformation _previous;
    [SerializeField] private bool _permitsBuildingWithinRange = false;
    [SerializeField] private BuildingType _type;
    [TextArea][SerializeField] private string _description;

    public TileBase[] Tiles => _tiles;
    public string Name => _name;
    public float BaseCost => _baseCost;
    public BuildingType Type => _type;
    public int BuildingTime => _buildingTime;
    public int InfluenceRadius => _influenceRadius;
    public float InfluenceValue => _influenceValue;
    public float Income => _income;
    public string Description => _description;
    public BuildingInformation Evolution => _evolution;
    public BuildingInformation Previous => _previous;
    public bool PermitsBuildingWithinRange => _permitsBuildingWithinRange;
}
