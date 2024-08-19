using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ToggleTilemap : MonoBehaviour
{
    [SerializeField] private bool _visibleAtStart = true;
    private TilemapRenderer _tilemapRenderer;

    private void Start()
    {
        _tilemapRenderer = GetComponent<TilemapRenderer>();
        _tilemapRenderer.enabled = _visibleAtStart;
    }

    public void ToggleRenderer()
    {
        _tilemapRenderer.enabled = !_tilemapRenderer.enabled;
    }
}
