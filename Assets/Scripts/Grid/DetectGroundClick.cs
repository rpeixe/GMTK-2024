using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectGroundClick : MonoBehaviour
{
    public event Action<Vector2> OnGroundClick;

    private void OnMouseUp()
    {
        OnGroundClick?.Invoke(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
}
