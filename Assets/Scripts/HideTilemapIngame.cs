using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HideTilemapIngame : MonoBehaviour
{
    private void Start()
    {
        GetComponent<TilemapRenderer>().enabled = false;
    }
}
