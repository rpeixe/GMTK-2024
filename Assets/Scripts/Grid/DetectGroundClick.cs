using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DetectGroundClick : MonoBehaviour
{
    public event Action<Vector2> OnGroundClick;

    private void OnMouseUp()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            OnGroundClick?.Invoke(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }
}
