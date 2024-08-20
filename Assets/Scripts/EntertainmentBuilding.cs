using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EntertainmentBuilding : Building
{
    private bool _entertaining = false;
    public void Entertain(Building target)
    {
        float _entertainmentFactor = Marketing/100; 
        target.Income *= _entertainmentFactor;
        target.Marketing *= _entertainmentFactor;
    }

    public void UndoEntertain(Building target)
    {
        float _entertainmentFactor = Marketing/100;
        target.Income /= _entertainmentFactor;
        target.Marketing /= _entertainmentFactor;
    }

    new void Update()
    {

        if (_entertaining)
        {

        }
    }
}
