using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectGroundClick : MonoBehaviour
{
    public event Action OnGroundClick;
    private void OnMouseUp()
    {
        OnGroundClick?.Invoke();
    }
}
