using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeViewButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _buttonText;

    private Button _button;

    private void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(HandleClick);
        Invoke(nameof(UpdateSprite), 0.01f);
    }

    private void HandleClick()
    {
        LevelManager.Instance.GridController.ChangeViewType();
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        if (LevelManager.Instance.GridController.TerrainView)
        {
            _buttonText.text = "Terrain View";
        }
        else
        {
            _buttonText.text = "Range View";
        }
    }
}
