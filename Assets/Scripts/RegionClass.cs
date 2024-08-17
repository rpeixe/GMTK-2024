using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RegionClass : ScriptableObject
{
    [SerializeField] private float _costFactor;
    [SerializeField] private float _influenceValueFactor;
    [SerializeField] private float _antiInfluenceValueFactor;
    [SerializeField] private float _resourceGenerationFactor;

    public float CostFactor => _costFactor;
    public float InfluenceValueFactor => _influenceValueFactor;
    public float AntiInfluenceValueFactor => _antiInfluenceValueFactor;
    public float ResourceGenerationFactor => _resourceGenerationFactor;
}
