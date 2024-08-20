using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BankruptcyDisplay : MonoBehaviour
{
    private TextMeshProUGUI _text;

    void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        _text.text = $"BANKRUPTCY IN\n{LevelManager.Instance.TimeBeforeBankruptcy:00.}";
    }
}
